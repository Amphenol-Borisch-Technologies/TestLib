using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace ABT.Test.TestLib.TestConfiguration {
    public static class Assertions {
        public static Boolean TestSpace(String NamespaceRoot, String Description, String TestOperations) {
            Boolean b = String.Equals(TestLib.testDefinition.TestSpace.NamespaceRoot, NamespaceRoot);
            b &= String.Equals(TestLib.testDefinition.TestSpace.Description, Description);
            b &= String.Equals(TestLib.testDefinition.TestSpace.TOs().Replace("\"", ""), TestOperations);
            return b;
        }

        public static Boolean TestOperation(String NamespaceLeaf, String Description, String TestGroups) {
            Boolean b = String.Equals(TestIndex.TestOperation.NamespaceTrunk, NamespaceLeaf);
            b &= String.Equals(TestIndex.TestOperation.Description, Description);
            b &= String.Equals(TestIndex.TestOperation.TGs().Replace("\"", ""), TestGroups);
            return b;
        }

        public static Boolean TestGroup(String Class, String Description, String CancelNotPassed, String Independent, String Methods) {
            Boolean b = String.Equals(TestIndex.TestGroup.Class, Class);
            b &= String.Equals(TestIndex.TestGroup.Description, Description);
            b &= String.Equals(TestIndex.TestGroup.CancelNotPassed.ToString().ToLower(), CancelNotPassed);
            b &= String.Equals(TestIndex.TestGroup.Independent.ToString().ToLower(), Independent);
            b &= String.Equals(TestIndex.TestGroup.Ms().Replace("\"", ""), Methods);
            return b;
        }

        public static Boolean TestGroupPrior(String Class) {
            if (TestIndex.TestOperation.TestGroups.First() == TestIndex.TestGroup) return String.Equals(Class, UUT.NONE.Replace("\"", ""));
            return (1 + TestIndex.TestOperation.TestGroups.FindIndex(c => c.Class == Class) == TestIndex.TestOperation.TestGroups.IndexOf(TestIndex.TestGroup));
        }

        public static Boolean TestGroupNext(String Class) {
            if (TestIndex.TestOperation.TestGroups.Last() == TestIndex.TestGroup) return String.Equals(Class, UUT.NONE.Replace("\"", ""));
            return (TestIndex.TestOperation.TestGroups.FindIndex(c => c.Class == Class) == TestIndex.TestOperation.TestGroups.IndexOf(TestIndex.TestGroup) + 1);
        }

        public static Boolean Method(String Name, String Description, String CancelNotPassed) {
            Boolean b = String.Equals(TestIndex.Method.Name, Name);
            b &= String.Equals(TestIndex.Method.Description, Description);
            b &= String.Equals(TestIndex.Method.CancelNotPassed.ToString().ToLower(), CancelNotPassed);
            return b;
        }

        public static Boolean MethodCustom(String Name, String Description, String CancelNotPassed, String Parameters = null) {
            Debug.Assert(TestIndex.Method is MethodCustom);
            Boolean b = Method(Name, Description, CancelNotPassed);
            MethodCustom methodCustom = (MethodCustom)TestIndex.Method;
            if (Parameters != null) b &= String.Equals(methodCustom.Ps().Replace("\"", ""), Parameters);
            return b;
        }

        public static Boolean MethodInterval(String Name, String Description, String CancelNotPassed, String LowComparator, String Low, String High, String HighComparator, String FractionalDigits, String UnitPrefix, String Units, String UnitSuffix) {
            Debug.Assert(TestIndex.Method is MethodInterval);
            Boolean b = Method(Name, Description, CancelNotPassed);
            MethodInterval methodInterval = (MethodInterval)TestIndex.Method;
            b &= String.Equals(methodInterval.LowComparator, LowComparator);
            b &= String.Equals(methodInterval.Low, Low);
            b &= String.Equals(methodInterval.High, High);
            b &= String.Equals(methodInterval.HighComparator, HighComparator);
            b &= String.Equals(methodInterval.FractionalDigits, FractionalDigits);
            b &= String.Equals(methodInterval.UnitPrefix, UnitPrefix);
            b &= String.Equals(methodInterval.Units, Units);
            b &= String.Equals(methodInterval.UnitSuffix, UnitSuffix);
            return b;
        }

        public static Boolean MethodProcess(String Name, String Description, String CancelNotPassed, String Path, String Executable, String Parameters, String Expected) {
            Debug.Assert(TestIndex.Method is MethodProcess);
            Boolean b = Method(Name, Description, CancelNotPassed);
            MethodProcess methodProcess = (MethodProcess)TestIndex.Method;
            b &= String.Equals(methodProcess.Path, Path);
            b &= String.Equals(methodProcess.Executable, Executable);
            b &= String.Equals(methodProcess.Parameters, Parameters);
            b &= String.Equals(methodProcess.Expected, Expected);
            return b;
        }

        public static Boolean MethodTextual(String Name, String Description, String CancelNotPassed, String Text) {
            Debug.Assert(TestIndex.Method is MethodTextual);
            Boolean b = Method(Name, Description, CancelNotPassed);
            MethodTextual methodTextual = (MethodTextual)TestIndex.Method;
            b &= String.Equals(methodTextual.Text, Text);
            return b;
        }

        public static Boolean MethodPrior(String Name) {
            if (TestIndex.TestGroup.Methods.First() == TestIndex.Method) return String.Equals(Name, UUT.NONE.Replace("\"", ""));
            return (1 + TestIndex.TestGroup.Methods.FindIndex(m => m.Name == Name) == TestIndex.TestGroup.Methods.IndexOf(TestIndex.Method));
        }

        public static Boolean MethodNext(String Name) {
            if (TestIndex.TestGroup.Methods.Last() == TestIndex.Method) return String.Equals(Name, UUT.NONE.Replace("\"", ""));
            return (TestIndex.TestGroup.Methods.FindIndex(m => m.Name == Name) == TestIndex.TestGroup.Methods.IndexOf(TestIndex.Method) + 1);
        }
    }
}
