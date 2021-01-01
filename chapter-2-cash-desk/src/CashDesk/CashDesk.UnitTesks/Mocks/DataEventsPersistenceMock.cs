using CashDesk.DataPersistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk.UnitTesks.Mocks
{
    class DataEventsPersistenceMock : IDataEventsPersistence
    {
        // Hold the data in memory to simulate the read and write to disk
        private string _streamName;
        private string _dataStream;

        public void Append(string streamName, string dataStream)
        {
            _streamName = streamName;
            _dataStream = dataStream;
        }

        public string[] Read(string streamName)
        {
            if (_streamName != streamName)
            {
                throw new Exception($"Stream name should be {_streamName} but is {streamName}");
            }

            return new[] { _dataStream };
        }
    }
}
