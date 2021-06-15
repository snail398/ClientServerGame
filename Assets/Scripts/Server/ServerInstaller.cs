using System;
using System.Collections.Generic;
using Server.Coins;
using Server.GameFlow;
using Server.Score;
using Shared;
using Shared.Spawn;
using Zenject;

namespace Server {
    public class ServerInstaller : Installer<ServerInstaller> {
        public override void InstallBindings() {
            Container.Bind(typeof(BaseServerService), typeof(IInitializable), typeof(IDisposable)).To<BaseServerService>().AsSingle();
            Container.Bind(typeof(ClientConnectionService), typeof(IInitializable), typeof(IDisposable)).To<ClientConnectionService>().AsSingle();
            Container.Bind(typeof(SpawnServerService), typeof(IInitializable), typeof(IDisposable)).To<SpawnServerService>().AsSingle();
            Container.Bind(typeof(MoveServerService), typeof(IGameStateProvider<List<PlayerState>>),typeof(IInitializable), typeof(IDisposable)).To<MoveServerService>().AsSingle();
            Container.Bind(typeof(CoinsServerService), typeof(IGameStateProvider<List<CoinState>>),typeof(IInitializable), typeof(IDisposable)).To<CoinsServerService>().AsSingle();
            Container.Bind(typeof(ScoreServerService), typeof(IGameStateProvider<List<ScoreState>>),typeof(IInitializable), typeof(IDisposable)).To<ScoreServerService>().AsSingle();
            Container.Bind(typeof(GameFlowServerService), typeof(IGameStateProvider<bool>),typeof(IInitializable), typeof(IDisposable)).To<GameFlowServerService>().AsSingle();
            Container.Bind(typeof(SynchronizationServerService), typeof(IInitializable), typeof(IDisposable)).To<SynchronizationServerService>().AsSingle();
            // Container.Bind(typeof(BotService), typeof(IInitializable), typeof(IDisposable)).To<BotService>().AsSingle();
        }
    }
}