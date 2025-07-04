﻿//---------------------------------------------------------------------
// <copyright file="SyntacticTreeVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    using System;
    using Microsoft.OData.UriParser.Aggregation;

    /// <summary>
    /// Visitor interface for walking the Syntactic Tree.
    /// </summary>
    /// <typeparam name="T">Generic type produced by the visitor.</typeparam>
    internal abstract class SyntacticTreeVisitor<T> : ISyntacticTreeVisitor<T>
    {
        /// <summary>
        /// Visit an AllToken
        /// </summary>
        /// <param name="tokenIn">The All token to visit</param>
        /// <returns>An AllNode bound to this token</returns>
        public virtual T Visit(AllToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits an AnyToken
        /// </summary>
        /// <param name="tokenIn">The Any token to visit</param>
        /// <returns>An AnyNode that's bound to this token</returns>
        public virtual T Visit(AnyToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a BinaryOperatorToken
        /// </summary>
        /// <param name="tokenIn">The Binary operator token to visit.</param>
        /// <returns>A BinaryOperatorNode that's bound to this token</returns>
        public virtual T Visit(BinaryOperatorToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a CountSegmentToken
        /// </summary>
        /// <param name="tokenIn">The Count segment token to visit.</param>
        /// <returns>A CountNode that's bound to this token</returns>
        public virtual T Visit(CountSegmentToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits an InToken
        /// </summary>
        /// <param name="tokenIn">The In token to visit.</param>
        /// <returns>An InNode that's bound to this token</returns>
        public virtual T Visit(InToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a DottedIdentifierToken
        /// </summary>
        /// <param name="tokenIn">The DottedIdentifierToken to visit</param>
        /// <returns>Either a SingleResourceCastNode, or CollectionResourceCastNode bound to this DottedIdentifierToken</returns>
        public virtual T Visit(DottedIdentifierToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits an ExpandToken
        /// </summary>
        /// <param name="tokenIn">The ExpandToken to visit</param>
        /// <returns>A QueryNode bound to this ExpandToken</returns>
        public virtual T Visit(ExpandToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits an ExpandTermToken
        /// </summary>
        /// <param name="tokenIn">The ExpandTermToken to visit</param>
        /// <returns>A QueryNode bound to this ExpandTermToken</returns>
        public virtual T Visit(ExpandTermToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a FunctionCallToken
        /// </summary>
        /// <param name="tokenIn">The FunctionCallToken to visit</param>
        /// <returns>A SingleValueFunctionCallNode bound to this FunctionCallToken</returns>
        public virtual T Visit(FunctionCallToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a LiteralToken
        /// </summary>
        /// <param name="tokenIn">The LiteralToken to visit</param>
        /// <returns>A ConstantNode bound to this LambdaToken</returns>
        public virtual T Visit(LiteralToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits an AggregateToken
        /// </summary>
        /// <param name="tokenIn">The AggregateToken to visit</param>
        /// <returns>A T bound to this AggregateToken</returns>
        public virtual T Visit(AggregateToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a GroupByToken
        /// </summary>
        /// <param name="tokenIn">The GroupByToken to visit</param>
        /// <returns>A T bound to this GroupByToken</returns>
        public virtual T Visit(GroupByToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits an AggregateExpressionToken
        /// </summary>
        /// <param name="tokenIn">The AggregateExpressionToken to visit</param>
        /// <returns>A T bound to this AggregateExpressionToken</returns>
        public virtual T Visit(AggregateExpressionToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits an EntitySetAggregateToken
        /// </summary>
        /// <param name="tokenIn">The EntitySetAggregateToken to visit</param>
        /// <returns>A T bound to this EntitySetAggregateToken</returns>
        public virtual T Visit(EntitySetAggregateToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a LambdaToken
        /// </summary>
        /// <param name="tokenIn">The LambdaToken to visit</param>
        /// <returns>A LambdaNode bound to this LambdaToken</returns>
        public virtual T Visit(LambdaToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a InnerPathToken
        /// </summary>
        /// <param name="tokenIn">The InnerPathToken to bind</param>
        /// <returns>A SingleValueNode or SingleEntityNode bound to this InnerPathToken</returns>
        public virtual T Visit(InnerPathToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits an OrderByToken
        /// </summary>
        /// <param name="tokenIn">The OrderByToken to bind</param>
        /// <returns>An OrderByClause bound to this OrderByToken</returns>
        public virtual T Visit(OrderByToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits an EndPathToken
        /// </summary>
        /// <param name="tokenIn">The EndPathToken to bind</param>
        /// <returns>A PropertyAccessClause bound to this EndPathToken</returns>
        public virtual T Visit(EndPathToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a CustomQueryOptionToken
        /// </summary>
        /// <param name="tokenIn">The CustomQueryOptionToken to bind</param>
        /// <returns>A CustomQueryOptionNode bound to this CustomQueryOptionToken</returns>
        public virtual T Visit(CustomQueryOptionToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a RangeVariableToken
        /// </summary>
        /// <param name="tokenIn">The RangeVariableToken to bind</param>
        /// <returns>A Resource or NonResource RangeVariableReferenceNode bound to this RangeVariableToken</returns>
        public virtual T Visit(RangeVariableToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a SelectToken
        /// </summary>
        /// <param name="tokenIn">The SelectToken to bind</param>
        /// <returns>A QueryNode bound to this SelectToken</returns>
        public virtual T Visit(SelectToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits an SelectTermToken
        /// </summary>
        /// <param name="tokenIn">The SelectTermToken to visit</param>
        /// <returns>A QueryNode bound to this SelectTermToken</returns>
        public virtual T Visit(SelectTermToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a StarToken
        /// </summary>
        /// <param name="tokenIn">The StarToken to bind</param>
        /// <returns>A QueryNode bound to this StarToken</returns>
        public virtual T Visit(StarToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a UnaryOperatorToken
        /// </summary>
        /// <param name="tokenIn">The UnaryOperatorToken to bind</param>
        /// <returns>A UnaryOperatorNode bound to this UnaryOperatorToken</returns>
        public virtual T Visit(UnaryOperatorToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a FunctionParameterToken
        /// </summary>
        /// <param name="tokenIn">The FunctionParameterToken to bind</param>
        /// <returns>A user defined value</returns>
        public virtual T Visit(FunctionParameterToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a ComputeToken
        /// </summary>
        /// <param name="tokenIn">The ComputeToken to bind</param>
        /// <returns>A user defined value</returns>
        public virtual T Visit(ComputeToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a ComputeExpressionToken
        /// </summary>
        /// <param name="tokenIn">The ComputeExpressionToken to bind</param>
        /// <returns>A user defined value</returns>
        public virtual T Visit(ComputeExpressionToken tokenIn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Visits a RootPathToken
        /// </summary>
        /// <param name="tokenIn">The RootPathToken to bind</param>
        /// <returns>A user defined value</returns>
        public virtual T Visit(RootPathToken tokenIn)
        {
            throw new NotImplementedException();
        }
    }
}
