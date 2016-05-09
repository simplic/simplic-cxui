using Microsoft.Build.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI
{
    public class XamlItem : ITaskItem
    {
        private IDictionary<string, string> metadata;

        public XamlItem(string path)
        {
            metadata = new Dictionary<string, string>();
            ItemSpec = path;
        }

        public string ItemSpec
        {
            get;
            set;
        }

        public string GetMetadata(string metadataName)
        {
            if (metadata.ContainsKey(metadataName))
            {
                return metadata[metadataName];
            }

            return "";
        }

        public void SetMetadata(string metadataName, string metadataValue)
        {
            if (metadata.ContainsKey(metadataName))
            {
                metadata[metadataName] = metadataValue;
            }
            else
            {
                metadata.Add(metadataName, metadataValue);
            }
        }

        public void RemoveMetadata(string metadataName)
        {
            if (metadata.ContainsKey(metadataName))
            {
                metadata.Remove(metadataName);
            }
        }

        public void CopyMetadataTo(ITaskItem destinationItem)
        {
            foreach (var data in metadata)
            {
                destinationItem.SetMetadata(data.Key, data.Value);
            }
        }

        public IDictionary CloneCustomMetadata()
        {
            return metadata.ToDictionary(key => key.Key, value => value.Value);
        }

        public ICollection MetadataNames
        {
            get
            {
                return metadata.Select(item => item.Key).ToList();
            }
        }

        public int MetadataCount
        {
            get
            {
                return metadata.Count;
            }
        }
    }
}
