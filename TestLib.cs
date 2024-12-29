using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ABT.Test.TestLib.AppConfig;
using ABT.Test.TestLib.TestDefinition;

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
        //public static Dictionary<String, Object> InstrumentDrivers = null;
        //public static readonly AppConfigLogger ConfigLogger = AppConfigLogger.Get();
        //public static AppConfigUUT ConfigUUT = AppConfigUUT.Get();

        public static String TestDefinitionXSD = @"C:\Users\phils\source\repos\ABT\Test\TestLib\TestDefinition\TestDefinition.xsd";
        public static String BaseDirectory = null;      // Requires instantiated TestExec form; initialized by ButtonSelectTests_Click method.
        public static CancellationToken CT_Cancel;
        public static CancellationToken CT_EmergencyStop;
        // NOTE: Commented on 11/5/24.  Original code in 11/5/24 Git commit.
        //public static Configuration ConfigMap = GetConfiguration();
        //public static Configuration GetConfiguration() {
        //    ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap {
        //        ExeConfigFilename = @"C:\Users\phils\source\repos\ABT\TestExec\Tests\Diagnostics\bin\x64\Debug\Diagnostics.exe.config"
        //    };
        //    return ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
        //}
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
        public static M M { get; set; } = null;
        public static void Nullify() {
            TestOperation = null;
            TestGroup = null;
            M = null;
        }
    }
}
