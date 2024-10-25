using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ABT.Test.Lib.AppConfig;

namespace ABT.Test.Lib {
    public static class TestData {
        public const String NONE = "NONE";
        public const String MutexTestPlanName = "MutexTestPlan";
        public static Mutex MutexTestPlan = null;
        public static Dictionary<String, Object> InstrumentDrivers = null;
        public static readonly AppConfigLogger ConfigLogger = AppConfigLogger.Get();
        public static AppConfigUUT ConfigUUT = AppConfigUUT.Get();
        public static AppConfigTest ConfigTest = null; // Requires form; instantiated by ButtonSelectTests_Click method.
        public static CancellationToken CT_Cancel;
        public static CancellationToken CT_EmergencyStop;
        public static String MeasurementIDPresent = String.Empty;
        public static Measurement MeasurementPresent = null;

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
                String.Equals(MeasurementPresent.ClassObject.GetType().Name, ClassName) &&
                MeasurementPresent.CancelNotPassed == CancelNotPassed &&
                String.Equals((String)MeasurementPresent.ClassObject.GetType().GetMethod("ArgumentsGet").Invoke(MeasurementPresent.ClassObject, null), Arguments);
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
            return (String)mn.GetType().GetMethod("ArgumentsGet").Invoke(mn, null);
        }
    }

    public static class TestEvents {
        public const String CANCEL = "CANCEL";
        public const String EMERGENCY_STOP = "EMERGENCY STOP";
        public const String ERROR = "ERROR";
        public const String FAIL = "FAIL";
        public const String PASS = "PASS";
        public const String UNSET = "UNSET";

        public static Color GetColor(String Event) {
            Dictionary<String, Color> codesToColors = new Dictionary<String, Color>() {
                { CANCEL, Color.Yellow },
                { EMERGENCY_STOP, Color.Firebrick },
                { ERROR, Color.Aqua },
                { FAIL, Color.Red },
                { PASS, Color.Green },
                { UNSET, Color.Gray }
            };
            return codesToColors[Event];
        }
    }
}
