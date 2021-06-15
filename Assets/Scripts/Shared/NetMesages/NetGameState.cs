using System;
using System.Collections.Generic;
using Server.Score;
using Unity.Networking.Transport;

namespace Shared.NetMessages {
    public class NetGameState : NetMessage {
        public int LocalId;
        public bool GameActive;
        public int PlayerCount;
        public List<PlayerState> PlayerStates;
        public List<ScoreState> ScoreStates;

        public int CoinCount;
        public List<CoinState> CoinStates;
        
        
        public NetGameState() {
            OpCode = OpCode.GameState;
        }

        public NetGameState(DataStreamReader reader) {
            OpCode = OpCode.GameState;
            Deserialize(reader);
        }
        
        public NetGameState(bool gameActive, List<PlayerState> playerStates, List<CoinState> coinStates, List<ScoreState> scoreStates) {
            OpCode = OpCode.GameState;
            GameActive = gameActive;
            PlayerCount = playerStates.Count;
            PlayerStates = playerStates;
            ScoreStates = scoreStates;
            CoinCount = coinStates.Count;
            CoinStates = coinStates;
        }

        public override void Serialize(ref DataStreamWriter writer) {
            base.Serialize(ref writer);
            writer.WriteByte(Convert.ToByte(GameActive));
            writer.WriteInt(PlayerCount);
            for (int i = 0; i < PlayerCount; i++) {
                var playerState = PlayerStates[i];
                var scoreState = ScoreStates[i];
                writer.WriteInt(playerState.PlayerId);
                writer.WriteUInt(playerState.LastHandledCommand);
                writer.WriteFloat(playerState.Position.X);
                writer.WriteFloat(playerState.Position.Y);
                writer.WriteFloat(playerState.Position.Z);
                writer.WriteInt(scoreState.Score);
            }
            
            writer.WriteInt(CoinCount);
            for (int i = 0; i < CoinCount; i++) {
                var coinState = CoinStates[i];
                writer.WriteInt(coinState.CoinId);
                writer.WriteByte(Convert.ToByte(coinState.IsActive));
                writer.WriteFloat(coinState.Position.X);
                writer.WriteFloat(coinState.Position.Y);
                writer.WriteFloat(coinState.Position.Z);
            }
        }

        public override void Deserialize(DataStreamReader reader) {
            GameActive = Convert.ToBoolean(reader.ReadByte());
            PlayerCount = reader.ReadInt();
            PlayerStates = new List<PlayerState>();
            ScoreStates = new List<ScoreState>();
            for (int i = 0; i < PlayerCount; i++) {
                var playerState = new PlayerState();
                var scoreState = new ScoreState();
                playerState.PlayerId = reader.ReadInt();
                playerState.LastHandledCommand = reader.ReadUInt();
                playerState.Position = new Vector3d() {
                    X = reader.ReadFloat(),
                    Y = reader.ReadFloat(),
                    Z = reader.ReadFloat(),
                };
                PlayerStates.Add(playerState);
                scoreState.PlayerId = playerState.PlayerId;
                scoreState.Score = reader.ReadInt();
                ScoreStates.Add(scoreState);
            }
            
            CoinCount = reader.ReadInt();
            CoinStates = new List<CoinState>();
            for (int i = 0; i < CoinCount; i++) {
                var coinState = new CoinState();
                coinState.CoinId = reader.ReadInt();
                coinState.IsActive = Convert.ToBoolean(reader.ReadByte());
                coinState.Position = new Vector3d() {
                    X = reader.ReadFloat(),
                    Y = reader.ReadFloat(),
                    Z = reader.ReadFloat(),
                };
                CoinStates.Add(coinState);
            }

            LocalId = reader.ReadInt();
        }
    }
}