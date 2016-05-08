using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.BuildTask.ViewModel
{
    /// <summary>
    /// Class which contains all information a MetaViewModel must contain
    /// </summary>
    public class MetaViewModel
    {
        /// <summary>
        /// Create new MetaViewModel
        /// </summary>
        public MetaViewModel()
        {

        }

        /// <summary>
        /// Name of the model/class
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Base class from which the view-model inherits.
        /// </summary>
        public MetaBaseViewModel BaseViewModel
        {
            get;
            set;
        }

        /// <summary>
        /// Namespace in which the model will be generated
        /// </summary>
        public string Namespace
        {
            get;
            set;
        }

        /// <summary>
        /// List of properties the model contains
        /// </summary>
        public IList<MetaViewModelProperty> Properties
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
