﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ABT.Test.TestLib.TestConfiguration {

    [XmlRoot(ElementName = nameof(TestSequence), Namespace = "", IsNullable = false)]
    public class TestSequence {
        [XmlAttribute("noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public String NoNamespaceSchemaLocation { get; set; } = "file://C://Users//phils//source//repos//ABT//Test//TestLib//TestConfiguration//TestSequence.xsd";
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
            TimeEnd = DateTime.Now;
            TimeTotal = (TimeEnd - TimeStart).ToString(@"dd\.hh\:mm\:ss");
        }
    }
}