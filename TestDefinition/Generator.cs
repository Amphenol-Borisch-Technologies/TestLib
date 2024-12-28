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
            TS ts;
            using (FileStream fileStream = new FileStream(TestDefinitionXML, FileMode.Open)) { ts = (TS)(new XmlSerializer(typeof(TS))).Deserialize(fileStream); }
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();

            for (Int32 testOperation = 0; testOperation < ts.TestOperations.Count; testOperation++) {
                CodeNamespace codeNamespace = GetNamespace(ts, testOperation);
                _ = codeCompileUnit.Namespaces.Add(codeNamespace);
                for (Int32 testGroup = 0; testGroup < ts.TestOperations[testOperation].TestGroups.Count; testGroup++) {
                    CodeTypeDeclaration codeTypeDeclaration = AddClass(codeNamespace, ts.TestOperations[testOperation].TestGroups[testGroup]);
                    for (Int32 method = 0; method < ts.TestOperations[testOperation].TestGroups[testGroup].Methods.Count; method++) {
                        AddMethod(codeTypeDeclaration, ts.TestOperations[testOperation], testGroup, method);
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

        private static CodeNamespace GetNamespace(TS ts, Int32 testOperation) {
            CodeNamespace codeNamespace = new CodeNamespace(ts.NamespaceRoot + "." + ts.TestOperations[testOperation].NamespaceTrunk);
            codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
            codeNamespace.Imports.Add(new CodeNamespaceImport("static ABT.Test.TestLib.TestDefinition.Assertions"));
            return codeNamespace;
        }

        private static CodeTypeDeclaration AddClass(CodeNamespace codeNamespace, TG tg) {
            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(tg.Class) {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.NotPublic | System.Reflection.TypeAttributes.Class,
            };
            _ = codeNamespace.Types.Add(codeTypeDeclaration);
            return codeTypeDeclaration;
        }


        private static void AddMethod(CodeTypeDeclaration codeTypeDeclaration, TO to, Int32 testGroup, Int32 method) {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod {
                Name = to.TestGroups[testGroup].Methods[method].Method,
                Attributes = MemberAttributes.Static | MemberAttributes.Assembly,
                ReturnType = new CodeTypeReference(typeof(String))
            };
            // Test Operation
            if (testGroup == 0 && method == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertionCurrent)to).AssertionCurrent()}"));

            // Test Groups
            if (method == 0) {
                if (testGroup == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{TS.DEBUG_ASSERT}{nameof(Assertions.TG_Prior)}{TS.BEGIN}{nameof(TG.Class)}{TS.CS}{TS.NONE}{TS.END}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(to.TestGroups[testGroup - 1]).AssertionPrior()}"));

                _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertionCurrent)to.TestGroups[testGroup]).AssertionCurrent()}"));

                if (testGroup < to.TestGroups.Count - 1) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(to.TestGroups[testGroup + 1]).AssertionNext()}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{TS.DEBUG_ASSERT}{nameof(Assertions.TG_Next)}{TS.BEGIN}{nameof(TG.Class)}{TS.CS}{TS.NONE}{TS.END}"));
            }

            // Methods
            {
                if (method == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{TS.DEBUG_ASSERT}{nameof(Assertions.M_Prior)}{TS.BEGIN}{nameof(M.Method)}{TS.CS}{TS.NONE}{TS.END}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(to.TestGroups[testGroup].Methods[method - 1]).AssertionPrior()}"));

                _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertionCurrent)to.TestGroups[testGroup].Methods[method]).AssertionCurrent()}"));

                if (method < to.TestGroups[testGroup].Methods.Count - 1) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(to.TestGroups[testGroup].Methods[method + 1]).AssertionNext()}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{TS.DEBUG_ASSERT}{nameof(Assertions.M_Next)}{TS.BEGIN}{nameof(M.Method)}{TS.CS}{TS.NONE}{TS.END}"));
            }

            _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement("\t\t\treturn String.Empty;"));
            _ = codeTypeDeclaration.Members.Add(codeMemberMethod);
        }
    }
}
