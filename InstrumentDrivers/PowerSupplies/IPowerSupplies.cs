using System;

namespace ABT.Test.Lib.InstrumentDrivers.PowerSupplies {
    public enum AC { Amps, Volts }
    public enum DC { Amps, Volts }
    public enum OUTPUTS2 { OUTput1, OUTput2 };
    public enum OUTPUTS3 { OUTput1, OUTput2, OUTput3 };
    public enum MMD { MINimum, MAXimum, DEFault }

    public interface IPowerSupplyOutputs1 {
        STATES StateGet();
        void StateSet(STATES State);
        (Double AmpsDC, Double VoltsDC) Get(DC DC);
        void Set(Double Volts, Double Amps, Double OVP, STATES State);
    }

    public interface IPowerSupplyOutputs2ExceptStateSet {
        STATES StateGet(OUTPUTS2 Output);
        (Double AmpsDC, Double VoltsDC) Get(OUTPUTS2 Output, DC DC);
        void Set(OUTPUTS2 Output, Double Volts, Double Amps, Double OVP, STATES State);
    }

    public interface IPowerSupplyOutputs2 : IPowerSupplyOutputs2ExceptStateSet {
        void StateSet(OUTPUTS2 Output, STATES State);
    }

    public interface IPowerSupplyOutputs3 {
        STATES StateGet(OUTPUTS3 Output);
        void StateSet(OUTPUTS3 Output, STATES State);
        (Double AmpsDC, Double VoltsDC) Get(OUTPUTS3 Output, DC DC);
        void Set(OUTPUTS3 Output, Double Volts, Double Amps, Double OVP, STATES State);
    }

    public interface IPowerSupplyE3649A: IPowerSupplyOutputs2ExceptStateSet {
        // NOTE: Most multi-output supplies like the E3649A permit individual control of outputs,
        // but the E3649A does not; all supplies are set to the same STATE, off or ON.
        void StateSet(STATES State);
    }
}
