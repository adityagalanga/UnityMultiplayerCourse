using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>,INetworkSerializable
{
    public ulong clientID;
    public int colorID;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerID;

    public bool Equals(PlayerData other)
    {
        return clientID == other.clientID && colorID == other.colorID && playerName == other.playerName && playerID == other.playerID;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID); 
        serializer.SerializeValue(ref colorID);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerID);
    }
}
