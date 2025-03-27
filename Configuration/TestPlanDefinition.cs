using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using static ABT.Test.TestLib.Data;

namespace ABT.Test.TestLib.Configuration {
    [XmlRoot(nameof(TestPlanDefinition))]
    public class TestPlanDefinition {
        [XmlElement(nameof(UUT))] public UUT UUT { get; set; }
        [XmlElement(nameof(Development))] public Development Development { get; set; }
        [XmlArray(nameof(Modifications))] public List<Modification> Modifications { get; set; }
        [XmlElement(nameof(SerialNumberEntry))] public SerialNumberEntry SerialNumberEntry { get; set; }
        [XmlElement(nameof(InstrumentsTestPlan))] public InstrumentsTestPlan InstrumentsTestPlan { get; set; }
        [XmlElement(nameof(TestSpace))] public TestSpace TestSpace { get; set; }

        public TestPlanDefinition() { }
    }

    public class UUT {
        [XmlElement(nameof(Customer))] public Customer Customer { get; set; }
        [XmlElement(nameof(TestSpecification))] public List<TestSpecification> TestSpecification { get; set; }
        [XmlElement(nameof(Documentation))] public List<Documentation> Documentation { get; set; }
        [XmlAttribute(nameof(Number))] public String Number { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(Revision))] public String Revision { get; set; }
        [XmlAttribute(nameof(Category))] public Category Category { get; set; }
        internal const String DIVIDER = "|";

        public UUT() { }

        public static String EF(Object o) {
            String s = (o.ToString()).Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'");
            return $"\"{s}\"";
        }
    }

    public class Customer {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Division))] public String Division { get; set; }
        [XmlAttribute(nameof(Location))] public String Location { get; set; }

        public Customer() { }
    }

    public class TestSpecification {
        [XmlAttribute(nameof(Document))] public String Document { get; set; }
        [XmlAttribute(nameof(Revision))] public String Revision { get; set; }
        [XmlAttribute(nameof(Title))] public String Title { get; set; }
        [XmlAttribute(nameof(Date))] public String Date { get; set; }

        public TestSpecification() { }
    }

    public class Documentation {
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }

        public Documentation() { }
    }

    public enum Category { Component, CircuitCard, Harness, Unit, System }

    public class SerialNumberEntry {
        [XmlAttribute(nameof(EntryType))] public SerialNumberEntryType EntryType { get; set; }
        [XmlAttribute(nameof(RegularEx))] public String RegularEx { get; set; }
        [XmlAttribute(nameof(Format))] public String Format { get; set; }
        public Boolean IsEnabled() { return EntryType != SerialNumberEntryType.None; }
        public SerialNumberEntry() { }
    }

    public enum SerialNumberEntryType { Barcode, Keyboard, None }

    public class Development {
        [XmlElement(nameof(Developer))] public List<Developer> Developer { get; set; }
        [XmlElement(nameof(Documentation))] public List<Documentation> Documentation { get; set; }
        [XmlElement(nameof(Repository))] public List<Repository> Repository { get; set; }
        [XmlAttribute(nameof(Released))] public String Released { get; set; }
        [XmlIgnore] public String EMailAddresses { get; set; } = String.Empty;

        public Development() { }
    }

    public class Developer {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Language))] public Language Language { get; set; }
        [XmlAttribute(nameof(Comment))] public String Comment { get; set; }
        [XmlIgnore] public String EMailAddress { get; set; } = String.Empty;

        public Developer() { }
    }

    public enum Language { CSharp, Python, VEE }

    public class Repository {
        [XmlAttribute(nameof(URL))] public String URL { get; set; }

        public Repository() { }
    }

    public class Modification {
        [XmlAttribute(nameof(Who))] public String Who { get; set; }
        [XmlAttribute(nameof(What))] public String What { get; set; }
        [XmlAttribute(nameof(When))] public String When { get; set; }
        [XmlAttribute(nameof(Where))] public String Where { get; set; }
        [XmlAttribute(nameof(Why))] public String Why { get; set; }

        public Modification() { }
    }

    public class InstrumentsTestPlan {
        [XmlElement(nameof(Stationary))] public List<Stationary> Stationary { get; set; }
        [XmlElement(nameof(Mobile))] public List<Mobile> Mobile { get; set; }

        public InstrumentsTestPlan() { }

        public List<InstrumentInfo> GetInfo() {
            List<InstrumentInfo> instruments = new List<InstrumentInfo>();

            InstrumentTestExec instrumentTestExec = null;
            foreach (Stationary stationary in Data.testPlanDefinition.InstrumentsTestPlan.Stationary) {
                instrumentTestExec = Data.testExecDefinition.InstrumentsTestExec.InstrumentTestExec.Find(x => x.ID == stationary.ID) ?? throw new ArgumentException($"Instrument with ID '{stationary.ID}' not present in file '{Data.TestExecDefinitionXML}'.");
                instruments.Add(new InstrumentInfo(stationary.ID, stationary.Alias, instrumentTestExec.NameSpacedClassName));
            }

            foreach (Mobile mobile in Mobile) instruments.Add(new InstrumentInfo(mobile.ID, mobile.Alias, mobile.NameSpacedClassName));
            return instruments;
        }
    }

    public class InstrumentInfo {
        public String ID;
        public String Alias;
        public String NameSpacedClassName;

        public InstrumentInfo() { }

        public InstrumentInfo(String id, String alias, String nameSpaceClassName) {
            ID = id;
            Alias = alias;
            NameSpacedClassName = nameSpaceClassName;
        }
    }

    public class Stationary {
        [XmlAttribute(nameof(ID))] public String ID { get; set; }
        [XmlAttribute(nameof(Alias))] public String Alias { get; set; }

        public Stationary() { }
    }

    public class Mobile : Stationary {
        [XmlAttribute(nameof(Detail))] public String Detail { get; set; }
        [XmlAttribute(nameof(Address))] public String Address { get; set; }
        [XmlAttribute(nameof(NameSpacedClassName))] public String NameSpacedClassName { get; set; }

        public Mobile() { }
    }

    public interface IAssertion { String Assertion(); }

    public class TestSpace : IAssertion {
        [XmlAttribute(nameof(NamespaceRoot))] public String NamespaceRoot { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(Simulate))] public Boolean Simulate { get; set; }
        [XmlElement(nameof(TestOperation))] public List<TestOperation> TestOperations { get; set; }

        public TestSpace() { }

        public Statistics Statistics { get; set; } = new Statistics();

        public String StatisticsDisplay() {
            const Int32 L = 6; Int32 PR = 17;
            StringBuilder sb = new StringBuilder();


            sb.AppendLine($"{nameof(Statistics.EmergencyStopped)}".PadRight(PR) + $": {Statistics.EmergencyStopped,L}, {Statistics.FractionEmergencyStopped(),L:P1}");
            sb.AppendLine($"{nameof(Statistics.Errored)}".PadRight(PR) + $": {Statistics.Errored,L}, {Statistics.FractionErrored(),L:P1}");
            sb.AppendLine($"{nameof(Statistics.Cancelled)}".PadRight(PR) + $": {Statistics.Cancelled,L}, {Statistics.FractionCancelled(),L:P1}");
            sb.AppendLine($"{nameof(Statistics.Unset)}".PadRight(PR) + $": {Statistics.Unset,L}, {Statistics.FractionUnset(),L:P1}");
            sb.AppendLine($"{nameof(Statistics.Failed)}".PadRight(PR) + $": {Statistics.Failed,L}, {Statistics.FractionFailed(),L:P1}");
            sb.AppendLine($"{nameof(Statistics.Passed)}".PadRight(PR) + $": {Statistics.Passed,L}, {Statistics.FractionPassed(),L:P1}");
            sb.AppendLine($"{nameof(Statistics.Informed)}".PadRight(PR) + $": {Statistics.Informed,L}, {Statistics.FractionInformed(),L:P1}");

            sb.AppendLine($"------");
            sb.AppendLine($"Total     : {Statistics.Tested(),L}");
            return sb.ToString();
        }

        public String StatisticsStatus() { return $"   Failed: {Statistics.Failed}     Passed: {Statistics.Passed}   "; }

        public String StatusTime() { return $"   Time: {Statistics.Time()}"; }

        public String Assertion() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Debug.Assert({GetType().Name}(");
            sb.Append($"{nameof(NamespaceRoot)}: {UUT.EF(GetType().GetProperty(nameof(NamespaceRoot)).GetValue(this))}, ");
            sb.Append($"{nameof(Description)}: {UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}));");
            sb.Append($", {nameof(TestOperations)}: {String.Join(UUT.DIVIDER, TestOperations)}));");
            return sb.ToString();
        }

        public String TOs() { return String.Join(UUT.DIVIDER, TestOperations); }
    }

    public class TestOperation : IAssertion {
        [XmlAttribute(nameof(NamespaceTrunk))] public String NamespaceTrunk { get; set; }
        [XmlAttribute(nameof(ProductionTest))] public Boolean ProductionTest { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlElement(nameof(TestGroup))] public List<TestGroup> TestGroups { get; set; }

        public TestOperation() { }

        public String Assertion() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"if ({nameof(Data)}.{nameof(Data.testSequence)}.{nameof(Data.testSequence.IsOperation)}) Debug.Assert({GetType().Name}(");
            sb.Append($"{nameof(NamespaceTrunk)}: {UUT.EF(GetType().GetProperty(nameof(NamespaceTrunk)).GetValue(this))}, ");
            sb.Append($"{nameof(ProductionTest)}: {UUT.EF(GetType().GetProperty(nameof(ProductionTest)).GetValue(this).ToString().ToLower())}, ");
            sb.Append($"{nameof(Description)}: {UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}");
            sb.Append($", {nameof(TestGroups)}: {String.Join(UUT.DIVIDER, TestGroups)}));");
            return sb.ToString();
        }

        public String TGs() { return String.Join(UUT.DIVIDER, TestGroups); }
    }

    public class TestGroup : IAssertion {
        [XmlAttribute(nameof(Classname))] public String Classname { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(CancelNotPassed))] public Boolean CancelNotPassed { get; set; }
        [XmlAttribute(nameof(Independent))] public Boolean Independent { get; set; }
        [XmlElement(nameof(MethodCustom), typeof(MethodCustom))]
        [XmlElement(nameof(MethodInterval), typeof(MethodInterval))]
        [XmlElement(nameof(MethodProcess), typeof(MethodProcess))]
        [XmlElement(nameof(MethodTextual), typeof(MethodTextual))]
        public List<Method> Methods { get; set; }

        public TestGroup() { }

        // public String AssertionPrior() { return $"{UUT.CHECK_OPERATION}Debug.Assert({nameof(Assertions.TestGroupPrior)}({nameof(Classname)}: {UUT.EF(GetType().GetProperty(nameof(Classname)).GetValue(this))}));"; }
        // NOTE:  Presently unused.

        public String Assertion() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Debug.Assert({GetType().Name}(");
            sb.Append($"{nameof(Classname)}: {UUT.EF(GetType().GetProperty(nameof(Classname)).GetValue(this))}, ");
            sb.Append($"{nameof(Description)}: {UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}, ");
            sb.Append($"{nameof(CancelNotPassed)}: {UUT.EF(GetType().GetProperty(nameof(CancelNotPassed)).GetValue(this).ToString().ToLower())}, ");
            sb.Append($"{nameof(Independent)}: {UUT.EF(GetType().GetProperty(nameof(Independent)).GetValue(this).ToString().ToLower())}));");
            sb.Append($", {nameof(Methods)}: {String.Join(UUT.DIVIDER, Methods)}));");
            return sb.ToString();
        }
        public String Ms() { return String.Join(UUT.DIVIDER, Methods); }

        // public String AssertionNext() { return $"{UUT.CHECK_OPERATION}Debug.Assert({nameof(Assertions.TestGroupNext)}({nameof(Classname)}: {UUT.EF(GetType().GetProperty(nameof(Classname)).GetValue(this))}));"; }
        // NOTE:  Presently unused.
    }

    public interface IFormat { String Format(); }

    public interface IEvaluate { EVENTS Evaluate(); }

    public abstract class Method : IAssertion, IEvaluate, IFormat {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(CancelNotPassed))] public Boolean CancelNotPassed { get; set; }
        public String Value { get; set; }
        public EVENTS Event { get; set; }
        [XmlIgnore] public StringBuilder Log { get; set; } = new StringBuilder();
        public String LogString { get; set; } = String.Empty;
        internal const String EXPECTED = "Expected";
        internal const String ACTUAL = "Actual";

        public Method() { }

        public virtual String Assertion() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{nameof(Name)}: {UUT.EF(GetType().GetProperty(nameof(Name)).GetValue(this))}, ");
            sb.Append($"{nameof(Description)}: {UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}, ");
            sb.Append($"{nameof(CancelNotPassed)}: {UUT.EF(GetType().GetProperty(nameof(CancelNotPassed)).GetValue(this).ToString().ToLower())}");
            return sb.ToString();
        }

        public Boolean Assert(String Name, String Description, String CancelNotPassed) {
            return String.Equals(this.Name, Name) && String.Equals(this.Description, Description) && this.CancelNotPassed == Boolean.Parse(CancelNotPassed);
        }

        // public String AssertionPrior() { return $"Debug.Assert({nameof(Assertions.MethodPrior)}({nameof(Name)}: {UUT.EF(GetType().GetProperty(nameof(Name)).GetValue(this))}));"; }
        // NOTE:  Presently unused.

        // public String AssertionNext() { return $"Debug.Assert({nameof(Assertions.MethodNext)}({nameof(Name)}: {UUT.EF(GetType().GetProperty(nameof(Name)).GetValue(this))}));"; }
        // NOTE:  Presently unused.

        public abstract EVENTS Evaluate();

        public abstract String Format();

        public String LogFetchAndClear() {
            String s = Log.ToString();
            Log.Clear();
            return s;
        }
    }

    public class MethodCustom : Method, IAssertion, IEvaluate, IFormat {
        // TODO: Eventually; create XML object formatting method in class MethodCustom, so can actually serialize MethodCustom objects into XML in the Value property.
        [XmlElement(nameof(Parameter))] public List<Parameter> Parameters { get; set; }

        public MethodCustom() { }

        public override String Assertion() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Debug.Assert({GetType().Name}(");
            sb.Append($"{base.Assertion()}");
            if (Parameters.Count > 0) sb.Append($", {nameof(Parameters)}: {String.Join(UUT.DIVIDER, Parameters)}));");
            return sb.ToString();
        }

        public Boolean Assert(String Name, String Description, String CancelNotPassed, String Parameters = null) {
            Boolean boolean = base.Assert(Name, Description, CancelNotPassed);
            if (Parameters != null) boolean &= String.Equals(Ps().Replace("\"", ""), Parameters);
            return boolean;
        }

        public String Ps() { return String.Join(UUT.DIVIDER, Parameters); }

        public override EVENTS Evaluate() { return Event; } // NOTE:  MethodCustoms have their Events set in their TestPlan methods.

        public override String Format() { return Value; }
    }

    public class Parameter {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Value))] public String Value { get; set; }

        public Parameter() { }
    }

    public class MethodInterval : Method, IAssertion, IEvaluate, IFormat {
        [XmlAttribute(nameof(LowComparator))] public MI_LowComparator LowComparator { get; set; }
        [XmlAttribute(nameof(Low))] public Double Low { get; set; }
        [XmlAttribute(nameof(High))] public Double High { get; set; }
        [XmlAttribute(nameof(HighComparator))] public MI_HighComparator HighComparator { get; set; }
        [XmlAttribute(nameof(FractionalDigits))] public UInt32 FractionalDigits { get; set; }
        [XmlAttribute(nameof(UnitPrefix))] public MI_UnitPrefix UnitPrefix { get; set; }
        [XmlAttribute(nameof(Units))] public MI_Units Units { get; set; }
        [XmlAttribute(nameof(UnitSuffix))] public MI_UnitSuffix UnitSuffix { get; set; }
        [XmlIgnore]
        public static Dictionary<MI_UnitPrefix, Double> UnitPrefixes = new Dictionary<MI_UnitPrefix, Double>() {
            { MI_UnitPrefix.peta, 1E15 } ,
            { MI_UnitPrefix.tera, 1E12 },
            { MI_UnitPrefix.giga, 1E9 },
            { MI_UnitPrefix.mega, 1E6 },
            { MI_UnitPrefix.kilo, 1E3 },
            { MI_UnitPrefix.NONE, 1E0 },
            { MI_UnitPrefix.milli, 1E-3 },
            { MI_UnitPrefix.micro, 1E-6 },
            { MI_UnitPrefix.nano, 1E-9 },
            { MI_UnitPrefix.pico, 1E-12 },
            { MI_UnitPrefix.femto, 1E-15}
        };

        public MethodInterval() { }

        public override String Assertion() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Debug.Assert({GetType().Name}(");
            sb.Append($"{base.Assertion()}, ");
            sb.Append($"{nameof(LowComparator)}: {UUT.EF(GetType().GetProperty(nameof(LowComparator)).GetValue(this))}, ");
            sb.Append($"{nameof(Low)}: {UUT.EF(GetType().GetProperty(nameof(Low)).GetValue(this))}, ");
            sb.Append($"{nameof(High)}: {UUT.EF(GetType().GetProperty(nameof(High)).GetValue(this))}, ");
            sb.Append($"{nameof(HighComparator)}: {UUT.EF(GetType().GetProperty(nameof(HighComparator)).GetValue(this))}, ");
            sb.Append($"{nameof(FractionalDigits)}: {UUT.EF(GetType().GetProperty(nameof(FractionalDigits)).GetValue(this))}, ");
            sb.Append($"{nameof(UnitPrefix)}: {UUT.EF(GetType().GetProperty(nameof(UnitPrefix)).GetValue(this))}, ");
            sb.Append($"{nameof(Units)}: {UUT.EF(GetType().GetProperty(nameof(Units)).GetValue(this))}, ");
            sb.Append($"{nameof(UnitSuffix)}: {UUT.EF(GetType().GetProperty(nameof(UnitSuffix)).GetValue(this))}));");
            return sb.ToString();
        }

        public override EVENTS Evaluate() {
            if (!Double.TryParse(Value, NumberStyles.Float, CultureInfo.CurrentCulture, out Double d)) throw new InvalidOperationException($"{nameof(MethodInterval)} '{Name}' {nameof(Value)} '{Value}' ≠ System.Double.");
            d /= UnitPrefixes[UnitPrefix];
            Value = d.ToString("G");
            if (LowComparator is MI_LowComparator.GToE && HighComparator is MI_HighComparator.LToE) return ((Low <= d) && (d <= High)) ? EVENTS.PASS : EVENTS.FAIL;
            if (LowComparator is MI_LowComparator.GToE && HighComparator is MI_HighComparator.LT) return ((Low <= d) && (d < High)) ? EVENTS.PASS : EVENTS.FAIL;
            if (LowComparator is MI_LowComparator.GT && HighComparator is MI_HighComparator.LToE) return ((Low < d) && (d <= High)) ? EVENTS.PASS : EVENTS.FAIL;
            if (LowComparator is MI_LowComparator.GT && HighComparator is MI_HighComparator.LT) return ((Low < d) && (d < High)) ? EVENTS.PASS : EVENTS.FAIL;
            throw new NotImplementedException($"{nameof(MethodInterval)} '{Name}', {nameof(Description)} '{Description}', contains unimplemented comparators '{LowComparator}' and/or '{HighComparator}'.");
        }

        public override String Format() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(FormatMessage(nameof(High), $"{High:G}"));
            stringBuilder.AppendLine(FormatMessage(nameof(Value), $"{Math.Round(Double.Parse(Value), (Int32)FractionalDigits, MidpointRounding.ToEven)}"));
            stringBuilder.AppendLine(FormatMessage(nameof(Low), $"{Low:G}"));
            String units = String.Empty;
            if (UnitPrefix != MI_UnitPrefix.NONE) units += $"{Enum.GetName(typeof(MI_UnitPrefix), UnitPrefix)}";
            units += $"{Enum.GetName(typeof(MI_Units), Units)}";
            if (UnitSuffix != MI_UnitSuffix.NONE) units += $" {Enum.GetName(typeof(MI_UnitSuffix), UnitSuffix)}";
            stringBuilder.AppendLine(FormatMessage(nameof(Units), units));
            return stringBuilder.ToString();
        }
    }

    public enum MI_LowComparator { GToE, GT }
    public enum MI_HighComparator { LToE, LT }
    public enum MI_UnitPrefix { peta, tera, giga, mega, kilo, NONE, milli, micro, nano, pico, femto }
    public enum MI_Units { NONE, Amperes, Celcius, Farads, Henries, Hertz, Ohms, Seconds, Siemens, Volts, VoltAmperes, Watts }
    public enum MI_UnitSuffix { NONE, AC, DC, Peak, PP, RMS }

    public class MethodProcess : Method, IAssertion, IEvaluate, IFormat {
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
        [XmlAttribute(nameof(File))] public String File { get; set; }
        [XmlAttribute(nameof(Parameters))] public String Parameters { get; set; }
        [XmlAttribute(nameof(Expected))] public String Expected { get; set; }

        public MethodProcess() { }

        public override String Assertion() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Debug.Assert({GetType().Name}(");
            sb.Append($"{base.Assertion()}, ");
            sb.Append($"{nameof(Folder)}: {UUT.EF(GetType().GetProperty(nameof(Folder)).GetValue(this))}, ");
            sb.Append($"{nameof(File)}: {UUT.EF(GetType().GetProperty(nameof(File)).GetValue(this))}, ");
            sb.Append($"{nameof(Parameters)}: {UUT.EF(GetType().GetProperty(nameof(Parameters)).GetValue(this))}, ");
            sb.Append($"{nameof(Expected)}: {UUT.EF(GetType().GetProperty(nameof(Expected)).GetValue(this))}));");
            return sb.ToString();
        }

        public override EVENTS Evaluate() { return (String.Equals(Expected, Value, StringComparison.Ordinal)) ? EVENTS.PASS : EVENTS.FAIL; }

        public override String Format() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(FormatMessage(EXPECTED, Expected));
            stringBuilder.AppendLine(FormatMessage(ACTUAL, Value));
            return stringBuilder.ToString();
        }
    }

    public class MethodTextual : Method, IAssertion, IEvaluate, IFormat {
        [XmlAttribute(nameof(Text))] public String Text { get; set; }

        public MethodTextual() { }

        public override String Assertion() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Debug.Assert({GetType().Name}(");
            sb.Append($"{base.Assertion()}, ");
            sb.Append($"{nameof(Text)}: {UUT.EF(GetType().GetProperty(nameof(Text)).GetValue(this))}));");
            return sb.ToString();
        }

        public override EVENTS Evaluate() { return (String.Equals(Text, Value, StringComparison.Ordinal)) ? EVENTS.PASS : EVENTS.FAIL; }

        public override String Format() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(FormatMessage(EXPECTED, Text));
            stringBuilder.AppendLine(FormatMessage(ACTUAL, Value));
            return stringBuilder.ToString();
        }
    }

    public class Statistics {
        public UInt32 EmergencyStopped = 0;
        public UInt32 Errored = 0;
        public UInt32 Cancelled = 0;
        public UInt32 Unset = 0;
        public UInt32 Failed = 0;
        public UInt32 Passed = 0;
        public UInt32 Informed = 0;

        private readonly DateTime TestSelected = DateTime.Now;

        public Statistics() { }

        public void Update(EVENTS Event) {
            switch (Event) {
                case EVENTS.EMERGENCY_STOP: EmergencyStopped++; break;
                case EVENTS.ERROR: Errored++; break;
                case EVENTS.CANCEL: Cancelled++; break;
                case EVENTS.UNSET: Unset++; break;
                case EVENTS.FAIL: Failed++; break;
                case EVENTS.PASS: Passed++; break;
                case EVENTS.INFORMATION: Informed++; break;
                default: throw new NotImplementedException($"Event '{Event}' not implemented.");
            }
        }

        public String Time() {
            TimeSpan elapsedTime = DateTime.Now - TestSelected;
            return $"{(elapsedTime.Days != 0 ? elapsedTime.Days.ToString() + ":" : String.Empty)}{elapsedTime.Hours}:{elapsedTime.Minutes:00}";
        }
        public Double FractionEmergencyStopped() { return Convert.ToDouble(EmergencyStopped) / Convert.ToDouble(Tested()); }
        public Double FractionErrored() { return Convert.ToDouble(Errored) / Convert.ToDouble(Tested()); }
        public Double FractionCancelled() { return Convert.ToDouble(Cancelled) / Convert.ToDouble(Tested()); }
        public Double FractionUnset() { return Convert.ToDouble(Unset) / Convert.ToDouble(Tested()); }
        public Double FractionFailed() { return Convert.ToDouble(Failed) / Convert.ToDouble(Tested()); }
        public Double FractionPassed() { return Convert.ToDouble(Passed) / Convert.ToDouble(Tested()); }
        public Double FractionInformed() { return Convert.ToDouble(Informed) / Convert.ToDouble(Tested()); }
        public UInt32 Tested() { return EmergencyStopped + Errored + Cancelled + Unset + Failed + Passed + Informed; }
    }

    public static class TestIndices {
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