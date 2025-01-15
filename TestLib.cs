using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using ABT.Test.TestLib.TestConfiguration;
// TODO:  Eventually; mitigate or eliminate writeable global objects; change their access to pass by reference.

namespace ABT.Test.TestLib {
    public enum EVENTS { CANCEL, EMERGENCY_STOP, ERROR, FAIL, INFORMATION, PASS, UNSET }
    // NOTE:  If modifying EVENTS, update EventColors correspondingly.  Every EVENT requires an associated Color.

    public static class TestLib {
        public static readonly Dictionary<EVENTS, Color> EventColors = new Dictionary<EVENTS, Color> {
            { EVENTS.CANCEL, Color.Yellow },
            { EVENTS.EMERGENCY_STOP, Color.Firebrick },
            { EVENTS.ERROR, Color.Aqua },
            { EVENTS.FAIL, Color.Red },
            { EVENTS.INFORMATION, Color.Transparent },
            { EVENTS.PASS, Color.Green },
            { EVENTS.UNSET, Color.Yellow }
        };

        public const String NONE = "NONE";
        public static Mutex MutexTest = null;
        public const String MutexTestName = nameof(MutexTest);
        public static Dictionary<String, Object> InstrumentDrivers = null;
        public static TestSequence testSequence = null;
        public static TestDefinition testDefinition = null;
        public static String BaseDirectory = null;
        public static String TestDefinitionXML = null;
        public static String TestDefinitionXSD = GetExecutingStatementDirectory() + @"\TestConfiguration\TestDefinition.xsd";
        public static String SystemDefinitionXML = null;
        public static String SystemDefinitionXSD = null;
        public static String UserName = null;
        public static CancellationToken CT_Cancel;
        public static CancellationToken CT_EmergencyStop;

        public static String GetExecutingStatementDirectory() {
            StackFrame stackFrame = new StackFrame(1, true);
            String fileName = stackFrame.GetFileName();
            return fileName != null ? Path.GetDirectoryName(fileName) : null;
        }

        public static String BuildDate(Version version) {
            DateTime Y2K = new DateTime(year: 2000, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Local);
            return $"{Y2K + new TimeSpan(days: version.Build, hours: 0, minutes: 0, seconds: 2 * version.Revision):g}";
        }

        public static Dictionary<String, Object> GetInstrumentDriversSystemDefinition() {
            Dictionary<String, Object> instrumentDrivers = new Dictionary<String, Object>();
            IEnumerable<XElement> iEnumerableXElement = XElement.Load(SystemDefinitionXML).Elements("Instruments").Descendants("Instrument");
            Object instrumentDriver = null;
            foreach (XElement xElement in iEnumerableXElement) {
                if (!testDefinition.TestSpace.Simulate) instrumentDriver = Activator.CreateInstance(Type.GetType(xElement.Attribute("NameSpacedClassName").Value), new Object[] { xElement.Attribute("Address").Value, xElement.Attribute("Detail").Value });
                instrumentDrivers.Add(xElement.Attribute("ID").Value, instrumentDriver); // instrumentDriver is null if testDefinition.TestSpace.Simulate.
            }
            return instrumentDrivers;
        }

        public static Dictionary<String, Object> GetInstrumentDriversTestDefinition() {
            Dictionary<String, Object> instrumentDrivers = GetMobileTestDefinition();
            foreach (KeyValuePair<String, Object> kvp in GetStationaryTestDefinition()) instrumentDrivers.Add(kvp.Key, kvp.Value);
            return instrumentDrivers;
        }

        private static Dictionary<String, Object> GetMobileTestDefinition() {
            Dictionary<String, Object> instrumentDrivers = new Dictionary<String, Object>();
            Object instrumentDriver = null;
            foreach (Mobile mobile in testDefinition.Instruments.Mobile) try {
                    if (!testDefinition.TestSpace.Simulate) instrumentDriver = Activator.CreateInstance(Type.GetType(mobile.NameSpacedClassName), new Object[] { mobile.Address, mobile.Detail });
                    instrumentDrivers.Add(mobile.ID, instrumentDriver); // instrumentDriver is null if testDefinition.TestSpace.Simulate.
                } catch (Exception e) {
                    StringBuilder sb = new StringBuilder().AppendLine();
                    sb.AppendLine($"Issue with Mobile Instrument:");
                    sb.AppendLine($"   ID              : {mobile.ID}");
                    sb.AppendLine($"   Detail          : {mobile.Detail}");
                    sb.AppendLine($"   Address         : {mobile.Address}");
                    sb.AppendLine($"   Classname       : {mobile.NameSpacedClassName}{Environment.NewLine}");
                    sb.AppendLine($"Exception Message(s):");
                    sb.AppendLine($"{e}{Environment.NewLine}");
                    throw new ArgumentException(sb.ToString());
                }
            return instrumentDrivers;
        }

        private static Dictionary<String, Object> GetStationaryTestDefinition() {
            Dictionary<String, Object> instrumentDrivers = GetInstrumentDriversSystemDefinition();
            foreach (Stationary stationary in testDefinition.Instruments.Stationary) if (!instrumentDrivers.ContainsKey(stationary.ID)) instrumentDrivers.Remove(stationary.ID);
            return instrumentDrivers;
        }

        public static String ConvertWindowsPathToUrl(String path) {
            String url = path.Replace(@"\", "//");
            if (!url.StartsWith("file://")) url = "file:///" + url;
            return url;
        }
    }
}
