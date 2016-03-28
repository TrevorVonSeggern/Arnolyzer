using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Arnolyzer.Analyzers.Settings;
using Arnolyzer.RuleExceptionAttributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Arnolyzer.Analyzers.PureFunctionAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AA1001StaticMethodsShouldHaveAtLeastOneParameterAnalyzer : DiagnosticAnalyzer, IAnalyzerDetailsReporter
    {
        private static readonly IList<Type> SuppressionAttributes = new List<Type> { typeof(HasSideEffectsAttribute) };

        private static readonly AnalyzerDetails AA1001Details =
            new AnalyzerDetails(nameof(AA1001StaticMethodsShouldHaveAtLeastOneParameterAnalyzer),
                                AnalyzerCategories.PureFunctionAnalyzers,
                                DefaultState.EnabledByDefault,
                                DiagnosticSeverity.Error,
                                nameof(Resources.AA1001StaticMethodsShouldHaveAtLeastOneParameterTitle),
                                nameof(Resources.AA1001StaticMethodsShouldHaveAtLeastOneParameterDescription),
                                nameof(Resources.AA1001StaticMethodsShouldHaveAtLeastOneParameterMessageFormat),
                                SuppressionAttributes);

        public AnalyzerDetails GetAnalyzerDetails() => AA1001Details;

        private static readonly DiagnosticDescriptor Rule = AA1001Details.GetDiagnosticDescriptor();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStart);
        }

        [HasSideEffects]
        private static void CompilationStart(CompilationStartAnalysisContext context)
        {
            context.Options.InitialiseArnolyzerSettings();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
        }

        [MutatesParameter]
        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;

            if (!CommonFunctions.AutoGenerated(methodSymbol) &&
                !CommonFunctions.HasIgnoreRuleAttribute(methodSymbol, SuppressionAttributes) &&
                methodSymbol.IsStatic &&
                methodSymbol.Parameters.IsEmpty &&
                methodSymbol.MethodKind != MethodKind.PropertyGet)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, methodSymbol.Locations[0], methodSymbol.Name));
            }
        }
    }
}