using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coginov.Objects
{
    public class MetaListInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int BaseTemplate { get; set; } // https://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.listtemplatetype.aspx
        public int BaseType { get; set; } // https://msdn.microsoft.com/en-us/library/microsoft.sharepoint.client.basetype.aspx
        public DateTime LastItemDeletedDate { get; set; }
        public DateTime LastItemModifiedDate { get; set; }
        public List<MetaFileInfo> MetaFilesInfo = new List<MetaFileInfo>();
    }
}
