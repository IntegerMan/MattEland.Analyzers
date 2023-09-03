using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = MattEland.Analyzers.Test.CSharpCodeFixVerifier<
    MattEland.Analyzers.OverrideToStringAnalyzer,
    MattEland.Analyzers.OverrideToStringCodeFixProvider>;

namespace MattEland.Analyzers.Test {
    [TestClass]
    public class OverrideToStringTests {
        private const string badCode = @"
using System;
namespace MattEland.Analyzers.AnalyzeMe 
{
    public class {|#0:Program|} 
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }
    }
}";

        private const string goodCode = @"
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

        [TestMethod]
        public async Task DiagnosticShouldNotTriggerOnGoodCode() {
            await VerifyCS.VerifyAnalyzerAsync(goodCode);
        }

        [TestMethod]
        public async Task DiagnosticShouldTriggerOnBadCode() {
            var expected = VerifyCS.Diagnostic(OverrideToStringAnalyzer.Rule).WithLocation(0).WithArguments("Program");
            await VerifyCS.VerifyAnalyzerAsync(badCode, expected);
        }

        [TestMethod]
        public async Task CodeFixShouldMoveFromBadCodeToGoodCode() {
            var expected = VerifyCS.Diagnostic(OverrideToStringAnalyzer.Rule).WithLocation(0).WithArguments("Program");
            await VerifyCS.VerifyCodeFixAsync(badCode, expected, goodCode);
        }
    }
}
