using System;

namespace ABT.Test.TestLib.TestSpec {
    public static class Assertions {
        public static Boolean NS(String NamespaceRoot, String Description, String TestOperations) { return true; }
        public static Boolean TO(String NamespaceLeaf, String Description, String TestGroups) { return true; }
        public static Boolean TG(String Class, String Description, String CancelNotPassed, String Independent, String Methods) { return true; }
        public static Boolean TG_Prior(String Class) { return true; }
        public static Boolean TG_Next(String Class) { return true; }
        public static Boolean MC(String Method, String Description, String CancelNotPassed, String Parameters = null) { return true; }
        public static Boolean MI(String Method, String Description, String CancelNotPassed, String LowComparator, String Low, String High, String HighComparator, String FractionalDigits, String UnitPrefix, String Units, String UnitSuffix) { return true; }
        public static Boolean MP(String Method, String Description, String CancelNotPassed, String Path, String Executable, String Parameters, String Expected) { return true; }
        public static Boolean MT(String Method, String Description, String CancelNotPassed, String Text) { return true; }
        public static Boolean M_Prior(String Method) { return true; }
        public static Boolean M_Next(String Method) { return true; }
    }
}
