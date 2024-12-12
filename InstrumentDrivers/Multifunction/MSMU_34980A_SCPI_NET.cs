using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Agilent.CommandExpert.ScpiNet.Ag34980_2_43;
using ABT.TestExec.Lib.InstrumentDrivers.Interfaces;

namespace ABT.TestExec.Lib.InstrumentDrivers.Multifunction {

    public class MSMU_34980A_SCPI_NET : Ag34980, IInstruments, IRelays {
        public enum ABUS { A1, A2, A3, A4, AI };
        public enum COMS { C1 = 1, C2 = 2 }
        public enum SLOTS { S1 = 1, S2 = 2, S3 = 3, S4 = 4, S5 = 5, S6 = 6, S7 = 7, S8 = 8 }
        public enum TEMPERATURE_UNITS { C, F, K }
        public enum RELAY_STATES { opened, CLOSED }

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
            return (SELF_TEST_RESULTS)result; // Ag34980 returns 0 for passed, 1 for fail.
        }

        public class DiagnosticsResult {
            public readonly String Label;
            public readonly String Message;
            public Boolean Passed;
            public DiagnosticsResult(String label, String message) { Label = label; Message = message; }
            public DiagnosticsResult(String label, String message, Boolean passed) { Label = label; Message = message; Passed = passed; }
        }

        public (Boolean Result, List<DiagnosticsResult> Details) Diagnostics_34921A(SLOTS slot, Double Ω) {
            List<DiagnosticsResult> results = new List<DiagnosticsResult>();

            Dictionary<ABUS, Dictionary<COMS, String>> ABus = new Dictionary<ABUS, Dictionary<COMS, String>>() {
                {ABUS.A1, new Dictionary<COMS, String>() {{COMS.C1, "911"}, {COMS.C2, "921"}}},
                {ABUS.A2, new Dictionary<COMS, String>() {{COMS.C1, "912"}, {COMS.C2, "922"}}},
                {ABUS.A3, new Dictionary<COMS, String>() {{COMS.C1, "913"}, {COMS.C2, "923"}}},
                {ABUS.A4, new Dictionary<COMS, String>() {{COMS.C1, "914"}, {COMS.C2, "924"}}}
            };

            ResetClear();
            SCPI.ROUTe.OPEN.ALL.Command(null);
            SCPI.INSTrument.DMM.STATe.Command(true);
            SCPI.INSTrument.DMM.CONNect.Command();
            SCPI.SENSe.RESistance.RESolution.Command("MAXimum");

            Boolean passedChannel;
            Boolean passed_34921A = true;
            _ = MessageBox.Show($"Please connect both 34921A loopback connectors to SLOT {slot}.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SCPI.ROUTe.CLOSe.Command($"@{slot}{ABus[ABUS.A1][COMS.C1]},{slot}{ABus[ABUS.A2][COMS.C1]}");
            for (Int32 i = 1; i < 41; i++) { // Banks 1 & 2.
                String channel = $"@{slot}{i:D3}";
                SCPI.ROUTe.CLOSe.Command(channel);
                SCPI.MEASure.SCALar.RESistance.Query(25D, "MAXimum", out Double[] resistance);
                passedChannel = (0 <= resistance[0] && resistance[0] <= Ω);
                passed_34921A &= passedChannel;
                results.Add(new DiagnosticsResult(label: $"Channel {channel}: ", message: $"{Math.Round(resistance[0], 3, MidpointRounding.ToEven)}Ω", passed: passedChannel));
                SCPI.ROUTe.OPEN.Command(channel);
                if (i == 21) { // Bank 2.
                    SCPI.ROUTe.OPEN.Command($"@{slot}{ABus[ABUS.A1][COMS.C1]},{slot}{ABus[ABUS.A2][COMS.C1]}");
                    SCPI.ROUTe.CLOSe.Command($"@{slot}{ABus[ABUS.A1][COMS.C2]},{slot}{ABus[ABUS.A2][COMS.C2]}");
                }
            }
            SCPI.ROUTe.OPEN.Command($"@{slot}{ABus[ABUS.A1][COMS.C2]},{slot}{ABus[ABUS.A2][COMS.C2]}");

            _ = MessageBox.Show($"Please disconnect both 34921A loopback connectors from SLOT {slot}.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return (Result: passed_34921A, Details: results);
        }
        public String Diagnostics_34932A(SLOTS slot) { return String.Empty; }
        public String Diagnostics_34938A(SLOTS slot) { return String.Empty; }
        public String Diagnostics_34939A(SLOTS slot) { return String.Empty; }
        public String Diagnostics_34925A(SLOTS slot) { return String.Empty; }

        public void OpenAll() { SCPI.ROUTe.OPEN.ALL.Command(null); }

        public MSMU_34980A_SCPI_NET(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.MULTI_FUNCTION;
            DateTime now = DateTime.Now;
            SCPI.SYSTem.DATE.Command(now.Year, now.Month, now.Day);
            SCPI.SYSTem.TIME.Command(now.Hour, now.Minute, Convert.ToDouble(now.Second));
            SCPI.UNIT.TEMPerature.Command($"{TEMPERATURE_UNITS.F}");
        }

        public Boolean InstrumentDMM_Installed() {
            SCPI.INSTrument.DMM.INSTalled.Query(out Boolean installed);
            return installed;
        }
        public STATES InstrumentDMM_Get() {
            SCPI.INSTrument.DMM.STATe.Query(out Boolean mode);
            return mode ? STATES.ON : STATES.off;
        }
        public void InstrumentDMM_Set(STATES State) { SCPI.INSTrument.DMM.STATe.Command(State == STATES.ON); }
        public (Int32 Min, Int32 Max) ModuleChannels(SLOTS Slot) {
            switch (SystemType(Slot)) {
                case "34921A": return (Min: 1, Max: 44);
                case "34939A": return (Min: 1, Max: 68);
                case "34952A": return (Min: 1, Max: 7);
                default: throw new NotImplementedException($"Module Type '{SystemType(Slot)}' unimplemented.");
            }
        }
        public void RouteCloseExclusive(String Channels) {
            ValidateChannelS(Channels);
            SCPI.ROUTe.CLOSe.EXCLusive.Command($"({Channels})");
        }
        public Boolean RouteGet(String Channels, RELAY_STATES State) {
            ValidateChannelS(Channels);
            Boolean[] states;
            if (State is RELAY_STATES.opened) SCPI.ROUTe.OPEN.Query(Channels, out states);
            else SCPI.ROUTe.CLOSe.Query(Channels, out states);
            List<Boolean> lb = states.ToList();
            return lb.TrueForAll(b => b == true);
        }
        public void RouteSet(String Channels, RELAY_STATES State) {
            ValidateChannelS(Channels);
            if (State is RELAY_STATES.opened) SCPI.ROUTe.OPEN.Command(Channels);
            else SCPI.ROUTe.CLOSe.Command(Channels);
        }

        public String SystemDescriptionLong(SLOTS Slot) {
            SCPI.SYSTem.CDEScription.LONG.Query((Int32)Slot, out String description);
            return description;
        }
        public Double SystemModuleTemperature(SLOTS Slot) {
            SCPI.SYSTem.MODule.TEMPerature.Query("TRANsducer", (Int32)Slot, out Double temperature);
            return temperature;
        }

        public String SystemType(SLOTS Slot) {
            SCPI.SYSTem.CTYPe.Query((Int32)Slot, out String identity);
            return identity.Split(',')[(Int32)Generic.SCPI_NET.IDN_FIELDS.Model];
        }
        public TEMPERATURE_UNITS UnitsGet() {
            SCPI.UNIT.TEMPerature.Query(out String[] units);
            return (TEMPERATURE_UNITS)Enum.Parse(typeof(TEMPERATURE_UNITS), String.Join("", units).Replace("[", "").Replace("]", ""));
        }

        public void ValidateChannelS(String Channels) {
            // TODO: Debug.Print($"ChannelS: '{Channels}'.");
            if (!Regex.IsMatch(Channels, @"^@\d{4}((,|:)\d{4})*$")) {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Invalid syntax for Channels '{Channels}'.");
                sb.AppendLine(" - Must be in form of 1 or more discrete channels and/or ranges preceded by '@'.");
                sb.AppendLine(" - Channel:  '@####':       Discrete channels must be separated by commas; '@1001,1002'.");
                sb.AppendLine(" - Range:    '@####:####':  Channel ranges must be separated by colons; '@1001:1002'.");
                sb.AppendLine(" - Examples: '@1001', '@1001,2001,2005', '@1001,2001:2005' & '@1001,2001:2005,2017,3001:3015,3017' all valid.");
                sb.AppendLine();
                sb.AppendLine("Caveats:");
                sb.AppendLine(" - Whitespace not permitted; '@1001, 1005', '@1001 ,1005' '& '@1001: 1005' all invalid.");
                sb.AppendLine(" - Range cannot include ABus channels, denoted as #9##.  Thus range '@1001:1902' invalid, but discretes '@1001,1902' valid.");
                sb.AppendLine(" - Fist & only first channel begins with '@'.  Thus '1001,2001' & '@1001,@2001' both invalid.");
                throw new ArgumentException(sb.ToString());
                // https://regex101.com/.
            }
            if (Regex.IsMatch(Channels, @":\d{4}:")) throw new ArgumentException($"Invalid syntax for Channels '{Channels}'.  Invalid range ':####:'.");
            Channels = Channels.Replace("@", String.Empty);

            if (Channels.Length == 4) {
                ValidateChannel(Channels);
                return;
            }

            String[] channelsOrRanges = Channels.Split(new Char[] { ',' }, StringSplitOptions.None);
            foreach (String channelOrRange in channelsOrRanges) {
                if (Regex.IsMatch(channelOrRange, ":")) ValidateRange(channelOrRange);
                else ValidateChannel(channelOrRange);
            }
        }
        public void ValidateChannel(String Channel) {
            // TODO: Debug.Print($"Channel: '{Channel}'.");
            Int32 slotNumber = Int32.Parse(Channel.Substring(0, 2));
            // TODO: Debug.Print($"Slot Number: '{slotNumber}'.");
            if (!Enum.IsDefined(typeof(SLOTS), (SLOTS)slotNumber)) throw new ArgumentException($"Channel '{Channel}' must have valid integer Slot in interval [{(Int32)SLOTS.S1}..{(Int32)SLOTS.S8}].");
            Int32 channel = Int32.Parse(Channel.Substring(2));
            // TODO: Debug.Print($"Channel: '{channel}'.");
            (Int32 min, Int32 max) = ModuleChannels((SLOTS)slotNumber);
            // TODO: Debug.Print($"ModuleChannels min '{min}' & '{max}'.");
            if (channel < min || max < channel) throw new ArgumentException($"Channel '{Channel}' must have valid integer Channel in interval [{min:D3}..{max:D3}].");
        }
        public void ValidateRange(String Range) {
            // TODO: Debug.Print($"Range: '{Range}'.");
            String[] channels = Range.Split(new Char[] { ':' }, StringSplitOptions.None);
            // TODO: for (Int32 i=0; i < channels.Length; i++) Debug.Print($"channels[{i}]='{channels[i]}'.");
            if (channels[0][1].Equals('9') || channels[1][1].Equals('9')) throw new ArgumentException($"Channel Range '{Range}' cannot include ABus Channel #9##.");
            ValidateChannel(channels[0]);
            ValidateChannel(channels[1]);
            if (Convert.ToInt32(channels[0]) >= Convert.ToInt32(channels[1])) throw new ArgumentException($"Channel Range '{Range}' start Channel '{channels[0]}' must be < end Channel '{channels[1]}'.");
        }
    }
}