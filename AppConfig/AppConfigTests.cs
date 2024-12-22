using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ABT.Test.TestLib.TestSpec;

namespace ABT.Test.TestLib.AppConfig {

    public abstract class MeasurementAbstract {                                                                                 // Superceded by M.
        public const Char SA = '|';                                                                                             // Obsolete field.
        public const Char SK = '=';                                                                                             // Obsolete field.

        private protected MeasurementAbstract() { }                                                                             // Obsolete; no constructor needed.

        public abstract String ArgumentsGet();                                                                                  // Superceded by AssertionM().

        public static Dictionary<String, String> ArgumentsSplit(String Arguments) {
            Dictionary<String, String> argDictionary = new Dictionary<String, String>();
            if (String.Equals(Arguments, MeasurementCustom.NONE)) argDictionary.Add(MeasurementCustom.NONE, MeasurementCustom.NONE);
            else {
                String[] args = Arguments.Split(SA);
                String[] kvp;
                for (Int32 i = 0; i < args.Length; i++) {
                    kvp = args[i].Split(SK);
                    argDictionary.Add(kvp[0].Trim(), kvp[1].Trim());
                }
            }
            return argDictionary;
        }                                                                                                                       // Superceded by M's properties.

        public static String ArgumentsJoin(Dictionary<String, String> Arguments) {
            IEnumerable<String> keys = Arguments.Select(a => String.Format($"{a.Key}{Char.ToString(SK)}{a.Value}"));
            return String.Join(Char.ToString(SA), keys);
        }                                                                                                                       // Superceded by AssertionM().

        internal abstract void ArgumentsValidate(String id, String arguments, Dictionary<String, String> argsDict);             // Superceded by C:\Users\phils\source\repos\ABT\Test\TestLib\TestSpec\TestPlan.xsd
    }

    public class MeasurementCustom : MeasurementAbstract {                                                                      // Superceded by MC.
        public readonly String Arguments;                                                                                       // Superceded by MC's properties.
        public static readonly String NONE = Enum.GetName(typeof(MI_Units), MI_Units.NONE);

        public MeasurementCustom(String ID, String Arguments) {
            Dictionary<String, String> argsDict = ArgumentsSplit(Arguments);
            ArgumentsValidate(ID, Arguments, argsDict);
            this.Arguments = Arguments;
        }

        public override String ArgumentsGet() { return Arguments; }

        internal override void ArgumentsValidate(String id, String arguments, Dictionary<String, String> argsDict) {
            if (argsDict.Count == 0) throw new ArgumentException($"{nameof(MeasurementCustom)} ID '{id}' requires 1 or more case-sensitive arguments:{Environment.NewLine}" +
                $"   Example: '{NONE}'{Environment.NewLine}" +
                $"   Or     : 'Key{SK}Value'{Environment.NewLine}" +
                $"   Or     : 'Key1{SK}Value1{SA}{Environment.NewLine}" +
                $"             Key2{SK}Value2{SA}{Environment.NewLine}" +
                $"             Key3{SK}Value3'{Environment.NewLine}" +
                $"   Actual : '{arguments}'");
        }
    }

    public class MeasurementNumeric : MeasurementAbstract {
        public readonly Double Low;                                                 private const String _LOW = nameof(Low);
        public readonly Double High;                                                private const String _HIGH = nameof(High);
        public readonly Int32 FD;                                                   private const String _FD = nameof(FD);
        public readonly MI_Units Units_SI = MI_Units.NONE;                          private const String _UNITS_SI = nameof(Units_SI);
        public readonly MI_UnitSuffix Units_SI_Modifier = MI_UnitSuffix.NONE;       private const String _UNITS_SI_MODIFIER = nameof(Units_SI_Modifier);

        public MeasurementNumeric(String ID, String Arguments) {
            Dictionary<String, String> argsDict = ArgumentsSplit(Arguments);
            ArgumentsValidate(ID, Arguments, argsDict);
            High = Double.Parse(argsDict[_HIGH], NumberStyles.Float, CultureInfo.CurrentCulture);
            Low = Double.Parse(argsDict[_LOW], NumberStyles.Float, CultureInfo.CurrentCulture);
            FD = Int32.Parse(argsDict[_FD], NumberStyles.Integer, CultureInfo.CurrentCulture);

            String[] units_si = Enum.GetNames(typeof(MI_Units)).Select(s => s.ToLower()).ToArray();
            if (units_si.Any(argsDict[_UNITS_SI].ToLower().Contains)) {
                Units_SI = (MI_Units)Enum.Parse(typeof(MI_Units), argsDict[_UNITS_SI], ignoreCase: true);
                String[] si_units_modifiers = Enum.GetNames(typeof(MI_UnitSuffix)).Select(s => s.ToLower()).ToArray();
                if (si_units_modifiers.Any(argsDict[_UNITS_SI_MODIFIER].ToLower().Contains)) {
                    Units_SI_Modifier = (MI_UnitSuffix)Enum.Parse(typeof(MI_UnitSuffix), argsDict[_UNITS_SI_MODIFIER], ignoreCase: true);
                }
            }
        }

        public static MeasurementNumeric Get(String MeasurementCustomArgs) {
            Dictionary<String, String> args = ArgumentsSplit(MeasurementCustomArgs);
            List<String> keys = new List<String> { _HIGH, _LOW, _FD, _UNITS_SI, _UNITS_SI_MODIFIER };
            Dictionary<String, String> argsNumeric = args.Where(kvp => keys.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return new MeasurementNumeric("MN", ArgumentsJoin(argsNumeric));
        }

        public override String ArgumentsGet() { return $"{_HIGH}{SK}{High}{SA}{_LOW}{SK}{Low}{SA}{_FD}{SK}{FD}{SA}{_UNITS_SI}{SK}{_UNITS_SI}{SA}{_UNITS_SI_MODIFIER}{SK}{_UNITS_SI_MODIFIER}"; }

        internal override void ArgumentsValidate(String id, String arguments, Dictionary<String, String> argsDict) {
            const String ClassName = nameof(MeasurementNumeric);
            if (argsDict.Count != 5) throw new ArgumentException($"{ClassName} ID '{id}' requires 4 case-sensitive arguments:{Environment.NewLine}" +
                $"   Example: '{_HIGH}{SK}0.004{SA}{Environment.NewLine}" +
                $"             {_LOW}{SK}0.002{SA}{Environment.NewLine}" +
                $"             {_FD}{SK}3{SA}{Environment.NewLine}" +
                $"             {_UNITS_SI}{SK}volts{SA}{Environment.NewLine}" +
                $"             {_UNITS_SI_MODIFIER}{SK}DC'{Environment.NewLine}" +
                $"   Actual : '{arguments}'");
            if (!argsDict.ContainsKey(_HIGH)) throw new ArgumentException($"{ClassName} ID '{id}' does not contain '{_HIGH}' key-value pair.");
            if (!argsDict.ContainsKey(_LOW)) throw new ArgumentException($"{ClassName} ID '{id  }' does not contain '{_LOW}' key-value pair.");
            if (!argsDict.ContainsKey(_FD)) throw new ArgumentException($"{ClassName} ID '{id  }' does not contain '{_FD}' key-value pair.");
            if (!argsDict.ContainsKey(_UNITS_SI)) throw new ArgumentException($"{ClassName} ID '{id}' does not contain '{_UNITS_SI}' key-value pair.");
            if (!argsDict.ContainsKey(_UNITS_SI_MODIFIER)) throw new ArgumentException($"{ClassName} ID '{id}' does not contain '{_UNITS_SI_MODIFIER}' key-value pair.");
            if (!Double.TryParse(argsDict[_HIGH], NumberStyles.Float, CultureInfo.CurrentCulture, out Double high)) throw new ArgumentException($"{ClassName} ID '{id}' {_HIGH} '{argsDict[_HIGH]}' ≠ System.Double.");
            if (!Double.TryParse(argsDict[_LOW], NumberStyles.Float, CultureInfo.CurrentCulture, out Double low)) throw new ArgumentException($"{ClassName} ID '{id}' {_LOW} '{argsDict[_LOW]}' ≠ System.Double.");
            if (!Int32.TryParse(argsDict[_FD], NumberStyles.Integer, CultureInfo.CurrentCulture, out Int32 fd)) throw new ArgumentException($"{ClassName} ID '{id}' {_FD} '{argsDict[_FD]}' ≠ System.Int32.");
            if (fd < 0) throw new ArgumentException($"{ClassName} ID '{id}' {_FD} '{fd}' < 0.");
            if (low > high) throw new ArgumentException($"{ClassName} ID '{id}' {_LOW} '{low}' > {_HIGH} '{high}'.");
        }
    }

    public class MeasurementProcess : MeasurementAbstract {
        public readonly String ProcessFolder;           private const String _PROCESS_FOLDER = nameof(ProcessFolder);
        public readonly String ProcessExecutable;       private const String _PROCESS_EXECUTABLE = nameof(ProcessExecutable);
        public readonly String ProcessArguments;        private const String _PROCESS_ARGUMENTS = nameof(ProcessArguments);
        public readonly String ProcessExpected;         private const String _PROCESS_EXPECTED = nameof(ProcessExpected);

        public MeasurementProcess(String ID, String Arguments) {
            Dictionary<String, String> argsDict = ArgumentsSplit(Arguments);
            ArgumentsValidate(ID, Arguments, argsDict);
            ProcessFolder = argsDict[_PROCESS_FOLDER];
            ProcessExecutable = argsDict[_PROCESS_EXECUTABLE];
            ProcessArguments = argsDict[_PROCESS_ARGUMENTS];
            ProcessExpected = argsDict[_PROCESS_EXPECTED];
        }

        public static MeasurementProcess Get(String MeasurementCustomArgs) {
            Dictionary<String, String> args = ArgumentsSplit(MeasurementCustomArgs);
            List<String> keys = new List<String> { _PROCESS_FOLDER, _PROCESS_EXECUTABLE, _PROCESS_ARGUMENTS, _PROCESS_EXPECTED };
            Dictionary<String, String> argsProcess = args.Where(kvp => keys.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return new MeasurementProcess("MP", ArgumentsJoin(argsProcess));
        }

        public override String ArgumentsGet() {
            return $"{_PROCESS_FOLDER}{SK}{ProcessFolder}{SA}{_PROCESS_EXECUTABLE}{SK}{ProcessExecutable}{SA}{_PROCESS_ARGUMENTS}{SK}{ProcessArguments}{SA}{_PROCESS_EXPECTED}{SK}{ProcessExpected}";
        }

        internal override void ArgumentsValidate(String id, String arguments, Dictionary<String, String> argsDict) {
            const String ClassName = nameof(MeasurementProcess);
            if (argsDict.Count != 4) throw new ArgumentException($"{ClassName} ID '{id}' requires 4 case-sensitive arguments:{Environment.NewLine}" +
                $@"   Example: '{_PROCESS_EXECUTABLE}{SK}ipecmd.exe{SA}{Environment.NewLine}
                                {_PROCESS_FOLDER}{SK}C:\Program Files\Microchip\MPLABX\v6.15\mplab_platform\mplab_ipe\{SA}{Environment.NewLine}
                                {_PROCESS_ARGUMENTS}{SK}C:\TBD\U1_Firmware.hex{SA}{Environment.NewLine}
                                {_PROCESS_EXPECTED}{SK}0xAC0E'{Environment.NewLine}" +
                 $"   Actual : '{arguments}'");
            if (!argsDict.ContainsKey(_PROCESS_FOLDER)) throw new ArgumentException($"{ClassName} ID '{id}' does not contain '{_PROCESS_FOLDER}' key-value pair.");
            if (!argsDict.ContainsKey(_PROCESS_EXECUTABLE)) throw new ArgumentException($"{ClassName} ID '{id}' does not contain '{_PROCESS_EXECUTABLE}' key-value pair.");
            if (!argsDict.ContainsKey(_PROCESS_ARGUMENTS)) throw new ArgumentException($"{ClassName} ID '{id}' does not contain '{_PROCESS_ARGUMENTS}' key-value pair.");
            if (!argsDict.ContainsKey(_PROCESS_EXPECTED)) throw new ArgumentException($"{ClassName} ID '{id}' does not contain '{_PROCESS_EXPECTED}' key-value pair.");
            if (!argsDict[_PROCESS_FOLDER].EndsWith(@"\")) throw new ArgumentException($"{ClassName} ID '{id}' {_PROCESS_FOLDER} '{argsDict[_PROCESS_FOLDER]}' does not end with '\\'.");
            if (!Directory.Exists(argsDict[_PROCESS_FOLDER])) throw new ArgumentException($"{ClassName} ID '{id}' {_PROCESS_FOLDER} '{argsDict[_PROCESS_FOLDER]}' does not exist.");
            if (!File.Exists(argsDict[_PROCESS_FOLDER] + argsDict[_PROCESS_EXECUTABLE])) throw new ArgumentException($"{ClassName} ID '{id}' {_PROCESS_EXECUTABLE} '{argsDict[_PROCESS_FOLDER] + argsDict[_PROCESS_EXECUTABLE]}' does not exist.");
        }
    }

    public class MeasurementTextual : MeasurementAbstract {
        public readonly String Text;                                private const String _TEXT = nameof(Text);

        public MeasurementTextual(String ID, String Arguments) {
            Dictionary<String, String> argsDict = ArgumentsSplit(Arguments);
            ArgumentsValidate(ID, Arguments, argsDict);
            Text = argsDict[_TEXT];
        }

        public static MeasurementTextual Get(String MeasurementCustomArgs) {
            Dictionary<String, String> args = ArgumentsSplit(MeasurementCustomArgs);
            List<String> keys = new List<String> { _TEXT };
            Dictionary<String, String> argsTextual = args.Where(kvp => keys.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return new MeasurementTextual("MT", ArgumentsJoin(argsTextual));
        }

        public override String ArgumentsGet() {
            return $"{_TEXT}{SK}{Text}";
        }

        internal override void ArgumentsValidate(String id, String arguments, Dictionary<String, String> argsDict) {
            if (argsDict.Count != 1) throw new ArgumentException($"{nameof(MeasurementTextual)} ID '{id}' requires 1 case-sensitive argument:{Environment.NewLine}" +
                $"   Example: '{_TEXT}{SK}The quick brown fox jumps over the lazy dog.'{Environment.NewLine}" +
                $"   Actual : '{arguments}'");
            if (!argsDict.ContainsKey(_TEXT)) throw new ArgumentException($"{nameof(MeasurementTextual)} ID '{id}' does not contain '{_TEXT}' key-value pair.");
        }
    }

    public class AppConfigTest {
        public readonly String TestElementID;                                                                                   // TO.Namespace
        public readonly Boolean IsOperation;                                                                                    // Always true for TO.  True or false for TG.
        public readonly String TestElementDescription;                                                                          // TO.Description.
        public readonly String TestElementRevision;                                                                             // Obsolete field.
        public readonly List<String> GroupIDsSequence = new List<String>();                                                     // TO.TestGroups contains both sequence & TG objects.
        public readonly Dictionary<String, Group> Groups = new Dictionary<String, Group>();                                     // TO.TestGroups contains both sequence & TG objects.
        public readonly Dictionary<String, List<String>> GroupIDsToMeasurementIDs = new Dictionary<String, List<String>>();     // Obsolete field.
        public readonly List<String> TestMeasurementIDsSequence = new List<String>();                                           // TG.Methods contains both sequence & M objects.
        public readonly Dictionary<String, Measurement> Measurements = new Dictionary<String, Measurement>();                   // TG.Methods contains both sequence & M objects.
        public readonly Int32 FormattingLengthGroupID = 0;                                                                      // Added to TO.
        public readonly Int32 FormattingLengthMeasurementID = 0;                                                                // Added to TO.
        public Statistics Statistics { get; set; } = new Statistics();                                                          // Added to TO.
        private AppConfigTest() {
            Dictionary<String, Operation> allOperations = Operation.Get();
            Dictionary<String, Group> allGroups = Group.Get();
            Dictionary<String, Measurement> allMeasurements = Measurement.Get();
            (TestElementID, IsOperation) = SelectTests.Get(allOperations, allGroups);

            if (IsOperation) {
                TestElementDescription = allOperations[TestElementID].Description;
                TestElementRevision = allOperations[TestElementID].Revision;
                GroupIDsSequence = allOperations[TestElementID].TestGroupIDs.Split(MeasurementAbstract.SA).Select(id => id.Trim()).ToList();
                foreach (String groupID in GroupIDsSequence) {
                    Groups.Add(groupID, allGroups[groupID]);
                    GroupIDsToMeasurementIDs.Add(groupID, allGroups[groupID].TestMeasurementIDs.Split(MeasurementAbstract.SA).ToList());

                    if (groupID.Length > FormattingLengthGroupID) FormattingLengthGroupID = groupID.Length;
                    foreach (String measurementID in GroupIDsToMeasurementIDs[groupID]) {
                        Measurements.Add(measurementID, allMeasurements[measurementID]);
                        Measurements[measurementID].GroupID = groupID;
                        if (measurementID.Length > FormattingLengthMeasurementID) FormattingLengthMeasurementID = measurementID.Length;
                    }
                }
            } else {
                TestElementDescription = allGroups[TestElementID].Description;
                TestElementRevision = allGroups[TestElementID].Revision;
                GroupIDsSequence.Add(TestElementID);
                Groups.Add(TestElementID, allGroups[TestElementID]);
                GroupIDsToMeasurementIDs.Add(TestElementID, allGroups[TestElementID].TestMeasurementIDs.Split(MeasurementAbstract.SA).ToList());

                foreach (String measurementID in GroupIDsToMeasurementIDs[TestElementID]) {
                    Measurements.Add(measurementID, allMeasurements[measurementID]);
                    Measurements[measurementID].GroupID = TestElementID;
                    if (measurementID.Length > FormattingLengthMeasurementID) FormattingLengthMeasurementID = measurementID.Length;
                }
            }
            foreach (String groupID in GroupIDsSequence) TestMeasurementIDsSequence.AddRange(GroupIDsToMeasurementIDs[groupID]);

            IEnumerable<String> duplicateIDs = TestMeasurementIDsSequence.GroupBy(x => x).Where(g => g.Count() > 1).Select(x => x.Key);
            if (duplicateIDs.Count() !=0) throw new InvalidOperationException($"Duplicated TestMeasurementIDs '{String.Join("', '", duplicateIDs)}'.");
        }                                                                                             // Obsolete; no constructor needed.                                   

        public static AppConfigTest Get() { return new AppConfigTest(); }                                                       // Obsolete; no constructor needed.

        public String StatisticsDisplay() {
            const Int32 L = 6;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Cancelled : {Statistics.Cancelled,L}, {Statistics.PercentCancelled(),L:P1}");
            sb.AppendLine($"E-Stopped : {Statistics.EmergencyStopped,L}, {Statistics.PercentEmergencyStopped(),L:P1}");
            sb.AppendLine($"Errored   : {Statistics.Errored,L}, {Statistics.PercentErrored(),L:P1}");
            sb.AppendLine($"Failed    : {Statistics.Failed,L}, {Statistics.PercentFailed(),L:P1}");
            sb.AppendLine($"Ignored   : {Statistics.Ignored,L}, {Statistics.PercentIgnored(),L:P1}");
            sb.AppendLine($"Passed    : {Statistics.Passed,L}, {Statistics.PercentPassed(),L:P1}");
            sb.AppendLine($"------");
            sb.AppendLine($"Total     : {Statistics.Tested(),L}");
            return sb.ToString();
        }                                                                                   // Added to TO.

        public String StatisticsStatus() { return $"   Failed: {Statistics.Failed}     Passed: {Statistics.Passed}   "; }       // Added to TO.

        public String StatusTime() { return $"   Time: {Statistics.Time()}"; }                                                  // Added to TO.
    }
}
