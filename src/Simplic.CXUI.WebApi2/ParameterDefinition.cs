using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.WebApi2
{
    /// <summary>
    /// Contains the definition of an action parameter
    /// </summary>
    public class ParameterDefinition
    {
        /// <summary>
        /// Gets or sets the type of the parameter
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the parameter
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
}
