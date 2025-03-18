using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ABT.Test.TestLib.Configuration;

namespace ABT.Test.TestLib {
    [Flags] public enum EVENTS {
        // NOTE:  EVENTS are defined in order of criticality.
        // - EVENTS' ordering is crucial to correctly evaluating TestExec results.
        // - Reordering without consideration will break TestGroup & TestOperation evaluation logic.
        // NOTE: Per Microsoft's CoPilot, 'foreach (EVENTS events in Enum.GetValues(typeof(EVENTS)))' will iterate over EVENTS in their defined order.
        // - This is true with the below order and flag values, with most critical having lowest flag value; EMERGENCY_STOP = 0b0000_0001...INFORMATION = 0b0100_0000.
        // - However, initially I set their flags from most critical having highest flag value; EMERGENCY_STOP = 0b0100_0000...INFORMATION = 0b0000_0001.
        //   This caused the above foreach to iterate in flag value sequence, not definition order.
        // - When I confronted CoPilot with this info, it agreed that flag value overrides definition order.
        EMERGENCY_STOP  = 0b0000_0001, // Most critical event.
        ERROR           = 0b0000_0010, // Second most critical event.
        CANCEL          = 0b0000_0100, // Third most critical event.
        UNSET           = 0b0000_1000, //   .
        FAIL            = 0b0001_0000, //   .
        PASS            = 0b0010_0000, //   .
        INFORMATION     = 0b0100_0000  // Least critical event.
    }
    // NOTE:  If modifying EVENTS, update EventColors correspondingly.
    // - Every EVENT requires an associated Color.

    public static class Data {
        public static readonly Dictionary<EVENTS, Color> EventColors = new Dictionary<EVENTS, Color> {
            { EVENTS.EMERGENCY_STOP, Color.Fuchsia },
            { EVENTS.ERROR, Color.Aqua },
            { EVENTS.CANCEL, Color.Yellow },
            { EVENTS.UNSET, Color.Gray },
            { EVENTS.FAIL, Color.Red },
            { EVENTS.PASS, Color.Green },
            { EVENTS.INFORMATION, Color.White }
        };

        // TODO:  Eventually; mitigate or eliminate writeable global objects.
        public const String NONE = "NONE";
        public static Mutex MutexTest = null;
        public const String MutexTestName = nameof(MutexTest);
        public static Dictionary<String, Object> InstrumentDrivers = null;
        public static String BaseDirectory = null;
        public static String TestPlanDefinitionXML = null;
        public static String TestPlanDefinitionXSD = GetExecutingStatementDirectory() + @"\Configuration\TestPlanDefinition.xsd";
        public static TestPlanDefinition testPlanDefinition = null;
        public static TestSequence testSequence = null;
        public static String TestExecDefinitionXML = GetExecutingStatementDirectory() + @"\Configuration\TestExecDefinition.xml";
        public static TestExecDefinition testExecDefinition = Serializing.DeserializeFromFile<TestExecDefinition>(xmlFile: $"{TestExecDefinitionXML}");
        public static String UserName = null;
        public static CancellationTokenSource CTS_Cancel;
        public static CancellationTokenSource CTS_EmergencyStop;
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

        public static Dictionary<String, Object> GetInstrumentDriversTestExecDefinition() {
            Dictionary<String, Object> instrumentDrivers = new Dictionary<String, Object>();

            Object instrumentDriver = null;
            foreach (InstrumentTestExec instrumentTestExec in testExecDefinition.InstrumentsTestExec.InstrumentTestExec) {
                if (!testPlanDefinition.TestSpace.Simulate) instrumentDriver = Activator.CreateInstance(Type.GetType(instrumentTestExec.NameSpacedClassName), new Object[] { instrumentTestExec.Address, instrumentTestExec.Detail});
                instrumentDrivers.Add(instrumentTestExec.ID, instrumentDriver); // instrumentDriver is null if testPlanDefinition.TestSpace.Simulate.
            }
            return instrumentDrivers;
        }

        public static HashSet<String> GetDerivedClassnames<T>() where T : class {
            try {
                Assembly assembly = Assembly.GetAssembly(typeof(T));
                Type baseType = typeof(T);
                List<Type> derivedTypes = assembly.GetTypes().Where(t => !t.IsCOMObject && t.IsAssignableFrom(baseType)).ToList();
                return new HashSet<String>(derivedTypes.Select(t => t.Name));
            } catch (ReflectionTypeLoadException reflectionTypeLoadException) {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"{nameof(ReflectionTypeLoadException)}: '{reflectionTypeLoadException.Message}'.");
                foreach (Exception exception in reflectionTypeLoadException.LoaderExceptions) {
                    stringBuilder.AppendLine($"\tMessage: '{exception.Message}'.");
                    if (exception is TypeLoadException typeLoadException) stringBuilder.AppendLine($"\tTypeName: '{typeLoadException.TypeName}'.");
                }
                throw new Exception(stringBuilder.ToString());
            }
        }

        public static Dictionary<String, Object> GetInstrumentDriversTestPlanDefinition() {
            Dictionary<String, Object> instrumentDrivers = GetMobileTestPlanDefinition();
            foreach (KeyValuePair<String, Object> kvp in GetStationaryTestPlanDefinition()) instrumentDrivers.Add(kvp.Key, kvp.Value);
            return instrumentDrivers;
        }

        private static Dictionary<String, Object> GetMobileTestPlanDefinition() {
            Dictionary<String, Object> instrumentDrivers = new Dictionary<String, Object>();
            Object instrumentDriver = null;
            foreach (Mobile mobile in testPlanDefinition.InstrumentsTestPlan.Mobile) try {
                    if (!testPlanDefinition.TestSpace.Simulate) instrumentDriver = Activator.CreateInstance(Type.GetType(mobile.NameSpacedClassName), new Object[] { mobile.Address, mobile.Detail });
                    instrumentDrivers.Add(mobile.ID, instrumentDriver); // instrumentDriver is null if testPlanDefinition.TestSpace.Simulate.
                } catch (Exception e) {
                    StringBuilder sb = new StringBuilder().AppendLine();
                    const Int32 PR = 23;
                    sb.AppendLine($"Issue with {nameof(Mobile)}:");
                    sb.AppendLine($"   {nameof(mobile.ID)}".PadRight(PR) + $": {mobile.ID}");
                    sb.AppendLine($"   {nameof(mobile.Detail)}".PadRight(PR) + $": {mobile.Detail}");
                    sb.AppendLine($"   {nameof(mobile.Address)}".PadRight(PR) + $": {mobile.Address}");
                    sb.AppendLine($"   {nameof(mobile.NameSpacedClassName)}".PadRight(PR) + $": {mobile.NameSpacedClassName}{Environment.NewLine}");
                    sb.AppendLine($"{nameof(System.Exception)} {nameof(System.Exception.Message)}(s):");
                    sb.AppendLine($"{e}{Environment.NewLine}");
                    throw new ArgumentException(sb.ToString());
                }
            return instrumentDrivers;
        }

        private static Dictionary<String, Object> GetStationaryTestPlanDefinition() {
            Dictionary<String, Object> instrumentDrivers = GetInstrumentDriversTestExecDefinition();
            foreach (Stationary stationary in testPlanDefinition.InstrumentsTestPlan.Stationary) if (!instrumentDrivers.ContainsKey(stationary.ID)) instrumentDrivers.Remove(stationary.ID);
            return instrumentDrivers;
        }

        public static String ConvertWindowsPathToUrl(String path) {
            String url = path.Replace(@"\", "//");
            if (!url.StartsWith("file://")) url = "file:///" + url;
            return url;
        }
    }
}
