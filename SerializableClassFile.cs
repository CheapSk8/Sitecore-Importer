using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Specialized.Content.Import.XmlClasses
{
    [Serializable]
    public class AdvisoryReport
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string RequiresMembership { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string Edition { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }

        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }
        public string HomePageSummary { get; set; }

        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }


        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }


        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }


    [Serializable]
    public class DistanceLearning
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string Title { get; set; }
        public string LeftNavTitle { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string SectionImage { get; set; }
        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }

        public string HomePageSummary { get; set; }
        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }

        [XmlIgnore]
        public string BodyCopy { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }

        [XmlIgnore]
        public string SelfStudyContent { get; set; }

        [XmlElement("SelfStudyContent")]
        public XmlCDataSection SelfStudyContentCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(SelfStudyContent);
            }
            set { SelfStudyContent = (value != null) ? value.Data : null; }
        }

        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }


    [Serializable]
    public class DistanceLearningSection
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string Title { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string WebcastSummaries { get; set; }
        public string SectionImage { get; set; }

        [XmlIgnore]
        public string BodyCopy { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }


        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }

    [Serializable]
    public class DocumentDownload
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string RequiresMembership { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string SectionImage { get; set; }
        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }

        public string HomePageSummary { get; set; }
        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }


        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }

        public string DocumentToDownload { get; set; }
        public string DocumentFriendlyName { get; set; }

        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }

    [Serializable]
    public class EventDetail
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string Title { get; set; }
        public string ProgramDates { get; set; }
        public string Start_Date { get; set; }
        public string CPEs { get; set; }
        public string CPECategories { get; set; }
        public string WebService { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string Location { get; set; }
        public string WhoShouldAttend { get; set; }
        public string TopicArea { get; set; }

        public string SectionImage { get; set; }
        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }

        public string HomePageSummary { get; set; }
        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }


        [XmlIgnore]
        public string AdditionalCopy
        { set; get; }

        [XmlElement("AdditionalCopy")]
        public XmlCDataSection AdditionalCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(AdditionalCopy);
            }
            set { AdditionalCopy = (value != null) ? value.Data : null; }
        }

        public string PDFFile { get; set; }
        public string PDFFileFriendlyName { get; set; }

        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }


        [XmlArray("EventSponsors")]
        [XmlArrayItem("Sponsor")]
        public List<EventSponsor> Sponsors { get; set; }


        [XmlArray("EventSidebarItems")]
        [XmlArrayItem("EventSidebar")]
        public List<EventSidebar> Sidebars { get; set; }

    }

    [Serializable]
    public class FederalRegisterUpdate
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string Title { get; set; }
        public string Date { get; set; }
        public string PageNumber { get; set; }
        public string Agency { get; set; }
        public string Action { get; set; }
        public string Dates { get; set; }
        public string Addresses { get; set; }
        public string FurtherInformation { get; set; }
        public string GpoLinkYear { get; set; }
        public string GpoDocID { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }
        public string HomePageSummary { get; set; }
        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }


        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }

        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }

    [Serializable]
    public class LegislativeUpdate
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string Title { get; set; }
        public string Date { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string SectionImage { get; set; }
        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }

        public string HomePageSummary { get; set; }
        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }


        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }

        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }

    [Serializable]
    public class Podcast
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string RequiresMembership { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string SectionImage { get; set; }
        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }

        public string HomePageSummary { get; set; }
        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }


        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }

        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }

    [Serializable]
    public class PodcastEpisode
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string RequiresMembership { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string SectionImage { get; set; }
        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }

        public string HomePageSummary { get; set; }
        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }


        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }

        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }

    [Serializable]
    public class PressRelease
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string Title { get; set; }
        public string Date { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string SectionImage { get; set; }
        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }

        public string HomePageSummary { get; set; }
        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }


        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }

        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }

    [Serializable]
    public class ProductCategory
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string Title { get; set; }
        public string Abstract { get; set; }
        public string WebService { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string SectionImage { get; set; }
        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }

        public string HomePageSummary { get; set; }
        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }


        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }


        [XmlIgnore]
        public string BottomBodyCopy
        { set; get; }

        [XmlElement("BottomBodyCopy")]
        public XmlCDataSection BottomBodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BottomBodyCopy);
            }
            set { BottomBodyCopy = (value != null) ? value.Data : null; }
        }



        [XmlArray("ProductSidebarItems")]
        [XmlArrayItem("ProductSidebar")]
        public List<ProductSidebar> Sidebar { get; set; }


        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }

    [Serializable]
    public class ProductDetails
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string Title { get; set; }
        public string Abstract { get; set; }
        public string WebService { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string SectionImage { get; set; }
        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }

        public string HomePageSummary { get; set; }
        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }

        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }


        public string ProductTitle { get; set; }
        public string ProductAuthor { get; set; }

        [XmlIgnore]
        public string ProductSummary
        { set; get; }

        [XmlElement("ProductSummary")]
        public XmlCDataSection ProductSummaryCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(ProductSummary);
            }
            set { ProductSummary = (value != null) ? value.Data : null; }
        }

        public string ProductImage { get; set; }
        public string MemberPrice { get; set; }
        public string NonmemberPrice { get; set; }
        public string ProductDate { get; set; }


        [XmlArray("ProductSidebarItems")]
        [XmlArrayItem("ProductSidebar")]
        public List<ProductSidebar> Sidebar { get; set; }


        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }

    [Serializable]
    public class ProductsFront
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string Title { get; set; }
        public string LeftNavTitle { get; set; }
        public string WebService { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string SectionImage { get; set; }


        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }


        [XmlArray("ExternalProductSidebarItems")]
        [XmlArrayItem("ExternalProductSidebar")]
        public List<ExternalProductSidebar> Sidebar { get; set; }


        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }

    [Serializable]
    public class Section
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string LeftNavTitle { get; set; }
        public string Title { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }

        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string SectionImage { get; set; }


        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }


        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }

    }

    [Serializable]
    public class Webcast
    {
        public string PageID { get; set; }
        public string SchemaName { get; set; }
        public string Path { get; set; }

        public string Name { get; set; }
        public string CreationUser { get; set; }
        public DateTime Created { get; set; }

        public string LastModifiedUser { get; set; }
        public DateTime LastModified { get; set; }
        public int VersionNumber { get; set; }

        public string Title { get; set; }
        public string ProgramDates { get; set; }
        public string Start_Date { get; set; }
        public string CPEs { get; set; }
        public string CPECategories { get; set; }
        public string WebService { get; set; }

        public string BrowserBarTitle { get; set; }
        public string MetaDescription { get; set; }
        public string NoFollow { get; set; }
        public string NoIndex { get; set; }
        public string NoODP { get; set; }

        public string SectionImage { get; set; }
        public string HomePageImage { get; set; }
        public string FeaturedImage { get; set; }

        public string HomePageSummary { get; set; }
        public string FeaturedSummary { get; set; }
        public string EBulletinSummary { get; set; }

        public string MemberPrice { get; set; }
        public string NonMemberPrice { get; set; }
        public string Category { get; set; }
        public string DateRecorded { get; set; }
        public string ServiceKey { get; set; }


        public string PDFFile { get; set; }
        public string PDFFileFriendlyName { get; set; }

        [XmlArray("NacuboContacts")]
        [XmlArrayItem("NacuboContact")]
        public List<NacuboContact> NacuboContacts { get; set; }


        [XmlArray("EventSponsors")]
        [XmlArrayItem("Sponsor")]
        public List<EventSponsor> Sponsors { get; set; }


        [XmlArray("WebcastSidebarItems")]
        [XmlArrayItem("WebcastSidebar")]
        public List<WebcastSidebar> Sidebars { get; set; }

    }


    [Serializable]
    public class NacuboContact
    {
        public string ComponentId { get; set; }
        public string ComponentName { get; set; }
        public string Name { get; set; }

        public string Title { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    [Serializable]
    public class EventSidebar
    {
        public string ComponentId { get; set; }
        public string ComponentName { get; set; }
        public string SchemaName { get; set; }

        public string Title { get; set; }

        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }
    }

    [Serializable]
    public class EventSponsor
    {
        public string ComponentId { get; set; }
        public string ComponentName { get; set; }
        public string SponsorName { get; set; }

        public string SponsorImageFile { get; set; }
        public string SponsorImageWidth { get; set; }
        public string SponsorImageHeight { get; set; }
        public string SponsorImageAltText { get; set; }

        public string SponsorURL { get; set; }
        public string SponsorURLText { get; set; }
        public string SponsorFlash { get; set; }
        public string FlashWidth { get; set; }
        public string FlashHeight { get; set; }
        public string SponsorLevel { get; set; }
    }

    [Serializable]
    public class ProductSidebar
    {
        public string ComponentId { get; set; }
        public string ComponentName { get; set; }
        public string SchemaName { get; set; }

        public string Title { get; set; }

        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }
    }

    [Serializable]
    public class ExternalProductSidebar
    {
        public string ComponentId { get; set; }
        public string ComponentName { get; set; }
        public string SchemaName { get; set; }

        public string Title { get; set; }

        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }
    }

    [Serializable]
    public class WebcastSidebar
    {
        public string ComponentId { get; set; }
        public string ComponentName { get; set; }
        public string SchemaName { get; set; }

        public string Title { get; set; }

        [XmlIgnore]
        public string BodyCopy
        { set; get; }

        [XmlElement("BodyCopy")]
        public XmlCDataSection BodyCopyCData
        {
            get
            {
                XmlDocument _dummyDoc = new XmlDocument();
                return _dummyDoc.CreateCDataSection(BodyCopy);
            }
            set { BodyCopy = (value != null) ? value.Data : null; }
        }
    }

    [Serializable]
    public class AdvisoryReportList
    {
        public List<AdvisoryReport> PageList { get; set; }
    }

    [Serializable]
    public class DistanceLearningList
    {
        public List<DistanceLearning> PageList { get; set; }
    }

    [Serializable]
    public class DistanceLearningSectionList
    {
        public List<DistanceLearningSection> PageList { get; set; }
    }

    [Serializable]
    public class DocumentDownloadList
    {
        public List<DocumentDownload> PageList { get; set; }
    }

    [Serializable]
    public class EventDetailList
    {
        public List<EventDetail> PageList { get; set; }
    }

    [Serializable]
    public class FederalRegisterUpdateList
    {
        public List<FederalRegisterUpdate> PageList { get; set; }
    }

    [Serializable]
    public class LegislativeUpdateList
    {
        public List<LegislativeUpdate> PageList { get; set; }
    }

    [Serializable]
    public class PodcastList
    {
        public List<Podcast> PageList { get; set; }
    }

    [Serializable]
    public class PodcastEpisodeList
    {
        public List<PodcastEpisode> PageList { get; set; }
    }

    [Serializable]
    public class PressReleaseList
    {
        public List<PressRelease> PageList { get; set; }
    }

    [Serializable]
    public class ProductCategoryList
    {
        public List<ProductCategory> PageList { get; set; }
    }

    [Serializable]
    public class ProductDetailsList
    {
        public List<ProductDetails> PageList { get; set; }
    }

    [Serializable]
    public class ProductsFrontList
    {
        public List<ProductsFront> PageList { get; set; }
    }

    [Serializable]
    public class SectionList
    {
        public List<Section> PageList { get; set; }
    }

    [Serializable]
    public class WebcastList
    {
        public List<Webcast> PageList { get; set; }
    }

}