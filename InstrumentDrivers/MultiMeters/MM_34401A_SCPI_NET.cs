﻿using System;
using System.Windows.Forms;
using Agilent.CommandExpert.ScpiNet.Ag34401_11;
using ABT.TestExec.Lib.InstrumentDrivers.Interfaces;

namespace ABT.TestExec.Lib.InstrumentDrivers.MultiMeters {
    public class MM_34401A_SCPI_NET : Ag34401, IInstruments {
        public enum MMD { MIN, MAX, DEF }
        public enum TERMINALS { Front, Rear };
        public enum PROPERTY { AmperageAC, AmperageDC, Continuity, Frequency, Fresistance, Period, Resistance, VoltageAC, VoltageDC, VoltageDiodic }


        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ResetClear() {
            SCPI.RST.Command();
            SCPI.CLS.Command();
        }

        public DIAGNOSTICS_RESULTS Diagnostics() {
            Boolean result;
            try {
                SCPI.TST.Query(out result);
            } catch (Exception) {
                _ = MessageBox.Show($"Instrument with driver {GetType().Name} likely unpowered or not communicating:{Environment.NewLine}" + 
                    $"Type:      {InstrumentType}{Environment.NewLine}" +
                    $"Detail:    {Detail}{Environment.NewLine}" +
                    $"Address:   {Address}"
                    , "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // If unpowered or not communicating (comms cable possibly disconnected) SelfTest throws a
                // Keysight.CommandExpert.InstrumentAbstraction.CommunicationException exception,
                // which requires an apparently unavailable Keysight library to explicitly catch.
                return DIAGNOSTICS_RESULTS.FAIL;
            }
            return result ? DIAGNOSTICS_RESULTS.FAIL : DIAGNOSTICS_RESULTS.PASS; // Ag34401 returns 0 for passed, 1 for fail, opposite of C#'s Convert.ToBoolean(Int32).
        }

        public MM_34401A_SCPI_NET(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.MULTI_METER;
            TerminalsSetRear();
        }

        public Boolean DelayAutoIs() {
            SCPI.TRIGger.DELay.AUTO.Query(out Boolean state);
            return state;
        }

        public Double Get(PROPERTY property) {
            // SCPI FORMAT:DATA(ASCii/REAL) command unavailable on KS 34461A.
            switch (property) {
                case PROPERTY.AmperageAC:
                    SCPI.MEASure.CURRent.AC.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double acCurrent);
                    return acCurrent;
                case PROPERTY.AmperageDC:
                    SCPI.MEASure.CURRent.DC.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double dcCurrent);
                    return dcCurrent;
                case PROPERTY.Continuity:
                    SCPI.MEASure.CONTinuity.Query(out Double continuity);
                    return continuity;
                case PROPERTY.Frequency:
                    SCPI.MEASure.FREQuency.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double frequency);
                    return frequency;
                case PROPERTY.Fresistance:
                    SCPI.MEASure.FRESistance.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double fresistance);
                    return fresistance;
                case PROPERTY.Period:
                    SCPI.MEASure.PERiod.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double period);
                    return period;
                case PROPERTY.Resistance:
                    SCPI.MEASure.RESistance.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double resistance);
                    return resistance;
                case PROPERTY.VoltageAC:
                    SCPI.MEASure.VOLTage.AC.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double acVoltage);
                    return acVoltage;
                case PROPERTY.VoltageDC:
                    SCPI.MEASure.VOLTage.DC.Query($"{MMD.DEF}", $"{MMD.DEF}", out Double dcVoltage);
                    return dcVoltage;
                case PROPERTY.VoltageDiodic:
                    SCPI.MEASure.DIODe.Query(out Double diodeVoltage);
                    return diodeVoltage;
                default:
                    throw new NotImplementedException($"Unimplemented Enum item; switch/case must support all items in enum '{String.Join(",", Enum.GetNames(typeof(PROPERTY)))}'.");
            }
        }

        public void TerminalsSetRear() {
            if (TerminalsGet() == TERMINALS.Front) _ = MessageBox.Show("Please depress Keysight 34401A Front/Rear button.", "Paused, click OK to continue.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SCPI.TRIGger.DELay.AUTO.Command(true);
        }

        public TERMINALS TerminalsGet() {
            SCPI.ROUTe.TERMinals.Query(out String terminals);
            return String.Equals(terminals, "REAR") ? TERMINALS.Rear : TERMINALS.Front;
        }
    }
}