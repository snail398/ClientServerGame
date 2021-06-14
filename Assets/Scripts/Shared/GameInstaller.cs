using Client;
using Common;
using Server;
using Shared.Settings;
using Zenject;

namespace Shared {
    public class GameInstaller : MonoInstaller {

        private SettingsService _SettingsService;

        [Inject]
        public void Construct(SettingsService settingsService) {
            _SettingsService = settingsService;
        }

        public override void InstallBindings() {
            var settings = _SettingsService.GetSettings<NetworkSettings>();
            switch (settings.Settings.LaunchType) {
                case LaunchType.Server:
                    ServerInstaller.Install(Container);
                    break;
                case LaunchType.Client:
                    ClientInstaller.Install(Container);
                    break;
            }
        }
    }
}