using System;
using Unity.Netcode;

public struct PlayerData:IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public KitchenNetworkMultiplayer.PlayerCharacter playerCharacter;

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId &&
            playerCharacter == other.playerCharacter;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerCharacter);
    }
}
