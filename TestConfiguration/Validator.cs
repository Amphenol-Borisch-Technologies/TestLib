using ABT.Test.TestLib.InstrumentDrivers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace ABT.Test.TestLib.TestConfiguration {
    public static class Validator {
        private static Boolean validSpecification = true;
        private static readonly StringBuilder stringBuilder = new StringBuilder();
        private static XmlReader xmlReader;

        public static Boolean ValidSpecification(String TestDefinitionXSD, String TestDefinitionXML) {
            if (!File.Exists(TestDefinitionXSD)) throw new ArgumentException($"XSD Test Specification File '{TestDefinitionXSD}' does not exist.");
            if (!File.Exists(TestDefinitionXML)) throw new ArgumentException($"XML Test Specification File '{TestDefinitionXML}' does not exist.");
            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add(null, TestDefinitionXSD);
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings { ValidationType = ValidationType.Schema, Schemas = xmlSchemaSet };
            xmlReaderSettings.ValidationEventHandler += ValidationCallback;

            try {
                using (xmlReader = XmlReader.Create(TestDefinitionXML, xmlReaderSettings)) {
                    Double low, high;
                    String className = String.Empty;
                    HashSet<String> methodTypes = Method.GetMethodDerivedClassnames();
                    while (xmlReader.Read()) {
                        if (xmlReader.NodeType == XmlNodeType.Element) {
                            if (String.Equals(xmlReader.Name, nameof(MethodInterval))) {
                                // NOTE: This if block required because Microsoft's Visual Studio only supports XML Schema 1.0.
                                // - If Visual Studio supported XSD 1.1, then <xs:assert test="@Low le @High"/> would obviate this block.
                                #region TLDR below compares just some of the many mainstream XML editing options.
                                // NOTE: XML Liquid Studio Community Edition supports XML Schema 1.1.
                                // - Liquid Studio is a powerful but complex external XML editor.
                                // - It's co$t free and licensing permits commericial usage.
                                // - Confirmed it detects Low > High occurences via <xs:assert test="@Low le @High"/>.
                                // - Chose to not utilize Liquid Studio because it adds too much complexity at this time.
                                //   - Non-community/non-co$t free editions are integrated into Visual Studio.
                                //
                                // NOTE: XML Notepad supports XML Schema 1.0.
                                // - XML Notepad is a powerful but simple external XML editor.
                                // - It's co$t free and licensing permits commericial usage.
                                //
                                // NOTE: Visual Studio Code with Red Hat's XML extension supports XML Schema 1.0.
                                // - VS Code is a powerful but complex external multi-purpose editor.
                                // - It's co$t free and licensing permits commericial usage.
                                //   - Red Hat's XML extension provides XML Schema 1.0 support.
                                //   - Tried several other provider's XML extensions, but none supported XML schema 1.1.
                                //   - XML editing integrated with Visual Studio Code is incredibly convenient.
                                //   - As a multi-purpose editor, can develop C# .Net applications.  Plus many other languages.
                                //
                                // NOTE: Visual Studio's supports XML Schema 1.0.
                                // - Visual Studio's integrated XML editor is powerful but complex.
                                //   - Visual Studio isn't co$t free, but licensing permits commercial use.
                                //   - XML editing integrated with Visual Studio is incredibly convenient.
                                //   - As a multi-purpose editor, can develop C# .Net applications.  Plus many other languages.
                                #endregion
                                low = Double.Parse(xmlReader.GetAttribute(nameof(MethodInterval.Low)));
                                high = Double.Parse(xmlReader.GetAttribute(nameof(MethodInterval.High)));
                                if (low > high) {
                                    validSpecification = false;
                                    stringBuilder.AppendLine($"{nameof(MethodInterval)}'s {nameof(MethodInterval.Low)} > {nameof(MethodInterval.High)}:");
                                    stringBuilder.AppendLine($"\tLine Number   : {(xmlReader as IXmlLineInfo).LineNumber}");
                                    stringBuilder.AppendLine($"\tLine Position : {(xmlReader as IXmlLineInfo).LinePosition}");
                                    stringBuilder.AppendLine($"\tNode Type     : {xmlReader.NodeType}");
                                    stringBuilder.AppendLine($"\t\t{nameof(MethodInterval.Description)}   : {xmlReader.GetAttribute(nameof(MethodInterval.Description))}");
                                    stringBuilder.AppendLine($"\t\t{nameof(MethodInterval.Name)}          : {xmlReader.GetAttribute(nameof(MethodInterval.Name))}");
                                    stringBuilder.AppendLine($"\t\t{nameof(MethodInterval.Low)}           : {xmlReader.GetAttribute(nameof(MethodInterval.Low))}");
                                    stringBuilder.AppendLine($"\t\t{nameof(MethodInterval.High)}          : {xmlReader.GetAttribute(nameof(MethodInterval.High))}{Environment.NewLine}{Environment.NewLine}");
                                }
                            }

                            if (String.Equals(xmlReader.Name, nameof(TestGroup))) {
                                // NOTE: This if block required because Microsoft's Visual Studio only supports XML Schema 1.0.
                                // - If Visual Studio supported XSD 1.1, then below xs:assert would obviate this block:
                                // <xs:assert test="not(Classname = MethodInterval/@Name or Classname = MethodProcess/@Name or Classname = MethodTextual/@Name or Classname = MethodCustom/@Name)"/>.
                                className = xmlReader.GetAttribute(nameof(TestGroup.Classname));
                            }

                            if (methodTypes.Contains(xmlReader.Name)) {
                               // NOTE: This if block required because Microsoft's Visual Studio only supports XML Schema 1.0.
                                // - If Visual Studio supported XSD 1.1, then below xs:assert would obviate this block:
                                // <xs:assert test="not(Classname = MethodInterval/@Name or Classname = MethodProcess/@Name or Classname = MethodTextual/@Name or Classname = MethodCustom/@Name)"/>.
                                String methodName = xmlReader.GetAttribute(nameof(Method.Name));
                                if (className == methodName) {
                                    validSpecification = false;
                                    stringBuilder.AppendLine($"{nameof(Method)}'s {nameof(Method.Name)} '{methodName}' identical to {nameof(TestGroup)}'s {nameof(TestGroup.Classname)} '{className}':");
                                    stringBuilder.AppendLine($"\tLine Number   : {(xmlReader as IXmlLineInfo).LineNumber}");
                                    stringBuilder.AppendLine($"\tLine Position : {(xmlReader as IXmlLineInfo).LinePosition}");
                                    stringBuilder.AppendLine($"\tNode Type     : {xmlReader.NodeType}");
                                    stringBuilder.AppendLine($"\t\t{nameof(Method.Description)}   : {xmlReader.GetAttribute(nameof(Method.Description))}");
                                    stringBuilder.AppendLine($"\t\t{nameof(Method.Name)}          : {xmlReader.GetAttribute(nameof(Method.Name))}");
                                }
                            }
                        }
                    }
                }
            } catch (Exception e) {
                validSpecification = false;
                stringBuilder.AppendLine($"Exception:");
                stringBuilder.AppendLine($"\tException     : {e.Message}{Environment.NewLine}");
            }

            if (!validSpecification) {
                stringBuilder.AppendLine($"Invalid XML Test Specification File: file:///{TestDefinitionXML}.{Environment.NewLine}");
                Miscellaneous.CustomMessageBox.Show(Title: "Invalid XML Test Specification File", Message: stringBuilder.ToString(), OptionalIcon: System.Drawing.SystemIcons.Error);
            }
            return validSpecification;
        }

        private static void ValidationCallback(Object sender, ValidationEventArgs vea) {
            validSpecification = false;
            stringBuilder.AppendLine($"Validation Event:");
            stringBuilder.AppendLine($"\tLine Number   : {vea.Exception.LineNumber}");
            stringBuilder.AppendLine($"\tLine Position : {vea.Exception.LinePosition}");
            stringBuilder.AppendLine($"\tNode Type     : {xmlReader.NodeType}");
            stringBuilder.AppendLine($"\tDescription   : {xmlReader.GetAttribute("Description")}");
            stringBuilder.AppendLine($"\tSeverity      : {vea.Severity}");
            stringBuilder.AppendLine($"\tAttribute     : {xmlReader.Name} = {xmlReader.Value}");
            stringBuilder.AppendLine($"\tMessage       : {vea.Message}{Environment.NewLine}{Environment.NewLine}");
        }
    }
}
