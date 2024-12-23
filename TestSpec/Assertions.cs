using System;
using System.Diagnostics;
using System.Linq;

namespace ABT.Test.TestLib.TestSpec {
    public static class Assertions {
        public static Boolean NS(String NamespaceRoot, String Description, String TestOperations) {
            Boolean b = String.Equals(TestSelection.TS.NamespaceRoot, NamespaceRoot);
            b &= String.Equals(TestSelection.TS.Description, Description);
            b &= String.Equals(TestSelection.TS.TOs(), TestOperations);
            return b;
        }

        public static Boolean TO(String NamespaceLeaf, String Description, String TestGroups) {
            Boolean b = String.Equals(TestIndex.TO.NamespaceLeaf, NamespaceLeaf);
            b &= String.Equals(TestIndex.TO.Description, Description);
            b &= String.Equals(TestIndex.TO.TGs(), TestGroups);
            return b;
        }

        public static Boolean TG(String Class, String Description, String CancelNotPassed, String Independent, String Methods) {
            Boolean b = String.Equals(TestIndex.TG.Class, Class);
            b &= String.Equals(TestIndex.TG.Description, Description);
            b &= String.Equals(TestIndex.TG.CancelNotPassed.ToString(), CancelNotPassed);
            b &= String.Equals(TestIndex.TG.Independent.ToString(), Independent);
            b &= String.Equals(TestIndex.TG.Ms(), Methods);
            return b;
        }

        public static Boolean TG_Prior(String Class) {
            if (TestIndex.TO.TestGroups.First() == TestIndex.TG) return String.Equals(Class, TS.NONE);
            return (1 + TestIndex.TO.TestGroups.FindIndex(c => c.Class == Class) == TestIndex.TO.TestGroups.IndexOf(TestIndex.TG));
        }

        public static Boolean TG_Next(String Class) {
            if (TestIndex.TO.TestGroups.Last() == TestIndex.TG) return String.Equals(Class, TS.NONE);
            return (TestIndex.TO.TestGroups.FindIndex(c => c.Class == Class) == TestIndex.TO.TestGroups.IndexOf(TestIndex.TG) + 1);
        }

        public static Boolean MC(String Method, String Description, String CancelNotPassed, String Parameters = null) {
            Debug.Assert(TestIndex.M is MC);
            MC mc = (MC)TestIndex.M;
            Boolean b = String.Equals(mc.Method, Method);
            b &= String.Equals(mc.Description, Description);
            b &= String.Equals(mc.CancelNotPassed.ToString(), CancelNotPassed);
            if (Parameters != null) b &= String.Equals(mc.Ps(), Parameters);
            return b;
        }

        public static Boolean MI(String Method, String Description, String CancelNotPassed, String LowComparator, String Low, String High, String HighComparator, String FractionalDigits, String UnitPrefix, String Units, String UnitSuffix) {
            Debug.Assert(TestIndex.M is MI);
            MI mi = (MI)TestIndex.M;
            Boolean b = String.Equals(TestIndex.M.Method, Method);
            b &= String.Equals(mi.Description, Description);
            b &= String.Equals(mi.CancelNotPassed.ToString(), CancelNotPassed);
            b &= String.Equals(mi.LowComparator, LowComparator);
            b &= String.Equals(mi.Low, Low);
            b &= String.Equals(mi.High, High);
            b &= String.Equals(mi.HighComparator, HighComparator);
            b &= String.Equals(mi.FractionalDigits, FractionalDigits);
            b &= String.Equals(mi.UnitPrefix, UnitPrefix);
            b &= String.Equals(mi.Units, Units);
            b &= String.Equals(mi.UnitSuffix, UnitSuffix);
            return b;
        }

        public static Boolean MP(String Method, String Description, String CancelNotPassed, String Path, String Executable, String Parameters, String Expected) {
            Debug.Assert(TestIndex.M is MP);
            MP mp = (MP)TestIndex.M;
            Boolean b = String.Equals(TestIndex.M.Method, Method);
            b &= String.Equals(mp.Description, Description);
            b &= String.Equals(mp.CancelNotPassed.ToString(), CancelNotPassed);
            b &= String.Equals(mp.Path, Path);
            b &= String.Equals(mp.Executable, Executable);
            b &= String.Equals(mp.Parameters, Parameters);
            b &= String.Equals(mp.Expected, Expected);
            return b;
        }

        public static Boolean MT(String Method, String Description, String CancelNotPassed, String Text) {
            Debug.Assert(TestIndex.M is MT);
            MT mt = (MT)TestIndex.M;
            Boolean b = String.Equals(TestIndex.M.Method, Method);
            b &= String.Equals(mt.Description, Description);
            b &= String.Equals(mt.CancelNotPassed.ToString(), CancelNotPassed);
            b &= String.Equals(mt.Text, Text);
            return b;
        }

        public static Boolean M_Prior(String Method) {
            if (TestIndex.TG.Methods.First() == TestIndex.M) return String.Equals(Method, TS.NONE);
            return (1 + TestIndex.TG.Methods.FindIndex(m => m.Method == Method) == TestIndex.TG.Methods.IndexOf(TestIndex.M));
        }

        public static Boolean M_Next(String Method) {
            if (TestIndex.TG.Methods.Last() == TestIndex.M) return String.Equals(Method, TS.NONE);
            return (TestIndex.TG.Methods.FindIndex(m => m.Method == Method) == TestIndex.TG.Methods.IndexOf(TestIndex.M) + 1);
        }
    }
}
