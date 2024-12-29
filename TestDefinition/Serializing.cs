using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ABT.Test.TestLib.TestDefinition {

    public static class Serializing {
        public static void Serialize(TestSpace testSpace, String TestDefinitionXML) {
            if (!Directory.Exists(Path.GetDirectoryName(TestDefinitionXML))) throw new ArgumentException($"Folder '{Path.GetDirectoryName(TestDefinitionXML)}' does not exist.");
            using (FileStream fileStream = new FileStream(TestDefinitionXML, FileMode.Create)) new XmlSerializer(typeof(TestSpace)).Serialize(fileStream, testSpace);
        }

        public static TestSpace Deserialize(String TestDefinitionXML) {
            if (!File.Exists(TestDefinitionXML)) throw new ArgumentException($"XML Test Specification File '{TestDefinitionXML}' does not exist.");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestSpace));
            using (FileStream fileStream = new FileStream(TestDefinitionXML, FileMode.Open)) return (TestSpace)xmlSerializer.Deserialize(fileStream);
        }

        public static String Format(TestSpace testSpace) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{nameof(TestSpace.NamespaceRoot)}   : {testSpace.NamespaceRoot}");
            stringBuilder.AppendLine($"{nameof(TestOperation.Description)} : {testSpace.Description}");
            foreach (TestOperation testOperation in testSpace.TestOperations) {
                stringBuilder.AppendLine($"{nameof(TestOperation.NamespaceTrunk)} : {testOperation.NamespaceTrunk}");
                stringBuilder.AppendLine($"{nameof(TestOperation.Description)}    : {testOperation.Description}");
                foreach (TestGroup testGroup in testOperation.TestGroups) {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine($"\t{nameof(TestGroup.Class)}           : {testGroup.Class}");
                    stringBuilder.AppendLine($"\t{nameof(TestGroup.Description)}     : {testGroup.Description}");
                    stringBuilder.AppendLine($"\t{nameof(TestGroup.CancelNotPassed)} : {testGroup.CancelNotPassed}");
                    stringBuilder.AppendLine($"\t{nameof(TestGroup.Independent)}     : {testGroup.Independent}");
                    foreach (M m in testGroup.Methods) {
                        stringBuilder.AppendLine($"\t\t\t{nameof(M.Method)}: {m.Method}, {nameof(M.Description)}: {m.Description}, {nameof(M.CancelNotPassed)}: {m.CancelNotPassed}");
                        if (m is MethodCustom mc) foreach (Parameter p in mc.Parameters) stringBuilder.AppendLine($"    {nameof(Parameter)} {nameof(Parameter.Key)}: {p.Key}, {nameof(Parameter.Value)}: {p.Value}");
                        else if (m is MethodInterval mi) stringBuilder.AppendLine($"\t\t\t\t{nameof(MethodInterval.LowComparator)}: {mi.LowComparator}, {nameof(MethodInterval.Low)}: {mi.Low}, {nameof(MethodInterval.High)}: {mi.High}, {nameof(MethodInterval.HighComparator)}: {mi.HighComparator}, {nameof(MethodInterval.FractionalDigits)}: {mi.FractionalDigits}, {nameof(MethodInterval.UnitPrefix)}: {mi.UnitPrefix}, {nameof(MethodInterval.Units)}: {mi.Units}, {nameof(MethodInterval.UnitSuffix)}: {mi.UnitSuffix}");
                        else if (m is MP mp) stringBuilder.AppendLine($"\t\t\t\t{nameof(MP.Path)}: {mp.Path}, {nameof(MP.Executable)}: {mp.Executable}, {nameof(MP.Parameters)}: {mp.Parameters}, {nameof(MP.Expected)}: {mp.Expected}");
                        else if (m is MT mt) stringBuilder.AppendLine($"\t\t\t\t{nameof(MT.Text)}: {mt.Text}");
                        else {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine($"Method '{nameof(M.Method)}' not implemented:");
                            sb.AppendLine($"\t{nameof(TestOperation.NamespaceTrunk)} : {testOperation.NamespaceTrunk}");
                            sb.AppendLine($"\t{nameof(TestOperation.Description)}    : {testOperation.Description}");
                            sb.AppendLine($"\t\t{nameof(TestGroup.Class)}            : {testGroup.Class}");
                            sb.AppendLine($"\t\t{nameof(TestGroup.Description)}      : {testGroup.Description}");
                            sb.AppendLine($"\t\t{nameof(TestGroup.CancelNotPassed)}  : {testGroup.CancelNotPassed}");
                            sb.AppendLine($"\t\t{nameof(TestGroup.Independent)}      : {testGroup.Independent}");
                            sb.AppendLine($"\t\t\t{nameof(M.Method)}                 : {m.Method}");
                            sb.AppendLine($"\t\t\t{nameof(M.Description)}            : {m.Description}");
                            sb.AppendLine($"\t\t\t{nameof(M.CancelNotPassed)}        : {m.CancelNotPassed}");
                            throw new NotImplementedException(sb.ToString());
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
}
