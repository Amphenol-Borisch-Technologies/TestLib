using ABT.Test.Lib.InstrumentDrivers.Generic;
using System;
using Tektronix.Tkdpo2k3k4k.Interop;

namespace ABT.Test.Lib.InstrumentDrivers.Oscilloscopes {
    public class MSO_3014_IVI_COM : Tkdpo2k3k4kClass, IInstruments {
        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ReInitialize() {
            Utility.Reset();
        }

        public Boolean ReInitialized() {
            return true;
        }
        
        public SELF_TEST_RESULTS SelfTest() {
            Int32 TestResult = 0;
            String TestMessage = String.Empty;
            UtilityEx.SelfTest(ref TestResult, ref TestMessage);
            return TestResult == 0 ? SELF_TEST_RESULTS.PASS : SELF_TEST_RESULTS.FAIL;
        }

        public MSO_3014_IVI_COM(String Address, String Detail) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.OSCILLOSCOPE;
            Initialize(ResourceName: Address, IdQuery: false, Reset: false, OptionString: String.Empty);
        }
    }
}
