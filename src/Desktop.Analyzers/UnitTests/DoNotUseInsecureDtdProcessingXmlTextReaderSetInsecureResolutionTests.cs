// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Test.Utilities;
using Xunit;

namespace Desktop.Analyzers.UnitTests
{
    public partial class DoNotUseInsecureDtdProcessingAnalyzerTests : DiagnosticAnalyzerTestBase
    {
        private static readonly string s_CA3075XmlTextReaderSetInsecureResolutionMessage = DesktopAnalyzersResources.XmlTextReaderSetInsecureResolutionMessage;

        private DiagnosticResult GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(int line, int column)
        {
            return GetCSharpResultAt(line, column, CA3075RuleId, s_CA3075XmlTextReaderSetInsecureResolutionMessage);
        }

        private DiagnosticResult GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(int line, int column)
        {
            return GetBasicResultAt(line, column, CA3075RuleId, s_CA3075XmlTextReaderSetInsecureResolutionMessage);
        }

        [Fact]
        public void UseXmlTextReaderNoCtorShouldNotGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(XmlTextReader reader)
        {
            var count = reader.AttributeCount;
        }
    }
}"
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(reader As XmlTextReader)
            Dim count = reader.AttributeCount
        End Sub
    End Class
End Namespace");
        }

        [Fact]
        public void XmlTextReaderNoCtorSetResolverToNullShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(XmlTextReader reader)
        {
            reader.XmlResolver = new XmlUrlResolver();
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(10, 13)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(reader As XmlTextReader)
            reader.XmlResolver = New XmlUrlResolver()
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(7, 13)
            );
        }

        [Fact]
        public void XmlTextReaderNoCtorSetDtdProcessingToParseShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(XmlTextReader reader)
        {
            reader.DtdProcessing = DtdProcessing.Parse;
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(10, 13)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(reader As XmlTextReader)
            reader.DtdProcessing = DtdProcessing.Parse
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(7, 13)
            );
        }

        [Fact]
        public void XmlTextReaderNoCtorSetBothToInsecureValuesShouldGenerateDiagnostics()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(XmlTextReader reader, XmlUrlResolver resolver)
        {
            reader.XmlResolver = resolver;
            reader.DtdProcessing = DtdProcessing.Parse;
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(10, 13),
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(11, 13)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(reader As XmlTextReader, resolver As XmlUrlResolver)
            reader.XmlResolver = resolver
            reader.DtdProcessing = DtdProcessing.Parse
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(7, 13),
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(8, 13)
            );
        }

        [Fact]
        public void XmlTextReaderNoCtorSetInSecureResolverInTryClauseShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(XmlTextReader reader)
        {
            try
            {
                reader.XmlResolver = new XmlUrlResolver();
            }
            catch { throw; }
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(12, 17)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(reader As XmlTextReader)
            Try
                reader.XmlResolver = New XmlUrlResolver()
            Catch
                Throw
            End Try
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(8, 17)
            );
        }

        [Fact]
        public void XmlTextReaderNoCtorSetInSecureResolverInCatchBlockShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(XmlTextReader reader)
        {
            try {   }
            catch { reader.XmlResolver = new XmlUrlResolver(); }
            finally {}
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(11, 21)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(reader As XmlTextReader)
            Try
            Catch
                reader.XmlResolver = New XmlUrlResolver()
            Finally
            End Try
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(9, 17)
            );
        }

        [Fact]
        public void XmlTextReaderNoCtorSetInSecureResolverInFinallyBlockShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(XmlTextReader reader)
        {
            try {   }
            catch { throw; }
            finally { reader.XmlResolver = new XmlUrlResolver(); }
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(12, 23)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(reader As XmlTextReader)
            Try
            Catch
                Throw
            Finally
                reader.XmlResolver = New XmlUrlResolver()
            End Try
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(11, 17)
            );
        }

        [Fact]
        public void XmlTextReaderNoCtorSetDtdprocessingToParseInTryClauseShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(XmlTextReader reader)
        {
            try
            {
                reader.DtdProcessing = DtdProcessing.Parse;
            }
            catch { throw; }
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(12, 17)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(reader As XmlTextReader)
            Try
                reader.DtdProcessing = DtdProcessing.Parse
            Catch
                Throw
            End Try
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(8, 17)
            );
        }

        [Fact]
        public void XmlTextReaderNoCtorSetDtdprocessingToParseInCatchBlockShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(XmlTextReader reader)
        {
            try {  }
            catch { reader.DtdProcessing = DtdProcessing.Parse; }
            finally {   }
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(11, 21)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(reader As XmlTextReader)
            Try
            Catch
                reader.DtdProcessing = DtdProcessing.Parse
            Finally
            End Try
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(9, 17)
            );
        }

        [Fact]
        public void XmlTextReaderNoCtorSetDtdprocessingToParseInFinallyBlockShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(XmlTextReader reader)
        {
            try {  }
            catch { throw; }
            finally { reader.DtdProcessing = DtdProcessing.Parse; }
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(12, 23)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(reader As XmlTextReader)
            Try
            Catch
                Throw
            Finally
                reader.DtdProcessing = DtdProcessing.Parse
            End Try
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(11, 17)
            );
        }

        [Fact]
        public void ConstructXmlTextReaderSetInsecureResolverInInitializerShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(string path)
        {
            XmlTextReader doc = new XmlTextReader(path)
            {
                XmlResolver = new XmlUrlResolver()
            };
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(10, 33)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(path As String)
            Dim doc As New XmlTextReader(path) With { _
                .XmlResolver = New XmlUrlResolver() _
            }
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(7, 24)
            );
        }

        [Fact]
        public void ConstructXmlTextReaderSetDtdProcessingParseInInitializerShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(string path)
        {
            XmlTextReader doc = new XmlTextReader(path)
            {
                DtdProcessing = DtdProcessing.Parse
            };
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(10, 33)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(path As String)
            Dim doc As New XmlTextReader(path) With { _
                .DtdProcessing = DtdProcessing.Parse _
            }
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(7, 24)
            );
        }

        [Fact]
        public void ConstructXmlTextReaderSetBothToInsecureValuesInInitializerShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {
        private static void TestMethod(string path)
        {
            XmlTextReader doc = new XmlTextReader(path)
            {
                DtdProcessing = DtdProcessing.Parse,
                XmlResolver = new XmlUrlResolver()
            };
        }
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(10, 33)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass
        Private Shared Sub TestMethod(path As String)
            Dim doc As New XmlTextReader(path) With { _
                .DtdProcessing = DtdProcessing.Parse, _
                .XmlResolver = New XmlUrlResolver() _
            }
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(7, 24)
            );
        }

        [Fact]
        public void XmlTextReaderDerivedTypeSetInsecureResolverShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System;
using System.Xml;

namespace TestNamespace
{
    class DerivedType : XmlTextReader {}   

    class TestClass
    {
        void TestMethod()
        {
            var c = new DerivedType(){ XmlResolver = new XmlUrlResolver() };
        }
    }
    
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(13, 21)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class DerivedType
        Inherits XmlTextReader
    End Class

    Class TestClass
        Private Sub TestMethod()
            Dim c = New DerivedType() With { _
                .XmlResolver = New XmlUrlResolver() _
            }
        End Sub
    End Class

End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(11, 21)
            );
        }

        [Fact]
        public void XmlTextReaderDerivedTypeSetDtdProcessingParseShouldGenerateDiagnostic()
        {
            VerifyCSharp(@"
using System;
using System.Xml;

namespace TestNamespace
{
    class DerivedType : XmlTextReader {}   

    class TestClass
    {
        void TestMethod()
        {
            var c = new DerivedType(){ DtdProcessing = DtdProcessing.Parse };
        }
    }
    
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(13, 21)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class DerivedType
        Inherits XmlTextReader
    End Class

    Class TestClass
        Private Sub TestMethod()
            Dim c = New DerivedType() With { _
                .DtdProcessing = DtdProcessing.Parse _
            }
        End Sub
    End Class

End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(11, 21)
            );
        }

        [Fact]
        public void XmlTextReaderCreatedAsTempSetSecureSettingsShouldNotGenerateDiagnostics()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {

        public void Method1(string path)
        {
            Method2(new XmlTextReader(path){ XmlResolver = null, DtdProcessing = DtdProcessing.Prohibit });
        }

        public void Method2(XmlTextReader reader){}
    }
}"
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass

        Public Sub Method1(path As String)
            Method2(New XmlTextReader(path) With { _
                .XmlResolver = Nothing, _
                .DtdProcessing = DtdProcessing.Prohibit _
            })
        End Sub

        Public Sub Method2(reader As XmlTextReader)
        End Sub
    End Class
End Namespace");
        }

        [Fact]
        public void XmlTextReaderCreatedAsTempSetInsecureResolverShouldGenerateDiagnostics()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {

        public void Method1(string path)
        {
            Method2(new XmlTextReader(path){ XmlResolver = new XmlUrlResolver(), DtdProcessing = DtdProcessing.Prohibit });
        }

        public void Method2(XmlTextReader reader){}
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(11, 21)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass

        Public Sub Method1(path As String)
            Method2(New XmlTextReader(path) With { _
                .XmlResolver = New XmlUrlResolver(), _
                .DtdProcessing = DtdProcessing.Prohibit _
            })
        End Sub

        Public Sub Method2(reader As XmlTextReader)
        End Sub
    End Class
End Namespace
",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(8, 21)
            );
        }

        [Fact]
        public void XmlTextReaderCreatedAsTempSetDtdProcessingParseShouldGenerateDiagnostics()
        {
            VerifyCSharp(@"
using System.Xml;

namespace TestNamespace
{
    class TestClass
    {

        public void Method1(string path)
        {
            Method2(new XmlTextReader(path){ XmlResolver = null, DtdProcessing = DtdProcessing.Parse });
        }

        public void Method2(XmlTextReader reader){}
    }
}",
                GetCA3075XmlTextReaderSetInsecureResolutionCSharpResultAt(11, 21)
            );

            VerifyBasic(@"
Imports System.Xml

Namespace TestNamespace
    Class TestClass

        Public Sub Method1(path As String)
            Method2(New XmlTextReader(path) With { _
                .XmlResolver = Nothing, _
                .DtdProcessing = DtdProcessing.Parse _
            })
        End Sub

        Public Sub Method2(reader As XmlTextReader)
        End Sub
    End Class
End Namespace",
                GetCA3075XmlTextReaderSetInsecureResolutionBasicResultAt(8, 21)
            );
        }
    }
}
