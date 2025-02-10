using System.ComponentModel;
using System.Runtime.CompilerServices;
using MahApps.Metro.Controls;
using Newtonsoft.Json;

namespace SteamP2PInfo.Config
{
    public class TableConfig : INotifyPropertyChanged
    {
        // ---------------- Relay ----------------
        [JsonProperty("show_relay")]
        [ConfigBindingElement("Show relay status", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show relay status (0 : direct, other : using relay server)",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowRelay { get; set; } = true;

        // ---------------- Ping ----------------
        [JsonProperty("show_ping")]
        [ConfigBindingElement("Show Ping", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show current Ping.",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowPing { get; set; } = true;

        [JsonProperty("show_ping_min")]
        [ConfigBindingElement("Show Ping Min", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show minimum Ping.",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowPingMin { get; set; }= true;

        [JsonProperty("show_ping_max")]
        [ConfigBindingElement("Show Ping Max", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show maximum Ping. ",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowPingMax { get; set; } = true;

        [JsonProperty("show_ping_avg")]
        [ConfigBindingElement("Show Ping Avg", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show average Ping. ",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowPingAvg { get; set; } = true;

        [JsonProperty("show_ping_stdev")]
        [ConfigBindingElement("Show Ping Stdev", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show Ping standard deviation. ",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowPingStdev { get; set; } = true;


        // ---------------- Connection Quality ----------------
        [JsonProperty("show_cq")]
        [ConfigBindingElement("Show Connection Quality (CQ)", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show current Connection Quality.",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
         })]

        public bool ShowCQ { get; set; } = true;

        [JsonProperty("show_cq_min")]
        [ConfigBindingElement("Show Connection Quality Min (CQ Min)", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show minimum Connection Quality.",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowCQMin { get; set; } = true;

        [JsonProperty("show_cq_max")]
        [ConfigBindingElement("Show Connection Quality Max (CQ Max)", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show maximum Connection Quality. ",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowCQMax { get; set; } = true;

        [JsonProperty("show_cq_avg")]
        [ConfigBindingElement("Show Connection Quality Avg (CQ Avg)", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show average Connection Quality. ",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowCQAvg { get; set; } = true;

        [JsonProperty("show_cq_stdev")]
        [ConfigBindingElement("Show Connection Quality Stdev (CQ Stdev)", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show Connection Quality standard deviation. ",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowCQStdev { get; set; } = true;

        // ---------------- Connection Quality Remote ----------------
        [JsonProperty("show_cqr")]
        [ConfigBindingElement("Show Connection Quality Remote (CQR)", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show current Connection Quality Remote. (available when connected by SteamNetworkingSocket)",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
         })]

        public bool ShowCQR { get; set; } = false;

        [JsonProperty("show_cqr_min")]
        [ConfigBindingElement("Show Connection Quality Remote Min (CQR Min)", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show minimum Connection Quality Remote. (available when connected by SteamNetworkingSocket)",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowCQRMin { get; set; } = false;

        [JsonProperty("show_cqr_max")]
        [ConfigBindingElement("Show Connection Quality Remote Max (CQR Max)", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show maximum Connection Quality Remote. (available when connected by SteamNetworkingSocket)",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowCQRMax { get; set; } = false;

        [JsonProperty("show_cqr_avg")]
        [ConfigBindingElement("Show Connection Quality Remote Avg (CQR Avg)", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show average Connection Quality Remote. (available when connected by SteamNetworkingSocket)",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowCQRAvg { get; set; } = false;

        [JsonProperty("show_cqr_stdev")]
        [ConfigBindingElement("Show Connection Quality Remote Stdev (CQR Stdev)", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show Connection Quality Remote standard deviation. (available when connected by SteamNetworkingSocket)",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowCQRStdev { get; set; } = false;

        // ---------------- Connection Type ----------------
        [JsonProperty("show_conn_type")]
        [ConfigBindingElement("Show Connection Type", typeof(ToggleSwitch), "IsOnProperty",
            Tooltip: "Show Connection Type",
            UIElementProperties: new object[] {
                new object[] { "OnContent", "Yes" },
                new object[] { "OffContent", "No" }
        })]
        public bool ShowConnType { get; set; } = false;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
