using ICities;
using System;

namespace Klyte.Commons.Interfaces.Warehouse
{
    public interface IDataExtension
    {
        string SaveId { get; }
        bool IsLegacyCompatOnly { get; }

        void LoadDefaults(ISerializableData serializableData);
        IDataExtension Deserialize(Type type, byte[] data);
        byte[] Serialize();
        void OnReleased();
    }
}
