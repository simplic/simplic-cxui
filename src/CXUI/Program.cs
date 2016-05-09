using CXUI.Helper;
using Newtonsoft.Json;
using Simplic.CommandShell;
using Simplic.CXUI;
using Simplic.CXUI.BuildTask;
using Simplic.CXUI.JsonViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CXUI
{
    class Program
    {
        /// <summary>
        /// Cmd entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Simplic Compiled XAML UI - Version 1.0.0.0");

            var context = CommandShellManager.Singleton.CreateShellContext("cxui");

            context.RegisterMethod("build", Build, "config");

            bool errorOccured = false;
            string cmd = "";

            foreach (string part in args)
            {
                if (cmd != "")
                {
                    cmd += " ";
                }

                cmd += part;
            }

            string result = context.Execute(cmd, out errorOccured);

            if (errorOccured)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.WriteLine(result);
            }
        }

        [CommandDescription("Compile xaml and code files into an assembly")]
        [ParameterDescription("config", true, "Path to the configuration file, which contains all information for building")]
        private static string Build(string commandName, CommandShellParameterCollection parameter)
        {
            Console.WriteLine("Start build process...");

            // Read project file
            string projectFile = parameter.GetParameterValueAsString("config");
            string projectDirectory = "";

            #region [Open project file]
            if (!File.Exists(projectFile))
            {
                using (var _consoleColor = new ConsoleColorChanger(ConsoleColor.Red))
                {
                    Console.WriteLine("Could not find project: {0}", projectFile);
                }
                return "";
            }
            else
            {
                projectDirectory = string.Format("{0}{1}", Path.GetFullPath(Path.GetDirectoryName(projectFile)), "\\");
                Console.WriteLine("Set build directory to: {0}", projectDirectory);
            }

            ProjectConfiguration configuration = null;

            try
            {
                configuration = JsonConvert.DeserializeObject<ProjectConfiguration>(File.ReadAllText(projectFile));
            }
            catch (Exception ex)
            {
                using (var _consoleColor = new ConsoleColorChanger(ConsoleColor.Red))
                {
                    Console.WriteLine("Could not read project file: {0}", ex.Message);
                    return "";
                }
            }
            #endregion

            #region [Prepare Project]
            if (!Path.IsPathRooted(configuration.Output))
            {
                configuration.Output = Path.GetFullPath(Path.Combine(projectDirectory, configuration.Output));
            }

            if (string.IsNullOrWhiteSpace(configuration.Assembly))
            {
                using (var _consoleColor = new ConsoleColorChanger(ConsoleColor.Red))
                {
                    Console.WriteLine("Assembly name not set: {0}", configuration.Assembly);
                    return "";
                }
            }

            Console.WriteLine("Compilation output: {0}", configuration.Output);

            Console.WriteLine("Xaml files");
            foreach (var xaml in configuration.Xaml)
            {
                string path = Path.Combine(projectDirectory, xaml);
                Console.WriteLine(path);
                if (!File.Exists(path))
                {
                    using (var _consoleColor = new ConsoleColorChanger(ConsoleColor.Red))
                    {
                        Console.WriteLine("Could not find xaml file: {0}", path);
                    }
                    return "";
                }
            }
            Console.WriteLine("...");

            Console.WriteLine("ViewModel files");
            foreach (var vm in configuration.ViewModels)
            {
                string path = Path.Combine(projectDirectory, vm);
                Console.WriteLine(Path.Combine(projectDirectory, vm));
                if (!File.Exists(path))
                {
                    using (var _consoleColor = new ConsoleColorChanger(ConsoleColor.Red))
                    {
                        Console.WriteLine("Could not find xaml file: {0}", path);
                    }
                    return "";
                }
            }
            Console.WriteLine("...");

            Console.WriteLine("Ressource files");
            foreach (var res in configuration.Ressources)
            {
                string path = Path.Combine(projectDirectory, res);
                Console.WriteLine(Path.Combine(projectDirectory, res));
                if (!File.Exists(path))
                {
                    using (var _consoleColor = new ConsoleColorChanger(ConsoleColor.Red))
                    {
                        Console.WriteLine("Could not find xaml file: {0}", path);
                    }
                    return "";
                }
            }
            Console.WriteLine("...");
            #endregion

            CXUIBuilder builder = new CXUIBuilder();
            builder.AssemblyName = configuration.Assembly;
            builder.RootNamespace = configuration.RootNamespace;
            builder.ProjectRoot = projectDirectory;

            // Set the minimum of required assemblies
            // Set references
            builder.References = new[]
            {
                typeof(object).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Uri).Assembly,
                typeof(System.Xml.XmlAttribute).Assembly,
                typeof(System.Xaml.XamlLanguage).Assembly,
                typeof(System.Windows.Point).Assembly,
                typeof(System.Windows.Application).Assembly,
                typeof(System.Windows.Ink.ApplicationGesture).Assembly,
                typeof(System.Xaml.XamlSchemaContext).Assembly
            };

            // Create xaml building task
            var xamlTask = new XamlBuildTaskPass1();

            foreach (var xaml in configuration.Xaml)
            {
                string path = Path.Combine(projectDirectory, xaml);
                xamlTask.AddXamlSourceFromFile(path);
            }

            // Add xaml task
            builder.Tasks.Add(xamlTask);

            // Compile View-Models
            var jsonViewModelTask = new JsonViewModelBuildTask();
            foreach (var file in configuration.ViewModels)
            {
                string path = Path.Combine(projectDirectory, file);
                jsonViewModelTask.ViewModelFiles.Add(path);
            }
            builder.Tasks.Add(jsonViewModelTask);

            if (configuration.UseAutogenerateViewModelBase)
            {
                // Add viewmodel task
                builder.Tasks.Add(new BuildViewModelBaseTask());
            }

            // Add assembly build task

            var compileTask = new BuildAssemblyTask();
            
            compileTask.OutputDirectory = configuration.Output;
            compileTask.WriteToFileSystem = true;

            builder.Tasks.Add(compileTask);

            builder.Build();

            return "";
        }
    }
}
