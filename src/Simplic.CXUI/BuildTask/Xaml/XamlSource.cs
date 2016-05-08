using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI
{
    /// <summary>
    /// Xaml source instance
    /// </summary>
    public class XamlSource
    {
        /// <summary>
        /// Name of the UI element
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Path to the source file
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Xaml source code
        /// </summary>
        public string XamlCode
        {
            get;
            set;
        }

        /// <summary>
        /// Relative path to the source file, without source name
        /// </summary>
        internal string RelativePath
        {
            get;
            set;
        }
    }
}
