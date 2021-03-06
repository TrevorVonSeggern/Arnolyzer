﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Arnolyzer.RuleExceptionAttributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Arnolyzer.Analyzers.CommonFunctions;

namespace Arnolyzer.Analyzers.EncapsulationAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AA1102InnerTypesMustBePrivateAnalyzer : DiagnosticAnalyzer, IAnalyzerDetailsReporter
    {
        private static readonly IList<Type> SuppressionAttributes = new List<Type>();

        private static readonly AnalyzerDetails AA1102Details =
            new AnalyzerDetails(nameof(AA1102InnerTypesMustBePrivateAnalyzer),
                                AnalyzerCategories.EncapsulationAnalyzers,
                                DefaultState.EnabledByDefault,
                                DiagnosticSeverity.Error,
                                nameof(Resources.AA1102InnerTypesMustBePrivateTitle),
                                nameof(Resources.AA1102InnerTypesMustBePrivateDescription),
                                nameof(Resources.AA1102InnerTypesMustBePrivateMessageFormat),
                                SuppressionAttributes);

        public AnalyzerDetails GetAnalyzerDetails() => AA1102Details;

        private static readonly DiagnosticDescriptor Rule = AA1102Details.GetDiagnosticDescriptor();

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
            => context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);

        [HasSideEffects]
        private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var syntaxRoot = context.Tree.GetRoot(context.CancellationToken);
            var outerTypeDeclarations =
                syntaxRoot.DescendantNodes(DoNotDescendIntoTypeDeclarations).Where(NodeIsTypeDeclaration).ToList();

            var innerTypeDeclarations = (from node in outerTypeDeclarations
                                         from innerNode in node.DescendantNodes().Where(NodeIsTypeDeclaration)
                                         select innerNode).Cast<BaseTypeDeclarationSyntax>();

            foreach (var identifier in 
                     from typeDeclaration in innerTypeDeclarations
                     where typeDeclaration.Modifiers.Count(t => t.Kind() == SyntaxKind.InternalKeyword ||
                                                                t.Kind() == SyntaxKind.PublicKeyword) > 0
                     select typeDeclaration.Identifier)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, identifier.GetLocation(), identifier.Text));
            }
        }

        private static bool DoNotDescendIntoTypeDeclarations(SyntaxNode node)
        {
            var kind = node?.Kind();
            return kind != SyntaxKind.ClassDeclaration &&
                   kind != SyntaxKind.StructDeclaration;
        }
    }
}