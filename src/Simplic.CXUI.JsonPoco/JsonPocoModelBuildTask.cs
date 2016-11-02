using Newtonsoft.Json;
using Simplic.CXUI.BuildTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.JsonPoco
{
    /// <summary>
    /// Contains the logic for compiliong json poco definitions into c# poco models
    /// </summary>
    public class JsonPocoModelBuildTask : BuildTaskBase
    {
        #region Field
        private IList<string> modelFiles;
        #endregion

        /// <summary>
        /// Initialize new json poco model build task
        /// </summary>
        public JsonPocoModelBuildTask() : base()
        {
            modelFiles = new List<string>();
        }

        /// <summary>
        /// Compile json to c# poco model
        /// </summary>
        /// <returns>True when the compiling process was successfull</returns>
        public override bool Execute()
        {
            var modelDefinitions = new List<ModelDefinition>();

            foreach (var file in modelFiles)
            {
                // Deserialize json and store as model to compile
                var json = File.ReadAllText(file);
                var model = JsonConvert.DeserializeObject<ModelDefinition>(json);

                model.__AbsolutePath__ = Path.GetDirectoryName(file);
                model.__RelativePath__ = Path.GetDirectoryName(model.__AbsolutePath__.Replace(CXUIBuildEngine.ProjectRoot, "") + "\\");

                modelDefinitions.Add(model);
            }

            foreach (var model in modelDefinitions)
            {
                string tempOutputPath = $"{TempOutputDirectory}{Path.Combine(model.__RelativePath__, model.Name)}.cs";
                Console.WriteLine("Generate: " + tempOutputPath);

                // List of values which will be replaced in the file
                var values = new Dictionary<string, string>();

                StringBuilder fields = new StringBuilder();
                StringBuilder properties = new StringBuilder();

                values.Add("Namespace", model.Namespace);
                values.Add("ModelName", model.Name);

                // Generate fields and properties
                foreach (var property in model.Properties)
                {
                    // Generate field
                    fields.AppendLine(TemplateHelper.GetTemplate("Simplic.CXUI.JsonPoco.Templates.ModelField.cstemplate", new Dictionary<string, string> { { "Type", property.Type }, { "Name", property.Field } }, typeof(JsonPocoModelBuildTask).Assembly));
                    
                    // Generate property
                    Dictionary<string, string> propertyTemplateFields = new Dictionary<string, string>();
                    propertyTemplateFields.Add("Field", property.Field);
                    propertyTemplateFields.Add("Name", property.Name);
                    propertyTemplateFields.Add("Type", property.Type);

                    properties.AppendLine(TemplateHelper.GetTemplate("Simplic.CXUI.JsonPoco.Templates.ModelProperty.cstemplate", propertyTemplateFields, typeof(JsonPocoModelBuildTask).Assembly));
                    properties.AppendLine();
                }

                // Set
                values.Add("Fields", fields.ToString().TrimEnd());
                values.Add("Properties", properties.ToString().TrimEnd());

                // Generate file
                string template = TemplateHelper.GetTemplate("Simplic.CXUI.JsonPoco.Templates.Model.cstemplate", values, typeof(JsonPocoModelBuildTask).Assembly);

                if (!Directory.Exists(Path.GetDirectoryName(tempOutputPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(tempOutputPath));
                }

                System.IO.File.WriteAllText(tempOutputPath, template, Encoding.UTF8);

                CXUIBuildEngine.GeneratedFiles.Add(new GeneratedFile(tempOutputPath));
            }

            return true;
        }

        /// <summary>
        /// Gets or sets a list of json model files that will be compiled
        /// </summary>
        public IList<string> ModelFiles
        {
            get
            {
                return modelFiles;
            }
        }
    }
}
