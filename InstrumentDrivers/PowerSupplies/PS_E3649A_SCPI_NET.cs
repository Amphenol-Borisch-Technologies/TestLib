using System;
using Agilent.CommandExpert.ScpiNet.AgE364xD_1_7;
using ABT.Test.TestLib.InstrumentDrivers.Interfaces;
using System.Collections.Generic;

namespace ABT.Test.TestLib.InstrumentDrivers.PowerSupplies {
    public class PS_E3649A_SCPI_NET : AgE364xD, IInstrument, IPowerSupplyE3649A, IDiagnostics {
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
            // NOTE: Most multi-output supplies like the E3649A permit individual control of outputs,
            // but the E3649A does not; all supplies are set to the same STATE, off or ON.
            SCPI.OUTPut.STATe.Command(Convert.ToBoolean(STATES.off));
        }

        public OUTPUTS2 Selected() {
            SCPI.INSTrument.SELect.Query(out String select);
            return select == "OUTP1" ? OUTPUTS2.OUTput1 : OUTPUTS2.OUTput2;
        }

        public void Select(OUTPUTS2 Output) { SCPI.INSTrument.SELect.Command($"{Output}"); }

        public (Double AmpsDC, Double VoltsDC) Get(OUTPUTS2 Output, DC DC) {
            Select(Output);
            SCPI.MEASure.SCALar.CURRent.DC.Query(out Double AmpsDC);
            SCPI.MEASure.SCALar.VOLTage.DC.Query(out Double VoltsDC);
            return (AmpsDC, VoltsDC);
        }

        public void SetOffOn(OUTPUTS2 Output, Double Volts, Double Amps, Double OVP, STATES State) {
            Select(Output);
            SCPI.OUTPut.STATe.Command(false);
            SCPI.SOURce.VOLTage.PROTection.CLEar.Command();
            SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{MMD.MAXimum}");
            SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command($"{Volts}");
            SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command($"{Amps}");
            SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{OVP}");
            SCPI.OUTPut.STATe.Command(State == STATES.ON);
        }

        public STATES StateGet(OUTPUTS2 Output) {
            Select(Output);
            SCPI.OUTPut.STATe.Query(out Boolean state);
            return state ? STATES.ON : STATES.off;
        }

        public void StateSet(STATES State) {
            // NOTE: Most multi-output supplies like the E3649A permit individual control of outputs,
            // but the E3649A does not; all supplies are set to the same STATE, off or ON.
            SCPI.OUTPut.STATe.Command(State == STATES.ON);
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(Object o = null) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_E3649A = (passed, new List<DiagnosticsResult>()  { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                // TODO: Eventually; add voltage & current measurements of the E3649A's outputs using external instrumentation.

            }
            return result_E3649A;
        }

        public PS_E3649A_SCPI_NET(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.POWER_SUPPLY;
        }
    }
}