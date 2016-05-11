﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simplic.CXUI.BuildTask
{
    public class BuildCodeBehindTask : BuildTaskBase
    {
        /// <summary>
        /// Build autogenerated codebehind files for all controls, that does not have any code behind file yet.
        /// </summary>
        /// <returns>True if creating was successfull</returns>
        public override bool Execute()
        {
            foreach (var generated in CXUIBuildEngine.GeneratedFiles.Where(item => item.Name.Contains(".g")))
            {
                var cb = GetCodeBehind(generated.AbsolutePath);
            }

            return base.Execute();
        }

        /// <summary>
        /// Get the complete namespace by the starting namespace symbol
        /// </summary>
        /// <param name="namespaceSymbol">Symbol instance</param>
        /// <returns>Empty string if no namespace exists, else the full path. Your.Namespace.Whatever</returns>
        public string GetFullNamespace(INamespaceSymbol namespaceSymbol)
        {
            if (namespaceSymbol == null)
            {
                return "";
            }

            INamespaceSymbol nsSymbol = namespaceSymbol;
            StringBuilder fullNamespace = new StringBuilder();

            // Go over the sematic tree of namespaces and add them all
            while (nsSymbol != null && !string.IsNullOrWhiteSpace(nsSymbol.Name))
            {
                if (fullNamespace.Length > 0)
                {
                    fullNamespace.Insert(0, ".");
                }

                // Add namespace
                fullNamespace.Insert(0, nsSymbol.Name);
                nsSymbol = nsSymbol.ContainingNamespace;
            }

            return fullNamespace.ToString();
        }

        /// <summary>
        /// Generate a code behind file which bases on (YourXaml.g.cs) using roslyn
        /// </summary>
        /// <param name="xamlGeneratedCSFile">*.g.cs file as base</param>
        /// <returns>Empty string if no valid file was passed, else a valid code behind file</returns>
        public string GetCodeBehind(string xamlGeneratedCSFile)
        {
            var _st = CSharpSyntaxTree.ParseText(File.ReadAllText(xamlGeneratedCSFile));

            // Set references (Assemblies)
            List<MetadataReference> references = CXUIBuildEngine.References.Select(item => MetadataReference.CreateFromFile(item.Location)).ToList<MetadataReference>();

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!CXUIBuildEngine.References.Contains(asm) && !string.IsNullOrWhiteSpace(asm.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(asm.Location));
                }
            }

            var compilation = CSharpCompilation.Create("__detect_compile__",
                syntaxTrees: new[] { _st }, references: references.ToArray());

            // Get sematic model to find base-type
            var model = compilation.GetSemanticModel(_st);

            // Get class declaration
            var classDecl = _st.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            if (classDecl != null)
            {
                var classSymbol = model.GetDeclaredSymbol(classDecl);

                // Get the final base type which is important here
                string baseNamespace = GetFullNamespace(classSymbol.BaseType.ContainingNamespace);
                string rootNamespace = GetFullNamespace(classSymbol.ContainingNamespace);
                string baseClass = classSymbol.BaseType.Name;

                var properties = new Dictionary<string, string>();
                properties.Add("Class", classSymbol.Name);
                properties.Add("Namespace", rootNamespace);
                properties.Add("BaseClass", baseClass);
                properties.Add("BaseClassNamespace", baseNamespace);

                return TemplateHelper.GetTemplate(TemplateHelper.XAML_CODE_BEHIND_TEMPLATE, properties);
            }

            return "";
        }
    }
}
