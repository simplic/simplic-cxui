using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.WebApi2
{
    /// <summary>
    /// Contains all properties that are required for defining a web api 2 controlelr
    /// </summary>
    public class ControllerDefinition
    {
        /// <summary>
        /// Gets or sets the namespace of the controller
        /// </summary>
        public string Namespace
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the controller name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the base class of the controlelr
        /// </summary>
        public string Base
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of actions
        /// </summary>
        public IList<ActionDefinition> Actions
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
        /// Gets or sets a list of attributes
        /// </summary>
        public IList<string> Attributes
        {
            get;
            set;
        }

        /// <summary>
        /// Absolte path to the generation base (.json, .py, ...)
        /// </summary>
        internal string __AbsolutePath__
        {
            get;
            set;
        }

        /// <summary>
        /// Relative path to the generation base (.json, .py, ...)
        /// </summary>
        internal string __RelativePath__
        {
            get;
            set;
        }
    }
}
