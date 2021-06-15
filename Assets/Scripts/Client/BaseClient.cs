using System;
using Common;
using Shared.NetMessages;
using Shared.Settings;
using Shared.Signals;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;
using Zenject;

namespace Client {
    public class BaseClientService : IInitializable, IDisposable {
        private readonly UnityEventProvider _UnityEventProvider;
        private readonly SignalBus _SignalBus;
        private readonly SettingsService _SettingsService;

        private NetworkDriver _NetworkDriver;
        private NetworkPipeline _NetworkPipeline;
        private NetworkSettings _NetworkSettings;
        protected NetworkConnection Connection;

        public BaseClientService(UnityEventProvider unityEventProvider, SignalBus signalBus, SettingsService settingsService) {
            _UnityEventProvider = unityEventProvider;
            _SignalBus = signalBus;
            _SettingsService = settingsService;
        }

        public void Initialize() {
            _NetworkSettings = _SettingsService.GetSettings<NetworkSettings>();
            _UnityEventProvider.OnUpdate += UpdateServer;
            _SignalBus.Subscribe<SendMessageToServerSignal>(SendMessageToServer, this);
            InitServer();
        }

        protected virtual void InitServer() {
            _NetworkDriver = NetworkDriver.Create(new SimulatorUtility.Parameters {MaxPacketSize = NetworkParameterConstants.MTU, MaxPacketCount = 30, PacketDelayMs = _NetworkSettings.Settings.Ping, PacketDropPercentage = _NetworkSettings.Settings.PacketLossPercentage});
            _NetworkPipeline = _NetworkDriver.CreatePipeline(typeof(SimulatorPipelineStage));
            Connection = default;
            NetworkEndPoint endPoint = NetworkEndPoint.LoopbackIpv4;
            endPoint.Port = 5522;
            Connection = _NetworkDriver.Connect(endPoint);
        }

        protected virtual void UpdateServer() {
            _NetworkDriver.ScheduleUpdate().Complete();
            CheckAlive();
            UpdateMessagePump();
        }

        private void CheckAlive() {
            if (!Connection.IsCreated) {
                Debug.LogError("Error connection to server");
            }
        }

        private void UpdateMessagePump() {
            DataStreamReader streamReader;
            NetworkEvent.Type cmd;
            while ((cmd = Connection.PopEvent(_NetworkDriver, out streamReader)) != NetworkEvent.Type.Empty) {
                switch (cmd) {
                    case NetworkEvent.Type.Data:
                        OnData(streamReader);
                        break;
                    case NetworkEvent.Type.Disconnect:
                        Connection = default;
                        break;
                    case NetworkEvent.Type.Connect:
                        Debug.Log($"You are connected to server");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void OnData(DataStreamReader reader) {

            NetMessage message = null;
            OpCode opCode = (OpCode)reader.ReadByte();
            switch (opCode) {
                case OpCode.Position:
                    message = new NetPlayerPosition(reader);
                    break;
                case OpCode.Move:
                    break;
                case OpCode.StartGame:
                    break;
                case OpCode.EndGame:
                    break;
                case OpCode.SpawnPlayer:
                    message = new NetSpawnPlayer(reader);
                    break;
                case OpCode.GameState:
                    message = new NetGameState(reader);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _SignalBus.FireSignal(new MessageReceivedSignal(message));
        }

        private void SendMessageToServer(SendMessageToServerSignal obj) {
            DataStreamWriter writer;
            _NetworkDriver.BeginSend(_NetworkPipeline, Connection, out writer);
            obj.Message.Serialize(ref writer);
            _NetworkDriver.EndSend(writer);
        }

        protected virtual void Shutdown() {
            _NetworkDriver.Dispose();
        }

        public void Dispose() {
            _UnityEventProvider.OnUpdate -= UpdateServer;
            Shutdown();
        }
    }
}