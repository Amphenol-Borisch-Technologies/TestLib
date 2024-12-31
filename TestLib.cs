using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using ABT.Test.TestLib.TestConfiguration;

namespace ABT.Test.TestLib {
    public enum EVENTS { CANCEL, EMERGENCY_STOP, ERROR, FAIL, IGNORE, PASS, UNSET }

    public static class TestLib {
        public static readonly Dictionary<EVENTS, Color> EventColors = new Dictionary<EVENTS, Color> {
                { EVENTS.CANCEL, Color.Yellow },
                { EVENTS.EMERGENCY_STOP, Color.Firebrick },
                { EVENTS.ERROR, Color.Aqua },
                { EVENTS.FAIL, Color.Red },
                { EVENTS.IGNORE, Color.Transparent },
                { EVENTS.PASS, Color.Green },
                { EVENTS.UNSET, Color.Gray }
        };

        public const String NONE = "NONE";
        public static Mutex MutexTest = null;
        public const String MutexTestName = nameof(MutexTest);
        public static Dictionary<String, Object> InstrumentDrivers = null;

        public static String TestDefinitionXSD = AppDomain.CurrentDomain.BaseDirectory.Remove(AppDomain.CurrentDomain.BaseDirectory.IndexOf(@"\bin\")) + @"\TestDefinition.xsd";
        public static TestDefinition testDefinition = null;              // Requires instantiated TestExec form; initialized by ButtonSelectTests_Click method.
        public static Dictionary<String, Object> testInstruments = null; // Requires instantiated TestExec form; initialized by ButtonSelectTests_Click method.
        public static String BaseDirectory = null;                       // Requires instantiated TestExec form; initialized by ButtonSelectTests_Click method.
        public static CancellationToken CT_Cancel;
        public static CancellationToken CT_EmergencyStop;

        public static Dictionary<String, Object> GetInstruments(String ConfigurationTestExec) {
            Dictionary<String, Object> Instruments = GetMobile();
            foreach (KeyValuePair<String, Object> kvp in GetStationary(ConfigurationTestExec)) Instruments.Add(kvp.Key, kvp.Value);
            return Instruments;
        }

        private static Dictionary<String, Object> GetMobile() {
            Dictionary<String, Object> instruments = new Dictionary<String, Object>();
            foreach (Mobile mobile in testDefinition.Instruments.Mobile) try {
                    instruments.Add(mobile.ID, Activator.CreateInstance(Type.GetType(mobile.NameSpacedClassName), new Object[] { mobile.Address, mobile.Detail }));
                } catch (Exception e) {
                    StringBuilder sb = new StringBuilder().AppendLine();
                    sb.AppendLine($"Issue with Mobile Instrument:");
                    sb.AppendLine($"   ID              : {mobile.ID}");
                    sb.AppendLine($"   Detail          : {mobile.Detail}");
                    sb.AppendLine($"   Address         : {mobile.Address}");
                    sb.AppendLine($"   ClassName       : {mobile.NameSpacedClassName}{Environment.NewLine}");
                    sb.AppendLine($"Exception Message(s):");
                    sb.AppendLine($"{e}{Environment.NewLine}");
                    throw new ArgumentException(sb.ToString());
                }
            return instruments;
        }

        private static Dictionary<String, Object> GetStationary(String ConfigurationTestExec) {
            Dictionary<String, String> dictionary = new Dictionary<String, String>();
            foreach (Stationary stationary in testDefinition.Instruments.Stationary) {
                try {
                    dictionary.Add(stationary.ID, stationary.NameSpacedClassName);
                } catch (Exception e) {
                    StringBuilder sb = new StringBuilder().AppendLine();
                    sb.AppendLine($"Issue with Stationary Instrument:");
                    sb.AppendLine($"   ID              : {stationary.ID}");
                    sb.AppendLine($"   ClassName       : {stationary.NameSpacedClassName}{Environment.NewLine}");
                    sb.AppendLine($"Exception Message(s):");
                    sb.AppendLine($"{e}{Environment.NewLine}");
                    throw new ArgumentException(sb.ToString());
                }
            }

            IEnumerable<XElement> IS = XElement.Load(ConfigurationTestExec).Elements("Instruments");
            Dictionary<String, Object> instruments = new Dictionary<String, Object>();
            foreach (KeyValuePair<String, String> kvp in dictionary) {
                XElement XE = IS.Descendants("Stationary").First(xe => (String)xe.Attribute("ID") == kvp.Key) ?? throw new ArgumentException($"Instrument with ID '{kvp.Key}' not present in file '{ConfigurationTestExec}'.");
                instruments.Add(kvp.Key, Activator.CreateInstance(Type.GetType(kvp.Value), new Object[] { XE.Attribute("Address").Value, XE.Attribute("Detail").Value }));
            }
            return instruments;
        }
    }

    public static class TestSelection {
        public static TestSpace TestSpace { get; set; } = null;
        public static TestOperation TestOperation { get; set; } = null;
        public static TestGroup TestGroup { get; set; } = null;
        public static void Nullify() {
            TestSpace = null;
            TestOperation = null;
            TestGroup = null;
        }
        public static Boolean IsNotNull() { return TestSpace != null && TestOperation != null; }
        public static Boolean IsOperation() { return IsNotNull() && TestGroup == null; }
        public static Boolean IsGroup() { return IsNotNull() && TestGroup != null; }
    }

    public static class TestIndex {
        public static TestOperation TestOperation { get; set; } = null;
        public static TestGroup TestGroup { get; set; } = null;
        public static Method Method { get; set; } = null;
        public static void Nullify() {
            TestOperation = null;
            TestGroup = null;
            Method = null;
        }
    }
}
