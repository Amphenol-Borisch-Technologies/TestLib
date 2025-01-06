using System;
using System.Diagnostics;
using System.Linq;

namespace ABT.Test.TestLib.TestConfiguration {
    public static class Assertions {
        public static Boolean TestSpace(String NamespaceRoot, String Description, String TestOperations) {
            Boolean b = String.Equals(TestLib.testDefinition.TestSpace.NamespaceRoot, NamespaceRoot);
            b &= String.Equals(TestLib.testDefinition.TestSpace.Description, Description);
            b &= String.Equals(TestLib.testDefinition.TestSpace.TOs().Replace("\"", ""), TestOperations);
            return b;
        }

        public static Boolean TestOperation(String NamespaceTrunk, String Description, String TestGroups) {
            Boolean b = String.Equals(TestIndices.TestOperation.NamespaceTrunk, NamespaceTrunk);
            b &= String.Equals(TestIndices.TestOperation.Description, Description);
            b &= String.Equals(TestIndices.TestOperation.TGs().Replace("\"", ""), TestGroups);
            return b;
        }

        public static Boolean TestGroup(String Class, String Description, String CancelNotPassed, String Independent, String Methods) {
            Boolean b = String.Equals(TestIndices.TestGroup.Class, Class);
            b &= String.Equals(TestIndices.TestGroup.Description, Description);
            b &= String.Equals(TestIndices.TestGroup.CancelNotPassed.ToString().ToLower(), CancelNotPassed);
            b &= String.Equals(TestIndices.TestGroup.Independent.ToString().ToLower(), Independent);
            b &= String.Equals(TestIndices.TestGroup.Ms().Replace("\"", ""), Methods);
            return b;
        }

        public static Boolean TestGroupPrior(String Class) {
            if (TestIndices.TestOperation.TestGroups.First() == TestIndices.TestGroup) return String.Equals(Class, UUT.NONE.Replace("\"", ""));
            return (1 + TestIndices.TestOperation.TestGroups.FindIndex(c => c.Class == Class) == TestIndices.TestOperation.TestGroups.IndexOf(TestIndices.TestGroup));
        }

        public static Boolean TestGroupNext(String Class) {
            if (TestIndices.TestOperation.TestGroups.Last() == TestIndices.TestGroup) return String.Equals(Class, UUT.NONE.Replace("\"", ""));
            return (TestIndices.TestOperation.TestGroups.FindIndex(c => c.Class == Class) == TestIndices.TestOperation.TestGroups.IndexOf(TestIndices.TestGroup) + 1);
        }

        public static Boolean Method(String Name, String Description, String CancelNotPassed) {
            Boolean b = String.Equals(TestIndices.Method.Name, Name);
            b &= String.Equals(TestIndices.Method.Description, Description);
            b &= String.Equals(TestIndices.Method.CancelNotPassed.ToString().ToLower(), CancelNotPassed);
            return b;
        }

        public static Boolean MethodCustom(String Name, String Description, String CancelNotPassed, String Parameters = null) {
            Debug.Assert(TestIndices.Method is MethodCustom);
            Boolean b = Method(Name, Description, CancelNotPassed);
            MethodCustom methodCustom = (MethodCustom)TestIndices.Method;
            if (Parameters != null) b &= String.Equals(methodCustom.Ps().Replace("\"", ""), Parameters);
            return b;
        }

        public static Boolean MethodInterval(String Name, String Description, String CancelNotPassed, String LowComparator, String Low, String High, String HighComparator, String FractionalDigits, String UnitPrefix, String Units, String UnitSuffix) {
            Debug.Assert(TestIndices.Method is MethodInterval);
            Boolean b = Method(Name, Description, CancelNotPassed);
            MethodInterval methodInterval = (MethodInterval)TestIndices.Method;
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
            Debug.Assert(TestIndices.Method is MethodProcess);
            Boolean b = Method(Name, Description, CancelNotPassed);
            MethodProcess methodProcess = (MethodProcess)TestIndices.Method;
            b &= String.Equals(methodProcess.Path, Path);
            b &= String.Equals(methodProcess.Executable, Executable);
            b &= String.Equals(methodProcess.Parameters, Parameters);
            b &= String.Equals(methodProcess.Expected, Expected);
            return b;
        }

        public static Boolean MethodTextual(String Name, String Description, String CancelNotPassed, String Text) {
            Debug.Assert(TestIndices.Method is MethodTextual);
            Boolean b = Method(Name, Description, CancelNotPassed);
            MethodTextual methodTextual = (MethodTextual)TestIndices.Method;
            b &= String.Equals(methodTextual.Text, Text);
            return b;
        }

        public static Boolean MethodPrior(String Name) {
            if (TestIndices.TestGroup.Methods.First() == TestIndices.Method) return String.Equals(Name, UUT.NONE.Replace("\"", ""));
            return (1 + TestIndices.TestGroup.Methods.FindIndex(m => m.Name == Name) == TestIndices.TestGroup.Methods.IndexOf(TestIndices.Method));
        }

        public static Boolean MethodNext(String Name) {
            if (TestIndices.TestGroup.Methods.Last() == TestIndices.Method) return String.Equals(Name, UUT.NONE.Replace("\"", ""));
            return (TestIndices.TestGroup.Methods.FindIndex(m => m.Name == Name) == TestIndices.TestGroup.Methods.IndexOf(TestIndices.Method) + 1);
        }
    }
}
