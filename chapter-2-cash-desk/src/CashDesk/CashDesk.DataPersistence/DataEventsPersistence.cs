using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CashDesk.DataPersistence
{
    public class DataEventsPersistence : IDataEventsPersistence
    {
        public void Append(string streamName, string dataStream)
        {
            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }

            string fileName = $"data/{DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss")}-{streamName}";

            using var file = new StreamWriter(fileName, true);
            file.Write(dataStream);            
        }

        public string[] Read(string streamName)
        {
            if (!Directory.Exists("data"))
            {
                return new[] { string.Empty };
            }

            List<string> returnList = new List<string>();

            foreach (var file in Directory.GetFiles("data"))
            {
                if (!file.Contains(streamName))
                    continue;

                string streamText = File.ReadAllText(file);
                returnList.Add(streamText);
            }

            return returnList.OrderBy(a => a).ToArray();
        }
    }
}
