using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Specialized.Content.Import.SupportClasses;

namespace Specialized.Content.Import.Mapping
{
    [DataContract]
    public class MappingBase
    {
        /// <summary>
        /// Set option to clean item names of invalid Sitecore characters
        /// </summary>
        [DataMember]
        public bool CleanNames { get; set; } = true;

        /// <summary>
        /// Gets or sets the Parent ID Source
        /// </summary>
        [DataMember]
        public Enumerations.ParentSource ParentIDSource { get; set; } = Enumerations.ParentSource.Global;

        /// <summary>
        /// Gets or sets the Global Parent ID
        /// </summary>
        [DataMember]
        public Guid GlobalParentID { get; set; } = Guid.Empty;

        /// <summary>
        /// Sitecore ID of Template for items
        /// </summary>
        [DataMember]
        public Guid TemplateID { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class MappingSourceBase
    {
        /// <summary>
		/// Gets or sets the name of the source
		/// </summary>
        [DataMember]
        public string Name { get; set; }

    }

    [DataContract]
    public class MappingItemBase
    {
        /// <summary>
        /// Sitecore Field Name
        /// </summary>
        [DataMember]
        public string FieldName { get; set; }

        /// <summary>
        /// New empty object
        /// </summary>
        public MappingItemBase() { }

        /// <summary>
        /// Create new Mapping for a Sitecore Field. 
        /// </summary>
        /// <param name="fieldName">Name of Sitecore Field</param>
        public MappingItemBase(string fieldName)
        {
            this.FieldName = fieldName;
        }
    }
}
