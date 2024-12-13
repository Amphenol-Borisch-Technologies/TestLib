using System;
using System.Collections.Generic;

namespace ABT.TestExec.Lib.InstrumentDrivers.Interfaces {
    public interface IDiagnostics { (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(Object o = null); }

    public class DiagnosticsResult {
        public readonly String Label;
        public readonly String Message;
        public Boolean Passed;
        public DiagnosticsResult(String label, String message) { Label = label; Message = message; }
        public DiagnosticsResult(String label, String message, Boolean passed) { Label = label; Message = message; Passed = passed; }
    }
}
