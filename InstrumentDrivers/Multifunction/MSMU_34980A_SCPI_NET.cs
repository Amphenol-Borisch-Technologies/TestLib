﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Agilent.CommandExpert.ScpiNet.Ag34980_2_43;
using ABT.Test.Lib.InstrumentDrivers.Interfaces;

namespace ABT.Test.Lib.InstrumentDrivers.Multifunction {
    public class MSMU_34980A_SCPI_NET : Ag34980, IInstruments, IRelays {
        public enum ABUS { ABUS1, ABUS2, ABUS3, ABUS4, ALL };
        public enum SLOTS { SLOT1 = 1, SLOT2 = 2, SLOT3 = 3, SLOT4 = 4, SLOT5 = 5, SLOT6 = 6, SLOT7 = 7, SLOT8 = 8 }
        public enum TEMPERATURE_UNITS { C, F, K }
        public enum RELAY_STATES { opened, CLOSED }

        public String Address { get; }
        public String Detail { get; }
        public INSTRUMENT_TYPES InstrumentType { get; }

        public void ResetClear() {
            SCPI.RST.Command();
            SCPI.CLS.Command();
        }
        
        public DIAGNOSTICS_RESULTS Diagnostics() {
            Int32 result;
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
            return (DIAGNOSTICS_RESULTS)result; // Ag34980 returns 0 for passed, 1 for fail.
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
                default      : throw new NotImplementedException($"Module Type '{SystemType(Slot)}' unimplemented.");
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
            return (TEMPERATURE_UNITS)Enum.Parse(typeof(TEMPERATURE_UNITS), String.Join("", units).Replace("[", "").Replace("]", "")); }

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

            String[] channelsOrRanges = Channels.Split(new Char[] {','}, StringSplitOptions.None);
            foreach (String channelOrRange in channelsOrRanges) {
                if (Regex.IsMatch(channelOrRange, ":")) ValidateRange(channelOrRange);
                else ValidateChannel(channelOrRange);
            }
        }
        public void ValidateChannel(String Channel) {
            // TODO: Debug.Print($"Channel: '{Channel}'.");
            Int32 slotNumber = Int32.Parse(Channel.Substring(0, 2));
            // TODO: Debug.Print($"Slot Number: '{slotNumber}'.");
            if (!Enum.IsDefined(typeof(SLOTS), (SLOTS)slotNumber)) throw new ArgumentException($"Channel '{Channel}' must have valid integer Slot in interval [{(Int32)SLOTS.SLOT1}..{(Int32)SLOTS.SLOT8}].");
            Int32 channel = Int32.Parse(Channel.Substring(2));
            // TODO: Debug.Print($"Channel: '{channel}'.");
            (Int32 min, Int32 max) = ModuleChannels((SLOTS)slotNumber);
            // TODO: Debug.Print($"ModuleChannels min '{min}' & '{max}'.");
            if (channel < min || max < channel) throw new ArgumentException($"Channel '{Channel}' must have valid integer Channel in interval [{min:D3}..{max:D3}].");
        }
        public void ValidateRange(String Range) {
            // TODO: Debug.Print($"Range: '{Range}'.");
            String[] channels = Range.Split(new Char[] {':'}, StringSplitOptions.None);
            // TODO: for (Int32 i=0; i < channels.Length; i++) Debug.Print($"channels[{i}]='{channels[i]}'.");
            if (channels[0][1].Equals('9') || channels[1][1].Equals('9')) throw new ArgumentException($"Channel Range '{Range}' cannot include ABus Channel #9##.");
            ValidateChannel(channels[0]);
            ValidateChannel(channels[1]);
            if (Convert.ToInt32(channels[0]) >= Convert.ToInt32(channels[1])) throw new ArgumentException($"Channel Range '{Range}' start Channel '{channels[0]}' must be < end Channel '{channels[1]}'.");
        }
    }
}