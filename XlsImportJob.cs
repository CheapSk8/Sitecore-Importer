using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using OfficeOpenXml;
using SC = Sitecore;
using SCData = Sitecore.Data;
using SCItems = Sitecore.Data.Items;
using SCFields = Sitecore.Data.Fields;

using Specialized.Content.Import.Interfaces;
using Specialized.Content.Import.Mapping.Xls;
using Specialized.Content.Import.SupportClasses;
using Specialized.Content;

namespace Specialized.Content.Import
{

    /// <summary>
    /// Represents an ImportJob used to import Items into Sitecore
    /// </summary>
    public class XlsImportJob : ImportJobBase, IHasImportStatus<StatusMessage>, IImportJob
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the list of MappingSources available for job
        /// </summary>
        public XlsMappingSourceList MappingSources { get; set; }

        /// <summary>
        /// Gets list of ImportMappings defined for job
        /// </summary>
        public List<XlsMapping> ImportMappings { get; private set; }

        /// <summary>
        /// Gets list of XlsStatusMessages logged during ImportJob operation
        /// </summary>
        public new List<StatusMessage> ImportMessages { get; set; } = new List<StatusMessage>();

        #endregion

        #region Private vars
        private Regex _referenceRegEx = null;
        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new Mapping from ImportSources to the ImportMappings using ImportSource index
        /// </summary>
        /// <param name="sourceIndex">Index of MappingSource to add according to ExcelWorksheet number</param>
        /// <param name="firstRowLabels">Specify whether first row of Source is label row</param>
        /// <returns>New Mapping</returns>
        public XlsMapping AddMapping(int sourceIndex, bool firstRowLabels = false)
        {
            XlsMappingSource source = this.MappingSources.FirstOrDefault(ms => ms.WorksheetIndex == sourceIndex);
            if (source == null)
            {
                throw new NullReferenceException(string.Format("No MappingSource found with specified index of {0}.", sourceIndex));
            }

            XlsMapping mapping = new XlsMapping(source, firstRowLabels);
            this.addMapping(mapping);
            return mapping;
        }

        /// <summary>
        /// Adds a new Mapping from ImportSources to the ImportMappings using ImportSource name
        /// </summary>
        /// <param name="sourceName">Name of MappingSource to add according to ExcelWorksheet name</param>
        /// <param name="firstRowLabels">Specify whether first row of Source is label row</param>
        /// <returns>New Mapping</returns>
        public XlsMapping AddMapping(string sourceName, bool firstRowLabels = false)
        {
            XlsMappingSource mapSource = this.MappingSources.FirstOrDefault(ms => ms.Name == sourceName);
            if (mapSource == null)
            {
                StringBuilder sb = new StringBuilder();
                if (string.IsNullOrWhiteSpace(sourceName))
                {
                    sb.Append("SourceName cannot be empty.");
                }
                else
                {
                    sb.AppendFormat("Could not find MappingSource with name \"{0}\". Verify a MappingSource with the supplied name exists.", sourceName);
                }
                throw new ArgumentException(sb.ToString(), "sourceName");
            }

            XlsMapping mapping = new XlsMapping(mapSource, firstRowLabels);
            this.addMapping(mapping);
            return mapping;
        }

        /// <summary>
        /// Add a new mapping to the ImportMappings
        /// </summary>
        /// <param name="mapping">Mapping to add</param>
        /// <param name="skipInvalid">Skip over mappings that don't relate to valid sources. If not skipped, invalid mappings will throw error.</param>
        public void AddMapping(XlsMapping mapping, bool skipInvalid = false)
        {
            if (mapping != null && mapping.Source != null)
            {
                var source = this.MappingSources.FirstOrDefault(ms => ms.Name == mapping.Source.Name);
                if (source != null)
                {
                    mapping.Source.DataSource = source.DataSource;
                    this.addMapping(mapping);
                }
                else if (!skipInvalid)
                {
                    throw new ArgumentException("A mapping relating to a valid MappingSource must be provided.", "mapping.Source.Name: " + mapping.Source.Name);
                }
            }
            else if (!skipInvalid)
            {
                throw new ArgumentException("A mapping relating to a valid MappingSource must be provided.", "mapping.Source.Name: " + mapping.Source.Name);
            }
        }

        /// <summary>
        /// Remove a Mapping from ImportMappings using index of Source in ImportSources
        /// </summary>
        /// <param name="sourceIndex">Index of MappingSource for Mapping</param>
        public void RemoveMapping(int sourceIndex)
        {
            XlsMapping mapping = this.ImportMappings.FirstOrDefault(m => m.Source.WorksheetIndex == sourceIndex);
            if (mapping != null)
            {
                this.removeMapping(mapping);
            }
        }

        /// <summary>
        /// Remove a Mapping from ImportMappings using name of Source in ImportSources 
        /// </summary>
        /// <param name="sourceName">Name of MappingSource for Mapping</param>
        public void RemoveMapping(string sourceName)
        {
            XlsMapping mapping = this.ImportMappings.FirstOrDefault(m => m.Source.Name == sourceName);
            if (mapping != null)
            {
                this.removeMapping(mapping);
            }
        }

        /// <summary>
        /// Allows clearing of mapping list
        /// </summary>
        public void ClearMappings()
        {
            // created in case I need additional logic here
            this.ImportMappings.Clear();
        }

        /// <summary>
        /// Set mappings to new mapping list supplied
        /// </summary>
        /// <param name="mappings">List of mappings. Can be empty.</param>
        public void SetMappings(List<XlsMapping> mappings)
        {
            // created in case I need additional logic here
            this.ClearMappings();   // reset mappings 
            foreach (var mapping in mappings)
            {
                this.AddMapping(mapping, true);
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new ImportJob using an Excel file as source
        /// </summary>
        /// <param name="fileStream">Excel File as stream</param>
        public XlsImportJob(Stream fileStream)
            : this()
        {
            if (fileStream.Length <= 0)
            {
                throw new ArgumentException("Stream cannot be empty.", "fileStream");
            }
            loadWorksheetsFromFile(fileStream);
        }

        /// <summary>
        /// Create a new ImportJob using sources defined in MappingSourceList
        /// </summary>
        /// <param name="sources">DataTable of data to be imported</param>
        public XlsImportJob(XlsMappingSourceList sources)
            : this()
        {
            if (!sources.Any())
            {
                throw new ArgumentException("ImportSources cannot be empty.", nameof(sources));
            }
            if (sources.Where(s => string.IsNullOrEmpty(s.DataSource.TableName)).Any())
            {
                throw new ArgumentException("ImportSources DataSource must set TableName.", "MappingSource.DataSource.TableName");
            }
            this.MappingSources = sources;
        }

        /// <summary>
        /// Create new ImportJob with default property values
        /// </summary>
        private XlsImportJob()
        {
            this.ImportMappings = new List<XlsMapping>();
            this.MappingSources = new XlsMappingSourceList();
            this.ImportType = Enumerations.ImportJobType.Xls;
            this.DoWorkEvent += this.initializeImport;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Method to simplify addMapping
        /// </summary>
        /// <param name="mapping">Mapping to add</param>
        private void addMapping(XlsMapping mapping)
        {
            this.ImportMappings.Add(mapping);
        }

        /// <summary>
        /// Method to simplify removeMapping
        /// </summary>
        /// <param name="mapping">Mapping to remove</param>
        private void removeMapping(XlsMapping mapping)
        {
            this.ImportMappings.Remove(mapping);
        }

        /// <summary>
        /// Load all worksheets from given ExcelFile stream
        /// </summary>
        /// <param name="fileStream">FileStream of ExcelFile</param>
        private void loadWorksheetsFromFile(Stream fileStream)
        {
            using (ExcelPackage ep = new ExcelPackage(fileStream))
            {
                ExcelWorksheets sheets = ep.Workbook.Worksheets;

                string namesStr = string.Join("|", sheets.Select(s => s.Name));
                this._referenceRegEx = new Regex(string.Format(@"^'?({0})'?!(?:[A-Z]+(\d+)|(\d+)\:\d+)$", namesStr));

                foreach (ExcelWorksheet workSheet in sheets)
                {
                    loadDataFromWorksheet(workSheet);
                }
            }
        }

        /// <summary>
        /// Loads specified worksheet from an excel file to store as a DataTable
        /// </summary>
        /// <param name="fileStream">fileStream from Excel file</param>
        /// <param name="index">1 based index of worksheet</param>
        private void loadDataFromWorksheet(ExcelWorksheet workSheet)
        {
            if (workSheet?.Dimension == null) { return; }
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("SitecoreID"));

            var rows = workSheet.Dimension.End.Row;
            var cols = workSheet.Dimension.End.Column;

            for (int c = 1; c <= cols; c++)
            {
                dt.Columns.Add(new DataColumn());

                for (int r = 1; r <= rows; r++)
                {
                    // Add Row
                    if (dt.Rows.Count < rows)
                    {
                        dt.Rows.Add(dt.NewRow());
                    }

                    var val = workSheet.Cells[r, c].Value;
                    var formula = workSheet.Cells[r, c].Formula;
                    if (this._referenceRegEx.IsMatch(formula))
                    {
                        val = formula;
                    }

                    // Populate Row
                    dt.Rows[r - 1][c] = val;
                }
            }
            string sourceName = workSheet.Name;
            dt.TableName = sourceName;
            this.MappingSources.Add(new XlsMappingSource(sourceName, dt, workSheet.Index));
        }

        /// <summary>
        /// Import data from supplied mapping
        /// </summary>
        /// <param name="currentMapping">Mapping to import from</param>
        private void importData(XlsMapping currentMapping)
        {
            SetParent(currentMapping);

            var template = (SCItems.TemplateItem)this.GetItem(currentMapping.TemplateID);
            var ds = currentMapping.Source.DataSource;
            this._currentItemIndex = (currentMapping.IsFirstRowLabel ? 1 : 0);
            for (; _currentItemIndex < ds.Rows.Count; this._currentItemIndex++)
            {
                if (checkContinue())
                {
                    try
                    {
                        // run process
                        var row = ds.Rows[this._currentItemIndex];
                        var name = row[currentMapping.NameColumn].ToString();
                        if (currentMapping.CleanNames)
                        {
                            name = SCItems.ItemUtil.ProposeValidItemName(name);
                        }
                        var parent = this.getParent(currentMapping);
                        if (!string.IsNullOrWhiteSpace(name) && parent != null)
                        {
                            var newItem = template.CreateItemFrom(name, parent);
                            row["SitecoreID"] = newItem.ID;
                            var fields = newItem.GetEditableFields();

                            using (new SC.SecurityModel.SecurityDisabler())
                            {
                                using (new SCItems.EditContext(newItem))
                                {
                                    foreach (var mapping in currentMapping.FieldMappings)
                                    {
                                        try
                                        {
                                            var field = fields.FirstOrDefault(f => f.Name.ToLower() == mapping.FieldName.ToLower());
                                            if (field != null)
                                            {
                                                var fieldValue = row[mapping.Column].ToString();
                                                if (!string.IsNullOrWhiteSpace(fieldValue))
                                                {
                                                    field.SetValueByType(fieldValue);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            LogStatus($"[Field: {mapping.FieldName}] {ex.Message}");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            LogStatus("Name and Parent cannot be blank.");
                        }
                        // report progress
                        this.CurrentPercent = ((this._currentItemIndex + 1) * 100) / currentMapping.Source.DataSource.Rows.Count;
                        if (this.IsAsync)
                        {
                            this.BgWorker.ReportProgress(this.CurrentPercent, currentMapping.Source.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogStatus(ex.Message);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Validate required values for Import Job
        /// </summary>
        /// <returns>True if validation succeeds</returns>
        private bool validateData(XlsMapping mappingData)
        {
            var errorList = new List<string>();
            if (mappingData.Source.DataSource.Rows.Count == 0)
            {
                errorList.Add("Data source has no data. Verify supplied spreadsheet has valid data.");
            }
            if (mappingData.NameColumn < 0 || mappingData.TemplateID == Guid.Empty || (mappingData.ParentIDSource == Enumerations.ParentSource.Item && mappingData.ParentColumn < 0))
            {
                System.Text.StringBuilder argError = new System.Text.StringBuilder("MappingData must supply at least Name, ");
                if (mappingData.ParentIDSource == Enumerations.ParentSource.Global)
                {
                    argError.Append("Parent, and Template mappings.");
                }
                else
                {
                    argError.Append("and Template mappings.");
                }
                errorList.Add(argError.ToString());
            }
            if (mappingData.ParentIDSource == Enumerations.ParentSource.Global && mappingData.GlobalParentID == Guid.Empty)
            {
                errorList.Add("A valid GlobalParentID must be specified when TemplateMapSource is specified as Global");
            }
            // check errors and issue validation return
            if (errorList.Any())
            {
                foreach (var error in errorList)
                {
                    LogStatus(error);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Set local values needed for the import job
        /// </summary>
        private void initializeImport(object sender, DoWorkEventArgs e)
        {
            try {
                InitializeImportBase();
                foreach (var mapping in this.ImportMappings)
                {
                    this.CurrentPercent = 0;
                    this._currentItemIndex = -1;
                    this.CurrentMappingName = mapping.Source.Name;
                    if (checkContinue())
                    {
                        if (validateData(mapping))
                        {
                            this.importData(mapping);
                            this.LogStatus("Completed import of Mapping: " + this.CurrentMappingName);
                        }
                        else
                        {
                            this.ImportStatus = Enumerations.JobStatus.Error;
                        }
                    }
                    else
                    {
                        if (this.IsAsync)
                        {
                            e.Cancel = true;
                        }
                        this.ImportStatus = Enumerations.JobStatus.Canceled;
                        break;
                    }
                }
                if (this.IsJobRunning)
                {
                    this.ImportStatus = Enumerations.JobStatus.Completed;
                }
            }
            catch (Exception ex)
            {
                this.LogStatus(ex.Message);
                this.ImportStatus = Enumerations.JobStatus.Error;
            }
        }

        /// <summary>
        /// Get the template for an item given the TemplateSource and Global or spreadsheet values
        /// </summary>
        /// <returns>Sitecore TemplateItem</returns>
        private SCItems.Item getParent(XlsMapping mappingData)
        {
            if (mappingData.ParentIDSource == Enumerations.ParentSource.Global)
            {
                return this._parent;
            }
            else
            {
                string parentFieldVal = mappingData.Source.DataSource.Rows[this._currentItemIndex][mappingData.ParentColumn].ToString();
                var referenceMatch = this._referenceRegEx.Match(parentFieldVal);
                Guid parentGuid = Guid.Empty;

                if (referenceMatch.Success)
                {
                    var captures = referenceMatch.Groups.Cast<Group>().Skip(1).Where(g => g.Success);
                    string sheetName = captures.ElementAt(0).Value;
                    // convert to 0 index row
                    int row = int.Parse(captures.ElementAt(1).Value) - 1;

                    XlsMappingSource parentSource = this.MappingSources.FirstOrDefault(ms => ms.Name == sheetName);
                    if (parentSource != null)
                    {
                        parentFieldVal = parentSource.DataSource.Rows[row]["SitecoreID"].ToString();
                    }
                    else { parentFieldVal = ""; }
                }
                parentGuid = Guid.Parse(parentFieldVal);

                return this.GetItem(parentGuid);
            }
        }

        #endregion
    }
}