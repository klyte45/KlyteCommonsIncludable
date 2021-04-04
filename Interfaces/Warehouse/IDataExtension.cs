using System;

namespace Klyte.Commons.Interfaces
{
    public interface IDataExtension
    {
        string SaveId { get; }

        void LoadDefaults();
        IDataExtension Deserialize(Type type, byte[] data);
        byte[] Serialize();
        void OnReleased();
    }
}
