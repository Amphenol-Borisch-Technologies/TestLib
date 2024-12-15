using System;
using System.Collections.Generic;

namespace ABT.Test.Lib.InstrumentDrivers.Interfaces {
    public interface IDiagnostics { (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(Object o = null); }

    public class DiagnosticsResult {
        public readonly String Label;
        public readonly String Message;
        public EVENTS Event;
        public DiagnosticsResult(String Label, String Message, EVENTS Event) { this.Label = Label; this.Message = Message; this.Event = Event; }
    }
}
