using System;
using System.Diagnostics;
using System.Linq;

namespace ABT.Test.TestLib.Configuration {
    public static class Assertions {
        public static Boolean TestSpace(String NamespaceRoot, String Description, String TestOperations) {
            Boolean boolean = String.Equals(Data.testPlanDefinition.TestSpace.NamespaceRoot, NamespaceRoot);
            boolean &= String.Equals(Data.testPlanDefinition.TestSpace.Description, Description);
            boolean &= String.Equals(Data.testPlanDefinition.TestSpace.TOs().Replace("\"", ""), TestOperations);
            return boolean;
        }

        public static Boolean TestOperation(String NamespaceTrunk, String ProductionTest, String Description, String TestGroups) {
            Boolean boolean = String.Equals(TestIndices.TestOperation.NamespaceTrunk, NamespaceTrunk);
            boolean &= TestIndices.TestOperation.ProductionTest == Boolean.Parse(ProductionTest);
            boolean &= String.Equals(TestIndices.TestOperation.Description, Description);
            boolean &= String.Equals(TestIndices.TestOperation.TGs().Replace("\"", ""), TestGroups);
            return boolean;
        }

        public static Boolean TestGroup(String Classname, String Description, String CancelNotPassed, String Independent, String Methods) {
            Boolean boolean = String.Equals(TestIndices.TestGroup.Classname, Classname);
            boolean &= String.Equals(TestIndices.TestGroup.Description, Description);
            boolean &= TestIndices.TestGroup.CancelNotPassed == Boolean.Parse(CancelNotPassed);
            boolean &= TestIndices.TestGroup.Independent == Boolean.Parse(Independent);
            boolean &= String.Equals(TestIndices.TestGroup.Ms().Replace("\"", ""), Methods);
            return boolean;
        }

        //public static Boolean TestGroupPrior(String Classname) {
        //    if (TestIndices.TestOperation.TestGroups.First() == TestIndices.TestGroup) return String.Equals(Classname, UUT.NONE.Replace("\"", ""));
        //    return (1 + TestIndices.TestOperation.TestGroups.FindIndex(cn => cn.Classname == Classname) == TestIndices.TestOperation.TestGroups.IndexOf(TestIndices.TestGroup));
        //}  NOTE:  Presently unused.

        //public static Boolean TestGroupNext(String Classname) {
        //    if (TestIndices.TestOperation.TestGroups.Last() == TestIndices.TestGroup) return String.Equals(Classname, UUT.NONE.Replace("\"", ""));
        //    return (TestIndices.TestOperation.TestGroups.FindIndex(cn => cn.Classname == Classname) == TestIndices.TestOperation.TestGroups.IndexOf(TestIndices.TestGroup) + 1);
        //}  NOTE:  Presently unused.

        private static Boolean Method(String Name, String Description, String CancelNotPassed) {
            Boolean boolean = String.Equals(TestIndices.Method.Name, Name);
            boolean &= String.Equals(TestIndices.Method.Description, Description);
            boolean &= TestIndices.Method.CancelNotPassed == Boolean.Parse(CancelNotPassed);
            return boolean;
        }

        public static Boolean MethodCustom(String Name, String Description, String CancelNotPassed, String Parameters = null) {
            Debug.Assert(TestIndices.Method is MethodCustom);
            Boolean boolean = Method(Name, Description, CancelNotPassed);
            MethodCustom methodCustom = (MethodCustom)TestIndices.Method;
            if (Parameters != null) boolean &= String.Equals(methodCustom.Ps().Replace("\"", ""), Parameters);
            return boolean;
        }

        public static Boolean MethodInterval(String Name, String Description, String CancelNotPassed, String LowComparator, String Low, String High, String HighComparator, String FractionalDigits, String UnitPrefix, String Units, String UnitSuffix) {
            Debug.Assert(TestIndices.Method is MethodInterval);
            Boolean boolean = Method(Name, Description, CancelNotPassed);
            MethodInterval methodInterval = (MethodInterval)TestIndices.Method;
            boolean &= methodInterval.LowComparator == (MI_LowComparator)Enum.Parse(typeof(MI_LowComparator), LowComparator);
            boolean &= methodInterval.Low == Double.Parse(Low);
            boolean &= methodInterval.High == Double.Parse(High);
            boolean &= methodInterval.HighComparator == (MI_HighComparator)Enum.Parse(typeof(MI_HighComparator), HighComparator);
            boolean &= methodInterval.FractionalDigits == UInt32.Parse(FractionalDigits);
            boolean &= methodInterval.UnitPrefix == (MI_UnitPrefix)Enum.Parse(typeof(MI_UnitPrefix), UnitPrefix);
            boolean &= methodInterval.Units == (MI_Units)Enum.Parse(typeof(MI_Units), Units);
            boolean &= methodInterval.UnitSuffix == (MI_UnitSuffix)Enum.Parse(typeof(MI_UnitSuffix), UnitSuffix);
            return boolean;
        }

        public static Boolean MethodProcess(String Name, String Description, String CancelNotPassed, String Folder, String File, String Parameters, String Expected) {
            Debug.Assert(TestIndices.Method is MethodProcess);
            Boolean boolean = Method(Name, Description, CancelNotPassed);
            MethodProcess methodProcess = (MethodProcess)TestIndices.Method;
            boolean &= String.Equals(methodProcess.Folder, Folder);
            boolean &= String.Equals(methodProcess.File, File);
            boolean &= String.Equals(methodProcess.Parameters, Parameters);
            boolean &= String.Equals(methodProcess.Expected, Expected);
            return boolean;
        }

        public static Boolean MethodTextual(String Name, String Description, String CancelNotPassed, String Text) {
            Debug.Assert(TestIndices.Method is MethodTextual);
            Boolean boolean = Method(Name, Description, CancelNotPassed);
            MethodTextual methodTextual = (MethodTextual)TestIndices.Method;
            boolean &= String.Equals(methodTextual.Text, Text);
            return boolean;
        }

        //public static Boolean MethodPrior(String Name) {
        //    if (Data.testSequence.IsOperation && (TestIndices.TestGroup.Methods.First() == TestIndices.Method)) {
        //        if (TestIndices.TestOperation.TestGroups.First() == TestIndices.TestGroup) return String.Equals(Name, UUT.NONE.Replace("\"", ""));
        //        Int32 i = TestIndices.TestOperation.TestGroups.FindIndex(tg => tg == TestIndices.TestGroup);
        //        TestGroup testGroupPrior = TestIndices.TestOperation.TestGroups.ElementAt(i - 1);
        //        return String.Equals(testGroupPrior.Methods.Last().Name, Name);
        //    }
        //    if (TestIndices.TestGroup.Methods.First() == TestIndices.Method) return String.Equals(Name, UUT.NONE.Replace("\"", ""));
        //    return (1 + TestIndices.TestGroup.Methods.FindIndex(m => m.Name == Name) == TestIndices.TestGroup.Methods.IndexOf(TestIndices.Method));
        //}  NOTE:  Presently unused.

        //public static Boolean MethodNext(String Name) {
        //    if (Data.testSequence.IsOperation && (TestIndices.TestGroup.Methods.Last() == TestIndices.Method)) {
        //        if (TestIndices.TestOperation.TestGroups.Last() == TestIndices.TestGroup) return String.Equals(Name, UUT.NONE.Replace("\"", ""));
        //        Int32 i = TestIndices.TestOperation.TestGroups.FindIndex(tg => tg == TestIndices.TestGroup);
        //        TestGroup testGroupNext = TestIndices.TestOperation.TestGroups.ElementAt(i + 1);
        //        return String.Equals(testGroupNext.Methods.First().Name, Name);
        //    }
        //    if (TestIndices.TestGroup.Methods.Last() == TestIndices.Method) return String.Equals(Name, UUT.NONE.Replace("\"", ""));
        //    return (TestIndices.TestGroup.Methods.FindIndex(m => m.Name == Name) == TestIndices.TestGroup.Methods.IndexOf(TestIndices.Method) + 1);
        //}  NOTE:  Presently unused.
    }
}
