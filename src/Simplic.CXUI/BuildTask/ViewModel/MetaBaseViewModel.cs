using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.BuildTask.ViewModel
{
    /// <summary>
    /// Model which descripes the layout of the BaseViewModel from which a ViewModel inherits
    /// </summary>
    public class MetaBaseViewModel
    {
        /// <summary>
        /// Create and set default properties
        /// </summary>
        public MetaBaseViewModel()
        {
            this.Name = "ViewModelBase";
            this.Namespace = "Simplic.UI";
            this.NameIsDirtyProperty = "IsDirty";
            this.NameForceSaveProperty = "ForceSave";
            this.ParentViewModelProperty = "Parent";
            this.RaisePropertyChangedMethod = "RaisePropertyChanged";
        }

        /// <summary>
        /// Name of the class
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Namespace of the class
        /// </summary>
        public string Namespace
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the is dirty property
        /// </summary>
        public string NameIsDirtyProperty
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the force save property
        /// </summary>
        public string NameForceSaveProperty
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the property, which allows to create a parent-child relationship between ViewModels
        /// </summary>
        public string ParentViewModelProperty
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the raiseproperty changed method
        /// </summary>
        public string RaisePropertyChangedMethod
        {
            get;
            set;
        }
    }
}
