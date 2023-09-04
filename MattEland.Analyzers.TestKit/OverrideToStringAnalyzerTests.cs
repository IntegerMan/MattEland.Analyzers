using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynTestKit;

namespace MattEland.Analyzers.TestKit {
    public class OverrideToStringAnalyzerTests : AnalyzerTestFixture {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new OverrideToStringAnalyzer();

        public const string BadCode = @"
using System;
namespace MattEland.Analyzers.AnalyzeMe 
{
    public class [|Program|]
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }
    }
}";

        public const string GoodCode = @"
using System;
namespace MattEland.Analyzers.AnalyzeMe 
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}";

        [Fact]
        public void DiagnosticShouldBePresentInBadCode() {
            HasDiagnostic(BadCode, OverrideToStringAnalyzer.DiagnosticId);
        }

        [Fact]
        public void DiagnosticShouldBeAbsentInGoodCode() {
            NoDiagnostic(GoodCode, OverrideToStringAnalyzer.DiagnosticId);
        }
    }
}