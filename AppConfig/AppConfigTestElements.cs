using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace ABT.Test.TestLib.AppConfig {
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
}
