using System;
using System.Windows.Forms;
using Tektronix.Tkdpo2k3k4k.Interop;
using ABT.Test.TestLib.InstrumentDrivers.Interfaces;
using System.Collections.Generic;

namespace ABT.Test.TestLib.InstrumentDrivers.Oscilloscopes {
    public class MSO_3014_IVI_COM : Tkdpo2k3k4kClass, IInstruments, IDiagnostics {
        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ResetClear() {
            Utility.Reset();
        }

        public SELF_TEST_RESULTS SelfTests() {
            Int32 TestResult = 0;
            String TestMessage = String.Empty;
            try {
                UtilityEx.SelfTest(ref TestResult, ref TestMessage);
            } catch (Exception e) {
                _ = MessageBox.Show($"Instrument with driver {GetType().Name} failed its Self-Test:{Environment.NewLine}" + 
                    $"Type:      {InstrumentType}{Environment.NewLine}" +
                    $"Detail:    {Detail}{Environment.NewLine}" +
                    $"Address:   {Address}{Environment.NewLine}" +
                    $"Exception: {e}{Environment.NewLine}"
                    , "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // If unpowered or not communicating (comms cable possibly disconnected) SelfTest throws a
                // Keysight.CommandExpert.InstrumentAbstraction.CommunicationException exception,
                // which requires an apparently unavailable Keysight library to explicitly catch.
                return SELF_TEST_RESULTS.FAIL;
            }
            return (SELF_TEST_RESULTS)TestResult; // Tkdpo2k3k4kClass returns 0 for passed, 1 for fail.
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(Object o = null) {
            // TODO: Eventually; add verification measurements of the MSO-3014 mixed signal oscilloscope using external instrumentation.
            ResetClear();
            if (SelfTests() is SELF_TEST_RESULTS.PASS) return (true, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "34401A Diagnostics():", Message: "SelfTests() passed.", Event: EVENTS.PASS) });
            else return (false, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "34401A Diagnostics():", Message: "SelfTests() failed, aborted.", Event: EVENTS.FAIL) });
        }

        public MSO_3014_IVI_COM(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.OSCILLOSCOPE_MIXED_SIGNAL;
            Initialize(ResourceName: Address, IdQuery: false, Reset: false, OptionString: String.Empty);
        }
    }
}