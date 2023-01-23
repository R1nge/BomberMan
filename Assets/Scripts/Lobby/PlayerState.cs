using System;
using Unity.Netcode;

namespace Lobby
{
    [Serializable]
    public struct PlayerState : INetworkSerializable, IEquatable<PlayerState>
    {
        public ulong ClientId;
        public int SkinIndex;
        public bool IsReady;
        public NetworkString Nickname;

        public PlayerState(ulong clientId, int skinIndex, bool isReady, NetworkString nickname)
        {
            ClientId = clientId;
            SkinIndex = skinIndex;
            IsReady = isReady;
            Nickname = nickname;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref SkinIndex);
            serializer.SerializeValue(ref IsReady);
            serializer.SerializeValue(ref Nickname);
        }

        public bool Equals(PlayerState other)
        {
            return ClientId == other.ClientId && SkinIndex == other.SkinIndex && IsReady == other.IsReady;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientId, SkinIndex, IsReady);
        }
    }
}