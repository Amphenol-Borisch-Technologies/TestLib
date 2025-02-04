using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ABT.Test.TestLib.TestConfiguration {
    public interface IAssertionCurrent { String AssertionCurrent(); }

    [XmlRoot(nameof(TestDefinition))]
    public class TestDefinition {
        [XmlAttribute(nameof(Revision))] public String Revision { get; set; }
        [XmlAttribute(nameof(Date))] public System.DateTime Date { get; set; }
        [XmlElement(nameof(UUT))] public UUT UUT { get; set; }
        [XmlElement(nameof(Development))] public Development Development { get; set; }
        [XmlArray(nameof(Modifications))] public List<Modification> Modifications { get; set; }
        [XmlElement(nameof(TestData))] public TestData TestData { get; set; }
        [XmlElement(nameof(Instruments))] public Instruments Instruments { get; set; }
        [XmlElement(nameof(TestSpace))] public TestSpace TestSpace { get; set; }

        public TestDefinition() { }
    }

    public class UUT : IAssertionCurrent {
        [XmlElement(nameof(Customer))] public Customer Customer { get; set; }
        [XmlElement(nameof(TestSpecification))] public List<TestSpecification> TestSpecification { get; set; }
        [XmlIgnore][XmlElement(nameof(Documentation))] public List<Documentation> Documentation { get; set; }
        [XmlAttribute(nameof(Number))] public String Number { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(Revision))] public String Revision { get; set; }
        [XmlAttribute(nameof(Category))] public Category Category { get; set; }
        public static readonly String NONE = $"{nameof(Data.NONE)}";
        internal static readonly String CHECK_OPERATION = $"if ({nameof(Data)}.{nameof(Data.testSequence)}.{nameof(Data.testSequence.IsOperation)}) ";
        internal const String DEBUG_ASSERT = "Debug.Assert(";
        internal const String BEGIN = "(";
        internal const String CS = ": ";
        internal const String CONTINUE = ", ";
        internal const String END = "));";
        internal const String DIVIDER = "|";

        public UUT() { }

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
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Division))] public String Division { get; set; }
        [XmlAttribute(nameof(Location))] public String Location { get; set; }

        public Customer() { }

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
        [XmlAttribute(nameof(Document))] public String Document { get; set; }
        [XmlAttribute(nameof(Revision))] public String Revision { get; set; }
        [XmlAttribute(nameof(Title))] public String Title { get; set; }
        [XmlAttribute(nameof(Date))] public System.DateTime Date { get; set; }

        public TestSpecification() { }

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
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }

        public Documentation() { }
    }

    public enum Category { Component, CircuitCard, Harness, Unit, System }

    public class Development {
        [XmlElement(nameof(Developer))] public List<Developer> Developer { get; set; }
        [XmlElement(nameof(Documentation))] public List<Documentation> Documentation { get; set; }
        [XmlElement(nameof(Repository))] public List<Repository> Repository { get; set; }
        [XmlAttribute(nameof(Released))] public System.DateTime Released { get; set; }

        public Development() { }
    }

    public class Developer {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Language))] public Language Language { get; set; }
        [XmlAttribute(nameof(Comment))] public String Comment { get; set; }
        public String EMailAddress { get; set; } = String.Empty;

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
        [XmlAttribute(nameof(When))] public System.DateTime When { get; set; }
        [XmlAttribute(nameof(Where))] public String Where { get; set; }
        [XmlAttribute(nameof(Why))] public String Why { get; set; }

        public Modification() { }
    }

    public class TestData : IAssertionCurrent {
        [XmlElement(nameof(SQL), typeof(SQL))]
        [XmlElement(nameof(XML), typeof(XML))]
        public Object Item { get; set; }

        public TestData() { }

        public String AssertionCurrent() {
            if (Item == null) return String.Empty;
            else return ((IAssertionCurrent)Item).AssertionCurrent();
        }

        public Boolean IsEnabled() { return Item != null; }
    }

    public abstract class SerialNumber {
        [XmlAttribute(nameof(SerialNumberEntry))] public SerialNumberEntry SerialNumberEntry { get; set; }
        [XmlAttribute(nameof(SerialNumberRegEx))] public String SerialNumberRegEx { get; set; }
        [XmlAttribute(nameof(SerialNumberFormat))] public String SerialNumberFormat { get; set; }

        public SerialNumber() { }
    }

    public enum SerialNumberEntry { Barcode, Keyboard }

    public class SQL : SerialNumber, IAssertionCurrent {
        [XmlAttribute(nameof(ConnectionString))] public String ConnectionString { get; set; }

        public SQL() { }

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
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }

        public XML() { }

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
        [XmlElement(nameof(Stationary))] public List<Stationary> Stationary { get; set; }
        [XmlElement(nameof(Mobile))] public List<Mobile> Mobile { get; set; }

        public Instruments() { }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(Stationary)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Stationary)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Mobile)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Mobile)).GetValue(this))}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }

        public List<InstrumentInfo> GetInfo() {
            List<InstrumentInfo> instruments = new List<InstrumentInfo>();

            IEnumerable<XElement> iexe = XElement.Load(Data.SystemDefinitionXML).Elements("Instruments");
            foreach (Stationary stationary in Data.testDefinition.Instruments.Stationary) {
                XElement xElement = iexe.Descendants("Instrument").First(xe => (String)xe.Attribute("ID") == stationary.ID) ?? throw new ArgumentException($"Instrument with ID '{stationary.ID}' not present in file '{Data.SystemDefinitionXML}'.");
                instruments.Add(new InstrumentInfo(stationary.ID, stationary.Alias, xElement.Attribute("NameSpacedClassName").Value));
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

    public class Stationary : IAssertionCurrent {
        [XmlAttribute(nameof(ID))] public String ID { get; set; }
        [XmlAttribute(nameof(Alias))] public String Alias { get; set; }

        public Stationary() { }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(ID)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(ID)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Alias)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Alias)).GetValue(this))}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }
    }

    public class Mobile : Stationary {
        [XmlAttribute(nameof(Detail))] public String Detail { get; set; }
        [XmlAttribute(nameof(Address))] public String Address { get; set; }
        [XmlAttribute(nameof(NameSpacedClassName))] public String NameSpacedClassName { get; set; }

        public Mobile() { }

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
        [XmlAttribute(nameof(NamespaceRoot))] public String NamespaceRoot { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(Simulate))] public Boolean Simulate { get; set; }
        [XmlElement(nameof(TestOperation))] public List<TestOperation> TestOperations { get; set; }

        public TestSpace () { }

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
        [XmlAttribute(nameof(NamespaceTrunk))] public String NamespaceTrunk { get; set; }
        [XmlAttribute(nameof(DebugOnly))] public Boolean DebugOnly { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlElement(nameof(TestGroup))] public List<TestGroup> TestGroups { get; set; }

        public TestOperation() { }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.CHECK_OPERATION}{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(NamespaceTrunk)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(NamespaceTrunk)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(DebugOnly)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(DebugOnly)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Description)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}");
            sb.Append($"{UUT.CONTINUE}{nameof(TestGroups)}{UUT.CS}{TGs()}");
            sb.Append($"{UUT.END}");
            return sb.ToString();
        }

        public String TGs() {
            StringBuilder sb = new StringBuilder();
            foreach (TestGroup testGroup in TestGroups) sb.Append($"{testGroup.Classname}{UUT.DIVIDER}");
            return UUT.EF(sb.Remove(sb.Length - UUT.DIVIDER.Length, UUT.DIVIDER.Length).ToString()); // Remove trailing DIVIDER.
        }
    }

    public class TestGroup : IAssertionCurrent {
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

        public String AssertionPrior() { return $"{UUT.CHECK_OPERATION}{UUT.DEBUG_ASSERT}{nameof(Assertions.TestGroupPrior)}{UUT.BEGIN}{nameof(Classname)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Classname)).GetValue(this))}{UUT.END}"; }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{nameof(Classname)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Classname)).GetValue(this))}{UUT.CONTINUE}");
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

        public String AssertionNext() { return $"{UUT.CHECK_OPERATION}{UUT.DEBUG_ASSERT}{nameof(Assertions.TestGroupNext)}{UUT.BEGIN}{nameof(Classname)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Classname)).GetValue(this))}{UUT.END}"; }
    }

    public abstract class Method {
        [XmlAttribute(nameof(Name))] public String Name { get; set; }
        [XmlAttribute(nameof(Description))] public String Description { get; set; }
        [XmlAttribute(nameof(CancelNotPassed))] public Boolean CancelNotPassed { get; set; }
        public String Value { get; set; }
        public EVENTS Event { get; set; }
        [XmlIgnore] public StringBuilder Log { get; set; } = new StringBuilder();
        public String LogString { get; set; } = String.Empty;

        public Method() { }

        public String AssertionPrior() { return $"{UUT.DEBUG_ASSERT}{nameof(Assertions.MethodPrior)}{UUT.BEGIN}{nameof(Name)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Name)).GetValue(this))}{UUT.END}"; }

        public String AssertionBase() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{nameof(Name)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Name)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Description)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Description)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(CancelNotPassed)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(CancelNotPassed)).GetValue(this).ToString().ToLower())}");
            return sb.ToString();
        }

        public String AssertionNext() { return $"{UUT.DEBUG_ASSERT}{nameof(Assertions.MethodNext)}{UUT.BEGIN}{nameof(Name)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Name)).GetValue(this))}{UUT.END}"; }

        public String LogFetchAndClear() {
            String s = Log.ToString();
            Log.Clear();
            return s;
        }
    }

    public class MethodCustom : Method, IAssertionCurrent {
        [XmlElement(nameof(Parameter))] public List<Parameter> Parameters { get; set; }

        public MethodCustom() { }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{AssertionBase()}");
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
        [XmlAttribute(nameof(Key))] public String Key { get; set; }
        [XmlAttribute(nameof(Value))] public String Value { get; set; }

        public Parameter() { }
    }

    public class MethodInterval : Method, IAssertionCurrent {
        [XmlAttribute(nameof(LowComparator))] public MI_LowComparator LowComparator { get; set; }
        [XmlAttribute(nameof(Low))] public Double Low { get; set; }
        [XmlAttribute(nameof(High))] public Double High { get; set; }
        [XmlAttribute(nameof(HighComparator))] public MI_HighComparator HighComparator { get; set; }
        [XmlAttribute(nameof(FractionalDigits))] public UInt32 FractionalDigits { get; set; }
        [XmlAttribute(nameof(UnitPrefix))] public MI_UnitPrefix UnitPrefix { get; set; }
        [XmlAttribute(nameof(Units))] public MI_Units Units { get; set; }
        [XmlAttribute(nameof(UnitSuffix))] public MI_UnitSuffix UnitSuffix { get; set; }
        [XmlIgnore] public static Dictionary<MI_UnitPrefix, Double> UnitPrefixes = new Dictionary<MI_UnitPrefix, Double>() {
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

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{AssertionBase()}{UUT.CONTINUE}");
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
    public enum MI_UnitPrefix { peta, tera, giga, mega, kilo, NONE, milli, micro, nano, pico, femto }
    public enum MI_Units { NONE, Amperes, Celcius, Farads, Henries, Hertz, Ohms, Seconds, Siemens, Volts, VoltAmperes, Watts }
    public enum MI_UnitSuffix { NONE, AC, DC, Peak, PP, RMS }

    public class MethodProcess : Method, IAssertionCurrent {
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
        [XmlAttribute(nameof(File))] public String File { get; set; }
        [XmlAttribute(nameof(Parameters))] public String Parameters { get; set; }
        [XmlAttribute(nameof(Expected))] public String Expected { get; set; }

        public MethodProcess() { }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{AssertionBase()}{UUT.CONTINUE}");
            sb.Append($"{nameof(Folder)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Folder)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(File)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(File)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Parameters)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Parameters)).GetValue(this))}{UUT.CONTINUE}");
            sb.Append($"{nameof(Expected)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Expected)).GetValue(this))}{UUT.END}");
            return sb.ToString();
        }
    }

    public class MethodTextual : Method, IAssertionCurrent {
        [XmlAttribute(nameof(Text))] public String Text { get; set; }

        public MethodTextual() { }

        public String AssertionCurrent() {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{UUT.DEBUG_ASSERT}{GetType().Name}{UUT.BEGIN}");
            sb.Append($"{AssertionBase()}{UUT.CONTINUE}");
            sb.Append($"{nameof(Text)}{UUT.CS}{UUT.EF(GetType().GetProperty(nameof(Text)).GetValue(this))}{UUT.END}");
            return sb.ToString();
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

    public class TestSequence {
        public String Revision { get; set; }
        public System.DateTime Date { get; set; }
        public UUT UUT { get; set; } = Serializing.DeserializeFromFile<UUT>(xmlFile: Data.TestDefinitionXML);
        public TestOperation TestOperation { get; set; }
        [XmlIgnore] public Boolean IsOperation { get; set; } = false;
        public String Computer { get; set; } = Environment.MachineName;
        public String SerialNumber { get; set; } = String.Empty;
        public String Operator { get; set; } = Data.UserName;
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public String TimeTotal { get; set; } // NOTE:  XmlSerializer doesn't natively support TimeSpan, so have to serialize TimeTotal as a string.
        public EVENTS Event { get; set; } = EVENTS.UNSET;

        public TestSequence() { }

        public void PreRun() {
            TimeStart = DateTime.Now;
            Event = EVENTS.UNSET;
            foreach (TestGroup testGroup in TestOperation.TestGroups)
                foreach (Method method in testGroup.Methods) {
                    method.Event = EVENTS.UNSET;
                    _ = method.Log.Clear();
                    method.LogString = String.Empty;
                    method.Value = null;
                }
        }

        public void PostRun(EVENTS OperationEvent) {
            Event = OperationEvent;
            foreach (TestGroup testGroup in TestOperation.TestGroups)
                foreach (Method method in testGroup.Methods) {
                    method.LogString = method.Log.ToString(); // NOTE:  XmlSerializer doesn't support [OnSerializing] attribute, so have to explicitly invoke LogConvert().
                }
            TimeEnd = DateTime.Now;
            TimeTotal = (TimeEnd - TimeStart).ToString(@"dd\.hh\:mm\:ss");
        }
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