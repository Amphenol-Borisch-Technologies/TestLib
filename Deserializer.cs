using System;
using System.IO;
using System.Xml.Serialization;

namespace TestSequencer {

    class Deserializer {
        static void NotMain() {
            XmlSerializer serializer = new XmlSerializer(typeof(TO));
            using (FileStream fileStream = new FileStream(Properties.Resources.XML_File, FileMode.Open)) {
                TO to = (TO)serializer.Deserialize(fileStream);
                PrintTO(to);
            }
        }

        static void PrintTO(TO to) {
            Console.WriteLine($"{nameof(TO.Namespace)}: {to.Namespace}");
            Console.WriteLine($"{nameof(TO.Description)}: {to.Description}");
            foreach (TG tg in to.TestGroups) {
                Console.WriteLine();
                Console.WriteLine($"{nameof(TG.Class)}: {tg.Class}");
                Console.WriteLine($"{nameof(TG.Description)}: {tg.Description}");
                Console.WriteLine($"{nameof(TG.CancelIfFail)}: {tg.CancelIfFail}");
                Console.WriteLine($"{nameof(TG.Independent)}: {tg.Independent}");
                foreach (M m in tg.Methods) {
                    Console.WriteLine($"  {nameof(M.Method)}: {m.Method}, {nameof(M.Description)}: {m.Description}, {nameof(M.CancelIfFail)}: {m.CancelIfFail}");
                    if (m is MC mc) {
                        foreach (Parameter p in mc.Parameters) {
                            Console.WriteLine($"    {nameof(Parameter)} {nameof(Parameter.Key)}: {p.Key}, {nameof(Parameter.Value)}: {p.Value}");
                        }
                    } else if (m is MI mi) {
                        Console.WriteLine($"    {nameof(MI.LowComparator)}: {mi.LowComparator}, {nameof(MI.Low)}: {mi.Low}, {nameof(MI.High)}: {mi.High}, {nameof(MI.HighComparator)}: {mi.HighComparator}, {nameof(MI.FractionalDigits)}: {mi.FractionalDigits}, {nameof(MI.UnitPrefix)}: {mi.UnitPrefix}, {nameof(MI.Units)}: {mi.Units}, {nameof(MI.UnitSuffix)}: {mi.UnitSuffix}");
                    } else if (m is MP mp) {
                        Console.WriteLine($"    {nameof(MP.Path)}: {mp.Path}, {nameof(MP.Executable)}: {mp.Executable}, {nameof(MP.Parameters)}: {mp.Parameters}, {nameof(MP.Expected)}: {mp.Expected}");
                    } else if (m is MT mt) {
                        Console.WriteLine($"    {nameof(MT.Text)}: {mt.Text}");
                    }
                }
            }
        }
    }
}
