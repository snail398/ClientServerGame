using System;
using Common;
using Server.Signals;
using Shared.NetMessages;
using Shared.Settings;
using Shared.Signals;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;
using Zenject;

namespace Server {
    public class BaseServerService : IInitializable, IDisposable {

        public event Action<NetworkConnection> OnClientConnected;
        
        private readonly UnityEventProvider _UnityEventProvider;
        private readonly SignalBus _SignalBus;
        private readonly SettingsService _SettingsService;
        
        private NetworkDriver _NetworkDriver;       
        private NetworkPipeline _NetworkPipeline;
        private NetworkSettings _NetworkSettings;
        protected NativeList<NetworkConnection> Connections;
        
        public BaseServerService(UnityEventProvider unityEventProvider, SignalBus signalBus, SettingsService settingsService) {
            _UnityEventProvider = unityEventProvider;
            _SignalBus = signalBus;
            _SettingsService = settingsService;
        }

        public void Initialize() {
            _SignalBus.Subscribe<BroadcastMessageSignal>(BroadcastMessage, this);
            _NetworkSettings = _SettingsService.GetSettings<NetworkSettings>();
            _UnityEventProvider.OnUpdate += UpdateServer;
            InitServer();
        }

        protected virtual void InitServer() {
            _NetworkDriver = NetworkDriver.Create(new SimulatorUtility.Parameters {MaxPacketSize = NetworkParameterConstants.MTU, MaxPacketCount = 30, PacketDelayMs = _NetworkSettings.Settings.Ping, PacketDropPercentage = _NetworkSettings.Settings.PacketLossPercentage});
            _NetworkPipeline = _NetworkDriver.CreatePipeline(typeof(SimulatorPipelineStage));
            NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
            endPoint.Port = 5522;
            if (_NetworkDriver.Bind(endPoint) != 0) {
                Debug.LogError($"Error binding server to port {endPoint.Port}");
            }
            else {
                _NetworkDriver.Listen();
            }

            Connections = new NativeList<NetworkConnection>(4, Allocator.Persistent);
        }

        protected virtual void UpdateServer() {
            _NetworkDriver.ScheduleUpdate().Complete();
            CleanupConnections();
            AcceptNewConnections();
            UpdateMessagePump();
        }

        private void CleanupConnections() {
            for (int i = 0; i < Connections.Length; i++) {
                if (!Connections[i].IsCreated) {
                    Connections.RemoveAtSwapBack(i);
                    --i;
                }
            }
        }

        private void AcceptNewConnections() {
            NetworkConnection connection;
            while ((connection = _NetworkDriver.Accept()) != default) {
                Connections.Add(connection);
                OnClientConnected?.Invoke(connection);
                Debug.Log("Accepted a connection");
            }
        }

        private void UpdateMessagePump() {
            DataStreamReader streamReader;
            for (int i = 0; i < Connections.Length; i++) {
                NetworkEvent.Type cmd;
                while ((cmd = _NetworkDriver.PopEventForConnection(Connections[i], out streamReader)) !=
                       NetworkEvent.Type.Empty) {
                    switch (cmd) {
                        case NetworkEvent.Type.Data:
                            OnData(streamReader);
                            break;
                        case NetworkEvent.Type.Disconnect:
                            Connections[i] = default;
                            break;
                    }
                }
            }
        }
        
        public void OnData(DataStreamReader reader) {
            NetMessage message = null;
            OpCode opCode = (OpCode)reader.ReadByte();
            switch (opCode) {
                case OpCode.Position:
                    break;
                case OpCode.Move:
                    break;
                case OpCode.StartGame:
                    break;
                case OpCode.EndGame:
                    break;
                case OpCode.SpawnPlayer:
                    // message = new NetSpawnPlayer(reader);
                    break;
                case OpCode.Input:
                    message = new NetInputMessage(reader);
                    break;
                case OpCode.PlayerLoaded:
                    message = new NetPlayerLoaded(reader);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (message != null)
                _SignalBus.FireSignal(new MessageReceivedSignal(message));
        }
        
        private void BroadcastMessage(BroadcastMessageSignal signal) {
            for (int i = 0; i < Connections.Length; i++) {
                if (Connections[i].IsCreated)
                    SentToClient(Connections[i], signal.Message);
            }
        }

        public void SentToClient(NetworkConnection connection, NetMessage message) {
            DataStreamWriter writer;
            _NetworkDriver.BeginSend(_NetworkPipeline, connection, out writer);
            message.Serialize(ref writer);
            writer.WriteInt(connection.InternalId);
            _NetworkDriver.EndSend(writer);
        }

        protected virtual void Shutdown() {
            _NetworkDriver.Dispose();
            Connections.Dispose();
        }

        public void Dispose() {
            _UnityEventProvider.OnUpdate -= UpdateServer;
            Shutdown();
        }
    }
}