using Microsoft.Build.Framework;
using Microsoft.Build.Tasks.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CXUI.BuildTask
{
    /// <summary>
    /// Task for compiling xaml into baml and save in the temporary output directory
    /// </summary>
    public class XamlBuildTaskPass1 : BuildTaskBase
    {
        #region Fields
        private IList<XamlSource> xamlSources;
        private MarkupCompilePass1 _task;
        #endregion

        #region Constructor
        /// <summary>
        /// Create xaml compiler pass 1
        /// </summary>
        public XamlBuildTaskPass1() : base()
        {
            xamlSources = new List<XamlSource>();
        }
        #endregion

        #region Public Methods

        #region [Add Xaml Source]
        /// <summary>
        /// Add xaml source from file
        /// </summary>
        /// <param name="path">Path to the file</param>
        public void AddXamlSourceFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path", "Path must not be null or whitespace.");
            }

            xamlSources.Add(new XamlSource() { Path = Path.GetFullPath(path), Name = Path.GetFileName(path), XamlCode = File.ReadAllText(path) });
        }
        #endregion

        /// <summary>
        /// Execute baml creation/compilation
        /// </summary>
        /// <returns>Bool if compiling was successfull</returns>
        public override bool Execute()
        {
            // Compile every xaml files
            foreach (var _xaml in xamlSources)
            {
                _xaml.RelativePath = _xaml.RelativePath = Path.GetDirectoryName(_xaml.Path.Replace(CXUIBuildEngine.ProjectRoot, ""));

                _task = new MarkupCompilePass1();
                _task.BuildEngine = BuildEngine;
                _task.RequirePass2ForMainAssembly = false;
                _task.OutputType = "library";

                // List of xaml to compile, im this case just one,
                // because only one output is allowed so we have to set
                // the specific output per compile process...
                var _item = new XamlItem(_xaml.Path);
                _item.SetMetadata("Link", _xaml.RelativePath + "\\" + Path.GetFileName(_xaml.Path));
                _item.SetMetadata("LogicalName", _xaml.RelativePath + "\\" + Path.GetFileName(_xaml.Path));
                _task.PageMarkup = new[] { _item };

                // Set default namespace
                if (!string.IsNullOrWhiteSpace(CXUIBuildEngine.RootNamespace))
                {
                    _task.RootNamespace = CXUIBuildEngine.RootNamespace;
                }

                // Set default options
                _task.AssemblyName = CXUIBuildEngine.AssemblyName;
                _task.Language = "cs";
                _task.OutputPath = Path.Combine(TempOutputDirectory, _xaml.RelativePath);
                
                // Add all references as XamlItem
                if (CXUIBuildEngine.References != null)
                {
                    _task.References = CXUIBuildEngine.References.Select(item => new XamlItem(item.Location)).ToArray();
                }

                if (!_task.Execute())
                {
                    return false;
                }

                string generatedPath = Path.Combine(TempOutputDirectory, _xaml.RelativePath, Path.GetFileNameWithoutExtension(_xaml.Name) + ".g.cs");

                // Add all generated bamls
                CXUIBuildEngine.GeneratedFiles.Add(new GeneratedFile(generatedPath));
                foreach (var baml in _task.AllGeneratedFiles)
                {
                    CXUIBuildEngine.GeneratedFiles.Add(new GeneratedFile(baml.ItemSpec));
                }

                string codeBehind = string.Format("{0}.cs", _xaml.Path);
                if (File.Exists(codeBehind))
                {
                    CXUIBuildEngine.GeneratedFiles.Add(new GeneratedFile(codeBehind));
                }
            }

            return true;
        }
        #endregion

        #region Public Member
        /// <summary>
        /// List of XAML sources to compile
        /// </summary>
        public IList<XamlSource> XamlSources
        {
            get
            {
                return xamlSources;
            }

            set
            {
                xamlSources = value;
            }
        }
        #endregion
    }
}
