using System;
using System.Linq;

namespace ABT.Test.TestLib.Miscellaneous {
    public class TimerMilliSeconds {
        private readonly Int32 _milliSeconds;
        private readonly DateTime _start;
        public TimerMilliSeconds(Int32 MilliSeconds) { _milliSeconds = MilliSeconds; _start = DateTime.Now; }

        public DateTime GetStart() { return _start; }
        public Int32 GetMilliSecondsTotal() { return _milliSeconds; }
        public Int32 GetMilliSecondsElapsed() { return (Int32)Math.Round((DateTime.Now - _start).TotalMilliseconds); }
        public Int32 GetMilliSecondsRemaining() { return Expired() ? 0 : _milliSeconds - GetMilliSecondsElapsed(); }
        public Boolean Expired() { return GetMilliSecondsElapsed() >= _milliSeconds; }
        public Boolean NotExpired() { return !Expired(); }
    }

    public class TimerSeconds {
        private readonly Double _seconds;
        private readonly DateTime _start;
        public TimerSeconds(Double Seconds) { _seconds = Seconds; _start = DateTime.Now; }

        public DateTime GetStart() { return _start; }
        public Double GetSecondsTotal() { return _seconds; }
        public Double GetSecondsElapsed() { return Math.Round((DateTime.Now - _start).TotalSeconds, 2); }
        public Double GetSecondsRemaining() { return Expired() ? 0 : _seconds - GetSecondsElapsed(); }
        public Boolean Expired() { return GetSecondsElapsed() > _seconds; }
        public Boolean NotExpired() { return !Expired(); }
    }

    public static class GUIDs {
        public static String GetUrlSafeBase64GUID() {
            Byte[] guidBytes = Guid.Parse(Guid.NewGuid().ToString("N")).ToByteArray(); // "N" format specifier removes dashes
            return Convert.ToBase64String(guidBytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        public static Guid GetGUIDFromUrlSafeBase64(String urlSafeBase64String) {
            String base64String = urlSafeBase64String.Replace("-", "+").Replace("_", "/");
            switch (base64String.Length % 4) {
                case 2: base64String += "=="; break;
                case 3: base64String += "="; break;
            }
            return new Guid(Convert.FromBase64String(base64String));
        }

        public static String GuidToString(Guid guid) { return guid.ToString("D"); }
    }

    public static class Ext { public static Boolean In<T>(this T value, params T[] values) where T : struct { return values.Contains(value); } }
}