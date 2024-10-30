using System;

namespace ABT.TestExec.Lib.AppConfig {
    public class AppConfigLogger {
        public readonly Boolean FileEnabled = Boolean.Parse(TestLib.ConfigMap.AppSettings.Settings["LOGGER_FileEnabled"].Value.Trim());
        public readonly String FilePath = TestLib.ConfigMap.AppSettings.Settings["LOGGER_FilePath"].Value.Trim();
        public readonly Boolean SerialNumberDialogEnabled = Boolean.Parse(TestLib.ConfigMap.AppSettings.Settings["LOGGER_SerialNumberDialogEnabled"].Value.Trim());
        public readonly Boolean SQLEnabled = Boolean.Parse(TestLib.ConfigMap.AppSettings.Settings["LOGGER_SQLEnabled"].Value.Trim());
        public readonly String SQLConnectionString = TestLib.ConfigMap.AppSettings.Settings["LOGGER_SQLConnectionString"].Value.Trim();

        private AppConfigLogger() { if (!FilePath.EndsWith(@"\")) FilePath += @"\"; }
        // Logging.FileStop() requires terminating "\" character.

        public static AppConfigLogger Get() {
            return new AppConfigLogger();
        }
    }
}
