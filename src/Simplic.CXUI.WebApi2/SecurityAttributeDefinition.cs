using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.WebApi2
{
    /// <summary>
    /// Definition of a security attribute
    /// </summary>
    public class SecurityAttributeDefinition
    {
        /// <summary>
        /// Gets or sets the attribute name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of roles
        /// </summary>
        public IList<string> Roles
        {
            get;
            set;
        }
    }
}
