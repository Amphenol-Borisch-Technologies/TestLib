using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ABT.Test.TestLib.AppConfig {
    public static class InstrumentDrivers {
        public static Dictionary<String, Object> Get(String ConfigurationTestExec) {
            Dictionary<String, Object> Instruments = GetInstrumentsPortable();
            Dictionary<String, String> InstrumentsStationary = GetInstrumentsStationary();
            IEnumerable<XElement> IS = XElement.Load(ConfigurationTestExec).Elements("InstrumentsStationary");
            // Now add InstrumentsStationary listed in app.config, but must first read their Address, Detail & ClassName from TestExec.ConfigurationTestExec.
            foreach (KeyValuePair<String, String> kvp in InstrumentsStationary) {
                XElement XE = IS.Descendants("InstrumentStationary").FirstOrDefault(xe => (String)xe.Attribute("ID") == kvp.Key) ?? throw new ArgumentException($"Instrument with ID '{kvp.Key}' not present in file '{ConfigurationTestExec}'.");
                Instruments.Add(kvp.Key, Activator.CreateInstance(Type.GetType(kvp.Value), new Object[] { XE.Attribute("Address").Value, XE.Attribute("Detail").Value }));
            }
            return Instruments;
        }

        private static Dictionary<String, String> GetInstrumentsStationary() {
            InstrumentsStationaryRequiredSection ISRSs = (InstrumentsStationaryRequiredSection)ConfigurationManager.GetSection(nameof(InstrumentsStationaryRequiredSection));
            InstrumentsStationaryRequired ISRs = ISRSs.InstrumentsStationaryRequired;
            Dictionary<String, String> InstrumentsStationary = new Dictionary<String, String>();
            foreach (InstrumentStationaryRequired ISR in ISRs) try {
                InstrumentsStationary.Add(ISR.ID, ISR.ClassName);
            } catch (Exception e) {
                StringBuilder sb = new StringBuilder().AppendLine();
                sb.AppendLine($"App.config issue with InstrumentStationaryRequired:");
                sb.AppendLine($"   ID              : {ISR.ID}");
                sb.AppendLine($"   ClassName       : {ISR.ClassName}{Environment.NewLine}");
                sb.AppendLine($"Exception Message(s):");
                sb.AppendLine($"{e}{Environment.NewLine}");
                throw new ArgumentException(sb.ToString());
            }
            return InstrumentsStationary;
        }

        private static Dictionary<String, Object> GetInstrumentsPortable() {
            InstrumentsPortableRequiredSection IPRSs = (InstrumentsPortableRequiredSection)ConfigurationManager.GetSection(nameof(InstrumentsPortableRequiredSection));
            InstrumentsPortableRequired IPRs = IPRSs.InstrumentsPortableRequired;
            Dictionary<String, Object> Instruments = new Dictionary<String, Object>();
            foreach (InstrumentPortableRequired IPR in IPRs) try {
                Instruments.Add(IPR.ID, Activator.CreateInstance(Type.GetType(IPR.ClassName), new Object[] { IPR.Address, IPR.Detail }));
            } catch (Exception e) {
                StringBuilder sb = new StringBuilder().AppendLine();
                sb.AppendLine($"App.config issue with InstrumentPortableRequired:");
                sb.AppendLine($"   ID              : {IPR.ID}");
                sb.AppendLine($"   Detail          : {IPR.Detail}");
                sb.AppendLine($"   Address         : {IPR.Address}");
                sb.AppendLine($"   ClassName       : {IPR.ClassName}{Environment.NewLine}");
                sb.AppendLine($"Exception Message(s):");
                sb.AppendLine($"{e}{Environment.NewLine}");
                throw new ArgumentException(sb.ToString());
            }
            return Instruments;
        }
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
}
