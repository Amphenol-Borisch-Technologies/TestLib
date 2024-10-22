using System;

namespace ABT.Test.Lib.InstrumentDrivers {
    public enum SELF_TEST_RESULTS { FAIL=0, PASS=1 }

    public interface IInstruments {
        String Address { get; } // NOTE: Store in instrument objects for easy error reporting of addresses.  Not easily gotten otherwise.
        String Detail { get; }  // NOTE: Store in instrument objects for easy error reporting of detailed descriptions, similar but more useful than SCPI's *IDN query.
        INSTRUMENT_TYPES InstrumentType { get; }
        void ReInitialize();    // NOTE: After each test run, reinitialize instrument.  Typically performs SCPI's *RST & *CLS commands.
        Boolean ReInitialized();
        SELF_TEST_RESULTS SelfTest();
    }
}
