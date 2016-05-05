using Simplic.CommandShell;
using System;
using System.Collections.Generic;
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

            return "";
        }
    }
}
