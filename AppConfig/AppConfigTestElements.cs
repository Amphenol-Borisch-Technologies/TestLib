using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace ABT.Test.TestLib.AppConfig {
    public class TestOperationsSection : ConfigurationSection {
        [ConfigurationProperty("TestOperations")] public TestOperations TestOperations { get { return ((TestOperations)(base["TestOperations"])); } }
    }
    [ConfigurationCollection(typeof(TestOperation))]
    public class TestOperations : ConfigurationElementCollection {
        public const String PropertyName = "TestOperation";
        public TestOperation this[Int32 idx] { get { return (TestOperation)BaseGet(idx); } }
        public override ConfigurationElementCollectionType CollectionType { get { return ConfigurationElementCollectionType.BasicMapAlternate; } }
        protected override String ElementName { get { return PropertyName; } }
        protected override Boolean IsElementName(String elementName) { return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase); }
        public override Boolean IsReadOnly() { return false; }
        protected override ConfigurationElement CreateNewElement() { return new TestOperation(); }
        protected override Object GetElementKey(ConfigurationElement element) { return ((TestOperation)(element)).ID; }
    }
    public class TestOperation : ConfigurationElement {
        [ConfigurationProperty("ID", IsKey = true, IsRequired = true)] public String ID { get { return ((String)base["ID"]).Trim(); } }
        [ConfigurationProperty("Revision", IsKey = false, IsRequired = true)] public String Revision { get { return ((String)base["Revision"]).Trim(); } }
        [ConfigurationProperty("Description", IsKey = false, IsRequired = true)] public String Description { get { return ((String)base["Description"]).Trim(); } }
        [ConfigurationProperty("TestGroupIDs", IsKey = false, IsRequired = true)] public String TestGroupIDs { get { return ((String)base["TestGroupIDs"]).Trim(); } }
    }

    public class TestGroupsSection : ConfigurationSection {
        [ConfigurationProperty("TestGroups")] public TestGroups TestGroups { get { return ((TestGroups)(base["TestGroups"])); } }
    }
    [ConfigurationCollection(typeof(TestGroup))]
    public class TestGroups : ConfigurationElementCollection {
        public const String PropertyName = "TestGroup";
        public TestGroup this[Int32 idx] { get { return (TestGroup)BaseGet(idx); } }
        public override ConfigurationElementCollectionType CollectionType { get { return ConfigurationElementCollectionType.BasicMapAlternate; } }
        protected override String ElementName { get { return PropertyName; } }
        protected override Boolean IsElementName(String elementName) { return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase); }
        public override Boolean IsReadOnly() { return false; }
        protected override ConfigurationElement CreateNewElement() { return new TestGroup(); }
        protected override Object GetElementKey(ConfigurationElement element) { return ((TestGroup)(element)).ID; }
    }
    public class TestGroup : ConfigurationElement {
        [ConfigurationProperty("ID", IsKey = true, IsRequired = true)] public String ID { get { return ((String)base["ID"]).Trim(); } }
        [ConfigurationProperty("Revision", IsKey = false, IsRequired = true)] public String Revision { get { return ((String)base["Revision"]).Trim(); } }
        [ConfigurationProperty("Description", IsKey = false, IsRequired = true)] public String Description { get { return ((String)base["Description"]).Trim(); } }
        [ConfigurationProperty("Selectable", IsKey = false, IsRequired = true)] public Boolean Selectable { get { return ((Boolean)base["Selectable"]); } }
        [ConfigurationProperty("CancelNotPassed", IsKey = false, IsRequired = true)] public Boolean CancelNotPassed { get { return ((Boolean)base["CancelNotPassed"]); } }
        [ConfigurationProperty("TestMeasurementIDs", IsKey = false, IsRequired = true)] public String TestMeasurementIDs { get { return Regex.Replace(((String)base["TestMeasurementIDs"]), @"\s+", ""); } }
    }

    public class TestMeasurementsSection : ConfigurationSection {
        [ConfigurationProperty("TestMeasurements")] public TestMeasurements TestMeasurements { get { return ((TestMeasurements)(base["TestMeasurements"])); } }
    }
    [ConfigurationCollection(typeof(TestMeasurement))]
    public class TestMeasurements : ConfigurationElementCollection {
        public const String PropertyName = "TestMeasurement";
        public TestMeasurement this[Int32 idx] { get { return (TestMeasurement)BaseGet(idx); } }
        public override ConfigurationElementCollectionType CollectionType { get { return ConfigurationElementCollectionType.BasicMapAlternate; } }
        protected override String ElementName { get { return PropertyName; } }
        protected override Boolean IsElementName(String elementName) { return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase); }
        public override Boolean IsReadOnly() { return false; }
        protected override ConfigurationElement CreateNewElement() { return new TestMeasurement(); }
        protected override Object GetElementKey(ConfigurationElement element) { return ((TestMeasurement)(element)).ID; }
    }
    public class TestMeasurement : ConfigurationElement {
        [ConfigurationProperty("ID", IsKey = true, IsRequired = true)] public String ID { get { return Regex.Replace(((String)base["ID"]), @"\s+", ""); } }
        [ConfigurationProperty("Revision", IsKey = false, IsRequired = true)] public String Revision { get { return ((String)base["Revision"]).Trim(); } }
        [ConfigurationProperty("Description", IsKey = false, IsRequired = true)] public String Description { get { return ((String)base["Description"]).Trim(); } }
        [ConfigurationProperty("ClassName", IsKey = false, IsRequired = true)] public String ClassName { get { return ((String)base["ClassName"]).Trim(); } }
        [ConfigurationProperty("CancelNotPassed", IsKey = false, IsRequired = true)] public Boolean CancelNotPassed { get { return ((Boolean)base["CancelNotPassed"]); } }
        [ConfigurationProperty("Arguments", IsKey = false, IsRequired = true)] public String Arguments { get { return ((String)base["Arguments"]).Trim(); } }
    }

    public class InstrumentsStationaryRequiredSection : ConfigurationSection {
        public const String ClassName = nameof(InstrumentsStationaryRequiredSection);
        [ConfigurationProperty("InstrumentsStationaryRequired")] public InstrumentsStationaryRequired InstrumentsStationaryRequired { get { return ((InstrumentsStationaryRequired)(base["InstrumentsStationaryRequired"])); } }
    }
    [ConfigurationCollection(typeof(InstrumentStationaryRequired))]
    public class InstrumentsStationaryRequired : ConfigurationElementCollection {
        public const String PropertyName = "InstrumentStationaryRequired";
        public InstrumentStationaryRequired this[Int32 idx] { get { return (InstrumentStationaryRequired)BaseGet(idx); } }
        public override ConfigurationElementCollectionType CollectionType { get { return ConfigurationElementCollectionType.BasicMapAlternate; } }
        protected override String ElementName { get { return PropertyName; } }
        protected override Boolean IsElementName(String elementName) { return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase); }
        public override Boolean IsReadOnly() { return false; }
        protected override ConfigurationElement CreateNewElement() { return new InstrumentStationaryRequired(); }
        protected override Object GetElementKey(ConfigurationElement element) { return ((InstrumentStationaryRequired)(element)).ID; }
    }
    public class InstrumentStationaryRequired : ConfigurationElement {
        [ConfigurationProperty("ID", IsKey = true, IsRequired = true)] public String ID { get { return ((String)base["ID"]).Trim(); } }
        [ConfigurationProperty("ClassName", IsKey = false, IsRequired = true)] public String ClassName { get { return ((String)base["ClassName"]).Trim(); } }
    }

    public class InstrumentsPortableRequiredSection : ConfigurationSection {
        public const String ClassName = nameof(InstrumentsPortableRequiredSection);
        [ConfigurationProperty("InstrumentsPortableRequired")] public InstrumentsPortableRequired InstrumentsPortableRequired { get { return ((InstrumentsPortableRequired)(base["InstrumentsPortableRequired"])); } }
    }
    [ConfigurationCollection(typeof(InstrumentPortableRequired))]
    public class InstrumentsPortableRequired : ConfigurationElementCollection {
        public const String PropertyName = "InstrumentPortableRequired";
        public InstrumentPortableRequired this[Int32 idx] { get { return (InstrumentPortableRequired)BaseGet(idx); } }
        public override ConfigurationElementCollectionType CollectionType { get { return ConfigurationElementCollectionType.BasicMapAlternate; } }
        protected override String ElementName { get { return PropertyName; } }
        protected override Boolean IsElementName(String elementName) { return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase); }
        public override Boolean IsReadOnly() { return false; }
        protected override ConfigurationElement CreateNewElement() { return new InstrumentPortableRequired(); }
        protected override Object GetElementKey(ConfigurationElement element) { return ((InstrumentPortableRequired)(element)).ID; }
    }
    public class InstrumentPortableRequired : ConfigurationElement {
        [ConfigurationProperty("ID", IsKey = true, IsRequired = true)] public String ID { get { return ((String)base["ID"]).Trim(); } }
        [ConfigurationProperty("Detail", IsKey = false, IsRequired = true)] public String Detail { get { return ((String)base["Detail"]).Trim(); } }
        [ConfigurationProperty("Address", IsKey = true, IsRequired = true)] public String Address { get { return ((String)base["Address"]).Trim(); } }
        [ConfigurationProperty("ClassName", IsKey = false, IsRequired = true)] public String ClassName { get { return ((String)base["ClassName"]).Trim(); } }
    }

    public class ConfigTests {
        public TestMeasurementsSection TestMeasurementsSection { get { return (TestMeasurementsSection)ConfigurationManager.GetSection(nameof(TestMeasurementsSection)); } }
        public TestMeasurements TestMeasurements { get { return TestMeasurementsSection.TestMeasurements; } }
        public IEnumerable<TestMeasurement> TestMeasurement { get { foreach (TestMeasurement tm in TestMeasurements) if (tm != null) yield return tm; } }
    }

    public class Operation {
        public readonly String ID;
        public readonly String Revision;
        public readonly String Description;
        public readonly String TestGroupIDs;

        private Operation(String id, String revision, String description, String testGroupIDs) {
            ID = id;
            Revision = revision;
            Description = description;
            TestGroupIDs = testGroupIDs;
        }

        public static Dictionary<String, Operation> Get() {
            TestOperationsSection testOperationsSection = (TestOperationsSection)ConfigurationManager.GetSection(nameof(TestOperationsSection));
            TestOperations testOperations = testOperationsSection.TestOperations;
            Dictionary<String, Operation> dictionary = new Dictionary<String, Operation>();
            foreach (TestOperation to in testOperations) dictionary.Add(to.ID, new Operation(to.ID, to.Revision, to.Description, to.TestGroupIDs));
            return dictionary;
        }

        public static Operation Get(String TestOperationID) { return Get()[TestOperationID]; }
    }

    public class Group {
        public readonly String ID;
        public readonly String Revision;
        public readonly String Description;
        public readonly Boolean Selectable;
        public readonly Boolean CancelNotPassed;
        public readonly String TestMeasurementIDs;

        private Group(String id, String revision, String description, Boolean selectable, Boolean cancelNotPassed, String testMeasurementIDs) {
            ID = id;
            Revision = revision;
            Description = description;
            Selectable = selectable;
            CancelNotPassed = cancelNotPassed;
            TestMeasurementIDs = testMeasurementIDs;
        }

        public static Dictionary<String, Group> Get() {
            TestGroupsSection testGroupSection = (TestGroupsSection)ConfigurationManager.GetSection(nameof(TestGroupsSection));
            TestGroups testGroups = testGroupSection.TestGroups;
            Dictionary<String, Group> dictionary = new Dictionary<String, Group>();
            foreach (TestGroup tg in testGroups) dictionary.Add(tg.ID, new Group(tg.ID, tg.Revision, tg.Description, tg.Selectable, tg.CancelNotPassed, tg.TestMeasurementIDs));
            return dictionary;
        }

        public static Group Get(String TestGroupID) { return Get()[TestGroupID]; }
    }

    public class Measurement {
        public readonly String ID;
        public readonly String Revision;
        public readonly String Description;
        public readonly String ClassName;
        public readonly Object ClassObject;
        public readonly Boolean CancelNotPassed;
        public String GroupID { get; set; } = String.Empty;               // Determined pre-test.
        public String Value { get; set; } = String.Empty;                 // Determined at test.
        public EVENTS Event { get; set; } = EVENTS.UNSET;                 // Determined post-test.
        public StringBuilder Message { get; set; } = new StringBuilder(); // Determined at test.

        private Measurement(String id, String revision, String description, String className, Boolean cancelNotPassed, String arguments) {
            ID = id;
            Revision = revision;
            Description = description;
            ClassName = className;
            ClassObject = Activator.CreateInstance(Type.GetType(GetType().Namespace + "." + ClassName), new Object[] { ID, arguments });
            CancelNotPassed = cancelNotPassed;
            if (String.Equals(ClassName, nameof(MeasurementNumeric))) Value = Double.NaN.ToString();
        }

        public static Dictionary<String, Measurement> Get() {
            TestMeasurementsSection testMeasurementsSection = (TestMeasurementsSection)ConfigurationManager.GetSection(nameof(TestMeasurementsSection));
            TestMeasurements testMeasurements = testMeasurementsSection.TestMeasurements;
            Dictionary<String, Measurement> dictionary = new Dictionary<String, Measurement>();
            foreach (TestMeasurement tm in testMeasurements) try {
                    dictionary.Add(tm.ID, new Measurement(tm.ID, tm.Revision, tm.Description, tm.ClassName, tm.CancelNotPassed, tm.Arguments));
                } catch (Exception e) {
                    StringBuilder sb = new StringBuilder().AppendLine();
                    sb.AppendLine($"App.config issue with TestMeasurement:");
                    sb.AppendLine($"   ID              : {tm.ID}");
                    sb.AppendLine($"   Revision        : {tm.Revision}");
                    sb.AppendLine($"   Description     : {tm.Description}");
                    sb.AppendLine($"   ClassName       : {tm.ClassName}");
                    sb.AppendLine($"   CancelNotPassed : {tm.CancelNotPassed}");
                    sb.AppendLine($"   Arguments       : {tm.Arguments}{Environment.NewLine}");
                    sb.AppendLine($"Exception Message(s):");
                    sb.AppendLine($"{e}{Environment.NewLine}");
                    throw new ArgumentException(sb.ToString());
                }
            return dictionary;
        }

        public static Measurement Get(String TestMeasurementID) { return Get()[TestMeasurementID]; }
    }
}
