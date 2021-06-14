using System;
using Client.Coins;
using Client.GameFlow;
using Client.Input;
using Client.Spawn;
using Client.UI;
using Zenject;

namespace Client {
    public class ClientInstaller : Installer<ClientInstaller> {
        public override void InstallBindings() {
            Container.Bind(typeof(BaseClientService), typeof(IInitializable), typeof(IDisposable)).To<BaseClientService>().AsSingle();
            Container.Bind(typeof(SpawnClientService), typeof(IInitializable), typeof(IDisposable)).To<SpawnClientService>().AsSingle();
            Container.Bind(typeof(InputService), typeof(IInitializable), typeof(IDisposable)).To<InputService>().AsSingle();
            Container.Bind(typeof(MoveClientService), typeof(IInitializable), typeof(IDisposable)).To<MoveClientService>().AsSingle();
            Container.Bind(typeof(UiService), typeof(IInitializable), typeof(IDisposable)).To<UiService>().AsSingle();
            Container.Bind(typeof(CoinsClientService), typeof(IInitializable), typeof(IDisposable)).To<CoinsClientService>().AsSingle();
            Container.Bind(typeof(GameFlowClientService), typeof(IInitializable), typeof(IDisposable)).To<GameFlowClientService>().AsSingle();
        }   
    }
}