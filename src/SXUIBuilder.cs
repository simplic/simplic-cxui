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

namespace Simplic.DynamicUI
{
    /// <summary>
    /// Standalone-Xaml-UI builder class, to create UIs which only basis on XAML and pack tham into an assembly
    /// </summary>
    public class SXUIBuilder
    {
        #region Fields
        private string temporaryDirectory;
        private IList<XamlSource> xamlSources;
        private IList<Type> generatedTypes;
        #endregion

        #region Constructor
        /// <summary>
        /// Create standalone xaml ui builder
        /// </summary>
        public SXUIBuilder()
        {
            xamlSources = new List<XamlSource>();
            RootNamespace = "DynamicSXUI";
            temporaryDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Simplic.SXUI\\temp";

            foreach (var asm in this.GetType().Assembly.GetReferencedAssemblies())
            {
                Assembly.Load(asm.FullName);
            }
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
            string assemblyName = "SXUI" + Guid.NewGuid().ToString().Replace("-", "_");

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

            IList<XamlItem> xamlItems = new List<XamlItem>();

            // Write XAML-Source code to the filesystem
            foreach (var _xaml in xamlSources)
            {
                string path = String.Format("{0}{1}.xaml", inputPath, _xaml.Name);
                File.WriteAllText(path, _xaml.XamlCode);

                xamlItems.Add(new XamlItem(path));
            }

            var engine = new BuildEngine();

            // Create builder class
            var xamlBuildTask = new MarkupCompilePass1();
            xamlBuildTask.BuildEngine = engine;
            xamlBuildTask.RequirePass2ForMainAssembly = false;

            xamlBuildTask.PageMarkup = xamlItems.ToArray();

            if (!string.IsNullOrWhiteSpace(RootNamespace))
            {
                xamlBuildTask.RootNamespace = RootNamespace;
            }

            //xamlBuildTask.BuildTaskPath = typeof(Microsoft.Build.Tasks.Windows.MarkupCompilePass2).Assembly.Location;
            xamlBuildTask.AssemblyName = assemblyName;
            xamlBuildTask.Language = Language;
            xamlBuildTask.OutputPath = outputPath;

            // Add all references as XamlItem
            if (References != null)
            {
                xamlBuildTask.References = References.Select(item => new XamlItem(item.Location)).ToArray();
            }

            //xamlBuildTask.RequiresCompilationPass2 = false;

            // Start building
            if (xamlBuildTask.Execute())
            {
                // Start creating the assembly
                IList<SyntaxTree> syntaxTrees = new List<SyntaxTree>();

                // Load generated xaml-cs and code-behind files
                foreach (string file in Directory.GetFiles(outputPath).Where(item => item.EndsWith(".g.cs")))
                {
                    var _st = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                    syntaxTrees.Add(_st);
                }
                foreach (string file in Directory.GetFiles(inputPath).Where(item => item.EndsWith(".xaml.cs")))
                {
                    var _st = CSharpSyntaxTree.ParseText(File.ReadAllText(file));
                    syntaxTrees.Add(_st);
                }

                // Set references (Assemblies)
                List<MetadataReference> references = References.Select(item => MetadataReference.CreateFromFile(item.Location)).ToList<MetadataReference>();

                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!References.Contains(asm) && !string.IsNullOrWhiteSpace(asm.Location))
                    {
                        references.Add(MetadataReference.CreateFromFile(asm.Location));
                    }
                }

                // Set compiler options
                CSharpCompilation compilation = CSharpCompilation.Create(
                    assemblyName: assemblyName,
                    syntaxTrees: syntaxTrees.ToArray(),
                    references: references,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                using (var ms = new MemoryStream())
                {
                    // We also need to find and embedd some ressource
                    List<ResourceDescription> resourceDescriptions = new List<ResourceDescription>();

                    string resourcePath = string.Format("{0}{1}.g.resources", outputPath, RootNamespace);
                    ResourceWriter rsWriter = new ResourceWriter(resourcePath);
                    
                    foreach (string file in Directory.GetFiles(outputPath).Where(item => item.EndsWith(".baml")))
                    {
                        var fileName = Path.GetFileName(file.ToLower());
                        var data = File.OpenRead(file);
                        rsWriter.AddResource(fileName, data, true);
                    }

                    rsWriter.Generate();
                    rsWriter.Close();

                    // Add ressource under the namespace AND assembly
                    var resourceDescription = new ResourceDescription(
                                    string.Format("{0}.g.resources", RootNamespace),
                                    () => File.OpenRead(resourcePath),
                                    true);
                    resourceDescriptions.Add(resourceDescription);

                    if (RootNamespace != assemblyName)
                    {
                        resourceDescription = new ResourceDescription(
                                        string.Format("{0}.g.resources", assemblyName),
                                        () => File.OpenRead(resourcePath),
                                        true);
                        resourceDescriptions.Add(resourceDescription);
                    }

                    // Emit code and embedd ressources
                    EmitResult result = compilation.Emit(ms, manifestResources: resourceDescriptions.ToArray());
                    
                    if (!result.Success)
                    {
                        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error);

                        StringBuilder exBuilder = new StringBuilder();

                        foreach (Diagnostic diagnostic in failures)
                        {
                            exBuilder.AppendLine(string.Format("{0}: {1}", diagnostic.Id, diagnostic.GetMessage()));
                        }

                        throw new Exception(exBuilder.ToString());
                    }
                    else
                    {
                        ms.Seek(0, SeekOrigin.Begin);

                        byte[] asm = ms.ToArray();
                        File.WriteAllBytes(outputPath + "_output.dll", asm);

                        GeneratedAssembly = Assembly.Load(asm);

                        generatedTypes = GeneratedAssembly.GetTypes();
                    }
                }
            }

            return assembly;
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
            get
            {
                return generatedTypes;
            }
        }

        /// <summary>
        /// Get the created assembly
        /// </summary>
        public Assembly GeneratedAssembly
        {
            get;
            private set;
        }
        #endregion

    }
}
