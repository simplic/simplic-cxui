using Simplic.CXUI.BuildTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.ViewModel
{
    /// <summary>
    /// Embedd base viewmodel into the assembly
    /// </summary>
    public sealed class BuildViewModelBaseTask : BuildTaskBase
    {
        /// <summary>
        /// Write viewmodel base
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            string _path = Path.Combine(base.TempOutputDirectory, "__viewModel", "ViewModelBase.cs");

            var placeholder = new Dictionary<string, string>();
            placeholder.Add("Namespace", CXUIBuildEngine.RootNamespace);

            string code = TemplateHelper.GetTemplate("Simplic.CXUI.ViewModel.Templates.ViewModelBase.cstemplate", placeholder, typeof(BuildViewModelBaseTask).Assembly);

            if (!Directory.Exists(Path.GetDirectoryName(_path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_path));
            }

            File.WriteAllText(_path, code);

            // Add to list of generated files
            CXUIBuildEngine.GeneratedFiles.Add(new GeneratedFile(_path));

            return true;
        }
    }
}
