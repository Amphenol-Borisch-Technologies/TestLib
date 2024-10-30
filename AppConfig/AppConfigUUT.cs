using System;

namespace ABT.TestExec.Lib.AppConfig {
    public class AppConfigUUT {
        public readonly String Customer = TestLib.ConfigMap.AppSettings.Settings["UUT_Customer"].Value.Trim();
        public readonly String Type = TestLib.ConfigMap.AppSettings.Settings["UUT_Type"].Value  ;
        public readonly String Number = TestLib.ConfigMap.AppSettings.Settings["UUT_Number"].Value.Trim();
        public readonly String Revision = TestLib.ConfigMap.AppSettings.Settings["UUT_Revision"].Value.Trim();
        public readonly String Description = TestLib.ConfigMap.AppSettings.Settings["UUT_Description"].Value.Trim();
        public readonly String TestSpecification =      TestLib.ConfigMap.AppSettings.Settings["UUT_TestSpecification"].Value.Trim();
        public readonly String DocumentationFolder = TestLib.ConfigMap.AppSettings.Settings["UUT_DocumentationFolder"].Value.Trim();
        public readonly String ManualsFolder = TestLib.ConfigMap.AppSettings.Settings["UUT_ManualsFolder"].Value.Trim();
        public readonly String EMailTestEngineer =  TestLib.ConfigMap.AppSettings.Settings["UUT_TestEngineerEmail"].Value.Trim();
        public readonly String SerialNumberRegExCustom = TestLib.ConfigMap.AppSettings.Settings["UUT_SerialNumberRegExCustom"].Value.Trim();
        public readonly Boolean Simulate = Boolean.Parse(TestLib.ConfigMap.AppSettings.Settings["UUT_Simulate"].Value);
        public String SerialNumber { get; set; } = String.Empty; // Input during testing.
        public EVENTS Event { get; set; } = EVENTS.UNSET; // Determined post-test.

        private AppConfigUUT() { }

        public static AppConfigUUT Get() { return new AppConfigUUT(); }
    }
}
