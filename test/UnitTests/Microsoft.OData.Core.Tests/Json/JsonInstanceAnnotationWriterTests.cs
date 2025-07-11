﻿//---------------------------------------------------------------------
// <copyright file="JsonInstanceAnnotationWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Microsoft.Spatial;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Core.Tests.DependencyInjection;
using Microsoft.OData.Core;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests for the jsonInstanceAnnotationWriter.
    ///
    /// Uses mocks to test that the methods call the correct functions on JsonWriter and ODataJsonValueSerializer.
    /// </summary>
    public class JsonInstanceAnnotationWriterTests
    {
        private JsonInstanceAnnotationWriter jsonInstanceAnnotationWriter;
        private MockJsonWriter jsonWriter;
        private MockJsonValueSerializer valueWriter;
        private IEdmModel model;
        private EdmModel referencedModel;

        public JsonInstanceAnnotationWriterTests()
        {
            this.jsonWriter = new MockJsonWriter
            {
                WriteNameVerifier = name => { },
                WriteValueVerifier = str => { }
            };

            this.referencedModel = new EdmModel();
            model = TestUtils.WrapReferencedModelsToMainModel(referencedModel);

            // Version will be V3+ in production since it's Json only
            var stream = new MemoryStream();
            this.valueWriter = new MockJsonValueSerializer(CreateJsonOutputContext(stream, model, this.jsonWriter));
            this.jsonInstanceAnnotationWriter = new JsonInstanceAnnotationWriter(this.valueWriter, new JsonMinimalMetadataTypeNameOracle());
        }

        [Fact]
        public void WriteInstanceAnnotation_ForIntegerShouldUsePrimitiveCodePath()
        {
            var integerValue = new ODataPrimitiveValue(123);
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                Assert.Equal("@" + term, name);
                verifierCalls++;
            };
            this.valueWriter.WritePrimitiveVerifier = (value, reference) =>
            {
                Assert.Equal(integerValue.Value, value);
                Assert.Null(reference);
                Assert.Equal(1, verifierCalls);
                verifierCalls++;
            };

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, integerValue));
            Assert.Equal(2, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForDateShouldUsePrimitiveCodePathWithTypeName()
        {
            var date = new ODataPrimitiveValue(new Date(2014, 11, 11));
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                if (verifierCalls == 0)
                {
                    Assert.Equal(term + "@odata.type", name);
                    verifierCalls++;
                }
                else if (verifierCalls == 2)
                {
                    Assert.Equal("@" + term, name);
                    verifierCalls++;
                }
                else throw new Exception("unexpected call to JsonWriter.WriteName");
            };
            this.jsonWriter.WriteValueVerifier = (value) =>
            {
                Assert.Equal(1, verifierCalls);
                verifierCalls++;
            };
            this.valueWriter.WritePrimitiveVerifier = (value, reference) =>
            {
                Assert.Equal(date.Value, value);
                Assert.Null(reference);
                Assert.Equal(3, verifierCalls);
                verifierCalls++;
            };

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, date));
            Assert.Equal(4, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForDateTimeOffsetShouldUsePrimitiveCodePathWithTypeName()
        {
            var dateTime = new ODataPrimitiveValue(new DateTimeOffset(2012, 9, 5, 10, 27, 34, TimeSpan.Zero));
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                if (verifierCalls == 0)
                {
                    Assert.Equal(term + "@odata.type", name);
                    verifierCalls++;
                }
                else if (verifierCalls == 2)
                {
                    Assert.Equal("@" + term, name);
                    verifierCalls++;
                }
                else throw new Exception("unexpected call to JsonWriter.WriteName");
            };
            this.jsonWriter.WriteValueVerifier = (value) =>
            {
                Assert.Equal(1, verifierCalls);
                verifierCalls++;
            };
            this.valueWriter.WritePrimitiveVerifier = (value, reference) =>
            {
                Assert.Equal(dateTime.Value, value);
                Assert.Null(reference);
                Assert.Equal(3, verifierCalls);
                verifierCalls++;
            };

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, dateTime));
            Assert.Equal(4, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForTimeOfDayShouldUsePrimitiveCodePathWithTypeName()
        {
            var time = new ODataPrimitiveValue(new TimeOfDay(12, 5, 0, 90));
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                if (verifierCalls == 0)
                {
                    Assert.Equal(term + "@odata.type", name);
                    verifierCalls++;
                }
                else if (verifierCalls == 2)
                {
                    Assert.Equal("@" + term, name);
                    verifierCalls++;
                }
                else throw new Exception("unexpected call to JsonWriter.WriteName");
            };
            this.jsonWriter.WriteValueVerifier = (value) =>
            {
                Assert.Equal(1, verifierCalls);
                verifierCalls++;
            };
            this.valueWriter.WritePrimitiveVerifier = (value, reference) =>
            {
                Assert.Equal(time.Value, value);
                Assert.Null(reference);
                Assert.Equal(3, verifierCalls);
                verifierCalls++;
            };

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, time));
            Assert.Equal(4, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForSpatialShouldUsePrimitiveCodePathWithTypeName()
        {
            var point = new ODataPrimitiveValue(GeographyPoint.Create(10.5, 5.25));
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                if (verifierCalls == 0)
                {
                    Assert.Equal(term + "@odata.type", name);
                    verifierCalls++;
                }
                else if (verifierCalls == 2)
                {
                    Assert.Equal("@" + term, name);
                    verifierCalls++;
                }
                else throw new Exception("unexpected call to JsonWriter.WriteName");
            };
            this.jsonWriter.WriteValueVerifier = (value) =>
            {
                Assert.Equal(1, verifierCalls);
                verifierCalls++;
            };
            this.valueWriter.WritePrimitiveVerifier = (value, reference) =>
            {
                Assert.Equal(point.Value, value);
                Assert.Null(reference);
                Assert.Equal(3, verifierCalls);
                verifierCalls++;
            };

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, point));
            Assert.Equal(4, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForResourceShouldUseResourceCodePath()
        {
            var resourceValue = new ODataResourceValue();
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                Assert.Equal("@" + term, name);
                verifierCalls++;
            };
            this.valueWriter.WriteResourceValueVerifier = (value, reference, isOpenProperty, dupChecker) =>
            {
                Assert.Equal(resourceValue, value);
                Assert.Null(reference);
                Assert.True(isOpenProperty);
                Assert.Equal(1, verifierCalls);
                verifierCalls++;
            };
            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, resourceValue));
            Assert.Equal(2, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForCollectionShouldUseCollectionCodePath()
        {
            var collectionValue = new ODataCollectionValue() { TypeName = "Collection(String)" };
            collectionValue.TypeAnnotation = new ODataTypeAnnotation();
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                Assert.Equal("@" + term, name);
                verifierCalls++;
            };
            this.valueWriter.WriteCollectionVerifier = (value, reference, valueTypeReference, isTopLevelProperty, isInUri, isOpenProperty) =>
            {
                Assert.Equal(collectionValue, value);
                Assert.Null(reference);
                Assert.NotNull(valueTypeReference);
                Assert.True(valueTypeReference.IsCollection());
                Assert.True(valueTypeReference.AsCollection().ElementType().IsString());
                Assert.True(isOpenProperty);
                Assert.False(isTopLevelProperty);
                Assert.False(isInUri);
                Assert.Equal(1, verifierCalls);
                verifierCalls++;
            };
            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, collectionValue));
            Assert.Equal(2, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForNullValue()
        {
            const string term = "some.term";
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                Assert.Equal("@" + term, name);
                verifierCalls++;
            };
            this.valueWriter.WriteNullVerifier = () =>
            {
                Assert.Equal(1, verifierCalls);
                verifierCalls++;
            };
            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, ODataNullValue.Instance));
            Assert.Equal(2, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotation_ForEnumValue()
        {
            var enumValue = new ODataEnumValue("ReadOnly", "Org.OData.Core.V1.Permission");
            string term = CoreVocabularyModel.PermissionsTerm.FullName();
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                Assert.Equal("@" + term, name);
                verifierCalls++;
            };
            this.valueWriter.WriteEnumVerifier = (value, expectedType) =>
            {
                Assert.Equal(enumValue, value);
                Assert.Same(expectedType.Definition, CoreVocabularyModel.Instance.SchemaElements.FirstOrDefault(e => e.FullName() == "Org.OData.Core.V1.Permission"));
                Assert.Equal(1, verifierCalls);
                verifierCalls++;
            };
            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, enumValue));
            Assert.Equal(2, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotationWithNullValueShouldPassIfTheTermIsNullableInTheModel()
        {
            // Add a term of type Collection(Edm.String) to the model.
            this.referencedModel.AddElement(new EdmTerm(
                "My.Namespace",
                "Nullable",
                EdmCoreModel.Instance.GetInt32(isNullable: true)));

            var verifierCalls = 0;

            const string term = "My.Namespace.Nullable";
            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                Assert.Equal("@" + term, name);
                verifierCalls++;
            };
            this.valueWriter.WriteNullVerifier = () =>
            {
                Assert.Equal(1, verifierCalls);
                verifierCalls++;
            };
            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, ODataNullValue.Instance));
            Assert.Equal(2, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotationWithNullValueShouldThrowIfTheTermIsNotNullableInTheModel()
        {
            // Add a term of type Collection(Edm.String) to the model.
            this.referencedModel.AddElement(new EdmTerm(
                "My.Namespace",
                "NotNullable",
                EdmCoreModel.Instance.GetInt32(isNullable: false)));

            string term = "My.Namespace.NotNullable";
            Action action = () => this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(new ODataInstanceAnnotation(term, ODataNullValue.Instance));
            action.Throws<ODataException>(Error.Format(SRResources.JsonInstanceAnnotationWriter_NullValueNotAllowedForInstanceAnnotation, term, "Edm.Int32"));
        }

        [Fact]
        public void WriteInstanceAnnotations_EmptyDoesNothing()
        {
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) =>
            {
                verifierCalls++;
            };
            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(new List<ODataInstanceAnnotation>());
            Assert.Equal(0, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotation_AllAnnotationsGetWritten()
        {
            var annotations = new Collection<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(annotations);
            Assert.Equal(4, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotations_AnnotationsCannotBeWrittenTwice()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;
            InstanceAnnotationWriteTracker tracker = new InstanceAnnotationWriteTracker();

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            Assert.Equal(4, verifierCalls);

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            Assert.Equal(4, verifierCalls);

            Assert.True(tracker.IsAnnotationWritten("term.one"));
            Assert.True(tracker.IsAnnotationWritten("term.two"));
        }

        [Fact]
        public void WriteInstanceAnnotationCollection_NewAnnotationsGetWritten()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;
            InstanceAnnotationWriteTracker tracker = new InstanceAnnotationWriteTracker();

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            Assert.Equal(2, verifierCalls);

            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            Assert.False(tracker.IsAnnotationWritten("term.two"));

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            Assert.Equal(4, verifierCalls);

            Assert.True(tracker.IsAnnotationWritten("term.two"));
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldThrowOnDuplicatedAnnotationNames()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(789)));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            Action test = () => this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(annotations);
            test.Throws<ODataException>(Error.Format(SRResources.JsonInstanceAnnotationWriter_DuplicateAnnotationNameInCollection, "term.one"));
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldNotThrowOnNamesWithDifferentCasing()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.One", new ODataPrimitiveValue(456)));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(annotations);
            Assert.Equal(4, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotationsWithTrackerShouldThrowOnDuplicatedAnnotationNames()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            InstanceAnnotationWriteTracker tracker = new InstanceAnnotationWriteTracker();
            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            Assert.Equal(2, verifierCalls);
            Assert.True(tracker.IsAnnotationWritten("term.one"));

            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(456)));
            Action test = () => this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            test.Throws<ODataException>(Error.Format(SRResources.JsonInstanceAnnotationWriter_DuplicateAnnotationNameInCollection, "term.one"));
        }

        [Fact]
        public void WriteInstanceAnnotationsWithTrackerShouldNotThrowOnNamesWithDifferentCasing()
        {
            var annotations = new List<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            InstanceAnnotationWriteTracker tracker = new InstanceAnnotationWriteTracker();
            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            Assert.Equal(2, verifierCalls);
            Assert.True(tracker.IsAnnotationWritten("term.one"));

            annotations.Add(new ODataInstanceAnnotation("term.One", new ODataPrimitiveValue(456)));
            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotations(annotations, tracker);
            Assert.Equal(4, verifierCalls);
            Assert.True(tracker.IsAnnotationWritten("term.one"));
            Assert.True(tracker.IsAnnotationWritten("term.One"));
        }

        [Fact]
        public void WriteInstanceAnnotationShouldPassPrimitiveTypeFromModelToUnderlyingWriter()
        {
            // Add a term of type DateTimeOffset to the model.
            this.referencedModel.AddElement(new EdmTerm("My.Namespace", "DateTimeTerm", EdmPrimitiveTypeKind.DateTimeOffset));
            var instanceAnnotation = new ODataInstanceAnnotation("My.Namespace.DateTimeTerm", new ODataPrimitiveValue(DateTimeOffset.MinValue));

            bool calledWritePrimitive = false;

            this.valueWriter.WritePrimitiveVerifier = (o, reference) =>
            {
                Assert.NotNull(reference);
                Assert.True(reference.IsDateTimeOffset());
                calledWritePrimitive = true;
            };

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(instanceAnnotation);
            Assert.True(calledWritePrimitive);
        }

        [Fact]
        public void WriteInstanceAnnotationShouldWriteValueTypeIfMoreDerivedThanMetadataType()
        {
            // Add a term of type Geography to the model.
            this.referencedModel.AddElement(new EdmTerm("My.Namespace", "GeographyTerm", EdmPrimitiveTypeKind.Geography));
            var instanceAnnotation = new ODataInstanceAnnotation("My.Namespace.GeographyTerm", new ODataPrimitiveValue(GeographyPoint.Create(0.0, 0.0)));

            bool writingTypeName = false;
            bool wroteTypeName = false;
            this.jsonWriter.WriteNameVerifier = s =>
            {
                writingTypeName = s.EndsWith("odata.type");
            };

            this.jsonWriter.WriteValueVerifier = s =>
            {
                if (writingTypeName)
                {
                    Assert.Equal("#GeographyPoint", s);
                    wroteTypeName = true;
                }
            };

            this.valueWriter.WritePrimitiveVerifier = (o, reference) => { };

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(instanceAnnotation);

            Assert.True(wroteTypeName);
        }

        [Fact]
        public void WriteInstanceAnnotationShouldPassResourceTypeFromModelToUnderlyingWriter()
        {
            // Add a term of a complex type to the model.
            var complexTypeReference = new EdmComplexTypeReference(new EdmComplexType("My.Namespace", "ComplexType"), false);
            this.referencedModel.AddElement(new EdmTerm("My.Namespace", "StructuredTerm", complexTypeReference));
            var instanceAnnotation = new ODataInstanceAnnotation("My.Namespace.StructuredTerm", new ODataResourceValue { TypeName = "ComplexType" });

            bool calledWriteResource = false;

            this.valueWriter.WriteResourceValueVerifier = (resourceValue, typeReference, isOpenProperty, dupChecker) =>
            {
                Assert.NotNull(typeReference);
                Assert.True(typeReference.IsComplex());
                Assert.Equal("My.Namespace.ComplexType", typeReference.AsComplex().FullName());
                calledWriteResource = true;
            };

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(instanceAnnotation);
            Assert.True(calledWriteResource);
        }

        [Fact]
        public void WriteInstanceAnnotationShouldPassCollectionTypeFromModelToUnderlyingWriter()
        {
            // Add a term of type Collection(Edm.String) to the model.
            this.referencedModel.AddElement(new EdmTerm(
                "My.Namespace",
                "CollectionTerm",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false)))));

            var instanceAnnotation = new ODataInstanceAnnotation("My.Namespace.CollectionTerm", new ODataCollectionValue() { TypeName = "Collection(Edm.String)" });

            bool calledWriteCollection = false;

            this.valueWriter.WriteCollectionVerifier = (collectionValue, typeReference, valueTypeReference, isTopLevel, isOpenProperty, dupChecker) =>
            {
                Assert.NotNull(typeReference);
                Assert.True(typeReference.IsCollection());
                Assert.True(typeReference.AsCollection().ElementType().IsString());
                Assert.NotNull(valueTypeReference);
                Assert.True(valueTypeReference.IsCollection());
                Assert.True(valueTypeReference.AsCollection().ElementType().IsString());
                calledWriteCollection = true;
            };

            var result = WriteInstanceAnnotation(instanceAnnotation, this.referencedModel);
            Assert.Contains("{\"@My.Namespace.CollectionTerm\":[]}", result);
            Assert.DoesNotContain("odata.type", result);

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(instanceAnnotation);
            Assert.True(calledWriteCollection);
        }

        #region type name short-span integration tests
        [Fact]
        public void WritingPrimitiveAnnotationWithNonJsonNativeTypeShouldIncludeTypeName()
        {
            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.MyDateTimeOffsetTerm", new ODataPrimitiveValue(DateTimeOffset.MinValue)),
                EdmCoreModel.Instance);

            Assert.Contains("\"custom.namespace.MyDateTimeOffsetTerm@odata.type\":\"#DateTimeOffset\"", result);
        }

        [Fact]
        public void WritingPrimitiveAnnotationWithDeclaredTypeShouldNotIncludeTypeName()
        {
            EdmModel edmModel = new EdmModel();
            edmModel.AddElement(new EdmTerm("custom.namespace", "MyDateTimeOffsetTerm", EdmPrimitiveTypeKind.DateTimeOffset));

            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.MyDateTimeOffsetTerm", new ODataPrimitiveValue(DateTimeOffset.MinValue)),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            Assert.DoesNotContain("odata.type", result);
        }

        [Fact]
        public void WritingPrimitiveAnnotationWithTypeMismatchShouldThrow()
        {
            EdmModel edmModel = new EdmModel();
            edmModel.AddElement(new EdmTerm("custom.namespace", "MyDateTimeOffsetTerm", EdmPrimitiveTypeKind.DateTimeOffset));

            // Term is declared to be of type DateTimeOffset, but actual primitive value is a Guid.
            Action testSubject = () => WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.MyDateTimeOffsetTerm", new ODataPrimitiveValue(Guid.Empty)),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            testSubject.Throws<ODataException>(Error.Format(SRResources.ValidationUtils_IncompatiblePrimitiveItemType, "Edm.Guid", /*nullability*/ "False", "Edm.DateTimeOffset", /*nullability*/ "True"));
        }

        [Fact]
        public void WritingResourceAnnotationShouldNotIncludeTypeNameIfDeclaredOnTermMetadata()
        {
            EdmModel edmModel = new EdmModel();

            var complexType = new EdmComplexType("custom.namespace", "Address");
            edmModel.AddElement(complexType);
            edmModel.AddElement(new EdmTerm("custom.namespace", "AddressTerm", new EdmComplexTypeReference(complexType, false)));

            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.AddressTerm", new ODataResourceValue() { TypeName = "custom.namespace.Address", Properties = Enumerable.Empty<ODataProperty>() }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            Assert.DoesNotContain("odata.type", result);
        }

        [Fact]
        public void WritingResourceAnnotationShouldIncludeTypeNameIfNotDeclaredOnTermMetadata()
        {
            EdmModel edmModel = new EdmModel();

            var complexType = new EdmComplexType("custom.namespace", "Address");
            edmModel.AddElement(complexType);

            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.AddressTerm", new ODataResourceValue() { TypeName = "custom.namespace.Address", Properties = Enumerable.Empty<ODataProperty>() }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            Assert.Contains("\"@custom.namespace.AddressTerm\":{\"@odata.type\":\"#custom.namespace.Address\"", result);
        }

        [Fact]
        public void WritingCollectionAnnotationShouldNotIncludeTypeNameIfDeclaredOnTermMetadata()
        {
            EdmModel edmModel = new EdmModel();
            edmModel.AddElement(new EdmTerm("custom.namespace", "CollectionValueTerm", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false)))));

            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.CollectionValueTerm", new ODataCollectionValue { Items = new object[] { 42, 54 }, TypeName = "Collection(Int32)" }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            Assert.Contains("{\"@custom.namespace.CollectionValueTerm\":[42,54]}", result);
            Assert.DoesNotContain("odata.type", result);
        }

        [Fact]
        public void WritingCollectionAnnotationShouldIncludeTypeNameIfNotDeclaredOnTermMetadata()
        {
            EdmModel edmModel = new EdmModel();

            var result = WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.CollectionValueTerm", new ODataCollectionValue { Items = new object[] { 42, 54 }, TypeName = "Collection(Int32)" }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            Assert.Contains("\"custom.namespace.CollectionValueTerm@odata.type\":\"#Collection(Int32)\"", result);
        }

        [Fact]
        public void WritingResourceAnnotationWithNotDefinedResourceTypeShouldThrow()
        {
            // Note: this behavior may change in future releases, but capturing the behavior shipping in 5.3.
            EdmModel edmModel = new EdmModel();

            Action testSubject = () => WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.AddressTerm", new ODataResourceValue() { TypeName = "custom.namespace.Address", Properties = Enumerable.Empty<ODataProperty>() }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            testSubject.Throws<ODataException>(Error.Format(SRResources.ValidationUtils_UnrecognizedTypeName, "custom.namespace.Address"));
        }

        [Fact]
        public void WritingResourceAnnotationWithCollectionOfNotDefinedResourceTypeShouldThrow()
        {
            // Note: this behavior may change in future releases, but capturing the behavior shipping in 5.3.
            EdmModel edmModel = new EdmModel();

            Action testSubject = () => WriteInstanceAnnotation(
                new ODataInstanceAnnotation("custom.namespace.CollectionOfAddressTerm", new ODataCollectionValue { Items = Enumerable.Empty<ODataResourceValue>(), TypeName = "Collection(custom.namespace.Address)" }),
                TestUtils.WrapReferencedModelsToMainModel(edmModel));

            testSubject.Throws<ODataException>(Error.Format(SRResources.ValidationUtils_UnrecognizedTypeName, "Collection(custom.namespace.Address)"));
        }

        private static string WriteInstanceAnnotation(ODataInstanceAnnotation instanceAnnotation, IEdmModel model)
        {
            using (var stream = new MemoryStream())
            {
                using (var outputContext = new ODataJsonOutputContext(
                    stream,
                    new ODataMessageInfo { Model = model, IsResponse = false, IsAsync = false, Encoding = Encoding.UTF8 },
                    new ODataMessageWriterSettings { Version = ODataVersion.V4, ShouldIncludeAnnotationInternal = ODataUtils.CreateAnnotationFilter("*") }))
                {
                    var valueSerializer = new ODataJsonValueSerializer(outputContext);

                    // The JSON Writer will complain if there is no active scope, so start an object scope.
                    valueSerializer.JsonWriter.StartObjectScope();
                    var instanceAnnotationWriter = new JsonInstanceAnnotationWriter(valueSerializer, new JsonMinimalMetadataTypeNameOracle());

                    // The method under test.
                    instanceAnnotationWriter.WriteInstanceAnnotation(instanceAnnotation);

                    valueSerializer.JsonWriter.EndObjectScope();

                    outputContext.JsonWriter.Flush();
                    stream.Position = 0;
                    return new StreamReader(stream).ReadToEnd();
                }
            }
        }
        #endregion type name short-span integration tests

        [Fact]
        public void WriteInstanceAnnotationShouldWriteAnnotationsThatPassTheAnnotationFilter()
        {
            var annotation = new ODataInstanceAnnotation("ns1.name", new ODataPrimitiveValue(123));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotationInternal = name => name == "ns1.name";

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(annotation);
            Assert.Equal(2, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotationShouldSkipAnnotationsThatDoesNotPassTheAnnotationFilter()
        {
            var annotation = new ODataInstanceAnnotation("ns1.name", new ODataPrimitiveValue(123));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotationInternal = name => name != "ns1.name";

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(annotation);
            Assert.Equal(0, verifierCalls);
        }

        [Fact]
        public void WriteInstanceAnnotationShouldWriteAnnotationIfShouldIncludeAnnotationReturnsTrue()
        {
            var annotation = new ODataInstanceAnnotation("ns1.name", new ODataPrimitiveValue(123));
            var writtenNames = new List<string>();
            var writtenValues = new List<object>();

            this.jsonWriter.WriteNameVerifier = (name) => writtenNames.Add(name);
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => writtenValues.Add(value);
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotation = name => name == "ns1.name";

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(annotation);
            Assert.Single(writtenNames);
            Assert.Single(writtenValues);
            Assert.Equal("@ns1.name", writtenNames[0]);
            Assert.Equal(123, (int)writtenValues[0]);
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldWriteAnnotationThatDoesNotPassTheAnnotationFilterIfShouldIncludeAnnotationReturnsTrue()
        {
            var annotation = new ODataInstanceAnnotation("ns1.name", new ODataPrimitiveValue(123));
            var writtenNames = new List<string>();
            var writtenValues = new List<object>();

            this.jsonWriter.WriteNameVerifier = (name) => writtenNames.Add(name);
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => writtenValues.Add(value);
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotationInternal = name => name != "ns1.name";
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotation = name => name == "ns1.name";

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(annotation);
            Assert.Single(writtenNames);
            Assert.Single(writtenValues);
            Assert.Equal("@ns1.name", writtenNames[0]);
            Assert.Equal(123, (int)writtenValues[0]);
        }

        [Fact]
        public void WriteInstanceAnnotationsShouldWriteAnnotationThatPassesTheAnnotationFilterIfShouldIncludeAnnotationReturnsTrue()
        {
            var annotation = new ODataInstanceAnnotation("ns1.name", new ODataPrimitiveValue(123));
            var writtenNames = new List<string>();
            var writtenValues = new List<object>();

            this.jsonWriter.WriteNameVerifier = (name) => writtenNames.Add(name);
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => writtenValues.Add(value);
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotationInternal = name => name == "ns1.name";
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotation = name => name == "ns1.name";

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(annotation);
            Assert.Single(writtenNames);
            Assert.Single(writtenValues);
            Assert.Equal("@ns1.name", writtenNames[0]);
            Assert.Equal(123, (int)writtenValues[0]);
        }

        [Fact]
        public void WriteInstanceAnnotationShouldSkipAnnotationThatDoesNotPassTheAnnotationFilterIfShouldIncludeAnnotationReturnsFalse()
        {
            var annotation = new ODataInstanceAnnotation("ns1.name", new ODataPrimitiveValue(123));
            var writtenNames = new List<string>();
            var writtenValues = new List<object>();

            this.jsonWriter.WriteNameVerifier = (name) => writtenNames.Add(name);
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => writtenValues.Add(value);
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotationInternal = name => name != "ns1.name";
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotation = name => name != "ns1.name";

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(annotation);
            Assert.Empty(writtenNames);
            Assert.Empty(writtenValues);
        }

        [Fact]
        public void WriteInstanceAnnotationShouldWriteAnnotationThatPassesTheAnnotationFilterIfShouldIncludeAnnotationReturnsFalse()
        {
            var annotation = new ODataInstanceAnnotation("ns1.name", new ODataPrimitiveValue(123));
            var writtenNames = new List<string>();
            var writtenValues = new List<object>();

            this.jsonWriter.WriteNameVerifier = (name) => writtenNames.Add(name);
            this.valueWriter.WritePrimitiveVerifier = (value, reference) => writtenValues.Add(value);
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotationInternal = name => name == "ns1.name";
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotation = name => name != "ns1.name";

            this.jsonInstanceAnnotationWriter.WriteInstanceAnnotation(annotation);
            Assert.Single(writtenNames);
            Assert.Single(writtenValues);
            Assert.Equal("@ns1.name", writtenNames[0]);
            Assert.Equal(123, (int)writtenValues[0]);
        }

        [Fact]
        public void ShouldNotWriteAnyAnnotationByDefault()
        {
            var stream = new MemoryStream();
            var defaultValueWriter = new MockJsonValueSerializer(CreateJsonOutputContext(stream, model, this.jsonWriter, new ODataMessageWriterSettings { Version = ODataVersion.V4 }));
            var defaultAnnotationWriter = new JsonInstanceAnnotationWriter(defaultValueWriter, new JsonMinimalMetadataTypeNameOracle());

            var annotations = new Collection<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            defaultValueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            defaultAnnotationWriter.WriteInstanceAnnotations(annotations);
            Assert.Equal(0, verifierCalls);
        }

        [Fact]
        public void ShouldWriteAnyAnnotationByDefaultWithIgnoreFilterSetToTrue()
        {
            var stream = new MemoryStream();
            var defaultValueWriter = new MockJsonValueSerializer(CreateJsonOutputContext(stream, model, this.jsonWriter, new ODataMessageWriterSettings { Version = ODataVersion.V4 }));
            var defaultAnnotationWriter = new JsonInstanceAnnotationWriter(defaultValueWriter, new JsonMinimalMetadataTypeNameOracle());

            var annotations = new Collection<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            defaultValueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            defaultAnnotationWriter.WriteInstanceAnnotations(annotations, new InstanceAnnotationWriteTracker(), true);
            Assert.Equal(4, verifierCalls);
        }

        [Fact]
        public void ShouldWriteAnyAnnotationWithIgnoreFilterSetToTrueEvenIfShouldIncludeAnnotationsReturnsFalse()
        {
            var stream = new MemoryStream();
            var defaultValueWriter = new MockJsonValueSerializer(CreateJsonOutputContext(stream, model, this.jsonWriter, new ODataMessageWriterSettings { Version = ODataVersion.V4 }));
            var defaultAnnotationWriter = new JsonInstanceAnnotationWriter(defaultValueWriter, new JsonMinimalMetadataTypeNameOracle());

            var annotations = new Collection<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)),
                new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456"))
            };

            var writtenNames = new List<string>();
            var writtenValues = new List<object>(); ;

            this.jsonWriter.WriteNameVerifier = (name) => writtenNames.Add(name);
            defaultValueWriter.WritePrimitiveVerifier = (value, reference) => writtenValues.Add(value);
            this.valueWriter.MessageWriterSettings.ShouldIncludeAnnotation = (name) => false;

            defaultAnnotationWriter.WriteInstanceAnnotations(annotations, new InstanceAnnotationWriteTracker(), true);
            Assert.Collection(
                writtenNames,
                new Action<string>[]
                {
                    name => Assert.Equal("@term.one", name),
                    name => Assert.Equal("@term.two", name)
                });

            Assert.Collection(
                writtenValues,
                new Action<object>[]
                {
                    value => Assert.Equal(123, (int)value),
                    value => Assert.Equal("456", (string)value)
                });
        }

        [Fact]
        public void TestWriteInstanceAnnotationsForError()
        {
            var stream = new MemoryStream();
            var defaultValueWriter = new MockJsonValueSerializer(CreateJsonOutputContext(stream, model, this.jsonWriter, new ODataMessageWriterSettings { Version = ODataVersion.V4 }));
            var defaultAnnotationWriter = new JsonInstanceAnnotationWriter(defaultValueWriter, new JsonMinimalMetadataTypeNameOracle());

            var annotations = new Collection<ODataInstanceAnnotation>();
            annotations.Add(new ODataInstanceAnnotation("term.one", new ODataPrimitiveValue(123)));
            annotations.Add(new ODataInstanceAnnotation("term.two", new ODataPrimitiveValue("456")));
            var verifierCalls = 0;

            this.jsonWriter.WriteNameVerifier = (name) => verifierCalls++;
            defaultValueWriter.WritePrimitiveVerifier = (value, reference) => verifierCalls++;

            defaultAnnotationWriter.WriteInstanceAnnotationsForError(annotations);
            Assert.Equal(4, verifierCalls);
        }

        private ODataJsonOutputContext CreateJsonOutputContext(MemoryStream stream, IEdmModel model, IJsonWriter jsonWriter, ODataMessageWriterSettings settings = null)
        {
            if (settings == null)
            {
                settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
                settings.SetServiceDocumentUri(new Uri("http://example.com/"));
                settings.ShouldIncludeAnnotationInternal = ODataUtils.CreateAnnotationFilter("*");
            }

            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = true,
                IsAsync = false,
                Model = model,
                ServiceProvider =
                    ServiceProviderHelper.BuildServiceProvider(
                        builder =>
                            builder.AddSingleton<IJsonWriterFactory>(sp => new MockJsonWriterFactory(jsonWriter))),

            };

            return new ODataJsonOutputContext(messageInfo, settings);
        }
    }
}
