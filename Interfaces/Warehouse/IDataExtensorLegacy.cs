namespace Klyte.Commons.Interfaces
{
    public interface IDataExtensorLegacy
    {
        string SaveId { get; }

        IDataExtensor Deserialize(byte[] data);
    }
    public abstract class DataExtensorLegacyBase<X> : IDataExtensorLegacy where X : IDataExtensor
    {
        public abstract string SaveId { get; }

        public abstract X Deserialize(byte[] data);
        IDataExtensor IDataExtensorLegacy.Deserialize(byte[] data) => Deserialize(data);
    }
}
