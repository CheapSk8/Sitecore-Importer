using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using SC = Sitecore;
using SCData = Sitecore.Data;
using SCItems = Sitecore.Data.Items;
using SCFields = Sitecore.Data.Fields;

using Specialized.Content;
using Specialized.Content.Import;
using Specialized.Content.Import.Interfaces;
using Specialized.Content.Import.Mapping;
using Specialized.Content.Import.Mapping.Xls;
using Specialized.Content.Import.Mapping.Xml;
using Specialized.Content.Import.SupportClasses;


namespace Specialized.Content.Import.UI
{
    // the purpose of this page is to provide a user interface for creating and running import jobs.
    // the one drawback of using this approach is the javascript polling that has to be done in order
    // to update the progress bar.
    public partial class ContentImport : System.Web.UI.Page
    {
        public const string XmlPathFieldName = "xmlNewPath";    // values submitted as manual folder path entries have this name
        private const string TempFolderName = "tempImport";     // temp dir for extracting zip file contents

        private int _tabIndex = 0;
        private static string _tempDir = $"~/{TempFolderName}/";    // make temp folder relative to site root - could change to page location for more containment...
        public string PagePath = string.Empty;
        private List<string> Errors = new List<string>();

        // keys for session vars
        private const string jobSessionKey = "ImportJob";
        private const string jobTypeSessionKey = "ImportJobType";
        private const string jobFilenameKey = "ImportFileName";

        #region Page Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            PagePath = HttpContext.Current.Request.Url.AbsolutePath;    // determine path page is located for service calls
            IImportJob job = getJob();

            getVersion();

            // fire only on first load
            if (!Page.IsPostBack && !Page.IsCallback)
            {
                var resetQS = Request.QueryString["reset"];
                if (resetQS != null && resetQS == "reset")
                {
                    clearJob();
                }
                else
                {
                    // first hit here should clean temp directory
                    cleanTempFiles();
                    configureFields();
                }
            }
            else
            {
                string controlName = Request.Params["__EVENTTARGET"];   // help isolate specific button hits using this
                Control senderCtrl = Page.FindControl(controlName);

                // keep the active tab active - does not switch to "new mapping" added
                if (!int.TryParse(hdnTabIndex.Value, out this._tabIndex))
                {
                    this._tabIndex = 0;
                }


                // these buttons indicate new job actions - we should not try saving 
                var btnIdFilter = new List<string> { btnNewXlsImport.ID, btnNewXmlZipImport.ID, btnNewXmlFoldersImport.ID };
                // we don't want to save when one of these buttons is clicked
                var noSaveEventIds = new List<string> { btnClearImportJob.ID, btnImportMappings.ID, btnSaveMappings.ID };
                if (job == null && !btnIdFilter.Contains(controlName))
                {
                    // if job is null on PostBack, refresh page and start new
                    Response.Redirect(Request.Url.ToString(), true);
                }
                else if (job != null && !btnIdFilter.Contains(controlName) && !noSaveEventIds.Contains(controlName))
                {
                    saveMappings();
                }
            }
        }

        #endregion

        #region Protected Methods

        protected void btnNewXlsImport_ServerClick(object sender, EventArgs e)
        {
            var exts = new string[] { ".xls", ".xlsx" };

            if (inFileSource.PostedFile != null)
            {
                if (!exts.Contains(Path.GetExtension(inFileSource.PostedFile.FileName)))
                {
                    rfvFileSource.IsValid = false;
                }
                else
                {
                    try
                    {
                        XlsImportJob newJob = new XlsImportJob(inFileSource.PostedFile.InputStream);
                        setJob(newJob, inFileSource.PostedFile.FileName);
                    }
                    catch (Exception ex)
                    {
                        throwError(ex.Message);
                    }
                }
            }

            configureFields();
        }

        protected void btnNewXmlZipImport_Click(object sender, EventArgs e)
        {
            var exts = new string[] { ".zip" };
            if (inFileZip.PostedFile != null)
            {
                if (!exts.Contains(Path.GetExtension(inFileZip.PostedFile.FileName)))
                {
                    rfvFileZip.IsValid = false;
                }
                else
                {
                    try
                    {
                        var mappingSources = getMappingsFromZipFile(inFileZip.PostedFile);
                        XmlImportJob newJob = new XmlImportJob(mappingSources);
                        setJob(newJob, inFileZip.PostedFile.FileName);
                    }
                    catch (Exception ex)
                    {
                        throwError(ex.Message);
                    }
                }
            }
            configureFields();
        }

        protected void btnNewXmlFoldersImport_Click(object sender, EventArgs e)
        {
            var postedPaths = Request.Form.GetValues(XmlPathFieldName).Take(10);    // i am restricting front end to 10. This should as well. 
            if (postedPaths.Where(p => !string.IsNullOrWhiteSpace(p)).Any())
            {
                try
                {
                    var mappingSources = new XmlMappingSourceList();
                    foreach (var path in postedPaths)
                    {
                        var mappedPath = path;
                        if (!Path.IsPathRooted(mappedPath) || Path.GetPathRoot(mappedPath).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                        {
                            mappedPath = Server.MapPath(path);
                        }
                        if (Directory.Exists(mappedPath))
                        {
                            mappingSources.Add(new XmlMappingSource(mappedPath));
                        }
                    }
                    XmlImportJob newJob = new XmlImportJob(mappingSources);
                    setJob(newJob, string.Empty);
                }
                catch (Exception ex)
                {
                    throwError(ex.Message);
                }
            }
            else
            {
                throwError("No valid folders were specified. Please ensure all paths are valid, and relative to the web server.");
            }
            configureFields();
        }

        protected void clearImportJob_ServerClick(object sender, EventArgs e)
        {
            this.clearJob();
        }

        protected void btnXlsAddMapping_ServerClick(object sender, EventArgs e)
        {
            XlsImportJob job = getXlsImportJob();
            string name = ddlSourceOptions.SelectedValue;
            job.AddMapping(name);

            configureFields();
        }

        protected void btnXmlAddMapping_ServerClick(object sender, EventArgs e)
        {
            XmlImportJob job = getXmlImportJob();
            string name = ddlSourceOptions.SelectedValue;
            if (!Path.IsPathRooted(name))
            {
                name = Path.Combine(getTempFolderPath(), name);
            }
            if (Directory.Exists(name))
            {
                job.AddMapping(name, chkXmlIncludeAttributes.Checked);
            }

            configureFields();
        }

        protected void removeMapping_Click(object sender, EventArgs e)
        {
            Control trigger = (Control)sender;
            ListViewItem lvItem = (ListViewItem)trigger.NamingContainer;
            if (getJobType() == Enumerations.ImportJobType.Xls)
            {
                getXlsImportJob().ImportMappings.RemoveAt(lvItem.DataItemIndex);
            }
            else
            {
                getXmlImportJob().ImportMappings.RemoveAt(lvItem.DataItemIndex);
            }
            configureFields();
        }

        protected void lbClearTemplate_Click(object sender, EventArgs e)
        {
            Control trigger = (Control)sender;
            ListViewItem lvItem = (ListViewItem)trigger.NamingContainer;
            var importJob = getJob();
            MappingBase mapping;
            if (importJob.ImportType == Enumerations.ImportJobType.Xls)
            {
                mapping = ((XlsImportJob)importJob).ImportMappings[lvItem.DataItemIndex];
            }
            else
            {
                mapping = ((XmlImportJob)importJob).ImportMappings[lvItem.DataItemIndex];
            }

            mapping.TemplateID = Guid.Empty;
            configureFields();
        }

        protected void lvXlsMappingFields_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var mapItem = (XlsMapping)e.Item.DataItem;

                DropDownList ddlParentSource = getParentSourceControl(e.Item);

                bool isGlobalSource = (mapItem.ParentIDSource == Enumerations.ParentSource.Global);
                if (isGlobalSource)
                {
                    ddlParentSource.SelectedValue = "global";
                    ((HtmlInputControl)e.Item.FindControl("globalParentID")).Value = (mapItem.GlobalParentID != Guid.Empty ? mapItem.GlobalParentID.ToString() : "");
                }
                else
                {
                    ddlParentSource.SelectedValue = "item";
                }
                HtmlControl globalParentRow = (HtmlControl)e.Item.FindControl("globalParentRow");
                HtmlControl itemParentRow = (HtmlControl)e.Item.FindControl("itemParentRow");
                if (!isGlobalSource)
                {
                    globalParentRow.Attributes.Add("class", string.Join(" ",
                        globalParentRow.Attributes["class"].Split(' ').Concat(new string[] { "hidden" }))
                        );
                }
                else
                {
                    itemParentRow.Attributes.Add("class", string.Join(" ",
                        itemParentRow.Attributes["class"].Split(' ').Concat(new string[] { "hidden" }))
                    );
                }

                ((HtmlInputControl)e.Item.FindControl("nameColNumber")).Value = (mapItem.NameColumn >= 0 ? mapItem.NameColumn.ToString() : "");
                ((HtmlInputControl)e.Item.FindControl("parentColNumber")).Value = (mapItem.ParentColumn >= 0 ? mapItem.ParentColumn.ToString() : "");
                getCheckField(e.Item, "chkCleanNames").Checked = mapItem.CleanNames;
                getCheckField(e.Item, "chkFirstRowLabels").Checked = mapItem.IsFirstRowLabel;

                if (mapItem.TemplateID != Guid.Empty)
                {
                    var templateIDInput = ((HtmlInputControl)e.Item.FindControl("templateID"));
                    templateIDInput.Value = mapItem.TemplateID.ToString();
                    templateIDInput.Attributes.Add("readonly", "readonly");
                    ((Literal)e.Item.FindControl("litTemplateName")).Text = string.Empty;
                    ((Control)e.Item.FindControl("btnAddFields")).Visible = false;
                    ((Control)e.Item.FindControl("lbClearTemplate")).Visible = true;

                    listMappingFields(e.Item);
                }
            }
        }

        protected void lvXmlMappingFields_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var mapItem = (XmlMapping)e.Item.DataItem;

                DropDownList ddlParentSource = getParentSourceControl(e.Item);

                bool isGlobalSource = (mapItem.ParentIDSource == Enumerations.ParentSource.Global);
                if (isGlobalSource)
                {
                    ddlParentSource.SelectedValue = "global";
                    ((HtmlInputControl)e.Item.FindControl("globalParentID")).Value = (mapItem.GlobalParentID != Guid.Empty ? mapItem.GlobalParentID.ToString() : "");
                }
                else
                {
                    ddlParentSource.SelectedValue = "item";
                }
                HtmlControl globalParentRow = (HtmlControl)e.Item.FindControl("globalParentRow");
                HtmlControl itemParentRow = (HtmlControl)e.Item.FindControl("itemParentRow");
                if (!isGlobalSource)
                {
                    globalParentRow.Attributes.Add("class", string.Join(" ",
                        globalParentRow.Attributes["class"].Split(' ').Concat(new string[] { "hidden" }))
                        );
                }
                else
                {
                    itemParentRow.Attributes.Add("class", string.Join(" ",
                        itemParentRow.Attributes["class"].Split(' ').Concat(new string[] { "hidden" }))
                    );
                }

                if (!string.IsNullOrWhiteSpace(mapItem.NameNodePath))
                {
                    ((DropDownList)e.Item.FindControl("ddlNameField")).SelectedValue = mapItem.NameNodePath;
                }
                if (!string.IsNullOrWhiteSpace(mapItem.ParentNodePath))
                {
                    ((DropDownList)e.Item.FindControl("ddlParentField")).SelectedValue = mapItem.ParentNodePath;
                }
                getCheckField(e.Item, "chkCleanNames").Checked = mapItem.CleanNames;

                if (mapItem.TemplateID != Guid.Empty)
                {
                    var templateIDInput = ((HtmlInputControl)e.Item.FindControl("templateID"));
                    templateIDInput.Value = mapItem.TemplateID.ToString();
                    templateIDInput.Attributes.Add("readonly", "readonly");
                    ((Literal)e.Item.FindControl("litTemplateName")).Text = string.Empty;
                    ((Control)e.Item.FindControl("btnAddFields")).Visible = false;
                    ((Control)e.Item.FindControl("lbClearTemplate")).Visible = true;

                    listMappingFields(e.Item);
                }
            }
        }

        protected void nodePath_DataBound(object sender, EventArgs e)
        {
            if (sender is DropDownList)
            {
                var ddl = (DropDownList)sender;
                if (!string.IsNullOrWhiteSpace(ddl.ToolTip))
                {
                    ddl.SelectedValue = ddl.ToolTip;
                }
            }
        }
        protected void btnSaveMappings_ServerClick(object sender, EventArgs e)
        {
            // yes the postback alone does this but I wanted an explicit reason for the button to exist
            saveMappings();
            configureFields();
        }

        protected void btnExportSettings_ServerClick(object sender, EventArgs e)
        {
            var job = getJob();
            if (job == null)
            {
                configureFields();
                return;
            }

            var serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(MappingsExport));
            var mappingsExport = new MappingsExport();
            if (job.ImportType == Enumerations.ImportJobType.Xls)
            {
                mappingsExport = getExportMappings((XlsImportJob)job);
            }
            else
            {
                mappingsExport = getExportMappings((XmlImportJob)job);
            }

            if (mappingsExport != null)
            {
                string prefix = job.ImportType.ToString();
                Response.Clear();
                serializer.WriteObject(Response.OutputStream, mappingsExport);
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", $"attachment;filename={prefix}ImportMappings.ims");
                Response.End();
            }
            else { configureFields(); }
        }

        protected MappingsExport getExportMappings(XlsImportJob xlsJob)
        {
            if (xlsJob != null && xlsJob.ImportMappings.Any())
            {

                var saveMappings = new List<XlsMapping>(xlsJob.ImportMappings);
                saveMappings.ForEach(m => m.Source.DataSource = null);

                return new MappingsExport
                {
                    MappingsType = Enumerations.ImportJobType.Xls,
                    Mappings = saveMappings.Cast<object>().ToList()
                };
            }
            return null;
        }

        protected MappingsExport getExportMappings(XmlImportJob xmlJob)
        {
            if (xmlJob != null && xmlJob.ImportMappings.Any())
            {
                return new MappingsExport
                {
                    MappingsType = Enumerations.ImportJobType.Xml,
                    Mappings = xmlJob.ImportMappings.Cast<object>().ToList()
                };
            }

            return null;
        }

        protected void btnCompleteJob_Click(object sender, EventArgs e)
        {
            // just refresh state 
            configureFields();
        }
        protected void btnAddFields_ServerClick(object sender, EventArgs e)
        {
            Control trigger = (Control)sender;
            ListViewItem lvItem = (ListViewItem)trigger.NamingContainer;
            Guid templateID = getIDValue(lvItem, "templateID");
            var job = getJob();
            if (job.ImportType == Enumerations.ImportJobType.Xls)
            {
                var activeMapping = ((XlsImportJob)job).ImportMappings[lvItem.DataItemIndex];
                activeMapping.TemplateID = templateID;
            }
            else
            {
                var activeMapping = ((XmlImportJob)job).ImportMappings[lvItem.DataItemIndex];
                activeMapping.TemplateID = templateID;
            }
            configureFields();
        }

        #endregion

        #region XLS Specific methods

        protected void btnImportMappings_ServerClick(object sender, EventArgs e)
        {
            if (inFileSettings.PostedFile != null && Path.GetExtension(inFileSettings.PostedFile.FileName) == ".ims")
            {
                var serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(MappingsExport));
                var importedMappings = (MappingsExport)serializer.ReadObject(inFileSettings.PostedFile.InputStream);

                if (importedMappings.MappingsType == getJobType())
                {
                    if (importedMappings.MappingsType == Enumerations.ImportJobType.Xls)
                    {
                        importMappings(importedMappings.Mappings.Cast<XlsMapping>().ToList());
                    }
                    else
                    {
                        importMappings(importedMappings.Mappings.Cast<XmlMapping>().ToList());
                    }
                }
            }
            configureFields();
        }

        private void importMappings(List<XlsMapping> xlsMappings)
        {
            var xlsJob = getXlsImportJob();
            if (xlsJob?.MappingSources?.FirstOrDefault() != null && xlsMappings.Any())
            {
                xlsJob.SetMappings(xlsMappings);
                saveJob(xlsJob, getImportFileName());
            }
        }

        private void importMappings(List<XmlMapping> xmlMappings)
        {
            var xmlJob = getXmlImportJob();
            if (xmlJob?.MappingSources?.FirstOrDefault() != null && xmlMappings.Any())
            {
                foreach (var mapping in xmlMappings)
                {
                    if (mapping.Source != null)
                    {
                        var shortName = getSourceName(mapping.Source.Name, false);
                        var fullPath = Path.Combine(getTempFolderPath(), shortName);
                        mapping.UpdateSource(new XmlMappingSource(fullPath), mapping.IncludeAttributes);
                    }
                }
                xmlJob.SetMappings(xmlMappings);
                saveJob(xmlJob, getImportFileName());
            }
        }

        protected void btnXlsDoImport_ServerClick(object sender, EventArgs e)
        {
            bool startImport = false;
            if (getJobType() == Enumerations.ImportJobType.Xls)
            {
                var job = getXlsImportJob();
                if (job != null && job.ImportMappings.Any())
                {
                    startImport = true;
                }
            }
            else
            {
                var job = getXmlImportJob();
                if (job != null && job.ImportMappings.Any())
                {
                    startImport = true;
                }
            }

            if (startImport)
            {
                // /contentimport.aspx/ImportContent
                string script = $@"
				sendRequest('{PagePath + "/ImportContent"}', null, function (d) {{
                    startImport();
					handleUpdate(d);
				}});
				";
                ScriptManager.RegisterStartupScript(updImportWrapper, updImportWrapper.GetType(), "doImportScript", script, true);
                showProgress();
                updImportWrapper.Update();
            }
        }

        protected string getSourceName(string name, bool shorten = true)
        {
            var nameLength = 40;
            if (getJobType() == Enumerations.ImportJobType.Xml)
            {
                // session ID is used for temp path, which is 24 alphanum chars, non-special, no spaces.
                var regEx = new Regex($@"({TempFolderName}\\[a-zA-Z0-9]{{24}})\\");
                var match = regEx.Match(name);
                if (match.Success)
                {
                    // the path is fully absolute as in DirectoryInfo so split works. Drive letter part on left [0], relative temp dir on right [1].
                    var nameParts = name.Split(new[] { match.Captures[0].Value }, StringSplitOptions.None);
                    name = nameParts[1];
                }
                if (shorten && name.Length > nameLength)
                {
                    name = name.Substring(name.Length - nameLength, nameLength);    // shorten to a readable length if necessary
                }
            }

            return name;
        }

        private static XlsImportJob getXlsImportJob()
        {
            IImportJob importJob = getJob();
            if (importJob.ImportType == Enumerations.ImportJobType.Xls)
            {
                return (XlsImportJob)importJob;
            }
            return null;
        }

        private int getMappedColumn(XlsMapping mapping, string name)
        {
            var mapItem = mapping.FieldMappings.FirstOrDefault(fm => fm.FieldName == name);
            if (mapItem == null)
            {
                return -1;
            }
            else
            {
                return mapItem.Column;
            }
        }

        private string getMappedPath(XmlMapping mapping, string name)
        {
            var mapItem = mapping.FieldMappings.FirstOrDefault(fm => fm.FieldName == name);
            if (mapItem == null)
            {
                return string.Empty;
            }
            else
            {
                return mapItem.XPath;
            }
        }

        private void listMappingFields(ListViewItem item)
        {
            Guid templateID = getIDValue(item, "templateID");
            ListView lvFields = (ListView)item.FindControl("lvFields");
            Literal litTemplateName = (Literal)item.FindControl("litTemplateName");
            litTemplateName.Text = string.Empty;
            lvFields.DataSource = null;

            if (templateID != Guid.Empty)
            {
                SCData.Database db = SC.Configuration.Factory.GetDatabase("master");
                SCItems.TemplateItem templateItem = (SCItems.TemplateItem)db.GetItem(new SCData.ID(templateID));
                if (templateItem != null)
                {
                    litTemplateName.Text = "Template Name: " + templateItem.DisplayName;

                    var allFields = templateItem.GetEditableFields();
                    var job = getJob();
                    if (job.ImportType == Enumerations.ImportJobType.Xls)
                    {
                        var activeMapping = ((XlsImportJob)job).ImportMappings[item.DataItemIndex];
                        var fieldList = allFields.Select(f => new
                        {
                            Name = f.DisplayName,
                            Column = getMappedColumn(activeMapping, f.DisplayName)
                        }).OrderBy(f => f.Name);
                        lvFields.DataSource = fieldList;
                    }
                    else
                    {
                        var activeMapping = ((XmlImportJob)job).ImportMappings[item.DataItemIndex];
                        var xPathOptions = new List<string>(activeMapping.XPaths);
                        xPathOptions.Insert(0, "");
                        var fieldList = allFields.Select(f => new
                        {
                            Name = f.DisplayName,
                            XPath = getMappedPath(activeMapping, f.DisplayName),
                            XPathList = xPathOptions
                        }).OrderBy(f => f.Name);
                        lvFields.DataSource = fieldList;
                    }
                }
                else
                {
                    litTemplateName.Text = "No template exists for given ID. ";
                }
            }

            lvFields.DataBind();
        }

        private void saveXlsMappings()
        {
            XlsImportJob job = getXlsImportJob();
            if (job.ImportMappings.Any())
            {
                foreach (var dataItem in lvXlsMappingFields.Items)
                {
                    int index = dataItem.DataItemIndex;
                    XlsMapping thisMapping = job.ImportMappings[index];
                    thisMapping.ParentIDSource = getParentSource(dataItem);
                    if (thisMapping.TemplateID == Guid.Empty)
                    {
                        thisMapping.TemplateID = getIDValue(dataItem, "templateID");
                    }
                    if (thisMapping.ParentIDSource == Enumerations.ParentSource.Global)
                    {
                        thisMapping.GlobalParentID = getIDValue(dataItem, "globalParentID");
                    }
                    else
                    {
                        thisMapping.ParentColumn = getColumnValue(dataItem, "parentColNumber");
                    }
                    thisMapping.NameColumn = getColumnValue(dataItem, "nameColNumber");
                    thisMapping.CleanNames = getCheckField(dataItem, "chkCleanNames").Checked;
                    thisMapping.IsFirstRowLabel = getCheckField(dataItem, "chkFirstRowLabels").Checked;

                    ListView lvFields = (ListView)dataItem.FindControl("lvFields");
                    foreach (var fieldItem in lvFields.Items)
                    {
                        int column = getColumnValue(fieldItem, "columnNumber");
                        string fieldName = getInputValue(fieldItem, "fieldName");
                        if (column >= 0)
                        {
                            thisMapping.FieldMappings.Add(new XlsMappingItem(column, fieldName));
                        }
                    }
                }
            }
            saveJob(job, getImportFileName());
        }
        private void setMappingsDropDown(XlsImportJob xlsJob)
        {
            var sources = xlsJob.MappingSources.Where(ms => xlsJob.ImportMappings.FirstOrDefault(m => m.Source.Name == ms.Name) == null);
            ddlSourceOptions.Items.Clear();
            addMappingWrapper.Visible = true;
            foreach (var source in sources)
            {
                ddlSourceOptions.Items.Add(new ListItem(source.Name));
            }
            if (ddlSourceOptions.Items.Count > 0)
            {
                toggleAddMapping(true);
            }
            else
            {
                toggleAddMapping(false);
            }
        }
        #endregion

        #region XML Specific methods
        private static XmlImportJob getXmlImportJob()
        {
            IImportJob importJob = getJob();
            if (importJob.ImportType == Enumerations.ImportJobType.Xml)
            {
                return (XmlImportJob)importJob;
            }
            return null;
        }

        private XmlMappingSourceList getMappingsFromZipFile(HttpPostedFile postedZip)
        {
            // first, establish paths and save the zip file
            var zipPath = getTempZipPath();
            var fileName = Path.GetFileName(postedZip.FileName);
            var savePath = Path.Combine(zipPath, fileName);
            try
            {
                if (!Directory.Exists(zipPath))
                {
                    Directory.CreateDirectory(zipPath);
                }
                postedZip.SaveAs(savePath);

                // if we saved it, lets extract it now
                var xmlDir = getTempXmlDirectories();
                if (Directory.Exists(xmlDir))
                {
                    DeleteDirectory(xmlDir, true);   // instead of deleting each file and dir, just wipe and recreate parent
                }
                Directory.CreateDirectory(xmlDir);
                System.IO.Compression.ZipFile.ExtractToDirectory(savePath, xmlDir);

                var dirInfo = new DirectoryInfo(xmlDir);
                var xmlDirs = dirInfo.GetDirectories();

                var mappingList = new XmlMappingSourceList();
                if (dirInfo.EnumerateFiles("*.xml").Any())
                {
                    mappingList.Add(new XmlMappingSource(dirInfo));
                }

                foreach (var dir in dirInfo.GetDirectories())
                {
                    if (dir.EnumerateFiles("*.xml").Any())
                    {
                        mappingList.Add(new XmlMappingSource(dir));
                    }
                }

                if (!mappingList.Any())
                {
                    throwError("No XML files found in root or child folders in uploaded ZIP file.");
                }

                // this should contain all parent level folders that contain XML files - do not support nested folders
                return mappingList;
            }
            catch (Exception ex)
            {
                throwError(ex.Message);
            }
            finally
            {
                if (Directory.Exists(zipPath))
                {
                    DeleteDirectory(zipPath, true);
                }
            }

            return new XmlMappingSourceList();
        }

        private void saveXmlMappings()
        {
            XmlImportJob job = getXmlImportJob();

            if (job.ImportMappings.Any())
            {
                foreach (var dataItem in lvXmlMappingFields.Items)
                {
                    int index = dataItem.DataItemIndex;
                    XmlMapping thisMapping = job.ImportMappings[index];
                    thisMapping.ParentIDSource = getParentSource(dataItem);
                    if (thisMapping.TemplateID == Guid.Empty)
                    {
                        thisMapping.TemplateID = getIDValue(dataItem, "templateID");
                    }
                    if (thisMapping.ParentIDSource == Enumerations.ParentSource.Global)
                    {
                        thisMapping.GlobalParentID = getIDValue(dataItem, "globalParentID");
                    }
                    else
                    {
                        thisMapping.ParentNodePath = getSelectedValue(dataItem, "ddlParentField");
                    }
                    thisMapping.NameNodePath = getSelectedValue(dataItem, "ddlNameField");
                    thisMapping.CleanNames = getCheckField(dataItem, "chkCleanNames").Checked;

                    ListView lvFields = (ListView)dataItem.FindControl("lvFields");
                    foreach (var fieldItem in lvFields.Items)
                    {
                        string xpath = getSelectedValue(fieldItem, "nodePath");
                        string fieldName = getInputValue(fieldItem, "fieldName");
                        if (!string.IsNullOrWhiteSpace(xpath))
                        {
                            thisMapping.FieldMappings.Add(new XmlMappingItem(xpath, fieldName));
                        }
                    }
                }
            }

            saveJob(job, getImportFileName());
        }

        private void setMappingsDropDown(XmlImportJob xmlJob)
        {
            var sources = xmlJob.MappingSources.Where(ms => xmlJob.ImportMappings.FirstOrDefault(m => m.Source.Name == ms.Name) == null);
            ddlSourceOptions.Items.Clear();
            addMappingWrapper.Visible = true;
            foreach (var source in sources)
            {
                ddlSourceOptions.Items.Add(new ListItem(source.Name.Replace(getTempFolderPath(), "")));
            }
            if (ddlSourceOptions.Items.Count > 0)
            {
                toggleAddMapping(true);
            }
            else
            {
                toggleAddMapping(false);
            }
        }
        #endregion

        #region Private methods
        private static IImportJob getJob()
        {
            IImportJob importJob = (IImportJob)HttpContext.Current.Session[jobSessionKey];

            if (importJob == null)
            {
                // if we're null, try to load it from a file on the server
                importJob = loadJob();
            }

            return importJob;
        }

        private static Enumerations.ImportJobType getJobType()
        {
            var thisSession = HttpContext.Current.Session;
            var jobTypeValue = (thisSession[jobTypeSessionKey] ?? "").ToString();
            Enumerations.ImportJobType jobType;
            if (string.IsNullOrWhiteSpace(jobTypeValue) || !Enum.TryParse(jobTypeValue, out jobType))
            {
                var currentJob = getJob();
                thisSession[jobTypeSessionKey] = currentJob.ImportType;

                return currentJob.ImportType;
            }

            return jobType;
        }

        private static string getImportFileName()
        {
            return (HttpContext.Current.Session[jobFilenameKey] ?? "").ToString();
        }

        // this should check the server temp dir for a serialized job file and restore the session job if found
        private static IImportJob loadJob()
        {
            var tempPath = getTempFilePath();
            if (File.Exists(tempPath))
            {
                var savedJob = new SavedJob();
                var serializer = new DataContractSerializer(savedJob.GetType());
                using (FileStream fs = File.Open(tempPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    savedJob = (SavedJob)serializer.ReadObject(fs);
                }
                if (savedJob != null)
                {
                    if (savedJob.ImportJobType == Enumerations.ImportJobType.Xls)
                    {
                        var xlsSourceList = (XlsMappingSourceList)savedJob.MappingSources;
                        XlsImportJob job = new XlsImportJob(xlsSourceList);
                        var mappingsList = savedJob.ImportMappings.Cast<XlsMapping>().ToList();
                        job.SetMappings(mappingsList);
                        setSession(job, savedJob.FileName);
                        return job;
                    }
                    else
                    {
                        var xmlSourceList = (XmlMappingSourceList)savedJob.MappingSources;
                        XmlImportJob job = new XmlImportJob(xmlSourceList);
                        var mappingsList = savedJob.ImportMappings.Cast<XmlMapping>().ToList();
                        job.SetMappings(mappingsList);
                        setSession(job, savedJob.FileName);
                        return job;
                    }
                }
            }
            return null;
        }

        // set the session job and also save the job to a serialized file in a temp dir on server
        private void setJob(IImportJob job, string fileName)
        {
            setSession(job, fileName);
            saveJob(job, fileName);
        }

        private void saveJob(IImportJob job, string fileName)
        {
            // serialize to server for fail-safe
            SavedJob saveJob = new SavedJob();
            DataContractSerializer serializer;
            if (job.ImportType == Enumerations.ImportJobType.Xls)
            {
                var xlsJob = (XlsImportJob)job;
                saveJob = new SavedJob(xlsJob.ImportMappings.Cast<object>().ToList(), xlsJob.MappingSources, fileName, Enumerations.ImportJobType.Xls);
            }
            else
            {
                var xmlJob = (XmlImportJob)job;
                saveJob = new SavedJob(xmlJob.ImportMappings.Cast<object>().ToList(), xmlJob.MappingSources, fileName, Enumerations.ImportJobType.Xml);
            }
            serializer = new DataContractSerializer(saveJob.GetType());

            Directory.CreateDirectory(Server.MapPath(_tempDir));
            var tempPath = getTempFilePath();
            using (FileStream fs = File.Open(tempPath, FileMode.Create))
            {

                serializer.WriteObject(fs, saveJob);

            }
        }

        // set the job session vars
        private static void setSession(IImportJob job, string fileName)
        {
            var thisSession = HttpContext.Current.Session;
            thisSession[jobSessionKey] = job;
            thisSession.Timeout = 45;
            thisSession[jobFilenameKey] = fileName;
            thisSession[jobTypeSessionKey] = job.ImportType;
        }

        // clear session vars for job
        private void clearJob()
        {
            IImportJob job = getJob();
            if (job != null)
            {
                if (job.IsJobRunning)
                {
                    job.CancelImport();
                }
                else if (!job.IsJobRunning)
                {
                    Session.Remove(jobSessionKey);
                    Session.Remove(jobFilenameKey);
                    Session.Remove(jobTypeSessionKey);
                }
            }
            // check for serialized temp job file and remove
            string tempPath = getTempFolderPath();
            if (Directory.Exists(tempPath)) { DeleteDirectory(tempPath, true); }

            configureFields();
        }

        private void DeleteDirectory(string path, bool recursive = false)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory, recursive);
            }

            try
            {
                Directory.Delete(path, recursive);
            }
            catch (IOException)
            {
                Directory.Delete(path, recursive);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, recursive);
            }
            catch (Exception ex)
            {
                // do nothing if we fail. At this point, we shouldn't try too hard, or worry the user about anything.
            }
        }

        private void throwError(string message)
        {
            Errors.Add(message);
        }

        private void showHideErrors()
        {
            if (Errors?.FirstOrDefault() == null)
            {
                errorsWrapper.Visible = false;
                return;
            }

            // do something in here
            errorsWrapper.Visible = true;

            rptErrorList.DataSource = Errors;
            rptErrorList.DataBind();
        }

        private static string getTempFolderPath()
        {
            var tempFolder = HttpContext.Current.Server.MapPath(string.Format(@"{0}/{1}/", _tempDir, HttpContext.Current.Session.SessionID));
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }
            return tempFolder;
        }

        private static string getTempZipPath()
        {
            var thisPath = Path.Combine(getTempFolderPath(), "zip");
            return thisPath;
        }

        private static string getTempXmlDirectories()
        {
            var thisPath = Path.Combine(getTempFolderPath(), "XML");
            return thisPath;
        }

        private static string getTempFilePath()
        {
            var thisPath = Path.Combine(getTempFolderPath(), string.Format(@"{0}.isj", HttpContext.Current.Session.SessionID));
            return thisPath;
        }

        private void cleanTempFiles()
        {
            var tempDirs = new DirectoryInfo(Server.MapPath(_tempDir));
            if (tempDirs.Exists && tempDirs.GetDirectories().Any())
            {
                foreach (var dir in tempDirs.GetDirectories())
                {
                    if (dir.LastAccessTime < DateTime.Now.AddDays(-2))
                    {
                        DeleteDirectory(dir.FullName, true);
                    }
                }
            }

        }

        private void configureFields()
        {
            IImportJob job = getJob();
            if (job != null)
            {
                if (job.IsJobRunning || job.IsJobCanceled)
                {
                    showProgress();
                }
                else
                {
                    showConfiguration(job);
                }
            }
            else
            {
                configurationWrapper.Visible = true;
                importProgressWrapper.Visible = false;
                newJobWrapper.Visible = true;
                activeJobWrapper.Visible = false;
                mappingsWrapper.Visible = false;
                saveAndRunRow.Visible = false;
                addMappingWrapper.Visible = false;
            }

            showHideErrors();
            // update panel with all changes taking effect
            updImportWrapper.Update();
        }

        private void showProgress()
        {
            configurationWrapper.Visible = false;
            importProgressWrapper.Visible = true;
        }

        private void showConfiguration(IImportJob importJob)
        {
            configurationWrapper.Visible = true;
            importProgressWrapper.Visible = false;

            if (!ClientScript.IsStartupScriptRegistered(updImportWrapper.GetType(), "keepAliveScript"))
            {
                // start a session keepalive request - see WebMethods at bottom - /contentimport.aspx/KeepAlive
                string script = $@"
					function callKeepAlive() {{
						sendRequest('{PagePath + "/KeepAlive"}', null, function () {{
							window.setTimeout(callKeepAlive, 600000);
						}});
					}}
					callKeepAlive();
					";
                ClientScript.RegisterStartupScript(updImportWrapper.GetType(), "keepAliveScript", script, true);
            }
            newJobWrapper.Visible = false;
            activeJobWrapper.Visible = true;
            if (importJob.ImportType == Enumerations.ImportJobType.Xls)
            {
                showConfiguration((XlsImportJob)importJob);
            }
            if (importJob.ImportType == Enumerations.ImportJobType.Xml)
            {
                showConfiguration((XmlImportJob)importJob);
            }

            // databind to set listviews clear or populated

            lvXlsMappingFields.DataBind();
            lvXmlMappingFields.DataBind();
            lvMappingTabs.DataBind();
        }

        private void showConfiguration(XlsImportJob xlsJob)
        {
            xlsActiveJobWrapper.Visible = !(xmlActiveJobWrapper.Visible = false);
            litImportXlsFile.Text = getImportFileName();
            litWorksheetCount.Text = xlsJob.MappingSources.Count.ToString();
            litAddMappingLabel.Text = "Worksheet";
            setMappingsDropDown(xlsJob);

            // need to handle mappings at an individual type basis
            if (xlsJob.ImportMappings.Any())
            {
                mappingsWrapper.Visible = true;
                saveAndRunRow.Visible = true;
                btnMenuExportSettings.Visible = true;

                lvMappingTabs.DataSource = xlsJob.ImportMappings.Select(m => m.Source);
                lvXlsMappingFields.DataSource = xlsJob.ImportMappings;

                ScriptManager.RegisterStartupScript(updImportWrapper, updImportWrapper.GetType(), "tabScript", string.Format("setTabs({0});", this._tabIndex), true);
            }
            else
            {
                // clear out listViews
                lvMappingTabs.DataSource = null;
                lvXlsMappingFields.DataSource = null;
                mappingsWrapper.Visible = false;
                btnMenuExportSettings.Visible = false;
                saveAndRunRow.Visible = false;
            }
        }

        private void showConfiguration(XmlImportJob xmlJob)
        {
            xmlActiveJobWrapper.Visible = !(xlsActiveJobWrapper.Visible = false);
            var importFile = getImportFileName();
            if (string.IsNullOrWhiteSpace(importFile))
            {
                importFile = "Manual list";
            }
            litImportXmlFolder.Text = importFile;
            litDirectoriesCount.Text = xmlJob.MappingSources.Count.ToString();
            litAddMappingLabel.Text = "Directory";
            setMappingsDropDown(xmlJob);

            if (xmlJob.ImportMappings.Any())
            {
                mappingsWrapper.Visible = true;
                saveAndRunRow.Visible = true;
                btnMenuExportSettings.Visible = true;

                lvMappingTabs.DataSource = xmlJob.ImportMappings.Select(m => m.Source);
                lvXmlMappingFields.DataSource = xmlJob.ImportMappings;

                ScriptManager.RegisterStartupScript(updImportWrapper, updImportWrapper.GetType(), "tabScript", string.Format("setTabs({0});", this._tabIndex), true);
            }
            else
            {
                // clear out listViews
                lvMappingTabs.DataSource = null;
                lvXlsMappingFields.DataSource = null;
                mappingsWrapper.Visible = false;
                btnMenuExportSettings.Visible = false;
                saveAndRunRow.Visible = false;
            }
        }

        private void saveMappings()
        {
            var jobType = getJobType();
            if (jobType == Enumerations.ImportJobType.Xls)
            {
                saveXlsMappings();
            }
            else
            {
                saveXmlMappings();
            }
        }

        private DropDownList getParentSourceControl(ListViewItem item)
        {
            DropDownList ddlParentSource = (DropDownList)item.FindControl("ddlParentSource");
            return ddlParentSource;
        }

        private Enumerations.ParentSource getParentSource(ListViewItem item)
        {
            DropDownList ddlParentSource = getParentSourceControl(item);
            if (ddlParentSource.SelectedValue == "global")
            {
                return Enumerations.ParentSource.Global;
            }
            else
            {
                return Enumerations.ParentSource.Item;
            }
        }

        private HtmlInputCheckBox getCheckField(ListViewItem item, string fieldName)
        {
            return (HtmlInputCheckBox)item.FindControl(fieldName);
        }

        private string getInputValue(ListViewItem item, string fieldName)
        {
            return ((HtmlInputControl)item.FindControl(fieldName)).Value;
        }

        private int getColumnValue(ListViewItem item, string fieldName)
        {
            string valueString = getInputValue(item, fieldName);
            int returnVal = -1;
            if (!int.TryParse(valueString, out returnVal))
            {
                returnVal = -1;
            }
            return returnVal;
        }

        private string getSelectedValue(ListViewItem item, string fieldName)
        {
            var ddlControl = (DropDownList)item.FindControl(fieldName);
            if (ddlControl != null)
            {
                return ddlControl.SelectedValue;
            }
            return string.Empty;
        }

        private Guid getIDValue(ListViewItem item, string fieldName)
        {
            string templateIDStr = getInputValue(item, fieldName);
            Guid templateID = Guid.Empty;
            if (!Guid.TryParse(templateIDStr, out templateID))
            {
                templateID = Guid.Empty;
            }
            return templateID;
        }

        private void toggleAddMapping(bool show)
        {
            addMappingColumn.Visible = show;
            btnExportSettings.Visible = !show;

            btnXlsAddMapping.Visible = btnXmlAddMapping.Visible = false;
            btnXlsMenuAddMapping.Visible = btnXmlMenuAddMapping.Visible = false;
            xmlIncludeAttributeRow.Visible = false;
            if (getJobType() == Enumerations.ImportJobType.Xls)
            {
                btnXlsAddMapping.Visible = show;
                btnXlsMenuAddMapping.Visible = show;
            }
            else
            {
                btnXmlAddMapping.Visible = show;
                btnXmlMenuAddMapping.Visible = show;
                xmlIncludeAttributeRow.Visible = show;
            }

            ScriptManager.RegisterStartupScript(updImportWrapper, updImportWrapper.GetType(), "buttonGroupScript", "setButtonGroups();", true);
        }

        private void getVersion()
        {
            var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            litVersion.Text = "Session: " + Session.SessionID + " v" + versionInfo.FileVersion.ToString();
        }
        #endregion

        #region WebMethods
        // this keeps the session alive - prevents timeout - only when an ImportJob exists
        [WebMethod(EnableSession = false)]
        public static string KeepAlive()
        {
            var context = HttpContext.Current;
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetNoServerCaching();
            return "ok";
        }

        // setting it up this way allows me to isolate the requests away from the updatepanel and postbacks
        // though the script for this is registered to the update panel via Update() method on button click
        [WebMethod(EnableSession = true)]
        public static string ImportContent()
        {
            IImportJob job = getJob();
            if (job != null)
            {
                // isolating the import into a new thread allows an immediate return to user
                // because the job exists in memory (session) it can still be canceled and interacted
                // with by the UI
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                {
                    job.ImportData();
                }));
                t.Start();
                return "success";
            }
            return "error";
        }

        // this big mess assembles my object for progress bar and error checking
        [WebMethod(EnableSession = true)]
        public static string GetImportProgress()
        {
            if (getJobType() == Enumerations.ImportJobType.Xls) { return GetXlsImportProgress(); }
            else { return GetXmlImportProgress(); }
        }

        public static string GetXlsImportProgress()
        {
            var job = getXlsImportJob();
            if (job != null && job.ImportMappings.Any() && job.ImportMappings[0].Source.DataSource.Rows.Count > 0)
            {
                var activeMapping = job.ImportMappings.FirstOrDefault(im => im.Source.Name == job.CurrentMappingName);
                var jobCurrentRow = job.CurrentItemIndex - (activeMapping != null ? activeMapping.IsFirstRowLabel ? 1 : 0 : 0);
                var totalRows = job.ImportMappings.Select(im => im.Source.DataSource.Rows.Count).Sum() - job.ImportMappings.Count(im => im.IsFirstRowLabel);
                var compMappings = job.ImportMappings.TakeWhile(im => job.CurrentMappingName != null && im.Source.Name != job.CurrentMappingName);
                var totalComplete = compMappings.Select(im => im.Source.DataSource.Rows.Count).Sum() - (compMappings.Count(cm => cm.IsFirstRowLabel)) + jobCurrentRow;
                var totalPercent = (totalComplete * 100) / totalRows;
                var status = job.ImportStatus.ToString();
                var result = new
                {
                    Status = status,
                    TotalComplete = totalComplete,
                    TotalPercent = totalPercent,
                    TotalRows = totalRows,
                    CurrentMapping = job.CurrentMappingName,
                    CurrentPercent = job.CurrentPercent,
                    CurrentRow = jobCurrentRow,
                    CurrentTotalRows = (activeMapping != null ? activeMapping.Source.DataSource.Rows.Count - (activeMapping.IsFirstRowLabel ? 1 : 0) : 0)
                };
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                return serializer.Serialize(result);
            }
            else { return ""; }
        }

        public static string GetXmlImportProgress()
        {
            var job = getXmlImportJob();
            if (job != null && job.ImportMappings.Any())
            {
                var activeMapping = job.ImportMappings.FirstOrDefault(im => im.Source.Name == job.CurrentMappingName);
                var jobCurrentRow = job.CurrentItemIndex;
                var totalRows = job.ImportMappings.Select(im => im.Source.FileCount).Sum();
                var compMappings = job.ImportMappings.TakeWhile(im => job.CurrentMappingName != null && im.Source.Name != job.CurrentMappingName);
                var totalComplete = compMappings.Select(im => im.Source.FileCount).Sum() + jobCurrentRow;
                var totalPercent = totalRows > 0 ? (totalComplete * 100) / totalRows : 0;
                var status = job.ImportStatus.ToString();
                var result = new
                {
                    Status = status,
                    TotalComplete = totalComplete,
                    TotalPercent = totalPercent,
                    TotalRows = totalRows,
                    CurrentMapping = job.CurrentMappingName,
                    CurrentPercent = job.CurrentPercent,
                    CurrentRow = jobCurrentRow,
                    CurrentTotalRows = (activeMapping != null ? activeMapping.Source.FileCount : 0)
                };
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                return serializer.Serialize(result);
            }
            else { return ""; }
        }


        // i decided to separate the request for status messages to simplify things until the end
        // results are delivered in counts of 30 - include current page, total pages in return
        [WebMethod(EnableSession = true)]
        public static string GetImportMessages(int page)
        {
            IImportJob job = getJob();
            int perPage = 30;
            if (job != null)
            {
                int totalCount = job.ImportMessages.Count;
                var currentMessages = job.ImportMessages.Skip(page * perPage).Take(perPage);
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                return serializer.Serialize(new
                {
                    Messages = currentMessages,
                    TotalMessages = totalCount,
                    PageSize = perPage,
                    CurrentPage = page
                });
            }
            return "";
        }
        #endregion

        #region Supporting Classes
        /// <summary>
        ///  Class utilized for serializing and deserializing ImportJobs and necessary params to file as session fail-safe
        /// </summary>
        [KnownType(typeof(XmlMapping))]
        [KnownType(typeof(XlsMapping))]
        [KnownType(typeof(XlsMappingSourceList))]
        [KnownType(typeof(XmlMappingSourceList))]
        [DataContract]
        public class SavedJob
        {

            //[DataMember]
            //public Enumerations.ImportJobType ImportType { get; set; }
            [DataMember]
            public string FileName { get; set; }
            [DataMember]
            public List<object> ImportMappings { get; set; }
            [DataMember]
            public object MappingSources { get; set; }
            [DataMember]
            public Enumerations.ImportJobType ImportJobType { get; set; }

            public SavedJob(List<object> mappings, XlsMappingSourceList sources, string fileName, Enumerations.ImportJobType jobType) : this()
            {
                this.ImportMappings = mappings;
                this.FileName = fileName;
                this.ImportJobType = jobType;
                this.MappingSources = sources;
            }

            public SavedJob(List<object> mappings, XmlMappingSourceList sources, string fileName, Enumerations.ImportJobType jobType) : this()
            {
                this.ImportMappings = mappings;
                this.FileName = fileName;
                this.ImportJobType = jobType;
                this.MappingSources = sources;
            }

            public SavedJob()
            {
                this.ImportJobType = Enumerations.ImportJobType.Xls;
                this.ImportMappings = new List<object>();
                this.FileName = string.Empty;
            }
        }

        [KnownType(typeof(XmlMapping))]
        [KnownType(typeof(XlsMapping))]
        [DataContract]
        public class MappingsExport
        {
            [DataMember]
            public Enumerations.ImportJobType MappingsType { get; set; }
            [DataMember]
            public List<object> Mappings { get; set; }
        }
        #endregion
    }
}