using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using Simplic.CXUI.BuildTask.ViewModel;

namespace Simplic.CXUI.BuildTask
{
    /// <summary>
    /// Generates the viewmodels for the generated and compiled xaml files
    /// </summary>
    public abstract class BuildViewModelTask : BuildTaskBase
    {
        #region Fields
        private IList<MetaViewModel> viewModels;
        private MetaBaseViewModel defaultBaseViewModel;
        #endregion

        #region Constructor
        /// <summary>
        /// Create generator
        /// </summary>
        public BuildViewModelTask() : base()
        {
            // Default values
            defaultBaseViewModel = new MetaBaseViewModel();
            viewModels = new List<MetaViewModel>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Generate a metaviewmodel by passing it's code
        /// </summary>
        /// <param name="code">Code of the meta viewmodel in the specific language (json, c#, python, ...)</param>
        /// <returns>Throws an exception or returns the viewmodel meta information</returns>
        public abstract MetaViewModel GenerateMetaViewModel(string code);

        /// <summary>
        /// Generate viewmodel and save in output directory
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            foreach (var model in viewModels)
            {
                // List of values which will be replaced in the file
                var values = new Dictionary<string, string>();

                // Get default base viewmodel
                var baseViewModel = model.BaseViewModel ?? defaultBaseViewModel;

                StringBuilder fields = new StringBuilder();
                StringBuilder properties = new StringBuilder();
                StringBuilder constructor = new StringBuilder();

                values.Add("Namespace", model.Namespace);
                values.Add("ViewModelName", model.Name);
                values.Add("ViewModelName", (string.IsNullOrWhiteSpace(model.Namespace) ? "" : model.Namespace + ".") + model.Name);

                // Generate fields and properties
                foreach (var property in model.Properties)
                {
                    // Field
                    string field = property.Name.Trim();
                    StringBuilder _tmp = new StringBuilder(field);
                    _tmp[0] = field[0].ToString().ToLower().ToCharArray()[0];
                    field = string.Format("private {0} {1};", property.Type, _tmp.ToString());
                    fields.AppendLine(field);

                    // Property
                    // Create upper property case
                    _tmp = new StringBuilder(field);
                    _tmp[0] = field[0].ToString().ToUpper().ToCharArray()[0];
                    string propertyName = _tmp.ToString();

                    // Build property
                    StringBuilder propertyCode = new StringBuilder();
                    propertyCode.AppendLine(string.Format("public {0} {1}", property.Type, propertyName));
                    propertyCode.AppendLine("{");
                    propertyCode.AppendLine("\tget");
                    propertyCode.AppendLine("\t{");

                    propertyCode.AppendLine(string.Format("\t\treturn {0};", field));

                    propertyCode.AppendLine("\t}");
                    propertyCode.AppendLine("\tset");
                    propertyCode.AppendLine("\t{");

                    propertyCode.AppendLine(string.Format("\t\t{0} = value;", field));

                    // Is dirty and force save
                    if (!string.IsNullOrWhiteSpace(baseViewModel.NameIsDirtyProperty) && property.SetIsDirty)
                    {
                        propertyCode.AppendLine(string.Format("\t\t{0} = true;", baseViewModel.NameIsDirtyProperty));
                    }
                    if (!string.IsNullOrWhiteSpace(baseViewModel.NameForceSaveProperty) && property.SetForceSave)
                    {
                        propertyCode.AppendLine(string.Format("\t\t{0} = true;", baseViewModel.NameForceSaveProperty));
                    }

                    // Raise property changed
                    if (!string.IsNullOrWhiteSpace(baseViewModel.NameForceSaveProperty) && property.SetForceSave)
                    {
                        propertyCode.AppendLine(string.Format("\t\t{0}({1});", baseViewModel.RaisePropertyChangedMethod, propertyName));
                    }

                    propertyCode.AppendLine("\t}");
                    propertyCode.AppendLine("}");
                    properties.AppendLine(propertyCode.ToString());
                }

                // Set
                values.Add("Fields", fields.ToString().Trim());
                values.Add("ConstructorCode", constructor.ToString().Trim());
                values.Add("Properties", properties.ToString().Trim());

                // Generate file
                string template = TemplateHelper.GetTemplate(TemplateHelper.VIEWMODEL_TEMPLATE, values);
            }

            return base.Execute();
        }
        #endregion

        #region Public Member
        /// <summary>
        /// List of all viewmodels to generate
        /// </summary>
        public IList<MetaViewModel> ViewModels
        {
            get
            {
                return viewModels;
            }

        }

        /// <summary>
        /// Get or set the default viewmodel base, which will be used if no MetaBaseViewModel is set
        /// </summary>
        public MetaBaseViewModel DefaultBaseViewModel
        {
            get
            {
                return defaultBaseViewModel;
            }

            set
            {
                defaultBaseViewModel = value;
            }
        }
        #endregion
    }
}
