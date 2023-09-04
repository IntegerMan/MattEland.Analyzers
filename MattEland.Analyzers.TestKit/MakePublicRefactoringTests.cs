using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using RoslynTestKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MattEland.Analyzers.TestKit {
    public class MakePublicRefactoringTests : CodeRefactoringTestFixture {
        protected override string LanguageName => LanguageNames.CSharp;

        protected override CodeRefactoringProvider CreateProvider() => new MakePublicRefactoring();

        protected override IReadOnlyCollection<MetadataReference> References => new[]
        {
            ReferenceSource.NetStandard2_0,
            ReferenceSource.FromAssembly(Assembly.Load("System.Runtime, Version=7.0.0.0").Location),
        };

        private const string badCode = @"
using System;
namespace MattEland.Analyzers.AnalyzeMe 
{
    [|internal|] class Program
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

        [Fact]
        public void RefactoringTransformsCodeCorrectly() 
        { 
            TestCodeRefactoring(badCode, goodCode);
        }
    }
}
