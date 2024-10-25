using System;

namespace ABT.Test.Lib.AppConfig {
    public class AppConfigLogger {
        public readonly Boolean FileEnabled = Boolean.Parse(TestData.ConfigMapUUT.AppSettings.Settings["LOGGER_FileEnabled"].Value.Trim());
        public readonly String FilePath = TestData.ConfigMapUUT.AppSettings.Settings["LOGGER_FilePath"].Value.Trim();
        public readonly Boolean SerialNumberDialogEnabled = Boolean.Parse(TestData.ConfigMapUUT.AppSettings.Settings["LOGGER_SerialNumberDialogEnabled"].Value.Trim());
        public readonly Boolean SQLEnabled = Boolean.Parse(TestData.ConfigMapUUT.AppSettings.Settings["LOGGER_SQLEnabled"].Value.Trim());
        public readonly String SQLConnectionString = TestData.ConfigMapUUT.AppSettings.Settings["LOGGER_SQLConnectionString"].Value.Trim();

        private AppConfigLogger() { if (!FilePath.EndsWith(@"\")) FilePath += @"\"; }
        // Logging.FileStop() requires terminating "\" character.

        public static AppConfigLogger Get() { return new AppConfigLogger(); }
    }
}
