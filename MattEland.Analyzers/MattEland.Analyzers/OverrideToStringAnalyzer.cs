using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace MattEland.Analyzers {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OverrideToStringAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "ME1001";
        public const string Title = "ToString should be overridden";
        public const string MessageFormat = "ToString should be overridden";
        public const string Description = "Overriding the ToString method can help in debugging applications and logging.";
        public const string Category = "Maintainability";
        public const DiagnosticSeverity DefaultSeverity = DiagnosticSeverity.Info;

        public static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DefaultSeverity, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
            => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context) {
            var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

            // Check if the type overrides ToString()
            if (!namedTypeSymbol.GetMembers()
                             .OfType<IMethodSymbol>()
                             .Any(m => m.Name == "ToString" && m.IsOverride && m.Parameters.Length == 0)) {
                var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
