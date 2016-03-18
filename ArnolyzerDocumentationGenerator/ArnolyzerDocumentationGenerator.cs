﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Arnolyzer.RuleExceptionAttributes;
using Arnolyzer.SyntacticAnalyzers;
using static System.Environment;

namespace ArnolyzerDocumentationGenerator
{
    public static class ArnolyzerDocumentationGenerator
    {
        private static void Main()
        {
            var template = File.ReadAllText(@"..\..\..\Arnolyzer.Analyzers\DocumentationTemplates\AnalyzerTemplate.md");
            var type = typeof(IAnalyzerDetailsReporter);
            foreach (var analyzer in Assembly.Load(new AssemblyName("ArnolyzerAnalyzers")).DefinedTypes
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface))
            {
                var analyzerName = analyzer.Name;
                Console.WriteLine($"Generating {analyzerName}.md");

                var details = ((IAnalyzerDetailsReporter)Activator.CreateInstance(analyzer)).GetAnalyzerDetails();
                var extraWords = CreateExtraWordsSet(analyzerName);

                var processedContents = template
                    .Replace("%NAME%", details.Name)
                    .Replace("%CODE%", details.Code)
                    .Replace("%ID%", details.DiagnosticId)
                    .Replace("%DESCRIPTION%", details.Description.ToString())
                    .Replace("%CATEGORY%", details.Category)
                    .Replace("%CAUSE-WORDS%", extraWords.Cause)
                    .Replace("%PRE-CODEFIX-WORDS%", extraWords.PreCodeFix)
                    .Replace("%POST-CODEFIX-WORDS%", extraWords.PostCodeFix)
                    .Replace("%PRE-SUPPRESSION-WORDS%", extraWords.PreSuppression)
                    .Replace("%POST-SUPPRESSION-WORDS%", extraWords.PostSuppression)
                    .Replace("%CODEFIXES%", "There currently aren't any implemented code-fixes for this rule.")
                    .Replace("%SUPPRESSIONS%", GenerateSuppressionMessages(details.SuppressionAttributes));

                File.WriteAllText($@"..\..\..\..\Arnolyzer.wiki\{analyzerName}.md",
                                  processedContents);
            }
        }

        private static WordSectionContents CreateExtraWordsSet(string analyzerName) => 
            new WordSectionContents(FileContentsIfExists($"{analyzerName}.cause.txt"),
                                    FileContentsIfExists($"{analyzerName}.preCodeFix.txt"),
                                    FileContentsIfExists($"{analyzerName}.postCodeFix.txt"),
                                    FileContentsIfExists($"{analyzerName}.preSuppression.txt"),
                                    FileContentsIfExists($"{analyzerName}.postSuppression.txt"));

        private static string FileContentsIfExists(string fileName)
        {
            try
            {
                return File.ReadAllText($@"..\..\..\Arnolyzer.Analyzers\DocumentationTemplates\{fileName}");
            }
            catch (FileNotFoundException)
            {
                return "";
            }
        }

        private static string GenerateSuppressionMessages(IList<Type> suppressionAttributes) => 
            suppressionAttributes.Any()
                ? $"This rule can be suppressed using the following attributes: {DescribeEachAttribute(suppressionAttributes)}"
                : "";

        private static string DescribeEachAttribute(IEnumerable<Type> suppressionAttributes) => 
            suppressionAttributes.Aggregate("", (previous, attribute) => $"{previous}{NewLine}{NewLine}{FormatAttribute(attribute)}");

        private static string FormatAttribute(Type attribute)
        {
            var instance = (IAttributeDescriber) Activator.CreateInstance(attribute);
            var name = attribute.Name.Replace("Attribute", "");
            var description = instance.AttributeDescription.Replace("Attribute", " attribute");
            return $"**[{name}]**<br/>{NewLine}{description}";
        }

        private class WordSectionContents
        {
            public WordSectionContents(string cause, string preCodeFix, string postCodeFix, string preSuppression, string postSuppression)
            {
                Cause = cause;
                PreCodeFix = preCodeFix;
                PostCodeFix = postCodeFix;
                PreSuppression = preSuppression;
                PostSuppression = postSuppression;
            }

            internal string Cause { get; }
            internal string PreCodeFix { get; }
            internal string PostCodeFix { get; }
            internal string PreSuppression { get; }
            internal string PostSuppression { get; }
        }
    }
}