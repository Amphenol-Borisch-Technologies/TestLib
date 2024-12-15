using System;

namespace TestSequencer {
    internal static class Assertions {
        internal static Boolean TO(String Namespace, String Description, String TestGroups) { return true; }
        internal static Boolean TG(String Class, String Description, String CancelIfFail, String Independent, String Methods) { return true; }
        internal static Boolean TG_Prior(String Class) { return true; }
        internal static Boolean TG_Next(String Class) { return true; }
        internal static Boolean MC(String Method, String Description, String CancelIfFail, String Parameters = null) { return true; }
        internal static Boolean MI(String Method, String Description, String CancelIfFail, String LowComparator, String Low, String High, String HighComparator, String FractionalDigits, String UnitPrefix, String Units, String UnitSuffix) { return true; }
        internal static Boolean MP(String Method, String Description, String CancelIfFail, String Path, String Executable, String Parameters, String Expected) { return true; }
        internal static Boolean MT(String Method, String Description, String CancelIfFail, String Text) { return true; }
        internal static Boolean M_Prior(String Method) { return true; }
        internal static Boolean M_Next(String Method) { return true; }
    }
}
