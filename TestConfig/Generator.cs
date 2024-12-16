using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml.Serialization;
using Microsoft.CSharp;

namespace ABT.Test.TestLib.TestConfig {

    public static class Generator {
        public static void Generate(String FileImplementationCSharp) {
            if (!Directory.Exists(Path.GetDirectoryName(FileImplementationCSharp))) throw new ArgumentException($"Folder '{Path.GetDirectoryName(FileImplementationCSharp)}' does not exist.");
            TO to;
            using (FileStream fileStream = new FileStream(FileImplementationCSharp, FileMode.Create)) { to = (TO)(new XmlSerializer(typeof(TO))).Deserialize(fileStream); }

            CodeNamespace codeNameSpace = new CodeNamespace(to.Namespace);
            codeNameSpace.Imports.Add(new CodeNamespaceImport("System"));
            codeNameSpace.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));
            codeNameSpace.Imports.Add(new CodeNamespaceImport("static ABT.Test.TestLib.TestConfig "));
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            _ = codeCompileUnit.Namespaces.Add(codeNameSpace);

            for (Int32 testGroup = 0; testGroup < to.TestGroups.Count; testGroup++) {
                CodeTypeDeclaration codeTypeDeclaration = AddClass(codeNameSpace, to.TestGroups[testGroup]);
                for (Int32 method = 0; method < to.TestGroups[testGroup].Methods.Count; method++) {
                    AddMethod(codeTypeDeclaration, to, testGroup, method);
                }
            }

            CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();
            CodeGeneratorOptions codeGeneratorOptions = new CodeGeneratorOptions {
                BlankLinesBetweenMembers = true,
                BracingStyle = "Block",
                IndentString = "    "
            };

            using (StreamWriter streamWriter = new StreamWriter(FileImplementationCSharp)) { cSharpCodeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, streamWriter, codeGeneratorOptions); }
        }

        private static CodeTypeDeclaration AddClass(CodeNamespace codeNameSpace, TG tg) {
            CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(tg.Class) {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.NotPublic | System.Reflection.TypeAttributes.Class,
            };
            _ = codeNameSpace.Types.Add(codeTypeDeclaration);
            return codeTypeDeclaration;
        }


        private static void AddMethod(CodeTypeDeclaration codeTypeDeclaration, TO to, Int32 testGroup, Int32 method) {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod {
                Name = to.TestGroups[testGroup].Methods[method].Method,
                Attributes = MemberAttributes.Static,
                ReturnType = new CodeTypeReference(typeof(String))
            };
            // Test Operation
            if (testGroup == 0 && method == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertionCurrent)to).AssertionCurrent()}"));

            // Test Groups
            if (method == 0) {
                if (testGroup == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{TO.DEBUG_ASSERT}{nameof(Assertions.TG_Prior)}{TO.BEGIN}{nameof(TG.Class)}{TO.CS}{TO.NONE}{TO.END}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(to.TestGroups[testGroup - 1]).AssertionPrior()}"));

                _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertionCurrent)to.TestGroups[testGroup]).AssertionCurrent()}"));

                if (testGroup < to.TestGroups.Count - 1) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(to.TestGroups[testGroup + 1]).AssertionNext()}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{TO.DEBUG_ASSERT}{nameof(Assertions.TG_Next)}{TO.BEGIN}{nameof(TG.Class)}{TO.CS}{TO.NONE}{TO.END}"));
            }

            // Methods
            {
                if (method == 0) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{TO.DEBUG_ASSERT}{nameof(Assertions.M_Prior)}{TO.BEGIN}{nameof(M.Method)}{TO.CS}{TO.NONE}{TO.END}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(to.TestGroups[testGroup].Methods[method - 1]).AssertionPrior()}"));

                _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{((IAssertionCurrent)to.TestGroups[testGroup].Methods[method]).AssertionCurrent()}"));

                if (method < to.TestGroups[testGroup].Methods.Count - 1) _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{(to.TestGroups[testGroup].Methods[method + 1]).AssertionNext()}"));
                else _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement($"\t\t\t{TO.DEBUG_ASSERT}{nameof(Assertions.M_Next)}{TO.BEGIN}{nameof(M.Method)}{TO.CS}{TO.NONE}{TO.END}"));
            }

            _ = codeMemberMethod.Statements.Add(new CodeSnippetStatement("\t\t\treturn String.Empty;"));
            _ = codeTypeDeclaration.Members.Add(codeMemberMethod);
        }
    }
}
