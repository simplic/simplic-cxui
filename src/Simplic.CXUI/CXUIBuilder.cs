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
        private string temporaryDirectory;
        private IList<XamlSource> xamlSources;
        private IList<BuildTask.BuildTaskBase> tasks;
        private string assemblyName;
        #endregion

        #region Constructor
        /// <summary>
        /// Create standalone xaml ui builder
        /// </summary>
        public CXUIBuilder()
        {
            xamlSources = new List<XamlSource>();
            RootNamespace = "DynamicSXUI";
            temporaryDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Simplic.SXUI\\temp";

            foreach (var asm in this.GetType().Assembly.GetReferencedAssemblies())
            {
                Assembly.Load(asm.FullName);
            }

            tasks = new List<BuildTask.BuildTaskBase>();
            tasks.Add(new BuildTask.CompileXamlPass1());
            tasks.Add(new BuildTask.CompileAssembly());

            ContinueOnError = true;
        }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        #region [Add Xaml Source]
        /// <summary>
        /// Add xaml source code
        /// </summary>
        /// <param name="xamlSource">Xaml source and configuration</param>
        public void AddXamlSource(XamlSource xamlSource)
        {
            if (xamlSource == null)
            {
                throw new ArgumentNullException("xamlSource", "xamlSource must not be null.");
            }

            xamlSources.Add(xamlSource);
        }

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

            xamlSources.Add(new XamlSource() { Name = Path.GetFileName(path), XamlCode = File.ReadAllText(path) });
        }
        #endregion

        #region [Buid]
        /// <summary>
        /// Build the assembly for the SXUI system
        /// </summary>
        /// <returns>Stream containing the created assembly. Errors will throw an exception</returns>
        public Stream Build()
        {
            Stream assembly = null;
            assemblyName = "CXUI" + Guid.NewGuid().ToString().Replace("-", "_");

            string inputPath = temporaryDirectory + "\\input\\";
            string outputPath = temporaryDirectory + "\\output\\";

            // Create temp directory
            if (!Directory.Exists(inputPath))
            {
                Directory.CreateDirectory(inputPath);
            }
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
                task.InputDirectory = inputPath;

                if (!task.Execute() && ContinueOnError == false)
                {
                    break;
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
        /// Directory which will be used for temporary files during the building process
        /// </summary>
        public string TemporaryDirectory
        {
            get
            {
                return temporaryDirectory;
            }

            set
            {
                temporaryDirectory = value;
            }
        }

        /// <summary>
        /// Get a list of generated types
        /// </summary>
        public IList<Type> GeneratedTypes
        {
            get;
            internal set;
        }

        /// <summary>
        /// Get the created assembly
        /// </summary>
        public Assembly GeneratedAssembly
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
        #endregion

    }
}
