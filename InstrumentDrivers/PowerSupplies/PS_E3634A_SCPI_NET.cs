using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Agilent.CommandExpert.ScpiNet.AgE363x_1_7;
using ABT.Test.TestLib.InstrumentDrivers.Interfaces;
using ABT.Test.TestLib.InstrumentDrivers.Multifunction;

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

        public void SetOffOn(Double VoltsDC, Double AmpsDC, Double OVP, STATES State) {
            SCPI.OUTPut.STATe.Command(false);
            SCPI.SOURce.VOLTage.PROTection.CLEar.Command();
            SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{MMD.MAXimum}");
            SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command($"{VoltsDC}");
            SCPI.SOURce.CURRent.LEVel.IMMediate.AMPLitude.Command($"{AmpsDC}");
            SCPI.SOURce.VOLTage.PROTection.LEVel.Command($"{OVP}");
            SCPI.OUTPut.STATe.Command(State == STATES.ON);
        }

        public STATES StateGet() {
            SCPI.OUTPut.STATe.Query(out Boolean state);
            return state ? STATES.ON : STATES.off;
        }

        public void StateSet(STATES State) { SCPI.OUTPut.STATe.Command(State == STATES.ON); }

        #region Diagnostics
        public class DiagnosticParameter_E3649A {
            public Double AccuracyVDC = 0.1;
            public Double AccuracyADC = 0.1;
            public DiagnosticParameter_E3649A() { }
            public DiagnosticParameter_E3649A(Double AccuracyVDC, Double AccuracyADC) {
                this.AccuracyVDC = AccuracyVDC;
                this.AccuracyADC = AccuracyADC;
            }
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(Object o = null) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_E3634A = (passed, new List<DiagnosticsResult>()  { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                DiagnosticParameter_E3649A DP = (o is DiagnosticParameter_E3649A dp) ? dp : new DiagnosticParameter_E3649A();
                MSMU_34980A_SCPI_NET MSMU = ((MSMU_34980A_SCPI_NET)(Data.InstrumentDrivers["MSMU1_34980A"]));

                String message = 
                    $"Please connect BMC6030-5 from {Detail} with VISA Address {Address}{Environment.NewLine}" + 
                    $"to {MSMU.Detail} with VISA Address {MSMU.Address} Address Busses.{Environment.NewLine}{Environment.NewLine}" +
                    "Click Cancel if desired.";
                if (DialogResult.OK == MessageBox.Show(message, "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                    SCPI.SOURce.VOLTage.PROTection.LEVel.Query("MAXimum", out Double OVP);
                    Boolean passed_VDC = true, passed_ADC = true, passed_E3634A = true;
                    const Double adcApplied = 0.015;
                    SCPI.OUTPut.STATe.Command(false);
                    SCPI.SOURce.VOLTage.PROTection.STATe.Command(false);
                    SCPI.SOURce.CURRent.PROTection.CLEar.Command();
                    SCPI.SOURce.CURRent.PROTection.LEVel.Command(adcApplied);
                    SCPI.SOURce.CURRent.PROTection.STATe.Command(true);
                    SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command("MINimum");                    
                    SCPI.SOURce.VOLTage.LEVel.IMMediate.STEP.INCRement.Command(1D);
                    SCPI.OUTPut.STATe.Command(true);

                    for (Int32 vdcApplied = 0; vdcApplied <= 50; vdcApplied++) {
                        MSMU.SCPI.MEASure.SCALar.VOLTage.DC.Query("AUTO", $"{MMD.MAXimum}", ch_list: null, out Double[] vdcMeasured);
                        MSMU.SCPI.MEASure.SCALar.CURRent.DC.QueryQuery("AUTO", $"{MMD.MAXimum}", ch_list: null, out Double[] adcMeasured);
                        passed_VDC = Math.Abs(vdcMeasured[0] - vdcApplied) <= DP.AccuracyVDC;
                        passed_ADC = Math.Abs(adcMeasured[0] - adcApplied) <= DP.AccuracyVDC;

                        passed_E3634A &= passed_VDC &= passed_ADC;
                        result_E3634A.Details.Add(new DiagnosticsResult(Label: "Voltage DC", Message: $"Applied {vdcApplied}VDC, measured {Math.Round(vdcMeasured[0], 3, MidpointRounding.ToEven)}VDC", Event: (passed_VDC ? EVENTS.PASS : EVENTS.FAIL)));
                        result_E3634A.Details.Add(new DiagnosticsResult(Label: "Amperage DC", Message: $"Applied {adcApplied}ADC, measured {Math.Round(adcMeasured[0], 3, MidpointRounding.ToEven)}ADC", Event: (passed_ADC ? EVENTS.PASS : EVENTS.FAIL)));
                        SCPI.SOURce.VOLTage.LEVel.IMMediate.AMPLitude.Command("UP");
                    }
                }
            }
            return result_E3634A;
        }
        #endregion Diagnostics

        public PS_E3634A_SCPI_NET(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.POWER_SUPPLY;
        }
    }
}