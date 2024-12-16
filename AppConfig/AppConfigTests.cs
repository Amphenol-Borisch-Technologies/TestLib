using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ABT.Test.Lib.AppConfig {
    public enum UNITS_SI { amperes, celcius, farads, henries, hertz, NotApplicable, ohms, seconds, siemens, volt_amperes, volts, watts }
    public enum UNITS_SI_MODIFIER { AC, DC, Peak, PP, NotApplicable, RMS }

    public abstract class MeasurementAbstract {
        public const Char SA = '|'; // Arguments separator character.  Must match Arguments separator character used in Tests's App.config.
        public const Char SK = '='; // Key/Values separator character.  Must match Key/Values separator character used in Tests's App.config.

        private protected MeasurementAbstract() { }

        public abstract String ArgumentsGet();

        public static Dictionary<String, String> ArgumentsSplit(String Arguments) {
            Dictionary<String, String> argDictionary = new Dictionary<String, String>();
            if (String.Equals(Arguments, MeasurementCustom.NOT_APPLICABLE)) argDictionary.Add(MeasurementCustom.NOT_APPLICABLE, MeasurementCustom.NOT_APPLICABLE);
            else {
                String[] args = Arguments.Split(SA);
                String[] kvp;
                for (Int32 i = 0; i < args.Length; i++) {
                    kvp = args[i].Split(SK);
                    argDictionary.Add(kvp[0].Trim(), kvp[1].Trim());
                }
            }
            return argDictionary;
        }

        public static String ArgumentsJoin(Dictionary<String, String> Arguments) {
            IEnumerable<String> keys = Arguments.Select(a => String.Format($"{a.Key}{Char.ToString(SK)}{a.Value}"));
            return String.Join(Char.ToString(SA), keys);
        }

        internal abstract void ArgumentsValidate(String id, String arguments, Dictionary<String, String> argsDict);
    }

    public class MeasurementCustom : MeasurementAbstract {
        public readonly String Arguments;
        public static readonly String NOT_APPLICABLE = Enum.GetName(typeof(UNITS_SI), UNITS_SI.NotApplicable);

        public MeasurementCustom(String ID, String Arguments) {
            Dictionary<String, String> argsDict = ArgumentsSplit(Arguments);
            ArgumentsValidate(ID, Arguments, argsDict);
            this.Arguments = Arguments;
        }

        public override String ArgumentsGet() { return Arguments; }

        internal override void ArgumentsValidate(String id, String arguments, Dictionary<String, String> argsDict) {
            if (argsDict.Count == 0) throw new ArgumentException($"{nameof(MeasurementCustom)} ID '{id}' requires 1 or more case-sensitive arguments:{Environment.NewLine}" +
                $"   Example: '{NOT_APPLICABLE}'{Environment.NewLine}" +
                $"   Or     : 'Key{SK}Value'{Environment.NewLine}" +
                $"   Or     : 'Key1{SK}Value1{SA}{Environment.NewLine}" +
                $"             Key2{SK}Value2{SA}{Environment.NewLine}" +
                $"             Key3{SK}Value3'{Environment.NewLine}" +
                $"   Actual : '{arguments}'");
        }
    }

    public class MeasurementNumeric : MeasurementAbstract {
        public readonly Double Low;                                                                   private const String _LOW = nameof(Low);
        public readonly Double High;                                                                  private const String _HIGH = nameof(High);
        public readonly Int32 FD;                                                                     private const String _FD = nameof(FD);
        public readonly UNITS_SI Units_SI = UNITS_SI.NotApplicable;                                   private const String _UNITS_SI = nameof(Units_SI);
        public readonly UNITS_SI_MODIFIER Units_SI_Modifier = UNITS_SI_MODIFIER.NotApplicable;        private const String _UNITS_SI_MODIFIER = nameof(Units_SI_Modifier);

        public MeasurementNumeric(String ID, String Arguments) {
            Dictionary<String, String> argsDict = ArgumentsSplit(Arguments);
            ArgumentsValidate(ID, Arguments, argsDict);
            High = Double.Parse(argsDict[_HIGH], NumberStyles.Float, CultureInfo.CurrentCulture);
            Low = Double.Parse(argsDict[_LOW], NumberStyles.Float, CultureInfo.CurrentCulture);
            FD = Int32.Parse(argsDict[_FD], NumberStyles.Integer, CultureInfo.CurrentCulture);

            String[] units_si = Enum.GetNames(typeof(UNITS_SI)).Select(s => s.ToLower()).ToArray();
            if (units_si.Any(argsDict[_UNITS_SI].ToLower().Contains)) {
                Units_SI = (UNITS_SI)Enum.Parse(typeof(UNITS_SI), argsDict[_UNITS_SI], ignoreCase: true);
                String[] si_units_modifiers = Enum.GetNames(typeof(UNITS_SI_MODIFIER)).Select(s => s.ToLower()).ToArray();
                if (si_units_modifiers.Any(argsDict[_UNITS_SI_MODIFIER].ToLower().Contains)) {
                    Units_SI_Modifier = (UNITS_SI_MODIFIER)Enum.Parse(typeof(UNITS_SI_MODIFIER), argsDict[_UNITS_SI_MODIFIER], ignoreCase: true);
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
        public readonly String TestElementID;
        public readonly Boolean IsOperation;
        public readonly String TestElementDescription;
        public readonly String TestElementRevision;
        public readonly List<String> GroupIDsSequence = new List<String>();
        public readonly Dictionary<String, Group> Groups = new Dictionary<String, Group>();
        public readonly Dictionary<String, List<String>> GroupIDsToMeasurementIDs = new Dictionary<String, List<String>>();
        public readonly List<String> TestMeasurementIDsSequence = new List<String>();
        public readonly Dictionary<String, Measurement> Measurements = new Dictionary<String, Measurement>();
        public readonly Int32 FormattingLengthGroupID = 0;
        public readonly Int32 FormattingLengthMeasurementID = 0;
        public Statistics Statistics { get; set; } = new Statistics();
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
        }

        public static AppConfigTest Get() { return new AppConfigTest(); }

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
        }

        public String StatisticsStatus() { return $"   Failed: {Statistics.Failed}     Passed: {Statistics.Passed}   "; }

        public String StatusTime() { return $"   Time: {Statistics.Time()}"; }
    }

    public class Statistics {
        public UInt32 Cancelled = 0;
        public UInt32 EmergencyStopped = 0;
        public UInt32 Errored = 0;
        public UInt32 Failed = 0;
        public UInt32 Ignored = 0;
        public UInt32 Passed = 0;
        private readonly DateTime TestSelected = DateTime.Now;

        public Statistics() { }

        public void Update(EVENTS Event) { 
            switch(Event) {
                case EVENTS.CANCEL:
                    Cancelled++;
                    break;
                case EVENTS.EMERGENCY_STOP:
                    EmergencyStopped++;
                    break;
                case EVENTS.ERROR:
                    Errored++;
                    break;
                case EVENTS.FAIL:
                    Failed++;
                    break;
                case EVENTS.IGNORE:
                    Ignored++;
                    break;
                case EVENTS.PASS:
                    Passed++;
                    break;
                case EVENTS.UNSET:
                    throw new ArgumentException($"Event '{Event}' illegal argument for {System.Reflection.MethodBase.GetCurrentMethod().Name}.");
                default:
                    throw new NotImplementedException($"Event '{Event}' not implemented.");
            }
        }

        public String Time() {
            TimeSpan elapsedTime = DateTime.Now - TestSelected;
            return $"{(elapsedTime.Days != 0 ? elapsedTime.Days.ToString() + ":" : String.Empty)}{elapsedTime.Hours}:{elapsedTime.Minutes:00}";
        }
        public Double PercentCancelled() { return Convert.ToDouble(Cancelled) / Convert.ToDouble(Tested()); }
        public Double PercentEmergencyStopped() { return Convert.ToDouble(EmergencyStopped) / Convert.ToDouble(Tested()); }
        public Double PercentErrored() { return Convert.ToDouble(Errored) / Convert.ToDouble(Tested()); }
        public Double PercentFailed() { return Convert.ToDouble(Failed) / Convert.ToDouble(Tested()); }
        public Double PercentIgnored() { return Convert.ToDouble(Ignored) / Convert.ToDouble(Tested()); }
        public Double PercentPassed() { return Convert.ToDouble(Passed) / Convert.ToDouble(Tested()); }
        public UInt32 Tested() { return Cancelled + EmergencyStopped + Errored + Failed + Ignored + Passed; }
    }
}
