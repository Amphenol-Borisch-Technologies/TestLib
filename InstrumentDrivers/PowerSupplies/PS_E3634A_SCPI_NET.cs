using System;
using System.Windows.Forms;
using Agilent.CommandExpert.ScpiNet.AgE363x_1_7;
using ABT.Test.TestLib.InstrumentDrivers.Interfaces;
using System.Collections.Generic;

namespace ABT.Test.TestLib.InstrumentDrivers.PowerSupplies {
    public class PS_E3634A_SCPI_NET : AgE363x, IInstruments, IPowerSupplyOutputs1, IDiagnostics {
        public enum RANGE { P25V, P50V }

        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ResetClear() {
            SCPI.RST.Command();
            SCPI.CLS.Command();
        }
        
        public SELF_TEST_RESULTS SelfTests() {
            Int32 result;
            try {
                SCPI.TST.Query(out result);
            } catch (Exception e) {
                Instruments.SelfTestFailure(this, e);
                return SELF_TEST_RESULTS.FAIL;
            }
            return (SELF_TEST_RESULTS)result; // AgE363x returns 0 for passed, 1 for fail.
        }

        public void OutputsOff() {
            SCPI.OUTPut.STATe.Command(Convert.ToBoolean(STATES.off));
        }

        public RANGE RangeGet() { 
            SCPI.SOURce.VOLTage.RANGe.Query(out String range);
            return (RANGE)Enum.Parse(typeof(RANGE), range);
        }
        public void RangeSet(RANGE Range) { SCPI.SOURce.VOLTage.RANGe.Command($"{Range}"); }

        public (Double AmpsDC, Double VoltsDC) Get(DC DC) {
            SCPI.MEASure.CURRent.DC.Query(out Double AmpsDC);
            SCPI.MEASure.VOLTage.DC.Query(out Double VoltsDC);
            return (AmpsDC, VoltsDC);
        }

        public void Set(Double Volts, Double Amps, Double OVP, STATES State) {
            SCPI.OUTPut.STATe.Command(false);
            SCPI.SOURce.VOLTage.PROTection.CLEar.Command();
            SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{MMD.MAXimum}");
            SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command($"{Volts}");
            SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command($"{Amps}");
            SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{OVP}");
            SCPI.OUTPut.STATe.Command(State == STATES.ON);
        }

        public STATES StateGet() {
            SCPI.OUTPut.STATe.Query(out Boolean state);
            return state ? STATES.ON : STATES.off;
        }

        public void StateSet(STATES State) { SCPI.OUTPut.STATe.Command(State == STATES.ON); }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(Object o = null) {
            // TODO: Eventually; add voltage & current measurements of the E3634A power supplie's outputs using external instrumentation.
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            return (passed, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
        }

        public PS_E3634A_SCPI_NET(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.POWER_SUPPLY;
        }
    }
}