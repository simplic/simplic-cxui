using Microsoft.Build.Framework;
using Microsoft.Build.Tasks.Windows;
using Microsoft.Build.Tasks.Xaml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Simplic.CXUI.BuildTask;

namespace Simplic.CXUI
{
    /// <summary>
    /// Standalone-Xaml-UI builder class, to create UIs which only basis on XAML and pack tham into an assembly
    /// </summary>
    public class CXUIBuilder : IBuildEngine
    {
        #region Fields
        private IList<BuildTask.BuildTaskBase> tasks;
        private string assemblyName;
        #endregion

        #region Constructor
        /// <summary>
        /// Create standalone xaml ui builder
        /// </summary>
        public CXUIBuilder()
        {
            RootNamespace = "DynamicSXUI";

            foreach (var asm in this.GetType().Assembly.GetReferencedAssemblies())
            {
                Assembly.Load(asm.FullName);
            }

            tasks = new List<BuildTask.BuildTaskBase>();

            ContinueOnError = true;
        }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        
        #region [Buid]
        /// <summary>
        /// Build the assembly for the SXUI system
        /// </summary>
        /// <returns>Stream containing the created assembly. Errors will throw an exception</returns>
        public Stream Build()
        {
            Stream assembly = null;
            GeneratedFiles = new List<GeneratedFile>();

            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                assemblyName = "CXUI" + Guid.NewGuid().ToString().Replace("-", "_");
            }
            
            string outputPath = ProjectRoot + "\\obj\\";

            // Remove and reate temp directory
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            else
            {
                Directory.Delete(outputPath, true);
                Directory.CreateDirectory(outputPath);
            }

            foreach (var task in tasks)
            {
                LineNumberOfTaskNode = tasks.IndexOf(task);

                task.BuildEngine = this;
                task.TempOutputDirectory = outputPath;

                if (!task.Execute() && ContinueOnError == false)
                {
                    break;
                }

                // Resolve paths
                foreach (var file in GeneratedFiles)
                {
                    if (string.IsNullOrWhiteSpace(file.RelativeDirectoryPath))
                    {
                        string abs = Path.GetDirectoryName(file.AbsoluteDirectoryPath + "\\");
                        string root = Path.GetDirectoryName(outputPath + "\\");

                        file.RelativeDirectoryPath = Path.GetDirectoryName(abs.Replace(root, "") + "\\") ?? "";
                    }

                    if (string.IsNullOrWhiteSpace(file.AbsolutePath))
                    {
                        file.AbsolutePath = Path.Combine(file.AbsoluteDirectoryPath, file.Name + file.Extension);
                    }
                }
            }

            return assembly;
        }

        /// <summary>
        /// Log exceptions/errors
        /// </summary>
        /// <param name="e"></param>
        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            throw new Exception(e.Message);
        }

        /// <summary>
        /// Log warnings
        /// </summary>
        /// <param name="e"></param>
        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            
        }

        /// <summary>
        /// Log plain messages
        /// </summary>
        /// <param name="e"></param>
        public void LogMessageEvent(BuildMessageEventArgs e)
        {

        }

        /// <summary>
        /// Log custom events
        /// </summary>
        /// <param name="e"></param>
        public void LogCustomEvent(CustomBuildEventArgs e)
        {

        }

        /// <summary>
        /// Defines whether a project should be created
        /// </summary>
        /// <param name="projectFileName"></param>
        /// <param name="targetNames"></param>
        /// <param name="globalProperties"></param>
        /// <param name="targetOutputs"></param>
        /// <returns>Always false</returns>
        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return false;
        }
        #endregion

        #endregion

        #region Public Member
        /// <summary>
        /// Compile target language, currently static cs
        /// </summary>
        public string Language
        {
            get
            {
                return "cs";
            }
        }

        /// <summary>
        /// Root namespace for the assembly
        /// </summary>
        public string RootNamespace
        {
            get;
            set;
        }

        /// <summary>
        /// List of references, which are required for compiling
        /// </summary>
        public Assembly[] References
        {
            get;
            set;
        }

        /// <summary>
        /// Root path of the project
        /// </summary>
        public string ProjectRoot
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the assembly which is generated in this pipeline
        /// </summary>
        public byte[] RawAssembly
        {
            get;
            internal set;
        }

        /// <summary>
        /// Continue executing task when an error occured
        /// </summary>
        public bool ContinueOnError
        {
            get;
            set;
        }

        /// <summary>
        /// Task ID as "fake" line number
        /// </summary>
        public int LineNumberOfTaskNode
        {
            get;
            set;
        }

        /// <summary>
        /// Column, always nu,k
        /// </summary>
        public int ColumnNumberOfTaskNode
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Project file => always an empty string
        /// </summary>
        public string ProjectFileOfTaskNode
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Assembly name which should be used
        /// </summary>
        public string AssemblyName
        {
            get
            {
                return assemblyName;
            }

            set
            {
                assemblyName = value;
            }
        }

        /// <summary>
        /// List of tasks which will be exxecuted during building process.
        /// </summary>
        public IList<BuildTaskBase> Tasks
        {
            get
            {
                return tasks;
            }
        }

        /// <summary>
        /// Contains a list of generated files, which will be passed between build steps
        /// </summary>
        public IList<GeneratedFile> GeneratedFiles
        {
            get;
            private set;
        }
        #endregion

    }
}
