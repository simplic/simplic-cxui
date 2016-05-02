using Microsoft.Build.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.DynamicUI
{
    public class XamlItem : ITaskItem
    {
        public XamlItem(string path)
        {
            ItemSpec = path;
        }

        public string ItemSpec
        {
            get;
            set;
        }

        public string GetMetadata(string metadataName)
        {
            return "";
        }

        public void SetMetadata(string metadataName, string metadataValue)
        {

        }

        public void RemoveMetadata(string metadataName)
        {

        }

        public void CopyMetadataTo(ITaskItem destinationItem)
        {

        }

        public IDictionary CloneCustomMetadata()
        {
            return null;
        }

        public ICollection MetadataNames
        {
            get
            {
                return null;
            }
        }

        public int MetadataCount
        {
            get
            {
                return 0;
            }
        }
    }
}
