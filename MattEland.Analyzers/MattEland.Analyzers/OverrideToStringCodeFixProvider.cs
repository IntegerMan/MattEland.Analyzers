﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace MattEland.Analyzers {
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OverrideToStringCodeFixProvider)), Shared]
    public class OverrideToStringCodeFixProvider : CodeFixProvider {
        public sealed override ImmutableArray<string> FixableDiagnosticIds {
            get { return ImmutableArray.Create(OverrideToStringAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            TypeDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Override ToString",
                    createChangedDocument: c => FixAsync(context.Document, declaration),
                    equivalenceKey: OverrideToStringAnalyzer.DiagnosticId),
                diagnostic);
        }

        private Task<Document> FixAsync(Document document, TypeDeclarationSyntax typeDecl) {
            // Add a new override of ToString that throws a NotImplementedException
            MethodDeclarationSyntax newMethod = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                SyntaxFactory.Identifier("ToString"))
                .WithModifiers(SyntaxFactory.TokenList(new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.OverrideKeyword) }))
                .WithBody(SyntaxFactory.Block(
                    SyntaxFactory.ThrowStatement(SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName("NotImplementedException"))
                    .WithArgumentList(SyntaxFactory.ArgumentList()))));

            // Mutate the existing type declaration with the new method, then replace the Type in a copy of the document and then return that document
            TypeDeclarationSyntax updatedTypeDecl = typeDecl.AddMembers(newMethod);
            Document updatedDoc = document.WithSyntaxRoot(typeDecl.SyntaxTree.GetRoot().ReplaceNode(typeDecl, updatedTypeDecl));

            return Task.FromResult(updatedDoc);
        }
    }
}
