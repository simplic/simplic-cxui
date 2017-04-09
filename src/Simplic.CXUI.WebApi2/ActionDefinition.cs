using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.WebApi2
{
    /// <summary>
    /// Contains the definition for a web api 2 controller
    /// </summary>
    public class ActionDefinition
    {
        /// <summary>
        /// Gets or sets the action method, e.g. Get/Post
        /// </summary>
        public string Method
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the action name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of attributes
        /// </summary>
        public IList<AttributeDefinition> Attributes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of parameter
        /// </summary>
        public IList<ParameterDefinition> Parameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the usage of a security attribute
        /// </summary>

        public SecurityAttributeDefinition SecurityAttribute
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the values which will be used to generate action method bodies
        /// </summary>
        public IDictionary<string, object> ActionBodySettings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the returns definition
        /// </summary>
        public IList<ReturnDefinition> Returns
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Contains the return definition
    /// </summary>
    public class ReturnDefinition
    {
        /// <summary>
        /// Gets or sets the status-code
        /// </summary>
        public int StatusCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the return type
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the return message
        /// </summary>
        public string Message
        {
            get;
            set;
        }
    }
}
