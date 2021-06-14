using Settings;
using UnityEngine;

namespace Shared.Settings {
    
    [CreateAssetMenu(fileName = "NetworkSettings", menuName = "Settings/NetworkSettings", order = 1)]
    public class NetworkSettings : ScriptableObject, ISettings {
        public NetworkSettingsContainer Settings;
        
        public void Init() {
        }
    }
}