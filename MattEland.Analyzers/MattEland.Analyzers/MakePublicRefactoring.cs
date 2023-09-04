using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace MattEland.Analyzers {
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MakePublicRefactoring)), Shared]
    public class MakePublicRefactoring : CodeRefactoringProvider {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context) {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            SyntaxNode node = root.FindNode(context.Span);

            // Only offer a refactoring if the selected node is a type declaration node.
            var declaration = node as MemberDeclarationSyntax;
            if (declaration == null) {
                return;
            }

            // If it's already public, we can't do anything here
            if (declaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))) {
                return;
            }


            // For any type declaration node, create a code action to reverse the identifier text.
            var action = CodeAction.Create("Make Public", c => MakePublic(context.Document, declaration, c));

            // Register this code action.
            context.RegisterRefactoring(action);
        }

        private async Task<Document> MakePublic(Document document, MemberDeclarationSyntax declaration, CancellationToken cancellationToken) {
            SyntaxNode newTypeDecl = declaration.WithModifiers(new SyntaxTokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken) as CompilationUnitSyntax;
            SyntaxNode newRoot = root.ReplaceNode(declaration, newTypeDecl);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
