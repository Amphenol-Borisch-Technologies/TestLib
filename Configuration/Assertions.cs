using System;
using System.Diagnostics;
using System.Linq;

namespace ABT.Test.TestLib.Configuration {
    public static class Assertions {
        public static Boolean TestSpace(String NamespaceRoot, String Description, String TestOperations) {
            Boolean b = String.Equals(Data.testDefinition.TestSpace.NamespaceRoot, NamespaceRoot);
            b &= String.Equals(Data.testDefinition.TestSpace.Description, Description);
            b &= String.Equals(Data.testDefinition.TestSpace.TOs().Replace("\"", ""), TestOperations);
            return b;
        }

        public static Boolean TestOperation(String NamespaceTrunk, String ProductionTest, String Description, String TestGroups) {
            Boolean b = String.Equals(TestIndices.TestOperation.NamespaceTrunk, NamespaceTrunk);
            b &= TestIndices.TestOperation.ProductionTest == Boolean.Parse(ProductionTest);
            b &= String.Equals(TestIndices.TestOperation.Description, Description);
            b &= String.Equals(TestIndices.TestOperation.TGs().Replace("\"", ""), TestGroups);
            return b;
        }

        public static Boolean TestGroup(String Classname, String Description, String CancelNotPassed, String Independent, String Methods) {
            Boolean b = String.Equals(TestIndices.TestGroup.Classname, Classname);
            b &= String.Equals(TestIndices.TestGroup.Description, Description);
            b &= TestIndices.TestGroup.CancelNotPassed == Boolean.Parse(CancelNotPassed);
            b &= TestIndices.TestGroup.Independent == Boolean.Parse(Independent);
            b &= String.Equals(TestIndices.TestGroup.Ms().Replace("\"", ""), Methods);
            return b;
        }

        public static Boolean TestGroupPrior(String Classname) {
            if (TestIndices.TestOperation.TestGroups.First() == TestIndices.TestGroup) return String.Equals(Classname, UUT.NONE.Replace("\"", ""));
            return (1 + TestIndices.TestOperation.TestGroups.FindIndex(cn => cn.Classname == Classname) == TestIndices.TestOperation.TestGroups.IndexOf(TestIndices.TestGroup));
        }

        public static Boolean TestGroupNext(String Classname) {
            if (TestIndices.TestOperation.TestGroups.Last() == TestIndices.TestGroup) return String.Equals(Classname, UUT.NONE.Replace("\"", ""));
            return (TestIndices.TestOperation.TestGroups.FindIndex(cn => cn.Classname == Classname) == TestIndices.TestOperation.TestGroups.IndexOf(TestIndices.TestGroup) + 1);
        }

        public static Boolean Method(String Name, String Description, String CancelNotPassed) {
            Boolean b = String.Equals(TestIndices.Method.Name, Name);
            b &= String.Equals(TestIndices.Method.Description, Description);
            b &= TestIndices.Method.CancelNotPassed == Boolean.Parse(CancelNotPassed);
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
            b &= methodInterval.LowComparator == (MI_LowComparator)Enum.Parse(typeof(MI_LowComparator), LowComparator);
            b &= methodInterval.Low == Double.Parse(Low);
            b &= methodInterval.High == Double.Parse(High);
            b &= methodInterval.HighComparator == (MI_HighComparator)Enum.Parse(typeof(MI_HighComparator), HighComparator);
            b &= methodInterval.FractionalDigits == UInt32.Parse(FractionalDigits);
            b &= methodInterval.UnitPrefix == (MI_UnitPrefix)Enum.Parse(typeof(MI_UnitPrefix), UnitPrefix);
            b &= methodInterval.Units == (MI_Units)Enum.Parse(typeof(MI_Units), Units);
            b &= methodInterval.UnitSuffix == (MI_UnitSuffix)Enum.Parse(typeof(MI_UnitSuffix), UnitSuffix);
            return b;
        }

        public static Boolean MethodProcess(String Name, String Description, String CancelNotPassed, String Folder, String File, String Parameters, String Expected) {
            Debug.Assert(TestIndices.Method is MethodProcess);
            Boolean b = Method(Name, Description, CancelNotPassed);
            MethodProcess methodProcess = (MethodProcess)TestIndices.Method;
            b &= String.Equals(methodProcess.Folder, Folder);
            b &= String.Equals(methodProcess.File, File);
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
