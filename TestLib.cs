using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using ABT.Test.TestLib.TestConfiguration;
// TODO: Immediately; test TestDefinition.xml with MethodInterval, MethodProcess & MethodTextual.  Currently only MethodCustom tested.

namespace ABT.Test.TestLib {
    public enum EVENTS { CANCEL, EMERGENCY_STOP, ERROR, FAIL, IGNORE, PASS, UNSET }

    public static class TestLib {
        // TODO:  Eventually; mitigate or eliminate writeable global objects; change their access to pass by reference.
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

        public static TestSequence testSequence = null;
        public static TestDefinition testDefinition = null;
        public static Dictionary<String, Object> testInstruments = null;
        public static String BaseDirectory = null;
        public static String TestDefinitionXML = null;
        public static String TestDefinitionXSD = GetExecutingStatementDirectory() + @"\TestConfiguration\TestDefinition.xsd";
        public static CancellationToken CT_Cancel;
        public static CancellationToken CT_EmergencyStop;

        public static String GetExecutingStatementDirectory() {
            StackFrame stackFrame = new StackFrame(1, true);
            String fileName = stackFrame.GetFileName();
            return fileName != null ? Path.GetDirectoryName(fileName) : null;
        }

        public static Dictionary<String, Object> GetInstruments(String ConfigurationTestExec) {
            Dictionary<String, Object> Instruments = GetMobile();
            foreach (KeyValuePair<String, Object> kvp in GetStationary(ConfigurationTestExec)) Instruments.Add(kvp.Key, kvp.Value);
            return Instruments;
        }
        
        public static String BuildDate(Version version) {
            DateTime Y2K = new DateTime(year: 2000, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Local);
            return $"{Y2K + new TimeSpan(days: version.Build, hours: 0, minutes: 0, seconds: 2 * version.Revision):g}";
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

            IEnumerable<XElement> iexe = XElement.Load(ConfigurationTestExec).Elements("Instruments");
            Dictionary<String, Object> instruments = new Dictionary<String, Object>();
            foreach (KeyValuePair<String, String> kvp in dictionary) {
                XElement xElement = iexe.Descendants("Stationary").First(xe => (String)xe.Attribute("ID") == kvp.Key) ?? throw new ArgumentException($"Instrument with ID '{kvp.Key}' not present in file '{ConfigurationTestExec}'.");
                instruments.Add(kvp.Key, Activator.CreateInstance(Type.GetType(kvp.Value), new Object[] { xElement.Attribute("Address").Value, xElement.Attribute("Detail").Value }));
            }
            return instruments;
        }
    }
}
