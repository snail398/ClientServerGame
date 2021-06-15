using System;

namespace Shared.Settings {
    [Serializable]
    public class NetworkSettingsContainer {
        public LaunchType LaunchType;
        public int Ping = 100;
        public int PacketLossPercentage = 20;
        public int ServerUpdatePerSeconds = 20;
        public bool UseBot;
    }
}