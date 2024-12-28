using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
namespace ABT.Test.TestLib.TestDefinition {
    public interface IAssertionCurrent { String AssertionCurrent(); }

    [XmlRoot(nameof(TestDefinition))] public class TestDefinition {
        [XmlElement(nameof(UUT))] public UUT UUT { get; set; }
        [XmlElement(nameof(Development))] public Development Development { get; set; }
        [XmlArray(nameof(Modifications))] public Modification[] Modifications { get; set; }
        [XmlElement(nameof(TestData))] public TestData TestData { get; set; }
        [XmlElement(nameof(Instruments))] public Instruments Instruments { get; set; }
        [XmlElement(nameof(TS))] public TS TS { get; set; }
    }

    public class UUT {
        [XmlElement(nameof(Customer))] public Customer Customer { get; set; }
        [XmlElement(nameof(TestSpecification))] public TestSpecification[] TestSpecification { get; set; }
        [XmlElement(nameof(Documentation))] public Documentation[] Documentation { get; set; }
        [XmlAttribute(nameof(Number))] public String Number { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(Revision))] public String Revision { get; set; }
        [XmlAttribute(nameof(Category))] public Category Category { get; set; }
    }

    public class Customer {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Division))] public String Division { get; set; }
        [XmlAttribute(nameof(Location))] public String Location { get; set; }
    }

    public class TS : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(NamespaceRoot))] public String NamespaceRoot { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlElement(nameof(TO))] public List<TO> TestOperations { get; set; }
        public Statistics Statistics { get; set; } = new Statistics();
        internal const String DEBUG_ASSERT = "Debug.Assert(";
        internal const String BEGIN = "(";
        internal const String CS = ": ";
        internal const String CONTINUE = ", ";
        internal const String END = "));";
        internal const String DIVIDER = "|";
        internal const String NONE = "\"NONE\"";
        
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

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{DEBUG_ASSERT}{GetType().Name}{BEGIN}");
            sb.Append($"{nameof(NamespaceRoot)}{CS}{EF(GetType().GetProperty(nameof(NamespaceRoot)).GetValue(this))}{CONTINUE}");
            sb.Append($"{nameof(Description)}{CS}{EF(GetType().GetProperty(nameof(Description)).GetValue(this))}");
            sb.Append($"{CONTINUE}{nameof(TestOperations)}{CS}{TOs()}");
            sb.Append($"{END}");
            return sb.ToString();
        }

        public static String EF(Object o) {
            String s = (o.ToString()).Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'");
            return $"\"{s}\"";
        }

        public String TOs() {
            StringBuilder sb = new StringBuilder();
            foreach (TO to in TestOperations) sb.Append($"{to.NamespaceTrunk}{DIVIDER}");
            return EF(sb.Remove(sb.Length - DIVIDER.Length, DIVIDER.Length).ToString()); // Remove trailing DIVIDER.
        }
    }

    public class TO : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(NamespaceTrunk))] public String NamespaceTrunk { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlElement(nameof(TG))] public List<TG> TestGroups { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{TS.DEBUG_ASSERT}{GetType().Name}{TS.BEGIN}");
            sb.Append($"{nameof(NamespaceTrunk)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(NamespaceTrunk)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(Description)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}");
            sb.Append($"{TS.CONTINUE}{nameof(TestGroups)}{TS.CS}{TGs()}");
            sb.Append($"{TS.END}");
            return sb.ToString();
        }

        public String TGs() {
            StringBuilder sb = new StringBuilder();
            foreach (TG tg in TestGroups) sb.Append($"{tg.Class}{TS.DIVIDER}");
            return TS.EF(sb.Remove(sb.Length - TS.DIVIDER.Length, TS.DIVIDER.Length).ToString()); // Remove trailing DIVIDER.
        }
    }

    public class TG : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Class))] public String Class { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(CancelNotPassed))] public Boolean CancelNotPassed { get; set; }
        [XmlAttribute(nameof(Independent))] public Boolean Independent { get; set; }
        [XmlElement(nameof(MC), typeof(MC))]
        [XmlElement(nameof(MI), typeof(MI))]
        [XmlElement(nameof(MP), typeof(MP))]
        [XmlElement(nameof(MT), typeof(MT))]
        public List<M> Methods { get; set; }
        public readonly Int32 FormattingLengthGroupID = 0;
        public readonly Int32 FormattingLengthMeasurementID = 0;

        public String AssertionPrior() { return $"{TS.DEBUG_ASSERT}{nameof(Assertions.TG_Prior)}{TS.BEGIN}{nameof(Class)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Class)).GetValue(this))}{TS.END}"; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{TS.DEBUG_ASSERT}{GetType().Name}{TS.BEGIN}");
            sb.Append($"{nameof(Class)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Class)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(Description)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(CancelNotPassed)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(CancelNotPassed)).GetValue(this).ToString().ToLower())}{TS.CONTINUE}");
            sb.Append($"{nameof(Independent)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Independent)).GetValue(this).ToString().ToLower())}");
            sb.Append($"{TS.CONTINUE}{nameof(Methods)}{TS.CS}{Ms()}");
            sb.Append($"{TS.END}");
            return sb.ToString();
        }
        public String Ms() {
            StringBuilder sb = new StringBuilder();
            foreach (M m in Methods) sb.Append($"{m.Method}{TS.DIVIDER}");
            return TS.EF(sb.Remove(sb.Length - TS.DIVIDER.Length, TS.DIVIDER.Length).ToString()); // Remove trailing TS.DIVIDER.
        }

        public String AssertionNext() { return $"{TS.DEBUG_ASSERT}{nameof(Assertions.TG_Next)}{TS.BEGIN}{nameof(Class)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Class)).GetValue(this))}{TS.END}"; }
    }

    public abstract class M {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Method))] public String Method { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(CancelNotPassed))] public Boolean CancelNotPassed { get; set; }
        public Object Value { get; set; }
        public EVENTS Event { get; set; }
        public StringBuilder Log { get; set; } = new StringBuilder();
        public String AssertionPrior() { return $"{TS.DEBUG_ASSERT}{nameof(Assertions.M_Prior)}{TS.BEGIN}{nameof(Method)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Method)).GetValue(this))}{TS.END}"; }

        private protected String AssertionM() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{nameof(Method)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Method)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(Description)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(CancelNotPassed)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(CancelNotPassed)).GetValue(this).ToString().ToLower())}");
            return sb.ToString();
        }

        public String AssertionNext() { return $"{TS.DEBUG_ASSERT}{nameof(Assertions.M_Next)}{TS.BEGIN}{nameof(Method)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Method)).GetValue(this))}{TS.END}"; }
    }

    public class MC : M, IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlElement(nameof(Parameter))] public List<Parameter> Parameters { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{TS.DEBUG_ASSERT}{GetType().Name}{TS.BEGIN}");
            sb.Append($"{AssertionM()}");
            if (Parameters.Count > 0) sb.Append($"{TS.CONTINUE}{nameof(Parameters)}{TS.CS}{Ps()}");
            sb.Append($"{TS.END}");
            return sb.ToString();
        }
        public String Ps() {
            StringBuilder sb = new StringBuilder();
            foreach (Parameter p in Parameters) sb.Append($"{p.Key}={p.Value}{TS.DIVIDER}");
            return TS.EF(sb.Remove(sb.Length - TS.DIVIDER.Length, TS.DIVIDER.Length).ToString()); // Remove trailing TS.DIVIDER.
        }
    }

    public class Parameter {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Key))] public String Key { get; set; }
        [XmlAttribute(nameof(Value))] public String Value { get; set; }
    }

    public class MI : M, IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(LowComparator))] public MI_LowComparator LowComparator { get; set; }
        [XmlAttribute(nameof(Low))] public Double Low { get; set; }
        [XmlAttribute(nameof(High))] public Double High { get; set; }
        [XmlAttribute(nameof(HighComparator))] public MI_HighComparator HighComparator { get; set; }
        [XmlAttribute(nameof(FractionalDigits))] public UInt32 FractionalDigits { get; set; }
        [XmlAttribute(nameof(UnitPrefix))] public MI_UnitPrefix UnitPrefix { get; set; }
        [XmlAttribute(nameof(Units))] public MI_Units Units { get; set; }
        [XmlAttribute(nameof(UnitSuffix))] public MI_UnitSuffix UnitSuffix { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{TS.DEBUG_ASSERT}{GetType().Name}{TS.BEGIN}");
            sb.Append($"{AssertionM()}{TS.CONTINUE}");
            sb.Append($"{nameof(LowComparator)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(LowComparator)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(Low)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Low)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(High)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(High)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(HighComparator)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(HighComparator)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(FractionalDigits)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(FractionalDigits)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(UnitPrefix)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(UnitPrefix)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(Units)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Units)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(UnitSuffix)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(UnitSuffix)).GetValue(this))}{TS.END}");
            return sb.ToString();
        }
    }

    public enum MI_LowComparator { GE, GT }
    public enum MI_HighComparator { LE, LT }
    public enum MI_UnitPrefix { NONE, peta, tera, giga, mega, kilo, hecto, deca, deci, centi, milli, micro, nano, pico, femto }
    public enum MI_Units { NONE, Amperes, Celcius, Farads, Henries, Hertz, Ohms, Seconds, Siemens, Volts, VoltAmperes, Watts }
    public enum MI_UnitSuffix { NONE, AC, DC, Peak, PP, RMS }

    public class MP : M, IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Path))] public String Path { get; set; }
        [XmlAttribute(nameof(Executable))] public String Executable { get; set; }
        [XmlAttribute(nameof(Parameters))] public String Parameters { get; set; }
        [XmlAttribute(nameof(Expected))] public String Expected { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{TS.DEBUG_ASSERT}{GetType().Name}{TS.BEGIN}");
            sb.Append($"{AssertionM()}{TS.CONTINUE}");
            sb.Append($"{nameof(Path)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Path)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(Executable)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Executable)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(Parameters)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Parameters)).GetValue(this))}{TS.CONTINUE}");
            sb.Append($"{nameof(Expected)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Expected)).GetValue(this))}{TS.END}");
            return sb.ToString();
        }
    }

    public class MT : M, IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Text))] public String Text { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{TS.DEBUG_ASSERT}{GetType().Name}{TS.BEGIN}");
            sb.Append($"{AssertionM()}{TS.CONTINUE}");
            sb.Append($"{nameof(Text)}{TS.CS}{TS.EF(GetType().GetProperty(nameof(Text)).GetValue(this))}{TS.END}");
            return sb.ToString();
        }
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

    public class Mobile {
        [XmlAttribute(nameof(ID))] public String ID { get; set; }
        [XmlAttribute(nameof(FullyQualifiedMethod))] public String FullyQualifiedMethod { get; set; }
        [XmlAttribute(nameof(Detail))] public String Detail { get; set; }
        [XmlAttribute(nameof(Address))] public String Address { get; set; }
    }

    public class Stationary {
        [XmlAttribute(nameof(ID))] public String ID { get; set; }
        [XmlAttribute(nameof(FullyQualifiedMethod))] public String FullyQualifiedMethod { get; set; }
    }

    public class SQL {
        [XmlAttribute(nameof(ConnectionString))] public String ConnectionString { get; set; }
        [XmlAttribute(nameof(SerialEntry))] public SerialEntry SerialEntry { get; set; }
        [XmlAttribute(nameof(SerialRegEx))] public String SerialRegEx { get; set; }
    }

    public enum SerialEntry { Barcode, BarcodeεKeyboard, Keyboard }

    public class TDR {
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
        [XmlAttribute(nameof(SerialEntry))] public SerialEntry SerialEntry { get; set; }
        [XmlAttribute(nameof(SerialRegEx))] public String SerialRegEx { get; set; }
    }

    public class Modification {
        [XmlAttribute(nameof(Who))] public String Who { get; set; }
        [XmlAttribute(nameof(What))] public String What { get; set; }
        [XmlAttribute(nameof(When))] public System.DateTime When { get; set; }
        [XmlAttribute(nameof(Where))] public String Where { get; set; }
        [XmlAttribute(nameof(Why))] public String Why { get; set; }
    }

    public class Repository {
        [XmlAttribute(nameof(URL))] public String URL { get; set; }
    }

    public class Developer {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Language))] public Language Language { get; set; }
        [XmlAttribute(nameof(Comment))] public String Comment { get; set; }
    }

    public enum Language { CSharp, Python, VEE }

    public class Documentation {
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
    }

    public class TestSpecification {
        [XmlAttribute(nameof(Document))] public String Document { get; set; }
        [XmlAttribute(nameof(Revision))] public String Revision { get; set; }
        [XmlAttribute(nameof(Title))] public String Title { get; set; }
        [XmlAttribute(nameof(Date))] public System.DateTime Date { get; set; }
    }

    public enum Category { Component, CircuitCard, Harness, Unit, System }

    public class Development {
        [XmlElement(nameof(Developer))] public Developer[] Developer { get; set; }
        [XmlElement(nameof(Documentation))] public Documentation[] Documentation { get; set; }
        [XmlElement(nameof(Repository))] public Repository[] Repository { get; set; }
        [XmlAttribute(nameof(Released))] public System.DateTime Released { get; set; }
    }

    public class TestData {
        [XmlElement(nameof(SQL), typeof(SQL))]
        [XmlElement(nameof(TDR), typeof(TDR))]
        public Object Item { get; set; }
    }

    public class Instruments {
        [XmlElement(nameof(Stationary))] public Stationary[] Stationary { get; set; }
        [XmlElement(nameof(Mobile))] public Mobile[] Mobile { get; set; }
    }
}