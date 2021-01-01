namespace CashDesk.DataPersistence
{
    public interface IDataEventsPersistence
    {
        void Append(string streamName, string dataStream);        
        string[] Read(string streamName);
    }
}