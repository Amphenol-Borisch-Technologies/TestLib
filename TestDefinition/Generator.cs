using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using Microsoft.CSharp;

namespace ABT.Test.TestLib.TestDefinition {

    public static class Generator {
        public static void Generate(String TestDefinitionXML) {
            if (!Directory.Exists(Path.GetDirectoryName(TestDefinitionXML))) throw new ArgumentException($"Folder '{Path.GetDirectoryName(TestDefinitionXML)}' does not exist.");
            TestSpace testSpace;
            using (FileStream fileStream = new FileStream(TestDefinitionXML, FileMode.Open)) { testSpace = (TestSpace)(new XmlSerializer(typeof(TestSpace))).Deserialize(fileStream); }
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
            codeNamespace.Imports.Add(new CodeNamespaceImport("static ABT.Test.TestLib.TestDefinition.Assertions"));
            return codeNamespace;
        }

        private static CodeTypeDeclaration AddClass(CodeNamespace codeNamespace, TestGroup testGroup) {
            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(testGroup.Class) {
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
                if (testGroup == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{UUT.DEBUG_ASSERT}{nameof(Assertions.TestGroupPrior)}{UUT.BEGIN}{nameof(TestGroup.Class)}{UUT.CS}{UUT.NONE}{UUT.END}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(testOperation.TestGroups[testGroup - 1]).AssertionPrior()}"));

                _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertionCurrent)testOperation.TestGroups[testGroup]).AssertionCurrent()}"));

                if (testGroup < testOperation.TestGroups.Count - 1) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(testOperation.TestGroups[testGroup + 1]).AssertionNext()}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{UUT.DEBUG_ASSERT}{nameof(Assertions.TestGroupNext)}{UUT.BEGIN}{nameof(TestGroup.Class)}{UUT.CS}{UUT.NONE}{UUT.END}"));
            }

            // Methods
            {
                if (method == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{UUT.DEBUG_ASSERT} {nameof(Assertions.MethodPrior)} {UUT.BEGIN} {nameof(Method.Name)} {UUT.CS} {UUT.NONE}{UUT.END}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(testOperation.TestGroups[testGroup].Methods[method - 1]).AssertionPrior()}"));

                _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertionCurrent)testOperation.TestGroups[testGroup].Methods[method]).AssertionCurrent()}"));

                if (method < testOperation.TestGroups[testGroup].Methods.Count - 1) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(testOperation.TestGroups[testGroup].Methods[method + 1]).AssertionNext()}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{UUT.DEBUG_ASSERT}{nameof(Assertions.MethodNext)}{UUT.BEGIN}{nameof(Method.Name)}{UUT.CS}{UUT.NONE}{UUT.END}"));
            }

            _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement("\t\t\treturn String.Empty;"));
            _ = codeTypeDeclaration.Members.Add(codeMemberMethod);
        }
    }
}
