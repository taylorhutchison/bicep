// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public interface INamedDeclaration
    {
        string Name { get; }

        SyntaxBase DeclaringSyntax { get; }
    }
}
