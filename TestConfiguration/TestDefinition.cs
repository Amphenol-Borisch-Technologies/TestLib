using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ABT.Test.TestLib.TestConfiguration {
    public interface IAssertionCurrent { String AssertionCurrent(); }

    [XmlRoot(nameof(TestDefinition))]
    public class TestDefinition {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlElement(nameof(UUT))] public UUT UUT { get; set; }
        [XmlElement(nameof(Development))] public Development Development { get; set; }
        [XmlArray(nameof(Modifications))] public List<Modification> Modifications { get; set; }
        [XmlElement(nameof(TestData))] public TestData TestData { get; set; }
        [XmlElement(nameof(Instruments))] public Instruments Instruments { get; set; }
        [XmlElement(nameof(TestSpace))] public TestSpace TestSpace { get; set; }
    }

    public class UUT : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlElement(nameof(Customer))] public Customer Customer { get; set; }
        [XmlElement(nameof(TestSpecification))] public List<TestSpecification> TestSpecification { get; set; }
        [XmlElement(nameof(Documentation))] public List<Documentation> Documentation { get; set; }
        [XmlAttribute(nameof(Number))] public String Number { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(Revision))] public String Revision { get; set; }
        [XmlAttribute(nameof(Category))] public Category Category { get; set; }
        internal const String DEBUG_ASSERT = "Debug.Assert(";
        internal const String BEGIN = "(";
        internal const String CS = ": ";
        internal const String CONTINUE = ", ";
        internal const String END = "));";
        internal const String DIVIDER = "|";
        internal const String NONE = "\"NONE\"";

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{DEBUG_ASSERT}{GetType().Name}{BEGIN}");
            sb.Append($"{nameof(Number)}{CS}{EF(GetType().GetProperty(nameof(Number)).GetValue(this))}{CONTINUE}");
            sb.Append($"{nameof(Description)}{CS}{EF(GetType().GetProperty(nameof(Description)).GetValue(this))}{CONTINUE}");
            sb.Append($"{nameof(Revision)}{CS}{EF(GetType().GetProperty(nameof(Revision)).GetValue(this))}{CONTINUE}");
            sb.Append($"{nameof(Category)}{CS}{EF(GetType().GetProperty(nameof(Category)).GetValue(this))}");
            sb.Append($"{END}");
            return sb.ToString();
        }

        public static String EF(Object o) {
            String s = (o.ToString()).Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'");
            return $"\"{s}\"";
        }
    }

    public class Customer : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Division))] public String Division { get; set; }
        [XmlAttribute(nameof(Location))] public String Location { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(Name)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Name)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Division)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Division)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Location)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Location)).GetValue(this))}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }
    }

    public class TestSpecification : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Document))] public String Document { get; set; }
        [XmlAttribute(nameof(Revision))] public String Revision { get; set; }
        [XmlAttribute(nameof(Title))] public String Title { get; set; }
        [XmlAttribute(nameof(Date))] public System.DateTime Date { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(Document)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Document)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Revision)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Revision)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Title)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Title)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Date)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Date)).GetValue(this))}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }
    }

    public class Documentation {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
    }

    public enum Category { Component, CircuitCard, Harness, Unit, System }

    public class Development {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlElement(nameof(Developer))] public List<Developer> Developer { get; set; }
        [XmlElement(nameof(Documentation))] public List<Documentation> Documentation { get; set; }
        [XmlElement(nameof(Repository))] public List<Repository> Repository { get; set; }
        [XmlAttribute(nameof(Released))] public System.DateTime Released { get; set; }
    }

    public class Developer {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Language))] public Language Language { get; set; }
        [XmlAttribute(nameof(Comment))] public String Comment { get; set; }
        public String EMailAddress { get; set; } = String.Empty;
    }

    public enum Language { CSharp, Python, VEE }

    public class Repository {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(URL))] public String URL { get; set; }
    }

    public class Modification {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Who))] public String Who { get; set; }
        [XmlAttribute(nameof(What))] public String What { get; set; }
        [XmlAttribute(nameof(When))] public System.DateTime When { get; set; }
        [XmlAttribute(nameof(Where))] public String Where { get; set; }
        [XmlAttribute(nameof(Why))] public String Why { get; set; }
    }

    public class TestData : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlElement(nameof(SQL), typeof(SQL))]
        [XmlElement(nameof(XML), typeof(XML))]
        public Object Item { get; set; }

        public String AssertionCurrent() {
            if (Item == null) return String.Empty;
            else return ((IAssertionCurrent)Item).AssertionCurrent();
        }

        public Boolean IsEnabled() { return Item != null; }
    }

    public abstract class SerialNumber {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(SerialNumberEntry))] public SerialNumberEntry SerialNumberEntry { get; set; }
        [XmlAttribute(nameof(SerialNumberRegEx))] public String SerialNumberRegEx { get; set; }
        [XmlAttribute(nameof(SerialNumberFormat))] public String SerialNumberFormat { get; set; }
    }

    public enum SerialNumberEntry { Barcode, Keyboard }

    public class SQL : SerialNumber, IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(ConnectionString))] public String ConnectionString { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(ConnectionString)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(ConnectionString)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(SerialNumberEntry)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(SerialNumberEntry)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(SerialNumberRegEx)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(SerialNumberRegEx)).GetValue(this))}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }
    }

    public class XML : SerialNumber, IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(Folder)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Folder)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(SerialNumberEntry)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(SerialNumberEntry)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(SerialNumberRegEx)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(SerialNumberRegEx)).GetValue(this))}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }
    }

    public class Instruments : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlElement(nameof(Stationary))] public List<Stationary> Stationary { get; set; }
        [XmlElement(nameof(Mobile))] public List<Mobile> Mobile { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(Stationary)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Stationary)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Mobile)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Mobile)).GetValue(this))}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }
    }

    public class Stationary : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(ID))] public String ID { get; set; }
        [XmlAttribute(nameof(NameSpacedClassName))] public String NameSpacedClassName { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(ID)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(ID)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(NameSpacedClassName)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(NameSpacedClassName)).GetValue(this))}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }
    }

    public class Mobile : Stationary {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Detail))] public String Detail { get; set; }
        [XmlAttribute(nameof(Address))] public String Address { get; set; }

        public new String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(ID)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(ID)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(NameSpacedClassName)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(NameSpacedClassName)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Detail)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Detail)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Address)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Address)).GetValue(this))}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }
    }

    public class TestSpace : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(NamespaceRoot))] public String NamespaceRoot { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(Simulate))] public Boolean Simulate { get; set; }
        [XmlElement(nameof(TestOperation))] public List<TestOperation> TestOperations { get; set; }

        public Statistics Statistics { get; set; } = new Statistics();

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
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(NamespaceRoot)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(NamespaceRoot)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Description)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}");
            sb.Append($"{UUT.CONTINUE}{nameof(TestOperations)}{UUT.CS}{TOs()}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }

        public String TOs() {
            StringBuilder sb = new StringBuilder();
            foreach (TestOperation to in TestOperations) sb.Append($"{to.NamespaceTrunk}{UUT.DIVIDER}");
            return UUT.EF(sb.Remove(sb.Length - UUT.DIVIDER.Length, UUT.DIVIDER.Length).ToString()); // Remove trailing DIVIDER.
        }
    }

    public class TestOperation : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(NamespaceTrunk))] public String NamespaceTrunk { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlElement(nameof(TestGroup))] public List<TestGroup> TestGroups { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(NamespaceTrunk)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(NamespaceTrunk)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Description)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}");
            sb.Append($"{UUT.CONTINUE}{nameof(TestGroups)}{UUT.CS}{TGs()}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }

        public String TGs() {
            StringBuilder sb = new StringBuilder();
            foreach (TestGroup testGroup in TestGroups) sb.Append($"{testGroup.Class}{UUT.DIVIDER}");
            return UUT.EF(sb.Remove(sb.Length - UUT.DIVIDER.Length, UUT.DIVIDER.Length).ToString()); // Remove trailing DIVIDER.
        }
    }

    public class TestGroup : IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Class))] public String Class { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(CancelNotPassed))] public Boolean CancelNotPassed { get; set; }
        [XmlAttribute(nameof(Independent))] public Boolean Independent { get; set; }
        [XmlElement(nameof(MethodCustom), typeof(MethodCustom))]
        [XmlElement(nameof(MethodInterval), typeof(MethodInterval))]
        [XmlElement(nameof(MethodProcess), typeof(MethodProcess))]
        [XmlElement(nameof(MethodTextual), typeof(MethodTextual))]
        public List<Method> Methods { get; set; }
        public readonly Int32 FormattingLengthGroupID = 0;
        public readonly Int32 FormattingLengthMethodID = 0;

        public String AssertionPrior() { return $"{UUT.DEBUG_ASSERT}{nameof(Assertions.TestGroupPrior)}{UUT.BEGIN}{nameof(Class)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Class)).GetValue(this))}{UUT.END}"; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(Class)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Class)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Description)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(CancelNotPassed)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(CancelNotPassed)).GetValue(this).ToString().ToLower())}{UUT.CONTINUE}");
            sb.Append($"{nameof(Independent)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Independent)).GetValue(this).ToString().ToLower())}");
            sb.Append($"{UUT.CONTINUE}{nameof(Methods)}{UUT.CS}{Ms()}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }
        public String Ms() {
            StringBuilder sb = new StringBuilder();
            foreach (Method method in Methods) sb.Append($"{method.Name}{UUT.DIVIDER}");
            return UUT.EF(sb.Remove(sb.Length - UUT.DIVIDER.Length, UUT.DIVIDER.Length).ToString()); // Remove trailing UUT.DIVIDER.
        }

        public String AssertionNext() { return $"{UUT.DEBUG_ASSERT}{nameof(Assertions.TestGroupNext)}{UUT.BEGIN}{nameof(Class)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Class)).GetValue(this))}{UUT.END}"; }
    }

    public abstract class Method {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(CancelNotPassed))] public Boolean CancelNotPassed { get; set; }
        public Object Value { get; set; }
        public EVENTS Event { get; set; }
        public StringBuilder Log { get; set; } = new StringBuilder();
        public String AssertionPrior() { return $"{UUT.DEBUG_ASSERT}{nameof(Assertions.MethodPrior)}{UUT.BEGIN}{nameof(Name)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Name)).GetValue(this))}{UUT.END}"; }

        private protected String AssertionM() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{nameof(Name)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Name)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Description)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(CancelNotPassed)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(CancelNotPassed)).GetValue(this).ToString().ToLower())}");
            return sb.ToString();
        }

        public String AssertionNext() { return $"{UUT.DEBUG_ASSERT}{nameof(Assertions.MethodNext)}{UUT.BEGIN}{nameof(Name)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Name)).GetValue(this))}{UUT.END}"; }
    }

    public class MethodCustom : Method, IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlElement(nameof(Parameter))] public List<Parameter> Parameters { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{AssertionM()}");
            if (Parameters.Count > 0) sb.Append($"{UUT.CONTINUE}{nameof(Parameters)}{UUT.CS}{Ps()}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }
        public String Ps() {
            StringBuilder sb = new StringBuilder();
            foreach (Parameter p in Parameters) sb.Append($"{p.Key}={p.Value}{UUT.DIVIDER}");
            return UUT.EF(sb.Remove(sb.Length - UUT.DIVIDER.Length, UUT.DIVIDER.Length).ToString()); // Remove trailing UUT.DIVIDER.
        }
    }

    public class Parameter {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Key))] public String Key { get; set; }
        [XmlAttribute(nameof(Value))] public String Value { get; set; }
    }

    public class MethodInterval : Method, IAssertionCurrent {
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
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{AssertionM()}{UUT.CONTINUE}");
            sb.Append($"{nameof(LowComparator)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(LowComparator)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Low)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Low)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(High)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(High)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(HighComparator)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(HighComparator)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(FractionalDigits)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(FractionalDigits)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(UnitPrefix)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(UnitPrefix)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Units)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Units)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(UnitSuffix)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(UnitSuffix)).GetValue(this))}{UUT.END}");
            return sb.ToString();
        }
    }

    public enum MI_LowComparator { GToE, GT }
    public enum MI_HighComparator { LToE, LT }
    public enum MI_UnitPrefix { NONE, peta, tera, giga, mega, kilo, hecto, deca, deci, centi, milli, micro, nano, pico, femto }
    public enum MI_Units { NONE, Amperes, Celcius, Farads, Henries, Hertz, Ohms, Seconds, Siemens, Volts, VoltAmperes, Watts }
    public enum MI_UnitSuffix { NONE, AC, DC, Peak, PP, RMS }

    public class MethodProcess : Method, IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Path))] public String Path { get; set; }
        [XmlAttribute(nameof(Executable))] public String Executable { get; set; }
        [XmlAttribute(nameof(Parameters))] public String Parameters { get; set; }
        [XmlAttribute(nameof(Expected))] public String Expected { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{AssertionM()}{UUT.CONTINUE}");
            sb.Append($"{nameof(Path)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Path)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Executable)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Executable)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Parameters)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Parameters)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Expected)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Expected)).GetValue(this))}{UUT.END}");
            return sb.ToString();
        }
    }

    public class MethodTextual : Method, IAssertionCurrent {
        // NOTE: Constructor-less because only instantiated via System.Xml.Serialization.XmlSerializer, thus constructor unnecessary.
        [XmlAttribute(nameof(Text))] public String Text { get; set; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{AssertionM()}{UUT.CONTINUE}");
            sb.Append($"{nameof(Text)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Text)).GetValue(this))}{UUT.END}");
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
            switch (Event) {
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

    public class TestSequence {
        public UUT UUT { get; set; }
        public TestOperation TestOperation { get; set; }
        public Boolean IsOperation { get; set; } = false;
        public String SerialNumber { get; set; } = String.Empty;
        public EVENTS Event { get; set; } = EVENTS.UNSET;

        public TestSequence() { }

        public TestSequence(UUT uut, TestOperation testOperation) {
            UUT = uut;
            TestOperation = testOperation;
        }
    }

    // TODO:  Soon; incorporate TestIndices into TestSequence, so TestSequence contains its indices internally and the comparative complexity of 2 classes become the simplicity of 1.
    //        Add [XmlIgnore] attribute to TestIndices's 3 properties so they're not deserialized with the TestSequence test data.
    //        Note that we'll need an instance of TestSequence, as it's not a static class like TestIndices is.
    // TODO:  Soon; consider making TestDefinition a readonly singleton, and it's internal classes readonly.
    // TODO:  Soon; re-read TestDefinition every time TestSelect is invoked, so changes to TestDefinition.xml become active at the next selection.
    //        Currently would need to restart TestExec for TestDefiniton.xml changes to become active.
    // TODO:  Soon; in TestExec, update TSMI_UUT_AppConfig to TSMI_UUT_TestDefinition.
    //        Also, add a re-read option in TSMI_UUT_TestDefinition sub-menu.
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