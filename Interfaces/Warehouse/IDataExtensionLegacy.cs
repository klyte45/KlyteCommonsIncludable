namespace Klyte.Commons.Interfaces
{
    public interface IDataExtensionLegacy
    {
        string SaveId { get; }

        IDataExtension Deserialize(byte[] data);
    }
    public abstract class DataExtensionLegacyBase<X> : IDataExtensionLegacy where X : IDataExtension
    {
        public abstract string SaveId { get; }

        public abstract X Deserialize(byte[] data);
        IDataExtension IDataExtensionLegacy.Deserialize(byte[] data) => Deserialize(data);
    }
}
