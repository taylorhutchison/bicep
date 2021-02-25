// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class OutputDeclaration : INamedDeclaration
    {
        private readonly ISymbolContext context;

        public OutputDeclaration(ISymbolContext context, OutputDeclarationSyntax declaringOutput)
        {
            this.context = context;
            this.Syntax = declaringOutput;
        }

        public OutputDeclarationSyntax Syntax { get; }

        public string Name => Syntax.Name.IdentifierName;

        public SyntaxBase DeclaringSyntax => Syntax;

        public TypeSymbol GetTypeSymbol()
            => context.TypeManager.GetTypeInfo(Syntax);
    }
}