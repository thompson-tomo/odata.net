﻿//---------------------------------------------------------------------
// <copyright file="ShippingAssemblyAttributes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Tests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.OData.Edm.Tests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Microsoft.Test.OData.Tests.Client" + AssemblyRef.TestPublicKey)]
#pragma warning disable 436
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EdmLibTests" + AssemblyRef.TestPublicKey)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("EdmLibTests.SL" + AssemblyRef.TestPublicKey)]
#pragma warning restore 436
