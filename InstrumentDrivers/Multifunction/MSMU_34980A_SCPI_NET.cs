using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Agilent.CommandExpert.ScpiNet.Ag34980_2_43;
using ABT.TestExec.Lib.InstrumentDrivers.Interfaces;

namespace ABT.TestExec.Lib.InstrumentDrivers.Multifunction {

    public class MSMU_34980A_SCPI_NET : Ag34980, IInstruments, IRelays, IDiagnostics {
        public enum ABUS { A1, A2, A3, A4, AI };
        public enum BANKS_34921A { B1, B2 }
        public enum COMS { C1 = 1, C2 = 2 }
        public enum MODULES_34980A { M34921A, M34932A, M34938A, M34939A, M34952A }
        // NOTE: Update MODULES & Modules as necessary, along with Diagnostics region.
        public static Dictionary<MODULES_34980A, String> Modules = new Dictionary<MODULES_34980A, String>() {
            { MODULES_34980A.M34921A, "34921A" },
            { MODULES_34980A.M34932A, "34932A" },
            { MODULES_34980A.M34938A, "34938A" },
            { MODULES_34980A.M34939A, "34939A" },
            { MODULES_34980A.M34952A, "34952A" },
        };
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

        #region Diagnostics // NOTE: Update MODULES & Modules as necessary, along with Diagnostics region.
        // TODO: Complete Diagnostics for M34932A, M34938A, M34939A & M34952A modules.
        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics() {
            if (SelfTests() is SELF_TEST_RESULTS.FAIL) return (false, new List<DiagnosticsResult>() { new DiagnosticsResult(label: "34980A Diagnostics():", message: "SelfTests() failed, aborted.", passed: false) });

            (Boolean summary, List<DiagnosticsResult> details) result_Slot;
            (Boolean Summary, List<DiagnosticsResult> Details) result_34980A = (true, new List<DiagnosticsResult>());
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) {
                switch(SystemType(slot)) {
                    case String s when s == Modules[MODULES_34980A.M34921A]:
                        result_Slot = Diagnostic_34921A(slot, Ω: 3);
                        break;
                    case String s when s == Modules[MODULES_34980A.M34932A]:
                        result_Slot = Diagnostic_34932A(slot, Ω: 3);
                        break;
                    case String s when s == Modules[MODULES_34980A.M34938A]:
                        result_Slot = Diagnostic_34938A(slot, Ω: 3);
                        break;
                    case String s when s == Modules[MODULES_34980A.M34939A]:
                        result_Slot = Diagnostic_34939A(slot, Ω: 3);
                        break;
                    case String s when s == Modules[MODULES_34980A.M34952A]:
                        result_Slot = Diagnostic_34952A(slot, Ω: 3);
                        break;
                    default:
                        throw new NotImplementedException(
                            $"Diagnostics test for module '{SystemType(slot)}' unimplemented!{Environment.NewLine}{Environment.NewLine}" +
                            $"Instrument Type : '{InstrumentType}'.{Environment.NewLine}" +
                            $"Address         : '{Address}'.{Environment.NewLine}" +
                            $"Detail          :  '{Detail}'.{Environment.NewLine}");
                }
                result_34980A.Summary &= result_Slot.summary;
                result_34980A.Details.AddRange(result_Slot.details);
            }
            return result_34980A;
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34921As(Double Ω) {
            ResetClear();
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules[MODULES_34980A.M34921A]) Results.Add(slot, Diagnostic_34921A(slot, Ω));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34921A(SLOTS slot, Double Ω) {
            // TODO: Add current measurement tests for 34921A relays 931, 041, 042, 043 & 044.  Will require an external current source. 
            SCPI.ROUTe.OPEN.ALL.Command(null);
            SCPI.INSTrument.DMM.STATe.Command(true);
            SCPI.INSTrument.DMM.CONNect.Command();
            SCPI.SENSe.RESistance.RESolution.Command("MAXimum");

            List<DiagnosticsResult> results = new List<DiagnosticsResult>();
            Boolean passed_34921A = true;

            _ = MessageBox.Show($"Please connect a 34921A diagnostic connector to 34980A SLOT {slot} Bank 1.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MeasureAndRecord_34921A(slot, BANKS_34921A.B1, channels: new List<String> { "911" }, Ω, ref passed_34921A, ref results);
            MeasureAndRecord_34921A(slot, BANKS_34921A.B1, channels: new List<String> { "921", "912", "922" }, Ω, ref passed_34921A, ref results);
            MeasureAndRecord_34921A(slot, BANKS_34921A.B1, channels: new List<String> { "921", "913", "923" }, Ω, ref passed_34921A, ref results);
            MeasureAndRecord_34921A(slot, BANKS_34921A.B1, channels: new List<String> { "921", "914", "924" }, Ω, ref passed_34921A, ref results);

            SCPI.ROUTe.CLOSe.Command($"@{slot}911"); // DMM Measure.
            for (Int32 i = 1; i < 21; i++) MeasureAndRecord_34921A(channels: $"@{slot}{i:D3}", Ω,  ref passed_34921A, ref results); // Bank 1.
            SCPI.ROUTe.OPEN.Command($"@{slot}911");

            _ = MessageBox.Show($"Please move 34921A diagnostic connector from 34980A SLOT {slot} Bank 1 to Bank 2.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MeasureAndRecord_34921A(slot, BANKS_34921A.B2, channels: new List<String> { "921" }, Ω, ref passed_34921A, ref results);
            MeasureAndRecord_34921A(slot, BANKS_34921A.B2, channels: new List<String> { "911", "912", "922" }, Ω, ref passed_34921A, ref results);
            MeasureAndRecord_34921A(slot, BANKS_34921A.B2, channels: new List<String> { "911", "913", "923" }, Ω, ref passed_34921A, ref results);
            MeasureAndRecord_34921A(slot, BANKS_34921A.B2, channels: new List<String> { "911", "914", "924" }, Ω, ref passed_34921A, ref results);

            SCPI.ROUTe.CLOSe.Command($"@{slot}921"); // DMM Measure.
            for (Int32 i = 21; i < 41; i++) MeasureAndRecord_34921A(channels: $"@{slot}{i:D3}", Ω, ref passed_34921A, ref results); // Bank 2.
            SCPI.ROUTe.OPEN.Command($"@{slot}921");
            SCPI.INSTrument.DMM.DISConnect.Command();

            _ = MessageBox.Show($"Please disconnect 34921A diagnostic connector from 34980A SLOT {slot} Bank 2.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return (Summary: passed_34921A, Details: results);
        }

        private void MeasureAndRecord_34921A(SLOTS slot, BANKS_34921A bank, List<String> channels, Double Ω, ref Boolean passed_34921A, ref List<DiagnosticsResult> results) {
            SCPI.ROUTe.OPEN.Command($"@{slot}001:{slot}040"); // B1 & B2.
            if (bank is BANKS_34921A.B1) SCPI.ROUTe.CLOSe.Command($"@{slot}001:{slot}020"); // B1
            if (bank is BANKS_34921A.B2) SCPI.ROUTe.CLOSe.Command($"@{slot}021:{slot}040"); // B2
            MeasureAndRecord_34921A($"@{slot}" + String.Join($",{slot}", channels), Ω, ref passed_34921A, ref results);
            SCPI.ROUTe.OPEN.Command($"@{slot}001:{slot}040"); // B1 & B2.
        }

        private void MeasureAndRecord_34921A(String channels, Double Ω,  ref Boolean passed_34921A, ref List<DiagnosticsResult> results) {
            SCPI.ROUTe.CLOSe.Command(channels);
            SCPI.MEASure.SCALar.RESistance.Query(25D, "MAXimum", out Double[] resistance);
            SCPI.ROUTe.OPEN.Command(channels);
            Boolean passed_Ω = (0 <= resistance[0] && resistance[0] <= Ω);
            passed_34921A &= passed_Ω;
            results.Add(new DiagnosticsResult(label: $"Channel(s) {channels}: ", message: $"{Math.Round(resistance[0], 3, MidpointRounding.ToEven)}Ω", passed: passed_Ω));
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34932A(SLOTS slot, Double Ω) {
            return (false, new List<DiagnosticsResult>() { new DiagnosticsResult(label: "", message: "", passed: false) });
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34932As(Double Ω) {
            ResetClear();
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules[MODULES_34980A.M34932A]) Results.Add(slot, Diagnostic_34932A(slot, Ω));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34938A(SLOTS slot, Double Ω) {
            return (false, new List<DiagnosticsResult>() { new DiagnosticsResult(label: "", message: "", passed: false) });
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34938As(Double Ω) {
            ResetClear();
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules[MODULES_34980A.M34938A]) Results.Add(slot, Diagnostic_34938A(slot, Ω));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34939A(SLOTS slot, Double Ω) {
            return (false, new List<DiagnosticsResult>() { new DiagnosticsResult(label: "", message: "", passed: false) });
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34939As(Double Ω) {
            ResetClear();
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules[MODULES_34980A.M34932A]) Results.Add(slot, Diagnostic_34939A(slot, Ω));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34952A(SLOTS slot, Double Ω) {
            return (false, new List<DiagnosticsResult>() { new DiagnosticsResult(label: "", message: "", passed: false) });
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34952As(Double Ω) {
            ResetClear();
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules[MODULES_34980A.M34932A]) Results.Add(slot, Diagnostic_34952A(slot, Ω));
            return Results;
        }
        #endregion Diagnostics

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