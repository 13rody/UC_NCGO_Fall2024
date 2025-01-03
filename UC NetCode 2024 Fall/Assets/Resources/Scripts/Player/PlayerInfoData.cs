using System; 
using System.Collections;
using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct PlayerInfoData : INetworkSerializable, IEquatable<PlayerInfoData>
{
    public ulong _clinentId;
    public FixedString64Bytes _name;
    public bool _isPlayerReady;
    public Color _colorId;

    public PlayerInfoData(ulong id)
    {
        _clinentId = id;
        _name = "";
        _isPlayerReady = false;
        _colorId = Color.magenta;

    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if(serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out _clinentId);
            reader.ReadValueSafe(out _name);
            reader.ReadValueSafe(out _isPlayerReady);
            reader.ReadValueSafe(out _colorId);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(_clinentId);
            writer.WriteValueSafe(_name);
            writer.WriteValueSafe(_isPlayerReady);
            writer.WriteValueSafe(_colorId);
        }
    }

    public bool Equals(PlayerInfoData other)
    {
        return _clinentId == other._clinentId && _name.Equals(other._name) && _isPlayerReady == other._isPlayerReady && _colorId.Equals(other._colorId);
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerInfoData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_clinentId,_name,_isPlayerReady,_colorId);  
    }

    public override string ToString() => _name.Value.ToString();

    public static implicit operator string(PlayerInfoData name) => name.ToString();

    public static implicit operator PlayerInfoData(string s) => new PlayerInfoData { _name = new FixedString64Bytes(s)};
}
