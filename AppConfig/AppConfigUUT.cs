using System;

namespace ABT.Test.Lib.AppConfig {
    public class AppConfigUUT {
        public readonly String Customer = TestData.ConfigMapUUT.AppSettings.Settings["UUT_Customer"].Value.Trim();
        public readonly String Type = TestData.ConfigMapUUT.AppSettings.Settings["UUT_Type"].Value  ;
        public readonly String Number = TestData.ConfigMapUUT.AppSettings.Settings["UUT_Number"].Value.Trim();
        public readonly String Revision = TestData.ConfigMapUUT.AppSettings.Settings["UUT_Revision"].Value.Trim();
        public readonly String Description = TestData.ConfigMapUUT.AppSettings.Settings["UUT_Description"].Value.Trim();
        public readonly String TestSpecification =      TestData.ConfigMapUUT.AppSettings.Settings["UUT_TestSpecification"].Value.Trim();
        public readonly String DocumentationFolder = TestData.ConfigMapUUT.AppSettings.Settings["UUT_DocumentationFolder"].Value.Trim();
        public readonly String ManualsFolder = TestData.ConfigMapUUT.AppSettings.Settings["UUT_ManualsFolder"].Value.Trim();
        public readonly String EMailTestEngineer =  TestData.ConfigMapUUT.AppSettings.Settings["UUT_TestEngineerEmail"].Value.Trim();
        public readonly String SerialNumberRegExCustom = TestData.ConfigMapUUT.AppSettings.Settings["UUT_SerialNumberRegExCustom"].Value.Trim();
        public readonly Boolean Simulate = Boolean.Parse(TestData.ConfigMapUUT.AppSettings.Settings["UUT_Simulate"].Value);
        public String SerialNumber { get; set; } = String.Empty; // Input during testing.
        public String TestEvent { get; set; } = TestEvents.UNSET; // Determined post-test.

        private AppConfigUUT() { }

        public static AppConfigUUT Get() { return new AppConfigUUT(); }
    }
}
