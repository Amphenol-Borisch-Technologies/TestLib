using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Agilent.CommandExpert.ScpiNet.Ag34980_2_43;
using ABT.Test.TestLib.InstrumentDrivers.Interfaces;
using ABT.Test.TestLib.InstrumentDrivers.Generic;

namespace ABT.Test.TestLib.InstrumentDrivers.Multifunction {

    public class MSMU_34980A_SCPI_NET : Ag34980, IInstruments, IRelays, IDiagnostics {
        public enum SLOTS { S1 = 1, S2 = 2, S3 = 3, S4 = 4, S5 = 5, S6 = 6, S7 = 7, S8 = 8 }
        public struct Modules {
            public const String M34921A = "34921A";
            public const String M34932A = "34932A";
            public const String M34938A = "34938A";
            public const String M34939A = "34939A";
            public const String M34952A = "34952A";
        }
        public enum TEMPERATURE_UNITS { C, F, K }
        public enum RELAY_STATES { opened, CLOSED }

        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }
        private readonly String _34980A;

        public void ResetClear() {
            SCPI.RST.Command();
            SCPI.CLS.Command();
        }

        public SELF_TEST_RESULTS SelfTests() {
            if (DialogResult.Cancel == MessageBox.Show($"Please disconnect _all_ connectors from {_34980A} Slots.", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                Data.CTS_Cancel.Cancel();
                Data.CT_Cancel.ThrowIfCancellationRequested();
            };

            Int32 result;
            try {
                SCPI.TST.Query(out result);
            } catch (Exception e) {
                Instruments.SelfTestFailure(this, e);
                return SELF_TEST_RESULTS.FAIL;
            }
            return (SELF_TEST_RESULTS)result; // Ag34980 returns 0 for passed, 1 for fail.
        }

        public void OpenAll() { SCPI.ROUTe.OPEN.ALL.Command(null); }

        #region Diagnostics // NOTE: Update MODULES & Modules as necessary, along with Diagnostics region.
        // TODO: Eventually; complete Diagnostics for M34932A, M34938A, M34939A & M34952A modules.

        public class DiagnosticParameter_34980A {
            public (Double Ω_closed, Double Ω_open) M34921A { get; set; } = (3, 1E6);
            public (Double Ω_closed, Double Ω_open) M34932A { get; set; } = (3, 1E6);
            public (Double Ω_closed, Double Ω_open) M34938A { get; set; } = (3, 1E6);
            public (Double Ω_closed, Double Ω_open) M34939A { get; set; } = (3, 1E6);

            public DiagnosticParameter_34980A() { }
            public DiagnosticParameter_34980A(Double Ω_closed, Double Ω_open) { M34921A = M34932A = M34938A = M34939A = (Ω_closed, Ω_open); }
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostics(Object o = null) {
            ResetClear();
            Boolean passed = SelfTests() is SELF_TEST_RESULTS.PASS;
            (Boolean Summary, List<DiagnosticsResult> Details) result_34980A = (passed, new List<DiagnosticsResult>()  { new DiagnosticsResult(Label: "SelfTest", Message: String.Empty, Event: passed ? EVENTS.PASS : EVENTS.FAIL) });
            if (passed) {
                (Boolean summary, List<DiagnosticsResult> details) result_Slot;
                DiagnosticParameter_34980A DP = (o is DiagnosticParameter_34980A dp) ? dp : new DiagnosticParameter_34980A();

                foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) {
                    Data.CT_EmergencyStop.ThrowIfCancellationRequested();
                    Data.CT_Cancel.ThrowIfCancellationRequested();

                    switch (SystemType(slot)) {
                        case Modules.M34921A:
                            result_Slot = Diagnostic_34921A(slot, DP.M34921A);
                            break;
                        case Modules.M34932A:
                            result_Slot = Diagnostic_34932A(slot, DP.M34932A);
                            break;
                        case Modules.M34938A:
                            result_Slot = Diagnostic_34938A(slot, DP.M34938A);
                            break;
                        case Modules.M34939A:
                            result_Slot = Diagnostic_34939A(slot, DP.M34939A);
                            break;
                        case Modules.M34952A:
                            result_Slot = Diagnostic_34952A(slot);
                            break;
                        case "0":
                            result_Slot = (true, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: $"Slot '{slot}':", Message: "Empty.", Event: EVENTS.INFORMATION) });
                            break;
                        default:
                            const Int32 PR = 12;
                            throw new NotImplementedException(
                                $"Diagnostic test for module '{SystemType(slot)}' unimplemented!{Environment.NewLine}{Environment.NewLine}" +
                                $"{nameof(SystemDescriptionLong)}".PadRight(PR) + $": '{SystemDescriptionLong(slot)}'.{Environment.NewLine}" +
                                $"{nameof(Address)}".PadRight(PR) + $": '{Address}'.{Environment.NewLine}" +
                                $"{nameof(Detail)}".PadRight(PR) + $": '{Detail}'.{Environment.NewLine}");
                    }
                    result_34980A.Summary &= result_Slot.summary;
                    result_34980A.Details.AddRange(result_Slot.details);
                }
            }
            return result_34980A;
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34921As((Double Ω_closed, Double Ω_open) M34921A) {
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules.M34921A) Results.Add(slot, Diagnostic_34921A(slot, M34921A));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34921A(SLOTS Slot, (Double Ω_closed, Double Ω_open) M34921A) {
            // TODO: Eventually; add current measurement tests for 34921A relays 931, 041, 042, 043 & 044.  Will require an external current source.
            SCPI.ROUTe.OPEN.ALL.Command(null);
            SCPI.INSTrument.DMM.STATe.Command(true);
            SCPI.INSTrument.DMM.CONNect.Command();
            SCPI.SENSe.RESistance.RESolution.Command("MAXimum");

            List<DiagnosticsResult> results = new List<DiagnosticsResult>();
            Boolean passed_34921A = true;
            String s = ((Int32)Slot).ToString("D1");

            if (DialogResult.Cancel == MessageBox.Show($"Please connect {Modules.M34921A} diagnostic connector to {_34980A} SLOT {s} Banks 1 & 2.", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                Data.CTS_Cancel.Cancel();
                Data.CT_Cancel.ThrowIfCancellationRequested();
            };
            Data.CT_EmergencyStop.ThrowIfCancellationRequested();

            String D = nameof(Diagnostic_34921A);
            CloseMeasureOpenRecord(D, kelvin: false, closed: false, String.Empty, M34921A, ref passed_34921A, ref results);

            CloseMeasureOpenRecord(D, kelvin: false, closed: false, $"@{s}911", M34921A, ref passed_34921A, ref results);
            CloseMeasureOpenRecord(D, kelvin: false, closed: false, $"@{s}911,{s}912,{s}922", M34921A, ref passed_34921A, ref results);
            CloseMeasureOpenRecord(D, kelvin: false, closed: false, $"@{s}911,{s}913,{s}923", M34921A, ref passed_34921A, ref results);
            CloseMeasureOpenRecord(D, kelvin: false, closed: false, $"@{s}911,{s}914,{s}924", M34921A, ref passed_34921A, ref results);

            CloseMeasureOpenRecord(D, kelvin: false, closed: false, $"@{s}921", M34921A, ref passed_34921A, ref results);
            CloseMeasureOpenRecord(D, kelvin: false, closed: false, $"@{s}921,{s}912,{s}922", M34921A, ref passed_34921A, ref results);
            CloseMeasureOpenRecord(D, kelvin: false, closed: false, $"@{s}921,{s}913,{s}923", M34921A, ref passed_34921A, ref results);
            CloseMeasureOpenRecord(D, kelvin: false, closed: false, $"@{s}921,{s}914,{s}924", M34921A, ref passed_34921A, ref results);

            SCPI.ROUTe.CLOSe.Command($"@{s}001:{s}020"); // Bank 1 all relays connected to Bank 1 diagnostic shorting connector.
            CloseMeasureOpenRecord(D, kelvin: false, closed: true, $"@{s}911", M34921A, ref passed_34921A, ref results);               // ABus1 COM1 directly connected to all Bank 1 relays and thus diagnostic shorting connector.
            CloseMeasureOpenRecord(D, kelvin: false, closed: true, $"@{s}921,{s}912,{s}922", M34921A, ref passed_34921A, ref results); // ABus1 COM2 indirectly connected through ABus2, to test ABus2.
            CloseMeasureOpenRecord(D, kelvin: false, closed: true, $"@{s}921,{s}913,{s}923", M34921A, ref passed_34921A, ref results); // ABus1 COM2 indirectly connected through ABus3, to test ABus3.
            CloseMeasureOpenRecord(D, kelvin: false, closed: true, $"@{s}921,{s}914,{s}924", M34921A, ref passed_34921A, ref results); // ABus1 COM2 indirectly connected through ABus4, to test ABus4.
            SCPI.ROUTe.OPEN.Command($"@{s}001:{s}020"); // Reference 'Keysight 34921A-34925A Low Frequency Multiplexer Modules', '34921A Simplified Schematic'.

            SCPI.ROUTe.CLOSe.Command($"@{s}911"); // DMM Measure.
            for (Int32 i = 1; i < 21; i++) CloseMeasureOpenRecord(D, kelvin: false, closed: true, $"@{s}{i:D3}", M34921A, ref passed_34921A, ref results); // Bank 1 individual relays.
            SCPI.ROUTe.OPEN.Command($"@{s}911");

            SCPI.ROUTe.CLOSe.Command($"@{s}021:{s}040"); // Bank 2 all relays connected to Bank 2 diagnostic shorting connector.
            CloseMeasureOpenRecord(D, kelvin: false, closed: true, $"@{s}921", M34921A, ref passed_34921A, ref results);               // ABus1 COM2 directly connected to all Bank 2 relays and thus diagnostic shorting connector.
            CloseMeasureOpenRecord(D, kelvin: false, closed: true, $"@{s}911,{s}912,{s}922", M34921A, ref passed_34921A, ref results); // ABus1 COM1 indirectly connected through ABus2, to test ABus2.
            CloseMeasureOpenRecord(D, kelvin: false, closed: true, $"@{s}911,{s}913,{s}923", M34921A, ref passed_34921A, ref results); // ABus1 COM1 indirectly connected through ABus3, to test ABus3.
            CloseMeasureOpenRecord(D, kelvin: false, closed: true, $"@{s}911,{s}914,{s}924", M34921A, ref passed_34921A, ref results); // ABus1 COM1 indirectly connected through ABus4, to test ABus4.
            SCPI.ROUTe.OPEN.Command($"@{s}021:{s}040"); // Reference 'Keysight 34921A-34925A Low Frequency Multiplexer Modules', '34921A Simplified Schematic'.

            SCPI.ROUTe.CLOSe.Command($"@{s}921"); // DMM Measure.
            for (Int32 i = 21; i < 41; i++) CloseMeasureOpenRecord(D, kelvin: false, closed: true, $"@{s}{i:D3}", M34921A, ref passed_34921A, ref results); // Bank 2 individual relays.
            SCPI.ROUTe.OPEN.Command($"@{s}921");
            SCPI.INSTrument.DMM.DISConnect.Command();

            Data.CT_EmergencyStop.ThrowIfCancellationRequested();
            Data.CT_Cancel.ThrowIfCancellationRequested();

            if (DialogResult.Cancel == MessageBox.Show($"Please disconnect {Modules.M34921A} diagnostic connector from {_34980A} SLOT {s} Banks 1 & 2.", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                Data.CTS_Cancel.Cancel();
                Data.CT_Cancel.ThrowIfCancellationRequested();
            };
            return (Summary: passed_34921A, Details: results);
        }

        private void CloseMeasureOpenRecord(String diagnostic, Boolean kelvin, Boolean closed, String channels, (Double Ω_closed, Double Ω_open) Limits, ref Boolean passed, ref List<DiagnosticsResult> results) {
            if (!String.Equals(channels, String.Empty)) SCPI.ROUTe.CLOSe.Command(channels);
            Double[] resistance;
            if (kelvin) SCPI.MEASure.SCALar.FRESistance.Query(25D, "MAXimum", out resistance);
            else SCPI.MEASure.SCALar.RESistance.Query(25D, "MAXimum", out resistance);
            if (!String.Equals(channels, String.Empty)) SCPI.ROUTe.OPEN.Command(channels);
            Boolean passed_Ω;
            if (closed) passed_Ω = (0 <= resistance[0] && resistance[0] <= Limits.Ω_closed);
            else passed_Ω = (resistance[0] >= Limits.Ω_open);
            passed &= passed_Ω;
            results.Add(new DiagnosticsResult(Label: $"{diagnostic} channel(s) {channels}: ", Message: $"{Math.Round(resistance[0], 3, MidpointRounding.ToEven)}Ω", Event: (passed_Ω ? EVENTS.PASS : EVENTS.FAIL)));
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34932As((Double Ω_closed, Double Ω_open) M34932A) {
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules.M34932A) Results.Add(slot, Diagnostic_34932A(slot, M34932A));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34932A(SLOTS Slot, (Double Ω_closed, Double Ω_open) M34932A) {
            SCPI.ROUTe.OPEN.ALL.Command(null);
            return (true, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: nameof(Diagnostic_34932A), Message: " not implemented yet", Event: EVENTS.INFORMATION) });
            // TODO: Soon:
            // NOTE all measure Ω steps are 2-wire.
            // Place 34932T Loopback module into slot Slot, connect ABus DSub-9.
            // Open all, close s921, measure Ω; should equal ∞.
            // Close s101...s116 & s501...s516, measure Ω; should be ≤ DiagnosticParameter_34980A.Ω_34932A.
            // Open all, close s922, measure Ω; should equal ∞.
            // Close s201...s216 & s601...s616, measure Ω; should be ≤ DiagnosticParameter_34980A.Ω_34932A.
            // Open all, close s923, measure Ω; should equal ∞.
            // Close s301...s316 & s701...s716, measure Ω; should be ≤ DiagnosticParameter_34980A.Ω_34932A.
            // Open all, close s924, measure Ω; should equal ∞.
            // Close s401...s416 & s801...s816, measure Ω; should be ≤ DiagnosticParameter_34980A.Ω_34932A.
            //
            // Open all.
            // for ABus = s921 to s924:
            //     Close ABus, measure Ω; should equal ∞.
            //       for matrix = s101 to s116:
            //           close matrix, measure Ω; should be ≤ DiagnosticParameter_34980A.Ω_34932A.
            //           open matrix.
            //       next matrix.
            //       for matrix = s501 to s516:
            //           close matrix, measure Ω; should be ≤ DiagnosticParameter_34980A.Ω_34932A.
            //           open matrix.
            //       next matrix.
            //     Open ABus.
            // next ABus.
         }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34938As((Double Ω_closed, Double Ω_open) M34938A) {
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules.M34938A) Results.Add(slot, Diagnostic_34938A(slot, M34938A));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34938A(SLOTS Slot, (Double Ω_closed, Double Ω_open) M34938A) {
            SCPI.ROUTe.OPEN.ALL.Command(null);
            SCPI.INSTrument.DMM.STATe.Command(true);
            SCPI.INSTrument.DMM.CONNect.Command();
            SCPI.SENSe.RESistance.RESolution.Command("MAXimum");

            List<DiagnosticsResult> results = new List<DiagnosticsResult>();
            Boolean passed_34938A = true;
            String s = ((Int32)Slot).ToString("D1");

            if (DialogResult.Cancel == MessageBox.Show($"Please connect {Modules.M34938A} diagnostic connector to {_34980A} SLOT {s} Banks 1 & 2 and ABus DSub-9.", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                Data.CTS_Cancel.Cancel();
                Data.CT_Cancel.ThrowIfCancellationRequested();
            };

            String D = nameof(Diagnostic_34938A);
            CloseMeasureOpenRecord(D, kelvin: true, closed: false, String.Empty, M34938A, ref passed_34938A, ref results);
            CloseMeasureOpenRecord(D, kelvin: true, closed: true, $"@{s}001:{s}020", M34938A, ref passed_34938A, ref results);
            for (Int32 i = 1; i < 21; i++) CloseMeasureOpenRecord(D, kelvin: true, closed: true, $"@{s}{i:D3}", M34938A, ref passed_34938A, ref results);

            SCPI.INSTrument.DMM.DISConnect.Command();

            Data.CT_EmergencyStop.ThrowIfCancellationRequested();
            Data.CT_Cancel.ThrowIfCancellationRequested();

            if (DialogResult.Cancel == MessageBox.Show($"Please disconnect {Modules.M34938A} diagnostic connector from {_34980A} SLOT {s} Banks 1 & 2 and ABus DSub-9.", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly)) {
                Data.CTS_Cancel.Cancel();
                Data.CT_Cancel.ThrowIfCancellationRequested();
            };

            return (Summary: passed_34938A, Details: results);
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34939As((Double Ω_closed, Double Ω_open) M34939A) {
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules.M34939A) Results.Add(slot, Diagnostic_34939A(slot, M34939A));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34939A(SLOTS Slot, (Double Ω_closed, Double Ω_open) M34939A) {
            SCPI.ROUTe.OPEN.ALL.Command(null);
            return (true, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: nameof(Diagnostic_34939A), Message: " not implemented yet", Event: EVENTS.INFORMATION) });
        }

        public Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Diagnostics_34952As(Double Ω) {
            Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)> Results = new Dictionary<SLOTS, (Boolean Summary, List<DiagnosticsResult> Details)>();
            foreach (SLOTS slot in Enum.GetValues(typeof(SLOTS))) if (SystemType(slot) == Modules.M34952A) Results.Add(slot, Diagnostic_34952A(slot));
            return Results;
        }

        public (Boolean Summary, List<DiagnosticsResult> Details) Diagnostic_34952A(SLOTS Slot) {
            SCPI.ROUTe.OPEN.ALL.Command(null);
            return (true, new List<DiagnosticsResult>() { new DiagnosticsResult(Label: nameof(Diagnostic_34952A), Message: " not implemented yet", Event: EVENTS.INFORMATION) });
        }
        #endregion Diagnostics

        public MSMU_34980A_SCPI_NET(String Address, String Detail) : base(Address) {
            this.Address = Address;
            this.Detail = Detail;
            InstrumentType = INSTRUMENT_TYPES.MULTI_FUNCTION;
            DateTime now = DateTime.Now;
            SCPI.SYSTem.DATE.Command(now.Year, now.Month, now.Day);
            SCPI.SYSTem.TIME.Command(now.Hour, now.Minute, Convert.ToDouble(now.Second));
            SCPI.UNIT.TEMPerature.Command($"{TEMPERATURE_UNITS.F}");
            SCPI.IDN.Query(out String idn);
            _34980A = idn.Split(',')[(Int32)SCPI_NET.IDN_FIELDS.Model];

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
                case Modules.M34921A: return (Min: 1, Max: 44);
                case Modules.M34939A: return (Min: 1, Max: 68);
                case Modules.M34952A: return (Min: 1, Max: 7);
                default: throw new NotImplementedException($"Module Type '{SystemType(Slot)}' not implemented.");
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
            if (!Regex.IsMatch(Channels, @"^@\d{4}((,|:)\d{4})*$")) { // https://regex101.com/.
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Invalid syntax for {nameof(Channels)} '{Channels}'.");
                sb.AppendLine(" - Must be in form of 1 or more discrete channels and/or ranges preceded by '@'.");
                sb.AppendLine(" - Channel:  '@####':       Discrete channels must be separated by commas; '@1001,1002'.");
                sb.AppendLine(" - Range:    '@####:####':  Channel ranges must be separated by colons; '@1001:1002'.");
                sb.AppendLine(" - Examples: '@1001', '@1001,2001,2005', '@1001,2001:2005' & '@1001,2001:2005,2017,3001:3015,3017' all valid.");
                sb.AppendLine();
                sb.AppendLine("Caveats:");
                sb.AppendLine(" - Whitespace not permitted; '@1001, 1005', '@1001 ,1005' '& '@1001: 1005' all invalid.");
                sb.AppendLine(" - Range cannot include ABus channels, denoted as #9##.  Thus range '@1001:1902' invalid, but discretes '@1001,1902' valid.");
                sb.AppendLine(" - First & only first channel begins with '@'.  Thus '1001,2001' & '@1001,@2001' both invalid.");
                throw new ArgumentException(sb.ToString());
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
            Int32 slotNumber = Int32.Parse(Channel.Substring(0, 2));
            if (!Enum.IsDefined(typeof(SLOTS), (SLOTS)slotNumber)) throw new ArgumentException($"{nameof(Channel)} '{Channel}' must have valid integer Slot in interval [{(Int32)SLOTS.S1}..{(Int32)SLOTS.S8}].");
            Int32 channel = Int32.Parse(Channel.Substring(2));
            (Int32 min, Int32 max) = ModuleChannels((SLOTS)slotNumber);
            if (channel < min || max < channel) throw new ArgumentException($"{nameof(Channel)} '{Channel}' must have valid integer {nameof(Channel)} in interval [{min:D3}..{max:D3}].");
        }
        public void ValidateRange(String Range) {
            String[] channels = Range.Split(new Char[] { ':' }, StringSplitOptions.None);
            if (channels[0][1].Equals('9') || channels[1][1].Equals('9')) throw new ArgumentException($"{nameof(Range)} '{Range}' cannot include ABus #9##.");
            ValidateChannel(channels[0]);
            ValidateChannel(channels[1]);
            if (Convert.ToInt32(channels[0]) >= Convert.ToInt32(channels[1])) throw new ArgumentException($"{nameof(Range)} '{Range}' start {nameof(channels)} '{channels[0]}' must be < end {nameof(Range)} '{channels[1]}'.");
        }
    }
}