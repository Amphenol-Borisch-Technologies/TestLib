
namespace ABT.Test.TestLib.TestConfiguration {
    // TODO:  Eventually; modify TestDefinition.xsd, TestDefinition.xml, Generator & Validator to accomodate ABT.Test.TestPlans.Diagnostics.InstrumentsDrivers.ID.
    // - Enter Instrument aliases in TestDefinition.xml, Generator auto-generates ID alises in TestPlan folder.
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using Microsoft.CSharp;
    using System.IO;

    public class CodeGenerator {
        public static void GenerateCode() {
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            CodeNamespace testNamespace = new CodeNamespace("ABT.Test.TestPlans.Diagnostics.InstrumentsDrivers");
            compileUnit.Namespaces.Add(testNamespace);

            CodeTypeDeclaration classID = new CodeTypeDeclaration("ID") {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.NotPublic,
                Attributes = MemberAttributes.Static
            };

            testNamespace.Types.Add(classID);

            // Adding fields
            classID.Members.Add(CreateField("PS_E3634A_SCPI_NET", "V28_IN", "PS3_E3634A"));
            classID.Members.Add(CreateField("PS_E3649A_SCPI_NET", "SEAL", "PS1ε2_E3649A"));
            // classID.Members.Add(CreateField("MSO_3014_IVI_COM", "MSO", "MSO1_3014"));
            classID.Members.Add(CreateField("MSMU_34980A_SCPI_NET", "MSMU", "MSMU1_34980A"));
            // classID.Members.Add(CreateField("MM_34401A_SCPI_NET", "MM", "MM1_34401A"));

            CSharpCodeProvider provider = new CSharpCodeProvider();
            using (StreamWriter sourceWriter = new StreamWriter("ID.cs")) {
                CodeGeneratorOptions options = new CodeGeneratorOptions { BracingStyle = "C" };
                provider.GenerateCodeFromCompileUnit(compileUnit, sourceWriter, options);
            }
        }

        private static CodeMemberField CreateField(string typeName, string fieldName, string instrumentName) {
            CodeMemberField field = new CodeMemberField {
                Attributes = MemberAttributes.Static | MemberAttributes.Assembly,
                Type = new CodeTypeReference(typeName),
                Name = fieldName,
                InitExpression = new CodeCastExpression(typeName,
                    new CodeIndexerExpression(
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression("TestLib.TestLib"), "InstrumentDrivers"),
                        new CodePrimitiveExpression(instrumentName)))
            };

            return field;
        }
    }
}
