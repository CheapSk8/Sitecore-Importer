using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using Specialized.Content.Import;
using Specialized.Content.Import.Interfaces;
using Specialized.Content.Import.SupportClasses;


namespace Specialized.Content.Import.Mapping.Xml
{
    [DataContract]
    public class XmlMapping : MappingBase, IMapping
    {
        /// <summary>
        /// Source folder for XML Files to import data from
        /// </summary>
        [DataMember]
        public XmlMappingSource Source { get; private set; }

        /// <summary>
        /// XLM Node name for Name of item
        /// </summary>
        [DataMember]
        public string NameNodePath { get; set; }

        /// <summary>
        /// XML Node name for Parent Item of item
        /// </summary>
        [DataMember]
        public string ParentNodePath { get; set; }

        /// <summary>
        /// XML structure to base mapping structure and XPath options
        /// </summary>
        [DataMember]
        public List<string> XPaths { get; set; }

        /// <summary>
        /// List of Mappings for data fields
        /// </summary>
        [DataMember]
        public List<XmlMappingItem> FieldMappings { get; set; } = new List<XmlMappingItem>();

        [DataMember]
        public bool IncludeAttributes { get; private set; }

        public XmlMapping(XmlMappingSource mapSource, bool includeAttributeData = false)
        {
            validateSource(mapSource);
            this.Source = mapSource;
            this.LoadStructure(includeAttributeData);
        }

        public XmlMapping(DirectoryInfo source, bool includeAttributeData = false)
        {
            var newSource = new XmlMappingSource(source);
            validateSource(newSource);

            this.Source = newSource;
            this.LoadStructure(includeAttributeData);
        }

        public XmlMapping()
        {
        }

        public void UpdateSource(XmlMappingSource mapSource, bool includeAttributeData = false)
        {
            validateSource(mapSource);
            this.Source = mapSource;
            this.LoadStructure(includeAttributeData);
        }

        public void LoadStructure(bool includeAttributeData = false)
        {
            this.IncludeAttributes = includeAttributeData;
            if (this.Source?.Source == null || !this.Source.Source.Exists)
            {
                throw new ArgumentException("Source cannot be empty.", nameof(this.Source));
            }

            var firstFile = this.Source.Source.EnumerateFiles("*.xml", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (firstFile == null)
            {
                throw new FileNotFoundException("Directory must contain valid XML files.");
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(firstFile.FullName);
            if (xmlDoc == null)
            {
                throw new ArgumentNullException(nameof(this.Source), "Could not generate XML Paths for given directory.");
            }

            XPaths = new List<string>();

            xmlDoc.IterateXmlNodes((XmlNode node) =>
            {
                var xpath = XmlHelpers.FindXPath(node, true);
                if (!string.IsNullOrWhiteSpace(xpath))
                {
                    XPaths.Add(xpath);
                }
            }, includeAttributeData);
            XPaths.Sort();
        }

        private void validateSource(XmlMappingSource mapSource)
        {
            if (mapSource == null || mapSource.Source == null || !mapSource.Source.Exists)
            {
                throw new ArgumentException("MappingSource cannot be empty.", nameof(mapSource));
            }
        }
    }

    [DataContract]
    public class XmlMappingItem : MappingItemBase
    {
        [DataMember]
        public string XPath { get; set; }

        public XmlMappingItem(string xPath, string fieldName)
        {
            this.XPath = xPath;
            this.FieldName = fieldName;
        }
    }
    [DataContract]
    public class XmlMappingSource : MappingSourceBase
    {
        [DataMember]
        public DirectoryInfo Source { get; private set; }

        [DataMember]
        public int FileCount { get; set; }

        public XmlMappingSource(string path)
        {
            this.Source = new DirectoryInfo(path);
            if (!this.Source.Exists)
            {
                throw new DirectoryNotFoundException($"The given path \"{path}\" was not found. Please supply a valid directory.");
            }
            this.Name = this.Source.FullName;
        }

        public XmlMappingSource(DirectoryInfo directory)
        {
            this.Source = directory;
            this.Name = directory.FullName;
        }
    }

    #region MappingSourceList Class
    /// <summary>
    /// Collection of MappingSources. Note: Names must be unique.
    /// </summary>
    [CollectionDataContract]
    public class XmlMappingSourceList : Collection<XmlMappingSource>
    {

        /// <summary>
        /// Override Collection InsertItem to enforce unique Names
        /// </summary>
        /// <param name="index">Insert index</param>
        /// <param name="item">Insert item</param>
        protected override void InsertItem(int index, XmlMappingSource item)
        {
            if (this.Items.FirstOrDefault(ms => ms.Source.FullName == item.Source.FullName) != null)
            {
                throw new DuplicateNameException("Cannot add two MappingSource items with the same path.");
            }

            base.InsertItem(index, item);
        }
    }
    #endregion
}
