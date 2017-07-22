using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

using Specialized.Content.Import.Interfaces;
using Specialized.Content.Import.SupportClasses;

namespace Specialized.Content.Import.Mapping.Xls
{

    #region Mapping Class
    /// <summary>
    /// Represents a Mapping for use in an ImportJob
    /// </summary>
    [DataContract]
    public class XlsMapping : MappingBase, IMapping
    {

        #region Public Properties
        /// <summary>
        /// Source DataTable supplied from Excel file or DataTable
        /// </summary>
        [DataMember]
        public XlsMappingSource Source { get; private set; }

        /// <summary>
        /// Gets or sets whether first row of Source is label row
        /// </summary>
        [DataMember]
        public bool IsFirstRowLabel { get; set; }

        /// <summary>
        /// Column number for Name of item
        /// </summary>
        [DataMember]
        public int NameColumn { get; set; }

        /// <summary>
        /// Column number for Parent Item of item
        /// </summary>
        [DataMember]
        public int ParentColumn { get; set; }

        /// <summary>
        /// List of Mappings for data fields
        /// </summary>
        [DataMember]
        public List<XlsMappingItem> FieldMappings { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new Mapping using a MappingSource
        /// </summary>
        /// <param name="mapSource">MappingSource for this Mapping</param>
        /// <param name="firstRowLabels">Specify whether first row of Source is label row</param>
        public XlsMapping(XlsMappingSource mapSource, bool firstRowLabels = false)
            : this()
        {
            if (mapSource == null || mapSource.DataSource == null || mapSource.DataSource.Rows.Count == 0)
            {
                throw new ArgumentException("MappingSource cannot be empty.", nameof(mapSource));
            }
            this.Source = mapSource;
            this.IsFirstRowLabel = firstRowLabels;
        }

        /// <summary>
        /// Create a new Mapping with data from a DataTable
        /// </summary>
        /// <param name="dataSource">DataTable of data to be imported</param>
        /// <param name="sourceName">Name to create Mapping with</param>
        /// <param name="firstRowLabels">Specify whether first row of Source is label row</param>
        public XlsMapping(DataTable dataSource, string sourceName, bool firstRowLabels = false)
            : this()
        {
            if (dataSource == null || dataSource.Rows.Count == 0)
            {
                throw new ArgumentException("DataSource cannot be empty.", nameof(dataSource));
            }
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                throw new ArgumentException("A valid SourceName is required.", nameof(sourceName));
            }

            this.Source = new XlsMappingSource(sourceName, dataSource);
            this.IsFirstRowLabel = firstRowLabels;
        }

        /// <summary>
        /// Create new mapping with default property values
        /// </summary>
        private XlsMapping()
        {
            this.NameColumn = -1;
            this.ParentColumn = -1;
            this.FieldMappings = new List<XlsMappingItem>();
        }
        #endregion
    }
    #endregion

    #region MappingItem Class
    /// <summary>
    /// Represents a mapping of DataColumn to Sitecore Field by Field name
    /// </summary>
    [DataContract]
    public class XlsMappingItem : MappingItemBase
    {

        /// <summary>
        /// Data Column number
        /// </summary>
        [DataMember]
        public int Column { get; set; }

        /// <summary>
        /// New empty object
        /// </summary>
        private XlsMappingItem() { }

        /// <summary>
        /// Create new Mapping of Column to Sitecore Field. 
        /// </summary>
        /// <param name="column">Column number as 1 based index</param>
        /// <param name="fieldName">Name of Sitecore Field</param>
        public XlsMappingItem(int column, string fieldName)
        {
            this.Column = column;
            this.FieldName = fieldName;
        }
    }
    #endregion

    #region MappingSource Class
    /// <summary>
    /// Represents a named DataTable for source
    /// </summary>
    [DataContract]
    public class XlsMappingSource : MappingSourceBase
    {
        /// <summary>
        /// Gets or sets the DataSource for the source
        /// </summary>
        [DataMember]
        public DataTable DataSource { get; set; }

        /// <summary>
        /// Gets or sets the index of the Worksheet source was created from. If not created from a worksheet, default of -1 is used.
        /// </summary>
        [DataMember]
        public int WorksheetIndex { get; set; }

        /// <summary>
        /// New empty object
        /// </summary>
        public XlsMappingSource()
        {
        }

        /// <summary>
        /// Create named MappingSource
        /// </summary>
        /// <param name="name">Name of MappingSource</param>
        /// <param name="source">Data Source as DataTable</param>
        /// <param name="worksheetIndex">Index of Worksheet within Excel file. If Data Source is not Worksheet, use -1</param>
        public XlsMappingSource(string name, DataTable source, int worksheetIndex = -1)
        {
            this.Name = name;
            this.DataSource = source;
            this.WorksheetIndex = worksheetIndex;
        }
    }
    #endregion

    #region MappingSourceList Class
    /// <summary>
    /// Collection of MappingSources. Note: Names must be unique.
    /// </summary>
    [CollectionDataContract]
    public class XlsMappingSourceList : Collection<XlsMappingSource>
    {

        /// <summary>
        /// Override Collection InsertItem to enforce unique Names
        /// </summary>
        /// <param name="index">Insert index</param>
        /// <param name="item">Insert item</param>
        protected override void InsertItem(int index, XlsMappingSource item)
        {
            if (this.Items.FirstOrDefault(ms => ms.Name == item.Name) != null)
            {
                throw new DuplicateNameException("Cannot add two MappingSource items with the same name.");
            }
            if (string.IsNullOrEmpty(item.DataSource.TableName))
            {
                throw new ArgumentException("DataSource must set TableName.", "MappingSource.DataSource.TableName");
            }
            base.InsertItem(index, item);
        }
    }
    #endregion
}