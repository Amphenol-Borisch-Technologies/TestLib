using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ABT.Test.TestLib.AppConfig;
using ABT.Test.TestLib.TestSpec;

namespace ABT.Test.TestLib {
    public enum EVENTS { CANCEL, EMERGENCY_STOP, ERROR, FAIL, IGNORE, PASS, UNSET }

    public static class TestLib {
        public static Dictionary<EVENTS, Color> EventColors = new Dictionary<EVENTS, Color> {
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
        public static readonly AppConfigLogger ConfigLogger = AppConfigLogger.Get();
        public static AppConfigUUT ConfigUUT = AppConfigUUT.Get();

        public static AppConfigTest ConfigTest = null;  // Requires instantiated TestExec form; initialized by ButtonSelectTests_Click method.

        public static String TestSpecXSD = @"C:\Users\phils\source\repos\ABT\Test\TestLib\TestSpec\TestSpec.xsd";
        public static String BaseDirectory = null;      // Requires instantiated TestExec form; initialized by ButtonSelectTests_Click method.
        public static CancellationToken CT_Cancel;
        public static CancellationToken CT_EmergencyStop;
        public static String MeasurementIDPresent = String.Empty;
        public static Measurement MeasurementPresent = null;
        // NOTE: Commented on 11/5/24.  Original code in 11/5/24 Git commit.
        //public static Configuration ConfigMap = GetConfiguration();
        //public static Configuration GetConfiguration() {
        //    ExeConfigurationFileMap ecfm = new ExeConfigurationFileMap {
        //        ExeConfigFilename = @"C:\Users\phils\source\repos\ABT\TestExec\Tests\Diagnostics\bin\x64\Debug\Diagnostics.exe.config"
        //    };
        //    return ConfigurationManager.OpenMappedExeConfiguration(ecfm, ConfigurationUserLevel.None);
        //}

        public static Boolean AreMethodNamesPriorNext(String prior, String next) { return String.Equals(GetID_MeasurementPrior(), prior) && String.Equals(GetID_MeasurementNext(), next); }

        public static Boolean IsGroup(String GroupID) { return String.Equals(ConfigTest.Measurements[MeasurementIDPresent].GroupID, GroupID); }

        public static Boolean IsGroup(String GroupID, String Description, String MeasurementIDs, Boolean Selectable, Boolean CancelNotPassed) {
            return
                String.Equals(ConfigTest.Measurements[MeasurementIDPresent].GroupID, GroupID) &&
                String.Equals(ConfigTest.Groups[GetID_Group()].Description, Description) &&
                String.Equals(ConfigTest.Groups[GetID_Group()].TestMeasurementIDs, MeasurementIDs) &&
                ConfigTest.Groups[GetID_Group()].Selectable == Selectable &&
                ConfigTest.Groups[GetID_Group()].CancelNotPassed == CancelNotPassed;
        }

        public static Boolean IsMeasurement(String Description, String IDPrior, String IDNext, String ClassName, Boolean CancelNotPassed, String Arguments) {
            return
                IsMeasurement(Description, ClassName, CancelNotPassed, Arguments) &&
                String.Equals(GetID_MeasurementPrior(), IDPrior) &&
                String.Equals(GetID_MeasurementNext(), IDNext);
        }

        public static Boolean IsMeasurement(String Description, String ClassName, Boolean CancelNotPassed, String Arguments) {
            return
                String.Equals(MeasurementPresent.Description, Description) &&
                String.Equals(MeasurementPresent.ClassName, ClassName) &&
                MeasurementPresent.CancelNotPassed == CancelNotPassed &&
                String.Equals((String)MeasurementPresent.ClassObject.GetType().GetMethod(nameof(MeasurementAbstract.ArgumentsGet)).Invoke(MeasurementPresent.ClassObject, null), Arguments);
        }

        public static Boolean IsOperation(String OperationID) { return String.Equals(ConfigTest.TestElementID, OperationID); }

        public static Boolean IsOperation(String OperationID, String Description, String Revision, String GroupsIDs) {
            return
            String.Equals(ConfigTest.TestElementID, OperationID) &&
            String.Equals(ConfigTest.TestElementDescription, Description) &&
            String.Equals(ConfigTest.TestElementRevision, Revision) &&
            String.Equals(String.Join(MeasurementAbstract.SA.ToString(), ConfigTest.GroupIDsSequence.ToArray()), GroupsIDs);
        }

        private static String GetID_Group() { return ConfigTest.Measurements[MeasurementIDPresent].GroupID; }

        private static String GetID_MeasurementNext() {
            if (GetIDs_MeasurementSequence() == ConfigTest.TestMeasurementIDsSequence.Count - 1) return NONE;
            return ConfigTest.TestMeasurementIDsSequence[GetIDs_MeasurementSequence() + 1];
        }

        private static String GetID_MeasurementPrior() {
            if (GetIDs_MeasurementSequence() == 0) return NONE;
            return ConfigTest.TestMeasurementIDsSequence[GetIDs_MeasurementSequence() - 1];
        }

        private static Int32 GetIDs_MeasurementSequence() { return ConfigTest.TestMeasurementIDsSequence.FindIndex(x => x.Equals(MeasurementIDPresent)); }

        public static String GetMeasurementNumericArguments(String measurementID) {
            MeasurementNumeric mn = (MeasurementNumeric)Measurement.Get(measurementID).ClassObject;
            return (String)mn.GetType().GetMethod(nameof(MeasurementAbstract.ArgumentsGet)).Invoke(mn, null);
        }
    }

    public static class TestSelection {
        public static TS TS { get; set; } = null;
        public static TO TO { get; set; } = null;
        public static TG TG { get; set; } = null;
        public static void Nullify() {
            TS = null;
            TO = null;
            TG = null;
        }
        public static Boolean IsNotNull() { return TS != null && TO != null; }
        public static Boolean IsOperation() { return IsNotNull() && TG == null; }
        public static Boolean IsGroup() { return IsNotNull() && TG != null; }
    }

    public static class TestIndex {
        public static TO TO { get; set; } = null;
        public static TG TG { get; set; } = null;
        public static M M { get; set; } = null;
        public static void Nullify() {
            TO = null;
            TG = null;
            M = null;
        }
    }
}
