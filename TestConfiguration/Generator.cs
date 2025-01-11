using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Windows.Forms;
using Microsoft.CSharp;
using static ABT.Test.TestLib.TestLib;

namespace ABT.Test.TestLib.TestConfiguration {
    // TODO:  Eventually; modify TestDefinition.xsd, TestDefinition.xml, Generator & Validator to accomodate ABT.Test.TestPlans.Diagnostics.InstrumentsDrivers.ID.
    // - Enter Instrument aliases in TestDefinition.xml, Generator auto-generates ID alises in TestPlan folder.

    public static class Generator {

        public static void Generate(String TestDefinitionXML) {
            GenerateImplementation(TestDefinitionXML);
            GenerateIDs(TestDefinitionXML);
        }

        public static void GenerateImplementation(String TestDefinitionXML) {
            TestSpace testSpace = Serializing.DeserializeFromFile<TestSpace>(TestDefinitionXML);
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();

            for (Int32 testOperation = 0; testOperation < testSpace.TestOperations.Count; testOperation++) {
                CodeNamespace codeNamespace = GetNamespace(testSpace, testOperation);
                _ = codeCompileUnit.Namespaces.Add(codeNamespace);
                for (Int32 testGroup = 0; testGroup < testSpace.TestOperations[testOperation].TestGroups.Count; testGroup++) {
                    CodeTypeDeclaration codeTypeDeclaration = AddClass(codeNamespace, testSpace.TestOperations[testOperation].TestGroups[testGroup]);
                    for (Int32 method = 0; method < testSpace.TestOperations[testOperation].TestGroups[testGroup].Methods.Count; method++) {
                        AddMethod(codeTypeDeclaration, testSpace.TestOperations[testOperation], testGroup, method);
                    }
                }
            }

            CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
            CodeGeneratorOptions codeGeneratorOptions = new CodeGeneratorOptions {
                BlankLinesBetweenMembers = true,
                BracingStyle = "Block",
                IndentString = "    "
            };

            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "C# files (*.cs)|*.cs",
                Title = "Save the generated Test Program C# file",
                DefaultExt = "cs",
                FileName = "TestImplementation.cs",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName)) { cSharpCodeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, streamWriter, codeGeneratorOptions); }
            }
        }

        private static CodeNamespace GetNamespace(TestSpace testSpace, Int32 testOperation) {
            CodeNamespace codeNamespace = new CodeNamespace(testSpace.NamespaceRoot + "." + testSpace.TestOperations[testOperation].NamespaceTrunk);
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("ABT.Test.TestLib"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("ABT.Test.TestLib.TestConfiguration"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("static ABT.Test.TestLib.TestConfiguration.Assertions"));
            return codeNamespace;
        }

        private static CodeTypeDeclaration AddClass(CodeNamespace codeNamespace, TestGroup testGroup) {
            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(testGroup.Classname) {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.NotPublic | System.Reflection.TypeAttributes.Class,
            };
            _ = codeNamespace.Types.Add(codeTypeDeclaration);
            return codeTypeDeclaration;
        }

        private static void AddMethod(CodeTypeDeclaration codeTypeDeclaration, TestOperation testOperation, Int32 testGroup, Int32 method) {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod {
                Name = testOperation.TestGroups[testGroup].Methods[method].Name,
                Attributes = MemberAttributes.Static | MemberAttributes.Assembly,
                ReturnType = new CodeTypeReference(typeof(String))
            };
            // Test Operation
            if (testGroup == 0 && method == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertionCurrent)testOperation).AssertionCurrent()}"));

            // Test Groups
            if (method == 0) {
                if (testGroup == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{UUT.DEBUG_ASSERT}{nameof(Assertions.TestGroupPrior)}{UUT.BEGIN}{nameof(TestGroup.Classname)}{UUT.CS}{UUT.NONE}{UUT.END}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(testOperation.TestGroups[testGroup - 1]).AssertionPrior()}"));

                _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertionCurrent)testOperation.TestGroups[testGroup]).AssertionCurrent()}"));

                if (testGroup < testOperation.TestGroups.Count - 1) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(testOperation.TestGroups[testGroup + 1]).AssertionNext()}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{UUT.DEBUG_ASSERT}{nameof(Assertions.TestGroupNext)}{UUT.BEGIN}{nameof(TestGroup.Classname)}{UUT.CS}{UUT.NONE}{UUT.END}"));
            }

            { // Methods
                if (method == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{UUT.DEBUG_ASSERT}{nameof(Assertions.MethodPrior)}{UUT.BEGIN}{nameof(Method.Name)}{UUT.CS}{UUT.NONE}{UUT.END}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(testOperation.TestGroups[testGroup].Methods[method - 1]).AssertionPrior()}"));

                _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertionCurrent)testOperation.TestGroups[testGroup].Methods[method]).AssertionCurrent()}"));

                if (method < testOperation.TestGroups[testGroup].Methods.Count - 1) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(testOperation.TestGroups[testGroup].Methods[method + 1]).AssertionNext()}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{UUT.DEBUG_ASSERT}{nameof(Assertions.MethodNext)}{UUT.BEGIN}{nameof(Method.Name)}{UUT.CS}{UUT.NONE}{UUT.END}"));
            }

            Method m = testOperation.TestGroups[testGroup].Methods[method];
            if (m is MethodCustom) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\treturn nameof({typeof(EVENTS).Name}.{EVENTS.UNSET});"));
            else if (m is MethodInterval) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\treturn \"{Double.NaN}\";"));
            else if (m is MethodProcess) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\treturn \"{-1}\";"));
            else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\treturn \"{String.Empty}\";"));
            _ = codeTypeDeclaration.Members.Add(codeMemberMethod);
        }

        public static void GenerateIDs(String TestDefinitionXML) {
            Instruments instruments = Serializing.DeserializeFromFile<Instruments>(TestDefinitionXML);
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace("ABT.Test.TestPlans.Diagnostics.InstrumentsDrivers");
            codeNamespace.Imports.Add(new CodeNamespaceImport("ABT.Test.TestLib.InstrumentDrivers.Multifunction"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("ABT.Test.TestLib.InstrumentDrivers.PowerSupplies"));
            _ = codeCompileUnit.Namespaces.Add(codeNamespace);

            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration("ID") {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.NotPublic,
                Attributes = MemberAttributes.Static
            };

            codeNamespace.Types.Add(codeTypeDeclaration);

            foreach (InstrumentInfo instrumentInfo in instruments.GetInfo()) codeTypeDeclaration.Members.Add(CreateField(instrumentInfo));

            CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
            CodeGeneratorOptions codeGeneratorOptions = new CodeGeneratorOptions {
                BlankLinesBetweenMembers = false,
                BracingStyle = "Block",
                IndentString = "    "
            };

            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "C# files (*.cs)|*.cs",
                Title = "Save the generated Instrument IDs C# file",
                DefaultExt = "cs",
                FileName = "ID.cs",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName)) { cSharpCodeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, streamWriter, codeGeneratorOptions); }
            }
        }

        private static CodeMemberField CreateField(InstrumentInfo instrumentInfo) {
            Int32 i = instrumentInfo.NameSpacedClassName.LastIndexOf(".");
            String Classname = instrumentInfo.NameSpacedClassName.Substring(i + 1);

            CodeMemberField field = new CodeMemberField {
                Attributes = MemberAttributes.Static | MemberAttributes.Assembly,
                Type = new CodeTypeReference(Classname),
                Name = instrumentInfo.Alias,
                InitExpression = new CodeCastExpression(Classname,
                    new CodeIndexerExpression(
                        new CodePropertyReferenceExpression(new CodeTypeReferenceExpression("TestLib.TestLib"), nameof(InstrumentDrivers)),
                        new CodePrimitiveExpression(instrumentInfo.ID)))
            };

            return field;
        }
    }
}
