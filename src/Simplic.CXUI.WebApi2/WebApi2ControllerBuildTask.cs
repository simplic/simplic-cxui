using Newtonsoft.Json;
using Simplic.CXUI.BuildTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.WebApi2
{
    /// <summary>
    /// Buildtasks for creating a web api controller from json definition
    /// </summary>
    public class WebApi2ControllerBuildTask : BuildTaskBase
    {
        #region Fields
        private string controllerTemplate;
        private string actionTemplate;
        private IList<string> controllerDefinitionFiles;

        private Func<IDictionary<string, object>, string> actionBodyGenerator;
        #endregion

        /// <summary>
        /// Initialize new web api controller build task
        /// </summary>
        public WebApi2ControllerBuildTask(Func<IDictionary<string, object>, string> actionBodyGenerator) : base()
        {
            this.actionBodyGenerator = actionBodyGenerator;

            var empty = new Dictionary<string, string>();
            controllerTemplate = TemplateHelper.GetTemplate("Simplic.CXUI.WebApi2.Template.Controller.cstemplate", empty, typeof(WebApi2ControllerBuildTask).Assembly);
            actionTemplate = TemplateHelper.GetTemplate("Simplic.CXUI.WebApi2.Template.Action.cstemplate", empty, typeof(WebApi2ControllerBuildTask).Assembly);
            controllerDefinitionFiles = new List<string>();
        }

        /// <summary>
        /// Get security attribute as string
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        private string GetSecurityAttributeString(SecurityAttributeDefinition definition)
        {
            StringBuilder attributes = new StringBuilder();
            if (definition != null && !string.IsNullOrWhiteSpace(definition.Name))
            {
                string roles = "";

                // Generate role list
                if (definition.Roles != null)
                {
                    foreach (var role in definition.Roles)
                    {
                        if (role.Length > 0)
                        {
                            roles += ";";
                        }

                        roles += $"{role}";
                    }

                    if (!string.IsNullOrWhiteSpace(roles))
                    {
                        roles += $"Roles=\"{roles}\"";
                    }
                }

                attributes.Append($"[{definition.Name}({roles})]");
            }

            return attributes.ToString();
        }

        /// <summary>
        /// Create controller c# code
        /// </summary>
        /// <returns>Returns true, when creating the controller was successfull</returns>
        public override bool Execute()
        {
            var controllerDefinitions = new List<ControllerDefinition>();

            foreach (var file in controllerDefinitionFiles)
            {
                // Deserialize json and store as model to compile
                var json = File.ReadAllText(file);
                var ctrl = JsonConvert.DeserializeObject<ControllerDefinition>(json);

                ctrl.__AbsolutePath__ = Path.GetDirectoryName(file);
                ctrl.__RelativePath__ = Path.GetDirectoryName(ctrl.__AbsolutePath__.Replace(CXUIBuildEngine.ProjectRoot, "") + "\\");

                controllerDefinitions.Add(ctrl);
            }

            foreach (var controller in controllerDefinitions)
            {
                string tempOutputPath = Path.Combine(TempOutputDirectory, controller.__RelativePath__, controller.Name + ".cs");
                Console.WriteLine("Generate: " + tempOutputPath);

                // List of values which will be replaced in the file
                var values = new Dictionary<string, string>();

                StringBuilder actions = new StringBuilder();

                values.Add("Namespace", controller.Namespace);
                values.Add("ControllerName", controller.Name);
                values.Add("ControllerBase", controller.Base);

                StringBuilder controllerAttr = new StringBuilder();
                controllerAttr.Append(GetSecurityAttributeString(controller.SecurityAttribute));

                if (controller.Attributes != null)
                {
                    foreach (var attr in controller.Attributes)
                    {
                        if (controllerAttr.Length > 0)
                        {
                            controllerAttr.Append("\r\n");
                        }
                        controllerAttr.Append($"\t\t{attr}");
                    }
                }

                values.Add("Attributes", controllerAttr.ToString());

                // Generate fields and properties
                foreach (var action in controller.Actions)
                {
                    StringBuilder attributes = new StringBuilder();
                    if (!string.IsNullOrWhiteSpace(action.Method))
                    {
                        attributes.Append($"\t\t[Http{action.Method}]");
                    }
                    if (action.SecurityAttribute != null && !string.IsNullOrWhiteSpace(action.SecurityAttribute.Name))
                    {
                        if (attributes.Length > 0)
                        {
                            attributes.Append("\r\n");
                        }
                        attributes.Append($"\t\t{GetSecurityAttributeString(action.SecurityAttribute)}");
                    }

                    if (action.Attributes != null)
                    {
                        foreach (var attr in action.Attributes)
                        {
                            if (attributes.Length > 0)
                            {
                                attributes.Append("\r\n");
                            }
                            attributes.Append($"\t\t{attr}");
                        }
                    }

                    // Generate property
                    Dictionary<string, string> actionTemplateFields = new Dictionary<string, string>();
                    actionTemplateFields.Add("Attributes", attributes.ToString());
                    actionTemplateFields.Add("Name", action.Name);

                    if (action.Parameter != null)
                    {
                        StringBuilder parameters = new StringBuilder();
                        foreach (var parameter in action.Parameter)
                        {
                            if (parameters.Length > 0)
                            {
                                parameters.Append(", ");
                            }

                            parameters.Append($"{parameter.Type} {parameter.Name}{(parameter.Default == null ? "" : $" = {parameter.Default}")}");
                        }

                        actionTemplateFields.Add("Parameter", parameters.ToString().Trim());
                    }

                    // Generate action body settings
                    actionTemplateFields.Add("MethodBody", actionBodyGenerator(action.ActionBodySettings));

                    actions.AppendLine(TemplateHelper.ReplacePlaceholder(actionTemplate, actionTemplateFields));
                    actions.AppendLine();
                }

                // Set
                values.Add("Actions", actions.ToString().TrimEnd());

                // Generate file
                string template = TemplateHelper.ReplacePlaceholder(controllerTemplate, values);

                if (!Directory.Exists(Path.GetDirectoryName(tempOutputPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(tempOutputPath));
                }

                System.IO.File.WriteAllText(tempOutputPath, template, Encoding.UTF8);

                CXUIBuildEngine.GeneratedFiles.Add(new GeneratedFile(tempOutputPath));
            }

            return true;
        }

        #region Public Member
        /// <summary>
        /// Gets or sets the template which will be used for generating a controller
        /// </summary>
        public string ControllerTemplate
        {
            get
            {
                return controllerTemplate;
            }

            set
            {
                controllerTemplate = value;
            }
        }

        /// <summary>
        /// Gets or setst the template which will be used for generating an action in a controlelr
        /// </summary>
        public string ActionTemplate
        {
            get
            {
                return actionTemplate;
            }

            set
            {
                actionTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets a list of json controller definition files
        /// </summary>
        public IList<string> ControllerDefinitionFiles
        {
            get
            {
                return controllerDefinitionFiles;
            }

            set
            {
                controllerDefinitionFiles = value;
            }
        }
        #endregion
    }
}
