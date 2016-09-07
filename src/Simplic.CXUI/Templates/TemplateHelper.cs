using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI
{
    /// <summary>
    /// Helper to work with templates
    /// </summary>
    public static class TemplateHelper
    {
        /// <summary>
        /// Template for xaml code behind files
        /// </summary>
        public const string XAML_CODE_BEHIND_TEMPLATE = "Simplic.CXUI.Templates.XamlCodeBehind.cstemplate";

        /// <summary>
        /// Get a filled template.
        /// </summary>
        /// <param name="name">Name of the template (use the constant strings to access them)</param>
        /// <param name="values">Values which will be replaced within the template</param>
        /// <param name="asm">Optional assembly</param>
        /// <returns>Temaplte as string</returns>
        public static string GetTemplate(string name, IDictionary<string, string> values, Assembly asm)
        {
            var assembly = asm ?? typeof(TemplateHelper).Assembly;
            
            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    
                    return ReplacePlaceholder(result, values);
                }
            }
        }

        /// <summary>
        /// Replace a set of placeholder in a string
        /// </summary>
        /// <param name="template">Template code</param>
        /// <param name="values">Value (K/V)</param>
        /// <returns>Prepared template</returns>
        public static string ReplacePlaceholder(string template, IDictionary<string, string> values)
        {
            foreach (var val in values)
            {
                template = template.Replace("{" + val.Key + "}", val.Value);
            }

            return template;
        }
    }
}
