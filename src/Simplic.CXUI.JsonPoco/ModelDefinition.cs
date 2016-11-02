using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.JsonPoco
{
    /// <summary>
    /// Contains the definition for a mode class
    /// </summary>
    public class ModelDefinition
    {
        /// <summary>
        /// Gets or sets the model namespace
        /// </summary>
        public string Namespace
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the class
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets all properties that will be included into the class
        /// </summary>
        public IList<ModelPropertyDefinition> Properties
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
