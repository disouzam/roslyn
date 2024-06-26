﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Roslyn.LanguageServer.Protocol
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Class which represents workspace capabilities.
    ///
    /// See the <see href="https://microsoft.github.io/language-server-protocol/specifications/specification-current/#clientCapabilities">Language Server Protocol specification</see> for additional information.
    /// </summary>
    internal class WorkspaceClientCapabilities
    {
        /// <summary>
        /// Gets or sets a value indicating whether apply edit is supported.
        /// </summary>
        [JsonPropertyName("applyEdit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool ApplyEdit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the workspace edit setting.
        /// </summary>
        [JsonPropertyName("workspaceEdit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public WorkspaceEditSetting? WorkspaceEdit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the setting which determines if did change configuration can be dynamically registered.
        /// </summary>
        [JsonPropertyName("didChangeConfiguration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DynamicRegistrationSetting? DidChangeConfiguration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the setting which determines if did change watched files can be dynamically registered.
        /// </summary>
        [JsonPropertyName("didChangeWatchedFiles")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DynamicRegistrationSetting? DidChangeWatchedFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the setting which determines if symbols can be dynamically registered.
        /// </summary>
        [JsonPropertyName("symbol")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SymbolSetting? Symbol
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the setting which determines if execute command can be dynamically registered.
        /// </summary>
        [JsonPropertyName("executeCommand")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DynamicRegistrationSetting? ExecuteCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets capabilities specific to the semantic token requests scoped to the workspace.
        /// </summary>
        [JsonPropertyName("semanticTokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SemanticTokensWorkspaceSetting? SemanticTokens
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets capabilities indicating what support the client has for workspace pull diagnostics.
        /// </summary>
        [JsonPropertyName("diagnostics")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DiagnosticWorkspaceSetting? Diagnostics
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the capabilities if client support 'workspace/configuration' requests.
        /// </summary>
        [JsonPropertyName("configuration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Configuration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets of sets capabilities specific to the inlay hint requests scoped to the workspace.
        /// </summary>
        [JsonPropertyName("inlayHint")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InlayHintWorkspaceSetting? InlayHint
        {
            get;
            set;
        }

        /// <summary>
        /// Gets of sets capabilities specific to the code lens requests scoped to the workspace.
        /// </summary>
        [JsonPropertyName("codeLens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CodeLensWorkspaceSetting? CodeLens
        {
            get;
            set;
        }
    }
}
