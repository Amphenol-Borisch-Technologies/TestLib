using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ABT.Test.Lib {
    class Reader {
        // TODO: Modify this to read the Test Operation XML sequence files and auto-generate a skeleton C# project.
        // - Copy a standard template project into a folder auto-created based on CCA Number.
        // - Auto-generate C# files, classes & methods from the Test Operation XML sequence files.
        // - Include standard Debug.Assert statements for:
        //   - TO/TestOperation Folder & Description attributes added to first method in TestOperation.
        //     - Also list of TG/TestGroups in TestOperation.
        //   - TG/TestGroup Class, Description, CancelIfFail, Independent attributes added to first method in TestGroup.
        //     - Also list of {MC,MI,MP,MT) methods in TestGroup.
        // - Potentially include external invocations to Keysight VEE or Python executables, if test developer selects these options.
        // - Goal is to automate as much of the routine, standardized test generation as possible.
        [STAThread]
        public static void NotMain() {
            XDocument tocf = XDocument.Load(Properties.Resources.XML_File);
            XElement toElement = tocf.Element(nameof(TO));
            String nameSpace = (toElement.Attribute(nameof(TO.Namespace))?.Value);
            String description = (toElement.Attribute(nameof(TO.Description))?.Value);
            Console.WriteLine($"{nameof(TO.Namespace)}         : {nameSpace}");
            Console.WriteLine($"{nameof(TO.Description)}       : {description}");

            IEnumerable<XElement> tgElements = toElement.Elements(nameof(TG));
            foreach (XElement tg in tgElements) {
                String tgClass = tg.Attribute(nameof(TG.Class))?.Value;
                String tgDescription = tg.Attribute(nameof(TG.Description))?.Value;

                Boolean tgCancelIfFail = Boolean.Parse(tg.Attribute(nameof(TG.CancelIfFail))?.Value);
                Boolean i = Boolean.Parse(tg.Attribute(nameof(TG.Independent))?.Value);
                Console.WriteLine();
                Console.WriteLine($"{nameof(TG.Class)}             : {tgClass}");
                Console.WriteLine($"{nameof(TG.Description)}       : {tgDescription}");
                Console.WriteLine($"{nameof(TG.CancelIfFail)}      : {tgCancelIfFail}");
                Console.WriteLine($"{nameof(TG.Independent)}       : {i}");

                IEnumerable<XElement> methods = tg.Elements();
                foreach (XElement method in methods) {
                    String methodName = method.Attribute(nameof(M.Method))?.Value;
                    String methodDescription = method.Attribute(nameof(M.Description))?.Value;
                    Boolean methodCancelIfFail = Boolean.Parse(method.Attribute(nameof(M.CancelIfFail))?.Value);
                    Console.WriteLine();
                    Console.WriteLine($"Method Type       : {method.Name.LocalName}");
                    Console.WriteLine($"  {nameof(M.Method)}          : {methodName}");
                    Console.WriteLine($"  {nameof(M.Description)}     : {methodDescription}");
                    Console.WriteLine($"  {nameof(M.CancelIfFail)}    : {methodCancelIfFail}");

                    // TODO: Use Activator.CreateInstance to auto-create appropriate TestMeasurement objects, eliminating below switch.
                    // TODO: Rename TestMeasurement objects to TestMethod objects?
                    switch (method.Name.LocalName) {
                        case nameof(MC):
                            IEnumerable<XElement> parameters = method.Elements(nameof(Parameter));
                            foreach (XElement parameter in parameters) {
                                String key = parameter.Attribute(nameof(Parameter.Key))?.Value;
                                String value = parameter.Attribute(nameof(Parameter.Value))?.Value;
                                Console.WriteLine($"    {nameof(Parameter)} {nameof(Parameter.Key)} : {key}, {nameof(Parameter.Value)}: {value}");
                            }
                            break;
                        case nameof(MI):
                            Double low = Double.Parse(method.Attribute(nameof(MI.Low))?.Value);
                            Double high = Double.Parse(method.Attribute(nameof(MI.High))?.Value);
                            if (low > high) {
                                throw new ArgumentException($"Invalid Method element {method.Name.LocalName}:{Environment.NewLine}" +
                                    $"{nameof(MI.Method)}      :{methodName}{Environment.NewLine}" +
                                    $"{nameof(MI.Description)} :{methodDescription}{Environment.NewLine}" +
                                    $"Invalidity  : {nameof(MI.Low)} '{low}' is > {nameof(MI.High)} '{high}'.{Environment.NewLine}");
                            }
                            UInt32 fractionalDigits = UInt32.Parse(method.Attribute(nameof(MI.FractionalDigits))?.Value);
                            String unitPrefix = method.Attribute(nameof(MI.UnitPrefix))?.Value;
                            String units = method.Attribute(nameof(MI.Units))?.Value;
                            String unitSuffix = method.Attribute(nameof(MI.UnitSuffix))?.Value;
                            Console.WriteLine($"    {nameof(MI.Low)}           : {low}");
                            Console.WriteLine($"    {nameof(MI.High)}          : {high}");
                            Console.WriteLine($"    {nameof(MI.FractionalDigits)}    : {fractionalDigits}");
                            Console.WriteLine($"    {nameof(MI.UnitPrefix)}    : {unitPrefix}");
                            Console.WriteLine($"    {nameof(MI.Units)}         : {units}");
                            Console.WriteLine($"    {nameof(MI.UnitSuffix)}    : {unitSuffix}");
                            break;
                        case nameof(MP):
                            String path = method.Attribute(nameof(MP.Path))?.Value;
                            String executable = method.Attribute(nameof(MP.Executable))?.Value;
                            String parms = method.Attribute(nameof(MP.Parameters))?.Value;
                            String expected = method.Attribute(nameof(MP.Expected))?.Value;
                            Console.WriteLine($"    {nameof(MP.Path)}          : {path}");
                            Console.WriteLine($"    {nameof(MP.Executable)}    : {executable}");
                            Console.WriteLine($"    {nameof(MP.Parameters)}    : {parms}");
                            Console.WriteLine($"    {nameof(MP.Expected)}      : {expected}");
                            break;
                        case nameof(MT):
                            String text = method.Attribute(nameof(MT.Text))?.Value;
                            Console.WriteLine($"    {nameof(MT.Text)}          : {text}");
                            break;
                        default:
                            throw new InvalidOperationException($"METHODS '{method.Name.LocalName}' not handled.");
                    }
                }
            }
        }
    }
}
