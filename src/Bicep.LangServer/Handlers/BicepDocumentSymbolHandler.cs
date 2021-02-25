// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Semantics;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Utils;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SymbolKind = OmniSharp.Extensions.LanguageServer.Protocol.Models.SymbolKind;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDocumentSymbolHandler: DocumentSymbolHandler
    {
        private readonly ILogger<BicepDocumentSymbolHandler> logger;
        private readonly ICompilationManager compilationManager;

        public BicepDocumentSymbolHandler(ILogger<BicepDocumentSymbolHandler> logger, ICompilationManager compilationManager)
            : base(GetSymbolRegistrationOptions())
        {
            this.logger = logger;
            this.compilationManager = compilationManager;
        }

        public override Task<SymbolInformationOrDocumentSymbolContainer> Handle(DocumentSymbolParams request, CancellationToken cancellationToken)
        {
            CompilationContext? context = this.compilationManager.GetCompilation(request.TextDocument.Uri);
            if (context == null)
            {
                // we have not yet compiled this document, which shouldn't really happen
                this.logger.LogError("Document symbol request arrived before file {Uri} could be compiled.", request.TextDocument.Uri);

                return Task.FromResult(new SymbolInformationOrDocumentSymbolContainer(new SymbolInformationOrDocumentSymbol()));
            }

            return Task.FromResult(new SymbolInformationOrDocumentSymbolContainer(GetSymbols(context)));
        }

        private static DocumentSymbolRegistrationOptions GetSymbolRegistrationOptions()
        {
            return new DocumentSymbolRegistrationOptions
            {
                DocumentSelector = DocumentSelectorFactory.Create()
            };
        }

        private IEnumerable<SymbolInformationOrDocumentSymbol> GetSymbols(CompilationContext context)
        {
            return context.Compilation.GetEntrypointSemanticModel().Root.AllDeclarations
                .OrderBy(symbol=>symbol.DeclaringSyntax.Span.Position)
                .Select(symbol => new SymbolInformationOrDocumentSymbol(CreateDocumentSymbol(symbol, context.LineStarts)));
        }

        private static DocumentSymbol CreateDocumentSymbol(DeclaredSymbol symbol, ImmutableArray<int> lineStarts) =>
            new DocumentSymbol
            {
                Name = symbol.Name,
                Kind = SelectSymbolKind(symbol),
                Detail = FormatDetail(symbol),
                Range = symbol.DeclaringSyntax.ToRange(lineStarts),
                // use the name node span with fallback to entire declaration span
                SelectionRange = symbol.NameSyntax.ToRange(lineStarts)
            };

        private static SymbolKind SelectSymbolKind(INamedDeclaration declaration)
            => declaration switch
            {
                ParameterSymbol _ => SymbolKind.Field,
                VariableSymbol _ => SymbolKind.Variable,
                ResourceSymbol _ => SymbolKind.Object,
                ModuleSymbol _ => SymbolKind.Module,
                OutputDeclaration _ => SymbolKind.Interface,
                _ => SymbolKind.Key,
            };

        private static string FormatDetail(INamedDeclaration declaration)
            => declaration switch
            {
                ParameterSymbol parameter => parameter.Type.Name,
                VariableSymbol variable => variable.Type.Name,
                ResourceSymbol resource => resource.Type.Name,
                ModuleSymbol module => module.Type.Name,
                OutputDeclaration output => output.GetTypeSymbol().Name,
                _ => string.Empty,
            };
    }
}

