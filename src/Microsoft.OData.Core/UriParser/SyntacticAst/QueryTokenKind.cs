﻿//---------------------------------------------------------------------
// <copyright file="QueryTokenKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    /// <summary>
    /// Enumeration of kinds of query tokens.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum QueryTokenKind
    {
        /// <summary>
        /// The binary operator.
        /// </summary>
        BinaryOperator = 3,

        /// <summary>
        /// The unary operator.
        /// </summary>
        UnaryOperator = 4,

        /// <summary>
        /// The literal value.
        /// </summary>
        Literal = 5,

        /// <summary>
        /// The function call.
        /// </summary>
        FunctionCall = 6,

        /// <summary>
        /// The property access.
        /// </summary>
        EndPath = 7,

        /// <summary>
        /// The order by operation.
        /// </summary>
        OrderBy = 8,

        /// <summary>
        /// A query option.
        /// </summary>
        CustomQueryOption = 9,

        /// <summary>
        /// The Select query.
        /// </summary>
        Select = 10,

        /// <summary>
        /// The *.
        /// </summary>
        Star = 11,

        /// <summary>
        /// The Expand query.
        /// </summary>
        Expand = 13,

        /// <summary>
        /// Type segment.
        /// </summary>
        TypeSegment = 14,

        /// <summary>
        /// Any query.
        /// </summary>
        Any = 15,

        /// <summary>
        /// Non root segment.
        /// </summary>
        InnerPath = 16,

        /// <summary>
        /// type segment.
        /// </summary>
        DottedIdentifier = 17,

        /// <summary>
        /// Parameter token.
        /// </summary>
        RangeVariable = 18,

        /// <summary>
        /// All query.
        /// </summary>
        All = 19,

        /// <summary>
        /// ExpandTerm Token
        /// </summary>
        ExpandTerm = 20,

        /// <summary>
        /// FunctionParameterToken
        /// </summary>
        FunctionParameter = 21,

        /// <summary>
        /// FunctionParameterAlias
        /// </summary>
        FunctionParameterAlias = 22,

        /// <summary>
        /// the string literal for search query
        /// </summary>
        StringLiteral = 23,

        /// <summary>
        /// $apply aggregate token
        /// </summary>
        Aggregate = 24,

        /// <summary>
        /// $apply aggregate statement to a property token
        /// </summary>
        AggregateExpression = 25,

        /// <summary>
        /// $apply groupby token
        /// </summary>
        AggregateGroupBy = 26,

        /// <summary>
        /// $compute token
        /// </summary>
        Compute = 27,

        /// <summary>
        /// $compute expression token
        /// </summary>
        ComputeExpression = 28,

        /// <summary>
        /// $apply aggregate statement to a entity set token
        /// </summary>
        EntitySetAggregateExpression = 29,

        /// <summary>
        /// In operator.
        /// </summary>
        In = 30,

        /// <summary>
        /// SelectTerm Token
        /// </summary>
        SelectTerm = 31,

        /// <summary>
        /// $count segment
        /// </summary>
        CountSegment = 32,

        /// <summary>
        /// $root path
        /// </summary>
        RootPath = 33
    }
}