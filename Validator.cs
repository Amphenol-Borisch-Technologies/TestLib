using System;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace TestSequencer {
    internal class Validator {
        private static Boolean xmlValid = true;
        private static readonly StringBuilder messages = new StringBuilder();
        private static XmlReader reader;

        [STAThreadAttribute]
        public static void NotMain() {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, Properties.Resources.XSD_File);
            XmlReaderSettings settings = new XmlReaderSettings { ValidationType = ValidationType.Schema, Schemas = schemaSet };
            settings.ValidationEventHandler += ValidationCallback;

            try {
                using (reader = XmlReader.Create(Properties.Resources.XML_File, settings)) {
                    Double low, high;
                    while (reader.Read()) {
                        if (reader.NodeType == XmlNodeType.Element && String.Equals(reader.Name, nameof(MI))) {
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
                            low = Double.Parse(reader.GetAttribute(nameof(MI.Low)));
                            high = Double.Parse(reader.GetAttribute(nameof(MI.High)));
                            if (low > high) {
                                xmlValid = false;
                                messages.AppendLine($"MethodInterval's {nameof(MI.Low)} > {nameof(MI.High)}:");
                                messages.AppendLine($"  Line Number   : {(reader as IXmlLineInfo).LineNumber}");
                                messages.AppendLine($"  Line Position : {(reader as IXmlLineInfo).LinePosition}");
                                messages.AppendLine($"  Node Type     : {reader.NodeType}");
                                messages.AppendLine($"  {nameof(MI.Description)}   : {reader.GetAttribute(nameof(MI.Description))}");
                                messages.AppendLine($"  {nameof(MI.Method)}        : {reader.GetAttribute(nameof(MI.Method))}");
                                messages.AppendLine($"  {nameof(MI.Low)}           : {reader.GetAttribute(nameof(MI.Low))}");
                                messages.AppendLine($"  {nameof(MI.High)}          : {reader.GetAttribute(nameof(MI.High))}{Environment.NewLine}{Environment.NewLine}");
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                xmlValid = false;
                messages.AppendLine($"Exception:");
                messages.AppendLine($"  Exception     : {ex.Message}{Environment.NewLine}");
            }

            if (!xmlValid) {
                messages.AppendLine($"Invalid XML document: file:///{Properties.Resources.XML_File}.{Environment.NewLine}");
                CustomMessageBox.Show(Title: "Invalid XML document", Message: messages.ToString(), OptionalIcon: System.Drawing.SystemIcons.Error);
            }
        }

        private static void ValidationCallback(Object sender, ValidationEventArgs vea) {
            xmlValid = false;
            messages.AppendLine($"Validation Event:");
            messages.AppendLine($"  Line Number   : {vea.Exception.LineNumber}");
            messages.AppendLine($"  Line Position : {vea.Exception.LinePosition}");
            messages.AppendLine($"  Node Type     : {reader.NodeType}");
            messages.AppendLine($"  Description   : {reader.GetAttribute("Description")}");
            messages.AppendLine($"  Severity      : {vea.Severity}");
            messages.AppendLine($"  Attribute     : {reader.Name} = {reader.Value}");
            messages.AppendLine($"  Message       : {vea.Message}{Environment.NewLine}{Environment.NewLine}");
        }
    }
}
