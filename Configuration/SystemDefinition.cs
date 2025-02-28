using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ABT.Test.TestLib.Configuration{
    [XmlRoot(nameof(SystemDefinition))]
    public class SystemDefinition {
        [XmlElement(nameof(Development))] public Development Development { get; set; }
        [XmlElement(nameof(TestData))] public TestData TestData { get; set; }
        [XmlElement(nameof(BarcodeReader))] public BarcodeReader BarcodeReader { get; set; }
        [XmlElement(nameof(Apps))] public Apps Apps { get; set; }
        [XmlElement(nameof(InstrumentsSystem))] public InstrumentsSystem InstrumentsSystem { get; set; }
        public SystemDefinition() { }
    }

    public class TestData {
        [XmlElement(nameof(SQL), typeof(SQL))]
        [XmlElement(nameof(XML), typeof(XML))]
        public Object Item { get; set; }
        public TestData() { }
    }

    public class SQL {
        [XmlAttribute(nameof(ConnectionString))] public String ConnectionString { get; set; }
        public SQL() { }
    }

    public class XML {
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
        public XML() { }
    }

    public class BarcodeReader {
        [XmlAttribute(nameof(ID))] public String ID { get; set; }
        [XmlAttribute(nameof(Detail))] public String Detail { get; set; }
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
        public BarcodeReader() { }
    }

    public class  Apps {
        [XmlElement(nameof(Keysight))] public Keysight Keysight { get; set; }
        [XmlElement(nameof(Microsoft))] public Microsoft Microsoft { get; set; }
        public Apps() { }
    }

    public class Keysight {
        [XmlElement(nameof(CommandExpert))] public String CommandExpert { get; set; }
        [XmlElement(nameof(ConnectionExpert))] public String ConnectionExpert { get; set; }
        public Keysight() { }
    }

    public class Microsoft {
        [XmlElement(nameof(SQLServerManagementStudio))] public String SQLServerManagementStudio { get; set; }
        [XmlElement(nameof(VisualStudio))] public String VisualStudio { get; set; }
        [XmlElement(nameof(VisualStudioCode))] public String VisualStudioCode { get; set; }
        [XmlElement(nameof(XMLNotepad))] public String XMLNotepad { get; set; }
        public Microsoft() { }
    }
    
    public class InstrumentsSystem {
        [XmlElement(nameof(InstrumentSystem))] public List<InstrumentSystem> InstrumentSystem { get; set; }
        [XmlAttribute(nameof(Folder))] public String Folder { get; set; }
        public InstrumentsSystem() { }
    }

    public class InstrumentSystem {
        [XmlAttribute(nameof(ID))] public String ID { get; set; }
        [XmlAttribute(nameof(Detail))] public String Detail { get; set; }
        [XmlAttribute(nameof(Address))] public String Address { get; set; }
        [XmlAttribute(nameof(NameSpacedClassName))] public String NameSpacedClassName { get; set; }
        public InstrumentSystem() { }
    }
}
