using Microsoft.Build.Framework;
using Microsoft.Build.Tasks.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.DynamicUI.BuildTask
{
    /// <summary>
    /// Task for compiling xaml into baml and save in the temporary output directory
    /// </summary>
    public class CompileXamlPass1 : BuildTaskBase
    {
        #region Fields
        private MarkupCompilePass1 _task;
        #endregion

        #region Constructor
        /// <summary>
        /// Create xaml compiler pass 1
        /// </summary>
        public CompileXamlPass1() : base()
        {
            
        }
        #endregion
        
        #region Public Methods
        /// <summary>
        /// Execute baml creation/compilation
        /// </summary>
        /// <returns>Bool if compiling was successfull</returns>
        public override bool Execute()
        {
            _task = new MarkupCompilePass1();
            _task.BuildEngine = BuildEngine;
            _task.RequirePass2ForMainAssembly = false;

            IList<XamlItem> xamlItems = new List<XamlItem>();

            // Write XAML-Source code to the filesystem, if it is not existing yet.
            // If it exists, just override it.
            foreach (var _xaml in CXUIBuildEngine.XamlSources)
            {
                string path = String.Format("{0}{1}.xaml", InputDirectory, _xaml.Name);
                File.WriteAllText(path, _xaml.XamlCode);

                xamlItems.Add(new XamlItem(path));
            }
            _task.PageMarkup = xamlItems.ToArray();

            // Set default namespace
            if (!string.IsNullOrWhiteSpace(CXUIBuildEngine.RootNamespace))
            {
                _task.RootNamespace = CXUIBuildEngine.RootNamespace;
            }

            // Set default options
            _task.AssemblyName = CXUIBuildEngine.AssemblyName;
            _task.Language = "cs";
            _task.OutputPath = TempOutputDirectory;

            // Add all references as XamlItem
            if (CXUIBuildEngine.References != null)
            {
                _task.References = CXUIBuildEngine.References.Select(item => new XamlItem(item.Location)).ToArray();
            }

            return _task.Execute();
        }
        #endregion
    }
}
