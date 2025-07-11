﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonEntryAndFeedSerializerUndecalredTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OData;
using Microsoft.OData.Json;
using Microsoft.OData.Tests;
using Microsoft.OData.Edm;
using Xunit;
using Microsoft.OData.Core;

namespace Microsoft.Test.OData.TDD.Tests.Writer.Json
{
    public class ODataJsonEntryAndFeedSerializerUndeclaredTests
    {
        private Uri metadataDocumentUri = new Uri("http://odata.org/test/$metadata/");

        private EdmModel serverModel;
        private EdmEntityType serverEntityType;
        private EdmEntityType serverOpenEntityType;
        private EdmEntitySet serverEntitySet;
        private EdmEntitySet serverOpenEntitySet;

        public ODataJsonEntryAndFeedSerializerUndeclaredTests()
        {
            this.serverModel = new EdmModel();
            var addressType = new EdmComplexType("Server.NS", "Address");
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            this.serverModel.AddElement(addressType);

            // non-open entity type
            this.serverEntityType = new EdmEntityType("Server.NS", "ServerEntityType");
            this.serverModel.AddElement(this.serverEntityType);
            this.serverEntityType.AddKeys(this.serverEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.serverEntityType.AddStructuralProperty("Data", EdmCoreModel.Instance.GetUntyped());
            this.serverEntityType.AddStructuralProperty("Infos", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetUntyped())));
            this.serverEntityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));

            // open entity type
            this.serverOpenEntityType = new EdmEntityType("Server.NS", "ServerOpenEntityType",
                baseType: null, isAbstract: false, isOpen: true);
            this.serverModel.AddElement(this.serverOpenEntityType);
            this.serverOpenEntityType.AddKeys(this.serverOpenEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.serverOpenEntityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));

            EdmEntityContainer container = new EdmEntityContainer("Server.NS", "container1");
            this.serverEntitySet = container.AddEntitySet("serverEntitySet", this.serverEntityType);
            this.serverOpenEntitySet = container.AddEntitySet("serverOpenEntitySet", this.serverOpenEntityType);
            this.serverModel.AddElement(container);

            EdmEnumType enumType = new EdmEnumType("Server.NS", "EnumType");
            enumType.AddMember(new EdmEnumMember(enumType, "Red", new EdmEnumMemberValue(1)));
            enumType.AddMember(new EdmEnumMember(enumType, "Green", new EdmEnumMemberValue(2)));
            this.serverModel.AddElement(enumType);
        }

        private ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings
        {
            Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
        };

        #region Declared Untyped Properties
        [Fact]
        public void WriteResourceDeclaredSingleUntypedProperty_WorksForUntypedValue()
        {
            var property = new ODataProperty { Name = "Data", Value = new ODataUntypedValue { RawValue = "\"#lje324$$\"" } };
            string result = WriteDeclaredUntypedProperty(property);
            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Data\":\"#lje324$$\"}", result);
        }

        [Fact]
        public void WriteResourceDeclaredSingleUntypedProperty_WorksForPrimitiveValue()
        {
            // String is one of default type
            var property = new ODataProperty { Name = "Data", Value = new ODataPrimitiveValue("41") };
            string result = WriteDeclaredUntypedProperty(property);
            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Data@odata.type\":\"#String\",\"Data\":\"41\"}", result);

            // not-default type
            property = new ODataProperty { Name = "Data", Value = new ODataPrimitiveValue(41) };
            result = WriteDeclaredUntypedProperty(property);
            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Data@odata.type\":\"#Int32\",\"Data\":41}", result);
        }

        [Fact]
        public void WriteResourceDeclaredSingleUntypedProperty_WorksForEnumValue()
        {
            // Without type name
            var property = new ODataProperty { Name = "Data", Value = new ODataEnumValue("AnyMem") };
            string result = WriteDeclaredUntypedProperty(property);
            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Data\":\"AnyMem\"}", result);

            // with type name
            property = new ODataProperty { Name = "Data", Value = new ODataEnumValue("Green", "Server.NS.EnumType") };
            result = WriteDeclaredUntypedProperty(property);
            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Data\":\"Green\"}", result);
        }

        [Fact]
        public void WriteResourceDeclaredSingleUntypedProperty_WorksForBinaryStreamValue()
        {
            // With type name
            // ODataBinaryStreamValue
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write("1234567890");
            writer.Flush();
            stream.Position = 0;

            var property = new ODataProperty
            {
                Name = "Data",
                Value = new ODataBinaryStreamValue(stream)
            };

            string result = WriteDeclaredUntypedProperty(property);
            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Data\":\"CjEyMzQ1Njc4OTA=\"}", result);
        }

        [Fact]
        public void WriteResourceDeclaredSingleUntypedProperty_WorksForCollectionValue()
        {
            // With type name
            var property = new ODataProperty
            {
                Name = "Data",
                Value = new ODataCollectionValue
                {
                    TypeName = "Collection(Edm.String)",
                    Items = new[]
                    {
                        "abc",
                        "xyz"
                    }
                }
            };

            string result = WriteDeclaredUntypedProperty(property);
            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Data\":[\"abc\",\"xyz\"]}", result);

            // without type name
            property = new ODataProperty
            {
                Name = "Data",
                Value = new ODataCollectionValue
                {
                    Items = new object[]
                    {
                        "abc",
                        null,
                        42
                    }
                }
            };

            result = WriteDeclaredUntypedProperty(property);
            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Data\":[\"abc\",null,42]}", result);
        }

        [Fact]
        public void WriteResourceDeclaredCollectionUntypedProperty_ThrowsForNonCollectionValue()
        {
            // With type name
            var property = new ODataProperty
            {
                Name = "Infos",
                Value = new ODataPrimitiveValue(42)
            };

            Action test = () => WriteDeclaredUntypedProperty(property);
            ODataException exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Error.Format(SRResources.ValidationUtils_NonPrimitiveTypeForPrimitiveValue, "Collection(Edm.Untyped)"), exception.Message);
        }

        [Fact]
        public void WriteResourceDeclaredCollectionUntypedProperty_WorksForCollectionValue()
        {
            var property = new ODataProperty
            {
                Name = "Infos",
                Value = new ODataCollectionValue
                {
                    Items = new object[]
                    {
                        "abc",
                        null,
                        42
                    }
                }
            };

            string result = WriteDeclaredUntypedProperty(property);
            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Infos\":[\"abc\",null,42]}", result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WriteResourceCollectionUntypedProperty_WorksResourceInNestedCollectionOfCollection2(bool isOpen)
        {
            string typeName = isOpen ? "Server.NS.ServerOpenEntityType" : "Server.NS.ServerEntityType";
            EdmEntitySet entitySet = isOpen ? this.serverOpenEntitySet : this.serverEntitySet;
            EdmEntityType entityType = isOpen ? this.serverOpenEntityType : this.serverEntityType;
            string propertyName = isOpen ? "AnyDynamic" : "Infos";

            string actual = WriteEntryPayload(entitySet, entityType,
                writer =>
                {
                    writer.WriteStart(new ODataResource { TypeName = typeName });
                    writer.WriteStart(new ODataNestedResourceInfo { Name = propertyName, IsCollection = true });
                    writer.WriteStart(new ODataResourceSet { TypeName = "Collection(Edm.Untyped)" });
                    writer.WriteStart(new ODataResourceSet());
                    writer.WriteStart(new ODataResource
                    {
                        TypeName = "Edm.Untyped",
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty { Name = "FirstName", Value = "Kerry"}
                        }
                    });
                    writer.WriteEnd(); // End of "Edm.Untyped"
                    writer.WriteEnd();
                    writer.WriteEnd(); // End of "Infos" / AnyDynamic
                    writer.WriteEnd();
                    writer.WriteEnd();
                });

            string result = isOpen ?
                "{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity\",\"AnyDynamic\":[[{\"FirstName\":\"Kerry\"}]]}" :
                "{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Infos\":[[{\"FirstName\":\"Kerry\"}]]}";

            Assert.Equal(result, actual);
        }

        [Fact]
        public void ODataBinaryStreamValue_Closes_Stream_By_Default()
        {
            // With type name
            // ODataBinaryStreamValue
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write("1234567890");
            writer.Flush();
            stream.Position = 0;

            var property = new ODataProperty
            {
                Name = "Data",
                Value = new ODataBinaryStreamValue(stream)
            };

            string result = WriteDeclaredUntypedProperty(property);
            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Data\":\"CjEyMzQ1Njc4OTA=\"}", result);

            Assert.False(stream.CanRead);
        }

        private string WriteDeclaredUntypedProperty(ODataProperty untypedProperty)
        {
            var entry = new ODataResource
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                {
                    untypedProperty
                }
            };

            return this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType,
                writer =>
                {
                    writer.WriteStart(entry);
                    writer.WriteEnd();
                });
        }

        [Fact]
        public void WriteResourceDeclaredUntypedProperty_WorksForNestedResourceInfo()
        {
            string result = WriteEntryPayload(this.serverEntitySet, this.serverEntityType,
                writer =>
                {
                    writer.WriteStart(new ODataResource());

                    writer.WriteStart(new ODataNestedResourceInfo { Name = "Data", IsCollection = true });
                    writer.WriteStart(new ODataResourceSet());
                    writer.WriteStart(new ODataResource { TypeName = "Edm.Untyped" });
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();

                    writer.WriteStart(new ODataNestedResourceInfo { Name = "Infos", IsCollection = true });
                    writer.WriteStart(new ODataResourceSet());
                    writer.WriteStart(resource: null);
                    writer.WriteEnd();
                    writer.Write(new ODataPrimitiveValue(32));
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                });

            Assert.Equal("{\"@odata.context\":\"http://www.sampletest.com/$metadata#serverEntitySet/$entity\",\"Data\":[{}],\"Infos\":[null,32]}", result);
        }
        #endregion

        #region non-open entity's property unknown name + known value type
        [Fact]
        public void WriteEntryUndeclaredPropertiesTest()
        {
            var undeclaredFloat = new ODataProperty { Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D) };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredFloat, false);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        [Fact]
        public void WriteNonOpenEntryUndeclaredPropertiesWithNullValueTest()
        {
            var undeclaredNull = new ODataProperty { Name = "UndeclaredType1", Value = null };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredNull, false);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredType1"":null,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        [Fact]
        public void WriteNonOpenEntryUndeclaredComplexPropertiesTest()
        {
            var undeclaredComplex_Info = new ODataNestedResourceInfo()
            {
                Name = "UndeclaredComplexType1",
                IsCollection = false
            };

            var undeclaredComplex = new ODataResource()
            {
                TypeName = "Server.NS.Address",
                Properties = new[]
                {
                    new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.1000,Zixing Rd Minhang")},
                    new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.1001,Zixing Rd Minhang")},
                }
            };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredComplex_Info, undeclaredComplex, false);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredComplexType1"":{""@odata.type"":""#Server.NS.Address"",""Street"":""No.1000,Zixing Rd Minhang"",""UndeclaredStreet"":""No.1001,Zixing Rd Minhang""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        [Fact]
        public void WriteNonOpenEntryUndeclaredEmptyComplexPropertiesTest()
        {
            var undeclaredComplex_Info = new ODataNestedResourceInfo()
            {
                Name = "UndeclaredComplexType1",
                IsCollection = false
            };

            var undeclaredComplex = new ODataResource()
            {
                TypeName = "Server.NS.Address",
                Properties = new ODataProperty[] { },
            };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredComplex_Info, undeclaredComplex, false);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredComplexType1"":{""@odata.type"":""#Server.NS.Address""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        [Fact]
        public void WriteNonOpenEntryUndeclaredCollectionPropertiesTest()
        {
            var undeclaredCol = new ODataProperty
            {
                Name = "UndeclaredCollection1",
                Value = new ODataCollectionValue()
                {
                    TypeName = "Collection(Edm.String)",
                    Items = new[]
                    {
                        "mystr1",
                        "mystr2",
                        "mystr3"
                    }
                }
            };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredCol, false);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredCollection1"":[""mystr1"",""mystr2"",""mystr3""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        [Fact]
        public void WriteNonOpenEntryUndeclaredEmptyCollectionPropertiesTest()
        {
            var undeclaredCol = new ODataProperty
            {
                Name = "UndeclaredCollection1",
                Value = new ODataCollectionValue()
                {
                    TypeName = "Collection(Edm.String)",
                    Items = new string[] { },
                }
            };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredCol, false);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredCollection1"":[],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        #endregion

        #region non-open entity's property unknown name + unknown value type
        [Fact]
        public void WriteEntryUntypedFloatDoubleTest()
        {
            var entry = new ODataResource
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataUntypedValue(){RawValue="12.3"}},
                    },
            };

            var address_Info = new ODataNestedResourceInfo()
            {
                Name = "Address",
                IsCollection = false
            };

            var address = new ODataResource()
            {
                TypeName = "Server.NS.Address",
                Properties = new[]
                {
                    new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                    new ODataProperty{Name = "UndeclaredStreetNo", Value = new ODataUntypedValue(){RawValue="12.0"}},
                },
            };

            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(address_Info);
                writer.WriteStart(address);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreetNo"":12.0}}", result);
        }

        [Fact]
        public void WriteEntryUntypedStringTest()
        {
            var entry = new ODataResource
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                    },
            };

            var address_Info = new ODataNestedResourceInfo()
            {
                Name = "Address",
                IsCollection = false
            };

            var address = new ODataResource()
            {
                TypeName = "Server.NS.Address",
                Properties = new[]
                {
                    new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                    new ODataProperty{Name = "UndeclaredStreet", Value = new ODataUntypedValue(){RawValue=@"""No.10000000999,Zixing Rd Minhang"""}},
                },
            };

            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(address_Info);
                writer.WriteStart(address);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        [Fact]
        public void WriteEntryUntypedComplexTest()
        {
            var entry = new ODataResource
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "UndeclaredAddress1", Value = 
                            new ODataUntypedValue(){RawValue=@"{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}"}
                        },
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""UndeclaredAddress1"":{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}", result);
        }

        [Fact]
        public void WriteEntryUntypedCollectionTest()
        {
            var entry = new ODataResource
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "UndeclaredCollection1", Value = 
                            new ODataUntypedValue(){RawValue=@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]"}
                        },
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""UndeclaredCollection1"":[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]}", result);
        }

        [Fact]
        public void WriteEntryUntypedPrimitiveResourceCollectionTest()
        {
            string result = this.WriteUntypedCollectionsTest((ODataWriter writer) =>
            {
                writer.WriteStart(new ODataNestedResourceInfo { Name = "UntypedCollection", IsCollection = true });
                writer.WriteStart(new ODataResourceSet { TypeName = "Collection(Edm.Untyped)" });
                writer.Write(new ODataPrimitiveValue(1));
                writer.WriteStart((ODataResource)null);
                writer.WriteEnd(); // null value
                writer.Write(new ODataPrimitiveValue("abc"));
                writer.WriteEnd(); // resource set
                writer.WriteEnd(); // nested resource info
            },
            true );

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UntypedCollection"":[1,null,""abc""]}", result);
        }

        [Fact]
        public void WriteEntryUntypedResourceCollectionTest()
        {
            string result = this.WriteUntypedCollectionsTest((ODataWriter writer) =>
            {
                writer.WriteStart(new ODataNestedResourceInfo
                { Name = "UntypedCollection", IsCollection = true });   //  "UntypedCollection" :
                writer.WriteStart(new ODataResourceSet()                //  
                { TypeName = "Collection(Edm.Untyped)" });              //  [
                writer.Write(new ODataPrimitiveValue("string"));        //     "string",
                writer.WriteStart(new ODataResourceSet()                //  
                { TypeName = "Collection(Edm.Untyped)" });              //     [
                writer.Write((ODataResource)null);                      //        null,
                writer.Write(new ODataPrimitiveValue(1));               //        1,
                writer.WriteStart(new ODataResource()
                {
                    Properties = new ODataProperty[] {                  //        {
                        new ODataProperty {Name="prop",
                            Value = new ODataPrimitiveValue(1) },       //            "prop": 1,
                        new ODataProperty {Name="nullProp",
                            Value = ODataNullValue.Instance },             //            "nullProp": "null",
                        new ODataProperty {Name="collectionProp",       //            "collectionProp@odata.type":"#Collection(String)"
                            Value = new ODataCollectionValue {          //            "collectionProp":
                                TypeName="Collection(Edm.String)",      //             [
                                Items=new object[] {                    //
                                    "abc",                              //                  "abc",
                                    null                                //                  null
                                } } } } } );                            //             ],
                writer.WriteStart(new ODataNestedResourceInfo()
                { Name = "nestedCollection", IsCollection = true });    //             "nestedCollection" :
                writer.WriteStart(new ODataResourceSet()                //  
                { TypeName = "Collection(Edm.Untyped)" });              //             [
                writer.Write(new ODataPrimitiveValue(1));               //                  1,
                writer.Write((ODataResource)null);                      //                  null
                writer.WriteStart(new ODataResource());                 //                  {
                writer.WriteEnd();                                      //                  },
                writer.WriteStart(new ODataResourceSet());              //                  [
                writer.WriteEnd();                                      //                  ]
                writer.WriteEnd();                                      //             ]
                writer.WriteEnd();  // nestedCollection ResourceInfo
                writer.WriteEnd();                                      //         }
                writer.WriteEnd();                                      //      ]
                writer.WriteEnd();                                      //   ]
                writer.WriteEnd();  // untypedCollection ResourceInfo
            },
            true);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UntypedCollection"":[""string"",[null,1,{""prop"":1,""nullProp"":null,""collectionProp@odata.type"":""#Collection(String)"",""collectionProp"":[""abc"",null],""nestedCollection"":[1,null,{},[]]}]]}", result);
        }

        [Fact]
        public void WriteEntryUntypedNestedResourceCollectionTest()
        {
            string result = this.WriteUntypedCollectionsTest((ODataWriter writer) =>
            {
                writer.WriteStart(new ODataNestedResourceInfo
                { Name = "UntypedCollection", IsCollection = true });   // "UntypedCollection" :
                writer.WriteStart(new ODataResourceSet()                // 
                { TypeName = "Collection(Edm.Untyped)" });              // [    
                writer.Write((ODataResource)null);                      //    null,
                writer.WriteStart(new ODataResourceSet()
                { TypeName = "Collection(test.Undeclared)" });          //    [
                writer.Write((ODataResource)null);                      //       null,
                writer.Write(new ODataPrimitiveValue(1));               //       1,
                writer.Write((ODataResource)null);                      //       null,
                writer.Write(new ODataPrimitiveValue("abc"));           //       "abc"
                writer.WriteEnd();                                      //    ],
                writer.WriteStart(new ODataResourceSet()
                { TypeName = "Collection(Edm.Untyped)" });              //    [
                writer.Write(new ODataPrimitiveValue(2));               //       2,
                writer.Write(new ODataPrimitiveValue("def"));           //       "def",
                writer.Write((ODataResource)null);                      //       null,
                writer.WriteEnd();                                      //     ],
                writer.WriteStart(new ODataResourceSet()
                { TypeName = "Collection(Edm.Untyped)" });              //    [
                writer.WriteStart(new ODataResourceSet()
                { TypeName = "Collection(Edm.Int32)" });                //       [
                writer.Write(new ODataPrimitiveValue(3));               //         3,
                writer.Write((ODataResource)null);                      //         null,
                writer.WriteEnd();                                      //       ],
                writer.WriteStart(new ODataResourceSet()
                { TypeName = "Collection(Edm.Int32)" });                //       [
                writer.Write(new ODataPrimitiveValue(4));               //         4,
                writer.Write(new ODataPrimitiveValue(5));               //         5
                writer.WriteEnd();                                      //       ]
                writer.WriteEnd();                                      //     ]
                writer.WriteEnd();                                      //   ]
                writer.WriteEnd(); //nested ResourceInfo
            },
            true);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UntypedCollection"":[null,[null,1,null,""abc""],[2,""def"",null],[[3,null],[4,5]]]}", result);
        }


        #endregion

        #region open entity's property unknown name + known value type

        [Fact]
        public void WriteOpenEntryUndeclaredPropertiesWithNullValueTest()
        {
            var undeclaredNull = new ODataProperty { Name = "UndeclaredType1", Value = null };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredNull, true);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredType1"":null,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        [Fact]
        public void WriteOpenEntryUndeclaredPropertiesTest()
        {
            var undeclaredNull = new ODataProperty { Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D) };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredNull, true);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }
        [Fact]
        public void WriteOpenEntryUndeclaredComplexPropertiesTest()
        {
            var undeclaredComplex_Info = new ODataNestedResourceInfo()
            {
                Name = "UndeclaredComplexType1",
                IsCollection = false
            };

            var undeclaredComplex = new ODataResource()
            {
                TypeName = "Server.NS.Address",
                Properties = new[]
                {
                    new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.1000,Zixing Rd Minhang")},
                    new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.1001,Zixing Rd Minhang")},
                }
            };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredComplex_Info, undeclaredComplex, true);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredComplexType1"":{""@odata.type"":""#Server.NS.Address"",""Street"":""No.1000,Zixing Rd Minhang"",""UndeclaredStreet"":""No.1001,Zixing Rd Minhang""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        [Fact]
        public void WriteOpenEntryUndeclaredEmptyComplexPropertiesTest()
        {
            var undeclaredComplex_Info = new ODataNestedResourceInfo()
            {
                Name = "UndeclaredComplexType1",
                IsCollection = false
            };

            var undeclaredComplex = new ODataResource()
            {
                TypeName = "Server.NS.Address",
                Properties = new ODataProperty[] { },
            };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredComplex_Info, undeclaredComplex, true);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredComplexType1"":{""@odata.type"":""#Server.NS.Address""},""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        [Fact]
        public void WriteOpenEntryUndeclaredCollectionPropertiesTest()
        {
            var undeclaredCol = new ODataProperty
            {
                Name = "UndeclaredCollection1",
                Value = new ODataCollectionValue()
                {
                    TypeName = "Collection(Edm.String)",
                    Items = new[]
                    {
                        "mystr1",
                        "mystr2",
                        "mystr3"
                    }
                }
            };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredCol, true);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredCollection1@odata.type"":""#Collection(String)"",""UndeclaredCollection1"":[""mystr1"",""mystr2"",""mystr3""],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        [Fact]
        public void WriteOpenEntryUndeclaredEmptyCollectionPropertiesTest()
        {
            var undeclaredCol = new ODataProperty
            {
                Name = "UndeclaredCollection1",
                Value = new ODataCollectionValue()
                {
                    TypeName = "Collection(Edm.String)",
                    Items = new string[] { },
                }
            };

            string result = WriteNonOpenEntryUndeclaredPropertiesTest(undeclaredCol, true);

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredCollection1@odata.type"":""#Collection(String)"",""UndeclaredCollection1"":[],""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        #endregion

        #region open entity's property unknown name + unknown value type
        [Fact]
        public void WriteOpenEntryUntypedFloatDoubleTest()
        {
            var entry = new ODataResource
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataUntypedValue(){RawValue="12.3"}},
                    },
            };

            var address_Info = new ODataNestedResourceInfo()
            {
                Name = "Address",
                IsCollection = false
            };

            var address = new ODataResource()
            {
                TypeName = "Server.NS.Address",
                Properties = new[]
                {
                    new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                    new ODataProperty{Name = "UndeclaredStreetNo", Value = new ODataUntypedValue(){RawValue="12.0"}},
                },
            };

            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(address_Info);
                writer.WriteStart(address);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreetNo"":12.0}}", result);
        }

        [Fact]
        public void WriteOpenEntryUntypedStringTest()
        {
            var entry = new ODataResource
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                    },
            };

            var address_Info = new ODataNestedResourceInfo()
            {
                Name = "Address",
                IsCollection = false
            };

            var address = new ODataResource()
            {
                TypeName = "Server.NS.Address",
                Properties = new[]
                {
                    new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                    new ODataProperty{Name = "UndeclaredStreet", Value = new ODataUntypedValue(){RawValue=@"""No.10000000999,Zixing Rd Minhang"""}},
                },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteStart(address_Info);
                writer.WriteStart(address);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            });

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""Address"":{""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}}", result);
        }

        [Fact]
        public void WriteOpenEntryUntypedComplexTest()
        {
            var entry = new ODataResource
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "UndeclaredAddress1", Value = 
                            new ODataUntypedValue(){RawValue=@"{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}"}
                        },
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""UndeclaredAddress1"":{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}", result);
        }

        [Fact]
        public void WriteOpenEntryUntypedCollectionTest()
        {
            var entry = new ODataResource
            {
                TypeName = "Server.NS.ServerOpenEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "UndeclaredCollection1", Value = 
                            new ODataUntypedValue(){RawValue=@"[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]"}
                        },
                    },
            };
            string result = this.WriteEntryPayload(this.serverOpenEntitySet, this.serverOpenEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            Assert.Equal(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverOpenEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""UndeclaredCollection1"":[""email1@163.com"",""email2@gmail.com"",""email3@gmail2.com""]}", result);
        }

        #endregion

        private string WriteUntypedCollectionsTest(Action<ODataWriter> writeNestedCollection, bool isOpen)
        {
            var entry = new ODataResource
            {
                TypeName = isOpen ? "Server.NS.ServerOpenEntityType" : "Server.NS.ServerEntityType",
                Properties = new[]
                {
                    new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                }
            };

            return this.WriteEntryPayload(
                isOpen ? this.serverOpenEntitySet : this.serverEntitySet,
                isOpen ? this.serverOpenEntityType : this.serverEntityType,
                writer =>
                {
                    writer.WriteStart(entry);
                    writeNestedCollection(writer);
                    writer.WriteEnd();
                });
        }

        private string WriteNonOpenEntryUndeclaredPropertiesTest(ODataNestedResourceInfo undeclaredResourceInfo, ODataResource undeclaredResource, bool isOpen)
        {
            var entry = new ODataResource
            {
                TypeName = isOpen ? "Server.NS.ServerOpenEntityType" : "Server.NS.ServerEntityType",
                Properties = new[]
                {
                    new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                }
            };

            var address_Info = new ODataNestedResourceInfo()
            {
                Name = "Address",
                IsCollection = false
            };

            var address = new ODataResource()
            {
                TypeName = "Server.NS.Address",
                Properties = new[]
                {
                    new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                    new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                },
            };

            return this.WriteEntryPayload(
                isOpen ? this.serverOpenEntitySet : this.serverEntitySet,
                isOpen ? this.serverOpenEntityType : this.serverEntityType,
                writer =>
                {
                    writer.WriteStart(entry);
                    writer.WriteStart(undeclaredResourceInfo);
                    writer.WriteStart(undeclaredResource);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteStart(address_Info);
                    writer.WriteStart(address);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                });
        }

        private string WriteNonOpenEntryUndeclaredPropertiesTest(ODataProperty undeclaredProperty, bool isOpen)
        {
            var entry = new ODataResource
            {
                TypeName = isOpen ? "Server.NS.ServerOpenEntityType" : "Server.NS.ServerEntityType",
                Properties = new[]
                {
                    new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                    undeclaredProperty
                }
            };

            var address_Info = new ODataNestedResourceInfo()
            {
                Name = "Address",
                IsCollection = false
            };

            var address = new ODataResource()
            {
                TypeName = "Server.NS.Address",
                Properties = new[]
                {
                    new ODataProperty{Name = "Street", Value = new ODataPrimitiveValue("No.999,Zixing Rd Minhang")},
                    new ODataProperty{Name = "UndeclaredStreet", Value = new ODataPrimitiveValue("No.10000000999,Zixing Rd Minhang")},
                },
            };

            return this.WriteEntryPayload(
                isOpen ? this.serverOpenEntitySet : this.serverEntitySet,
                isOpen ? this.serverOpenEntityType : this.serverEntityType,
                writer =>
                {
                    writer.WriteStart(entry);
                    writer.WriteStart(address_Info);
                    writer.WriteStart(address);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                });
        }

        private string WriteEntryPayload(EdmEntitySet entitySet, EdmEntityType entityType, Action<ODataWriter> action)
        {
            MemoryStream stream = new MemoryStream();
            IODataResponseMessage message = new InMemoryMessage() { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            writerSettings.SetServiceDocumentUri(new Uri("http://www.sampletest.com"));
            using (var msgReader = new ODataMessageWriter((IODataResponseMessage)message, writerSettings, this.serverModel))
            {
                var writer = msgReader.CreateODataResourceWriter(entitySet, entityType);
                action(writer);

                stream.Seek(0, SeekOrigin.Begin);
                string payload = (new StreamReader(stream)).ReadToEnd();
                return payload;
            }
        }

        private ODataJsonOutputContext CreateJsonOutputContext(MemoryStream stream, bool writingResponse = true, bool setMetadataDocumentUri = true)
        {
            IEdmModel model = new EdmModel();

            ODataMessageWriterSettings settings = new ODataMessageWriterSettings
            {
                Version = ODataVersion.V4
            };
            if (setMetadataDocumentUri)
            {
                settings.SetServiceDocumentUri(this.metadataDocumentUri);
            }

            ODataMessageInfo messageInfo = new ODataMessageInfo
            {
                Model = model,
                IsAsync = false,
                IsResponse = false
            };
            return new ODataJsonOutputContext(messageInfo, settings);
            //return new ODataJsonOutputContext(
            //    ODataFormat.Json,
            //    new NonDisposingStream(stream),
            //    new ODataMediaType("application", "json"),
            //    Encoding.UTF8,
            //    settings,
            //    writingResponse,
            //    /*synchronous*/ true,
            //    model,
            //    /*urlResolver*/ null,
            //    /*container*/ null);
        }
    }
}
