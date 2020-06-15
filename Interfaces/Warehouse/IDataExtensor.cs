using System;

namespace Klyte.Commons.Interfaces
{
    public interface IDataExtensor
    {
        string SaveId { get; }

        void LoadDefaults();
        IDataExtensor Deserialize(Type type, byte[] data);
        byte[] Serialize();
        void OnReleased();
    }
}
