using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynTestKit;

namespace MattEland.Analyzers.TestKit {
    public class OverrideToStringCodeFixTests : CodeFixTestFixture {
        protected override string LanguageName => LanguageNames.CSharp;

        protected override CodeFixProvider CreateProvider() => new OverrideToStringCodeFixProvider();

        protected override IReadOnlyCollection<DiagnosticAnalyzer> CreateAdditionalAnalyzers() => new[] { new OverrideToStringAnalyzer() };

        [Fact]
        public void CodeFixMovesFromBadCodeToGoodCode() {
            TestCodeFix(OverrideToStringAnalyzerTests.BadCode, OverrideToStringAnalyzerTests.GoodCode, OverrideToStringAnalyzer.DiagnosticId);
        }

    }
}