using System;
using Agilent.CommandExpert.ScpiNet.AgE363x_1_7;

namespace ABT.Test.Lib.InstrumentDrivers.PowerSupplies  {
    public class PS_E3634A_SCPI : AgE363x, IInstruments, IPowerSupplyOutputs1 {
        public enum RANGE { P25V, P50V }

        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ReInitialize() {
            SCPI.RST.Command();
            SCPI.CLS.Command();
        }

        public Boolean ReInitialized() {
            return StateGet() == STATES.off;
        }
        
        public SELF_TEST_RESULTS SelfTest() {
            SCPI.TST.Query(out Int32 result);
            return (SELF_TEST_RESULTS)result;
        }

        public PS_E3634A_SCPI(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.POWER_SUPPLY;
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
    }
}
