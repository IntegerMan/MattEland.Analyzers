using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = MattEland.Analyzers.Test.CSharpCodeRefactoringVerifier<MattEland.Analyzers.MakePublicRefactoring>;

namespace MattEland.Analyzers.Test {
    [TestClass]
    public class MakePublicTests {
        private const string badCode = @"
using System;
namespace MattEland.Analyzers.AnalyzeMe 
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}";

        private const string goodCode = @"
using System;
namespace MattEland.Analyzers.AnalyzeMe 
{
    public class Program 
    {
        public static void Main(string[] args)
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
        public async Task RefactoringShouldMakeProgramPublic() {
            await VerifyCS.VerifyRefactoringAsync(badCode, goodCode);
        }
    }
}
