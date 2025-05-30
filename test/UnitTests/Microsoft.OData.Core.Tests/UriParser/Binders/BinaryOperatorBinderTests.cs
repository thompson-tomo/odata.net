﻿//---------------------------------------------------------------------
// <copyright file="BinaryOperatorBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using Microsoft.OData.Core;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    /// <summary>
    /// Unit tests for the BinaryOperatorBinder.
    /// </summary>
    public class BinaryOperatorBinderTests
    {
        private const string OpenPropertyName = "SomeProperty";
        private const string FuncName = "func";
        private readonly IEdmModel model = HardCodedTestModel.TestModel;
        private BinaryOperatorBinder binaryOperatorBinder;

        private SingleValueNode leftParameterSingleValueQueryNode;
        private SingleValueNode rightParameterSingleValueQueryNode;
        private QueryNode leftQueryNode;
        private QueryNode rightQueryNode;

        private bool shouldReturnLeft;

        public BinaryOperatorBinderTests()
        {
            this.shouldReturnLeft = true;
            this.binaryOperatorBinder = new BinaryOperatorBinder(this.BindMethodThatReturnsSingleValueQueryNode, ODataUriResolver.GetUriResolver(null));
        }

        [Fact]
        public void OrOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(false);
            this.rightParameterSingleValueQueryNode = new ConstantNode(true);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.Or, new LiteralToken(false), new LiteralToken(true));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Or);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode(false);
            binaryNode.Right.ShouldBeConstantQueryNode(true);
        }

        [Fact]
        public void AndOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(false);
            this.rightParameterSingleValueQueryNode = new SingleValueFunctionCallNode("func", null, EdmCoreModel.Instance.GetBoolean(false));
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.And, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode(false);
            binaryNode.Right.ShouldBeSingleValueFunctionCallQueryNode("func");
        }

        [Fact]
        public void AndOperatorCompatibleTypeShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new UnaryOperatorNode(UnaryOperatorKind.Negate, new UnaryOperatorNode(UnaryOperatorKind.Not, new ConstantNode(null)));
            this.rightParameterSingleValueQueryNode = new SingleValueFunctionCallNode("func", null, EdmCoreModel.Instance.GetBoolean(false));
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.And, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Boolean);
            binaryNode.Right.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Boolean);
        }

        [Fact]
        public void AndOperatorNullLiteralShouldResultInBinaryOperatorNodeWithConvert()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(null);
            this.rightParameterSingleValueQueryNode =
                new SingleValueFunctionCallNode(FuncName, null, EdmCoreModel.Instance.GetBoolean(false));

            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.And, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);

            Assert.True(binaryNode.TypeReference.IsNullable);

            var left = binaryNode.Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Boolean);
            Assert.True(left.TypeReference.IsNullable);
            left.Source.ShouldBeConstantQueryNode<object>(null);

            var right = binaryNode.Right.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Boolean);
            Assert.True(right.TypeReference.IsNullable);
            right.Source.ShouldBeSingleValueFunctionCallQueryNode(FuncName);
        }

        [Fact]
        public void AndOperatorTwoNullLiteralsShouldResultInBinaryOperatorNodeWithNullType()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(null);
            this.rightParameterSingleValueQueryNode = new ConstantNode(null);

            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.And, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);

            Assert.Null(binaryNode.TypeReference);
            binaryNode.Left.ShouldBeConstantQueryNode<object>(null);
            binaryNode.Right.ShouldBeConstantQueryNode<object>(null);
        }

        [Fact]
        public void AndOperatorOpenPropertyShouldResultInBinaryOperatorNodeWithConvert()
        {
            this.leftParameterSingleValueQueryNode = new SingleValueOpenPropertyAccessNode(new ConstantNode(null), OpenPropertyName);
            this.rightParameterSingleValueQueryNode =
                new SingleValueFunctionCallNode(FuncName, null, EdmCoreModel.Instance.GetBoolean(false));

            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.And, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);

            Assert.True(binaryNode.TypeReference.IsNullable);

            var left = binaryNode.Left.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Boolean);
            Assert.True(left.TypeReference.IsNullable);
            left.Source.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);

            var right = binaryNode.Right.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Boolean);
            Assert.True(right.TypeReference.IsNullable);
            right.Source.ShouldBeSingleValueFunctionCallQueryNode(FuncName);
        }

        [Fact]
        public void AndOperatorTwoOpenPropertiesShouldResultInBinaryOperatorNodeWithNullType()
        {
            this.leftParameterSingleValueQueryNode = new SingleValueOpenPropertyAccessNode(new ConstantNode(null), OpenPropertyName);
            this.rightParameterSingleValueQueryNode = new SingleValueOpenPropertyAccessNode(new ConstantNode(null), OpenPropertyName + "1");

            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.And, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);

            Assert.Null(binaryNode.TypeReference);
            binaryNode.Left.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
            binaryNode.Right.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName + "1");
        }

        [Fact]
        public void AndOperatorNullAndOpenPropertyShouldResultInBinaryOperatorNodeWithNullType()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(null);
            this.rightParameterSingleValueQueryNode = new SingleValueOpenPropertyAccessNode(new ConstantNode(null), OpenPropertyName);

            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.And, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);

            Assert.Null(binaryNode.TypeReference);
            binaryNode.Left.ShouldBeConstantQueryNode<object>(null);
            binaryNode.Right.ShouldBeSingleValueOpenPropertyAccessQueryNode(OpenPropertyName);
        }

        [Fact]
        public void EqualOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(2.5);
            this.rightParameterSingleValueQueryNode = new ConstantNode(2);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.Equal, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode(2.5);
            binaryNode.Right.ShouldBeConstantQueryNode(2d);
        }

        [Fact]
        public void NotEqualOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode("something");
            this.rightParameterSingleValueQueryNode = new ConstantNode("something else");
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.NotEqual, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode("something");
            binaryNode.Right.ShouldBeConstantQueryNode("something else");
        }

        [Fact]
        public void GreaterThanOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode("something");
            this.rightParameterSingleValueQueryNode = new ConstantNode("something else");
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.GreaterThan, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode("something");
            binaryNode.Right.ShouldBeConstantQueryNode("something else");
        }

        [Fact]
        public void GreaterThanOrEqualOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(99);
            this.rightParameterSingleValueQueryNode = new ConstantNode(99.1);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.GreaterThanOrEqual, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThanOrEqual);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode(99d);
            binaryNode.Right.ShouldBeConstantQueryNode(99.1);
        }

        [Fact]
        public void LessThanOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(55);
            this.rightParameterSingleValueQueryNode = new ConstantNode(66);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.LessThan, new LiteralToken(true), new LiteralToken(false));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThan);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode(55);
            binaryNode.Right.ShouldBeConstantQueryNode(66);
        }

        [Fact]
        public void LessThanOrEqualOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(new Guid());
            this.rightParameterSingleValueQueryNode = new ConstantNode(new Guid(1, 2, 3, new byte[8]));
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.LessThanOrEqual, new LiteralToken(true), new LiteralToken(false));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThanOrEqual);
            Assert.Equal(EdmPrimitiveTypeKind.Boolean, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode(new Guid());
            binaryNode.Right.ShouldBeConstantQueryNode(new Guid(1, 2, 3, new byte[8]));
        }

        [Fact]
        public void AddOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(1);
            this.rightParameterSingleValueQueryNode = new ConstantNode(2);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.Add, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Add);
            Assert.Equal(EdmPrimitiveTypeKind.Int32, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode(1);
            binaryNode.Right.ShouldBeConstantQueryNode(2);
        }

        [Fact]
        public void SubtractOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(99);
            this.rightParameterSingleValueQueryNode = new ConstantNode(99.1);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.Subtract, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Subtract);
            Assert.Equal(EdmPrimitiveTypeKind.Double, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode(99d);
            binaryNode.Right.ShouldBeConstantQueryNode(99.1);
        }

        [Fact]
        public void MultiplyOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(double.NaN);
            this.rightParameterSingleValueQueryNode = new ConstantNode(new sbyte());
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.Multiply, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Multiply);
            Assert.Equal(EdmPrimitiveTypeKind.Double, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode(double.NaN);
            binaryNode.Right.ShouldBeConvertQueryNode(EdmPrimitiveTypeKind.Double);
        }

        [Fact]
        public void DivideOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(0);
            this.rightParameterSingleValueQueryNode = new ConstantNode(0);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.Divide, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Divide);
            Assert.Equal(EdmPrimitiveTypeKind.Int32, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode(0);
            binaryNode.Right.ShouldBeConstantQueryNode(0);
        }

        [Fact]
        public void ModuloOperatorShouldResultInBinaryOperatorNode()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(-9.9);
            this.rightParameterSingleValueQueryNode = new ConstantNode(-100.9);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.Modulo, new LiteralToken("foo"), new LiteralToken("bar"));

            var resultNode = this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            var binaryNode = resultNode.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Modulo);
            Assert.Equal(EdmPrimitiveTypeKind.Double, binaryNode.TypeReference.PrimitiveKind());
            binaryNode.Left.ShouldBeConstantQueryNode(-9.9);
            binaryNode.Right.ShouldBeConstantQueryNode(-100.9);
        }

        [Fact]
        public void CollectionLeftTokenShouldFail()
        {
            this.binaryOperatorBinder = new BinaryOperatorBinder(this.BindMethodThatReturnsQueryNode, /*resolver*/ null);
            this.leftQueryNode = new EntitySetNode(this.model.FindDeclaredEntitySet("People"));
            this.rightQueryNode = new ConstantNode(true);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.And, new LiteralToken("foo"), new LiteralToken("bar"));
            Action bind = () => this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            bind.Throws<ODataException>(Error.Format(SRResources.MetadataBinder_BinaryOperatorOperandNotSingleValue, "And"));
        }

        [Fact]
        public void CollectionRightTokenShouldFail()
        {
            this.binaryOperatorBinder = new BinaryOperatorBinder(this.BindMethodThatReturnsQueryNode, /*resolver*/ null);
            this.leftQueryNode = new ConstantNode("People");
            this.rightQueryNode = new EntitySetNode(this.model.FindDeclaredEntitySet("People"));
            var binaryOperatorToken = new BinaryOperatorToken(BinaryOperatorKind.Equal, new LiteralToken("foo"), new LiteralToken("bar"));
            Action bind = () => this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorToken);

            bind.Throws<ODataException>(Error.Format(SRResources.MetadataBinder_BinaryOperatorOperandNotSingleValue, "Equal"));
        }

        [Fact]
        public void LeftTokenTypeImcompatibleWithOperatorShouldFail()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(true);
            this.rightParameterSingleValueQueryNode = new ConstantNode(100);
            var binaryOperatorToken = new BinaryOperatorToken(BinaryOperatorKind.GreaterThan, new LiteralToken("foo"), new LiteralToken("bar"));
            Action bind = () => this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorToken);

            bind.Throws<ODataException>(Error.Format(SRResources.MetadataBinder_IncompatibleOperandsError, "Edm.Boolean", "Edm.Int32", BinaryOperatorKind.GreaterThan));
        }

        [Fact]
        public void LeftTokenTypeIncompatibleWithOperatorAndRightTokenNullShouldFail()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(DateTimeOffset.Now);
            this.rightParameterSingleValueQueryNode = new ConstantNode(null);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.And, new LiteralToken("foo"), new LiteralToken("bar"));
            Action bind = () => this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            bind.Throws<ODataException>(Error.Format(SRResources.MetadataBinder_IncompatibleOperandsError, "Edm.DateTimeOffset", "<null>", BinaryOperatorKind.And));
        }

        [Fact]
        public void LeftTokenTypeIncompatibleWithOperatorAndRightTokenOpenPropertyShouldFail()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(DateTimeOffset.Now);
            this.rightParameterSingleValueQueryNode = new SingleValueOpenPropertyAccessNode(new ConstantNode(null), "SomeProperty");
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.And, new LiteralToken("foo"), new LiteralToken("bar"));
            Action bind = () => this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            bind.Throws<ODataException>(Error.Format(SRResources.MetadataBinder_IncompatibleOperandsError, "Edm.DateTimeOffset", "<null>", BinaryOperatorKind.And));
        }

        [Fact]
        public void RightTokenTypeImcompatibleWithOperatorShouldFail()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(999);
            this.rightParameterSingleValueQueryNode = new ConstantNode(string.Empty);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.Multiply, new LiteralToken("foo"), new LiteralToken("bar"));
            Action bind = () => this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            bind.Throws<ODataException>(Error.Format(SRResources.MetadataBinder_IncompatibleOperandsError, "Edm.Int32", "Edm.String", BinaryOperatorKind.Multiply));
        }

        [Fact]
        public void LeftTokenTypeImcompatibleWithRightTokenShouldFail()
        {
            this.leftParameterSingleValueQueryNode = new ConstantNode(true);
            this.rightParameterSingleValueQueryNode = new ConstantNode(1);
            var binaryOperatorQueryToken = new BinaryOperatorToken(BinaryOperatorKind.Equal, new LiteralToken("foo"), new LiteralToken("bar"));
            Action bind = () => this.binaryOperatorBinder.BindBinaryOperator(binaryOperatorQueryToken);

            bind.Throws<ODataException>(Error.Format(SRResources.MetadataBinder_IncompatibleOperandsError, "Edm.Boolean", "Edm.Int32", BinaryOperatorKind.Equal));
        }

        /// <summary>
        /// We substitute the following methods for the MetadataBinder.Bind method to keep the tests from growing too large in scope.
        /// In practice this does the same thing.
        /// </summary>
        private SingleValueNode BindMethodThatReturnsSingleValueQueryNode(QueryToken queryToken)
        {
            SingleValueNode QueryNodeToReturn;
            if (this.shouldReturnLeft)
            {
                QueryNodeToReturn = this.leftParameterSingleValueQueryNode;
            }
            else
            {
                QueryNodeToReturn = this.rightParameterSingleValueQueryNode;
            }

            this.shouldReturnLeft = !this.shouldReturnLeft;
            return QueryNodeToReturn;
        }

        private QueryNode BindMethodThatReturnsQueryNode(QueryToken queryToken)
        {
            QueryNode QueryNodeToReturn;
            if (this.shouldReturnLeft)
            {
                QueryNodeToReturn = this.leftQueryNode;
            }
            else
            {
                QueryNodeToReturn = this.rightQueryNode;
            }

            this.shouldReturnLeft = !this.shouldReturnLeft;
            return QueryNodeToReturn;
        }
    }
}
