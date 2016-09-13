using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.WebApi2
{
    /// <summary>
    /// Contains an attribute definition
    /// </summary>
    public class AttributeDefinition
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
        /// Gets or sets a list of attribute parameter
        /// </summary>
        public IList<ParameterDefinition> Parameter
        {
            get;
            set;
        }
    }
}
