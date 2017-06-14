using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coginov.Objects
{
    public class MetaFileInfo
    {
        public int Id { get; set; }
        public string EncodedAbsUrl { get; set; }
        public string LocalAbsUrl { get; set; }
        public MetaListInfo ParentMetaListInfo { get; set; }

        public List<string> Concepts { get; set; }
        public List<string> Category { get; set; }
        public List<string> Entities { get; set; }

        public string getFileName(){
            return EncodedAbsUrl.Split('/').Last();
        }

        public string getFileType() {
            return getFileName().Split('.').Last();
        }


    }
}
