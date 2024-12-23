using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ABT.Test.TestLib.TestSpec {

    public static class Serializing {
        public static void Serialize(TS ts, String TestSpecXML) {
            if (!Directory.Exists(Path.GetDirectoryName(TestSpecXML))) throw new ArgumentException($"Folder '{Path.GetDirectoryName(TestSpecXML)}' does not exist.");
            using (FileStream fileStream = new FileStream(TestSpecXML, FileMode.Create)) new XmlSerializer(typeof(TS)).Serialize(fileStream, ts);
        }

        public static TS Deserialize(String TestSpecXML) {
            if (!File.Exists(TestSpecXML)) throw new ArgumentException($"XML Test Specification File '{TestSpecXML}' does not exist.");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TS));
            using (FileStream fileStream = new FileStream(TestSpecXML, FileMode.Open)) return (TS)xmlSerializer.Deserialize(fileStream);
        }

        public static String Format(TS ts) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"{nameof(TS.NamespaceRoot)}   : {ts.NamespaceRoot}");
            stringBuilder.AppendLine($"{nameof(TO.Description)} : {ts.Description}");
            foreach (TO to in ts.TestOperations) {
                stringBuilder.AppendLine($"{nameof(TO.NamespaceLeaf)}   : {to.NamespaceLeaf}");
                stringBuilder.AppendLine($"{nameof(TO.Description)} : {to.Description}");
                foreach (TG tg in to.TestGroups) {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine($"\t{nameof(TG.Class)}        : {tg.Class}");
                    stringBuilder.AppendLine($"\t{nameof(TG.Description)}  : {tg.Description}");
                    stringBuilder.AppendLine($"\t{nameof(TG.CancelNotPassed)} : {tg.CancelNotPassed}");
                    stringBuilder.AppendLine($"\t{nameof(TG.Independent)}  : {tg.Independent}");
                    foreach (M m in tg.Methods) {
                        stringBuilder.AppendLine($"\t\t\t{nameof(M.Method)}: {m.Method}, {nameof(M.Description)}: {m.Description}, {nameof(M.CancelNotPassed)}: {m.CancelNotPassed}");
                        if (m is MC mc) foreach (Parameter p in mc.Parameters) stringBuilder.AppendLine($"    {nameof(Parameter)} {nameof(Parameter.Key)}: {p.Key}, {nameof(Parameter.Value)}: {p.Value}");
                        else if (m is MI mi) stringBuilder.AppendLine($"\t\t\t\t{nameof(MI.LowComparator)}: {mi.LowComparator}, {nameof(MI.Low)}: {mi.Low}, {nameof(MI.High)}: {mi.High}, {nameof(MI.HighComparator)}: {mi.HighComparator}, {nameof(MI.FractionalDigits)}: {mi.FractionalDigits}, {nameof(MI.UnitPrefix)}: {mi.UnitPrefix}, {nameof(MI.Units)}: {mi.Units}, {nameof(MI.UnitSuffix)}: {mi.UnitSuffix}");
                        else if (m is MP mp) stringBuilder.AppendLine($"\t\t\t\t{nameof(MP.Path)}: {mp.Path}, {nameof(MP.Executable)}: {mp.Executable}, {nameof(MP.Parameters)}: {mp.Parameters}, {nameof(MP.Expected)}: {mp.Expected}");
                        else if (m is MT mt) stringBuilder.AppendLine($"\t\t\t\t{nameof(MT.Text)}: {mt.Text}");
                        else {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine($"Method '{nameof(M.Method)}' not implemented:");
                            sb.AppendLine($"\t{nameof(TO.NamespaceLeaf)}   : {to.NamespaceLeaf}");
                            sb.AppendLine($"\t{nameof(TO.Description)} : {to.Description}");
                            sb.AppendLine($"\t\t{nameof(TG.Class)}        : {tg.Class}");
                            sb.AppendLine($"\t\t{nameof(TG.Description)}  : {tg.Description}");
                            sb.AppendLine($"\t\t{nameof(TG.CancelNotPassed)} : {tg.CancelNotPassed}");
                            sb.AppendLine($"\t\t{nameof(TG.Independent)}  : {tg.Independent}");
                            sb.AppendLine($"\t\t\t{nameof(M.Method)}       : {m.Method}");
                            sb.AppendLine($"\t\t\t{nameof(M.Description)}  : {m.Description}");
                            sb.AppendLine($"\t\t\t{nameof(M.CancelNotPassed)} : {m.CancelNotPassed}");
                            throw new NotImplementedException(sb.ToString());
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }
    }
}
