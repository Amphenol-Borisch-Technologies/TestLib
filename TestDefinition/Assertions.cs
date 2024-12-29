using System;
using System.Diagnostics;
using System.Linq;

namespace ABT.Test.TestLib.TestDefinition {
    public static class Assertions {
        public static Boolean NS(String NamespaceRoot, String Description, String TestOperations) {
            Boolean b = String.Equals(TestSelection.TestSpace.NamespaceRoot, NamespaceRoot);
            b &= String.Equals(TestSelection.TestSpace.Description, Description);
            b &= String.Equals(TestSelection.TestSpace.TOs().Replace("\"", ""), TestOperations);
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

        public static Boolean TG_Prior(String Class) {
            if (TestIndex.TestOperation.TestGroups.First() == TestIndex.TestGroup) return String.Equals(Class, UUT.NONE.Replace("\"", ""));
            return (1 + TestIndex.TestOperation.TestGroups.FindIndex(c => c.Class == Class) == TestIndex.TestOperation.TestGroups.IndexOf(TestIndex.TestGroup));
        }

        public static Boolean TG_Next(String Class) {
            if (TestIndex.TestOperation.TestGroups.Last() == TestIndex.TestGroup) return String.Equals(Class, UUT.NONE.Replace("\"", ""));
            return (TestIndex.TestOperation.TestGroups.FindIndex(c => c.Class == Class) == TestIndex.TestOperation.TestGroups.IndexOf(TestIndex.TestGroup) + 1);
        }

        public static Boolean M(String Method, String Description, String CancelNotPassed) {
            Boolean b = String.Equals(TestIndex.M.Method, Method);
            b &= String.Equals(TestIndex.M.Description, Description);
            b &= String.Equals(TestIndex.M.CancelNotPassed.ToString().ToLower(), CancelNotPassed);
            return b;
        }

        public static Boolean MethodCustom(String Method, String Description, String CancelNotPassed, String Parameters = null) {
            Debug.Assert(TestIndex.M is MethodCustom);
            Boolean b = M(Method, Description, CancelNotPassed);
            MethodCustom methodCustom = (MethodCustom)TestIndex.M;
            if (Parameters != null) b &= String.Equals(methodCustom.Ps().Replace("\"", ""), Parameters);
            return b;
        }

        public static Boolean MethodInterval(String Method, String Description, String CancelNotPassed, String LowComparator, String Low, String High, String HighComparator, String FractionalDigits, String UnitPrefix, String Units, String UnitSuffix) {
            Debug.Assert(TestIndex.M is MethodInterval);
            Boolean b = M(Method, Description, CancelNotPassed);
            MethodInterval methodInterval = (MethodInterval)TestIndex.M;
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

        public static Boolean MP(String Method, String Description, String CancelNotPassed, String Path, String Executable, String Parameters, String Expected) {
            Debug.Assert(TestIndex.M is MP);
            Boolean b = M(Method, Description, CancelNotPassed);
            MP mp = (MP)TestIndex.M;
            b &= String.Equals(mp.Path, Path);
            b &= String.Equals(mp.Executable, Executable);
            b &= String.Equals(mp.Parameters, Parameters);
            b &= String.Equals(mp.Expected, Expected);
            return b;
        }

        public static Boolean MT(String Method, String Description, String CancelNotPassed, String Text) {
            Debug.Assert(TestIndex.M is MT);
            Boolean b = M(Method, Description, CancelNotPassed);
            MT mt = (MT)TestIndex.M;
            b &= String.Equals(mt.Text, Text);
            return b;
        }

        public static Boolean M_Prior(String Method) {
            if (TestIndex.TestGroup.Methods.First() == TestIndex.M) return String.Equals(Method, UUT.NONE.Replace("\"", ""));
            return (1 + TestIndex.TestGroup.Methods.FindIndex(m => m.Method == Method) == TestIndex.TestGroup.Methods.IndexOf(TestIndex.M));
        }

        public static Boolean M_Next(String Method) {
            if (TestIndex.TestGroup.Methods.Last() == TestIndex.M) return String.Equals(Method, UUT.NONE.Replace("\"", ""));
            return (TestIndex.TestGroup.Methods.FindIndex(m => m.Method == Method) == TestIndex.TestGroup.Methods.IndexOf(TestIndex.M) + 1);
        }
    }
}
