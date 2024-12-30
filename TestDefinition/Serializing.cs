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
                    foreach (Method m in testGroup.Methods) {
                        stringBuilder.AppendLine($"\t\t\t{nameof(m.Name)}: {m.Name}, {nameof(m.Description)}: {m.Description}, {nameof(m.CancelNotPassed)}: {m.CancelNotPassed}");
                        if (m is MethodCustom mc) foreach (Parameter p in mc.Parameters) stringBuilder.AppendLine($"    {nameof(Parameter)} {nameof(Parameter.Key)}: {p.Key}, {nameof(Parameter.Value)}: {p.Value}");
                        else if (m is MethodInterval mi) stringBuilder.AppendLine($"\t\t\t\t{nameof(mi.LowComparator)}: {mi.LowComparator}, {nameof(mi.Low)}: {mi.Low}, {nameof(mi.High)}: {mi.High}, {nameof(mi.HighComparator)}: {mi.HighComparator}, {nameof(mi.FractionalDigits)}: {mi.FractionalDigits}, {nameof(mi.UnitPrefix)}: {mi.UnitPrefix}, {nameof(mi.Units)}: {mi.Units}, {nameof(mi.UnitSuffix)}: {mi.UnitSuffix}");
                        else if (m is MethodProcess mp) stringBuilder.AppendLine($"\t\t\t\t{nameof(mp.Path)}: {mp.Path}, {nameof(mp.Executable)}: {mp.Executable}, {nameof(mp.Parameters)}: {mp.Parameters}, {nameof(mp.Expected)}: {mp.Expected}");
                        else if (m is MethodTextual mt) stringBuilder.AppendLine($"\t\t\t\t{nameof(mt.Text)}: {mt.Text}");
                        else {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine($"Method '{nameof(Method.Name)}' not implemented:");
                            sb.AppendLine($"\t{nameof(TestOperation.NamespaceTrunk)} : {testOperation.NamespaceTrunk}");
                            sb.AppendLine($"\t{nameof(TestOperation.Description)}    : {testOperation.Description}");
                            sb.AppendLine($"\t\t{nameof(TestGroup.Class)}            : {testGroup.Class}");
                            sb.AppendLine($"\t\t{nameof(TestGroup.Description)}      : {testGroup.Description}");
                            sb.AppendLine($"\t\t{nameof(TestGroup.CancelNotPassed)}  : {testGroup.CancelNotPassed}");
                            sb.AppendLine($"\t\t{nameof(TestGroup.Independent)}      : {testGroup.Independent}");
                            sb.AppendLine($"\t\t\t{nameof(Method.Name)}                   : {m.Name}");
                            sb.AppendLine($"\t\t\t{nameof(Method.Description)}            : {m.Description}");
                            sb.AppendLine($"\t\t\t{nameof(Method.CancelNotPassed)}        : {m.CancelNotPassed}");
                            throw new NotImplementedException(sb.ToString());
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
}
