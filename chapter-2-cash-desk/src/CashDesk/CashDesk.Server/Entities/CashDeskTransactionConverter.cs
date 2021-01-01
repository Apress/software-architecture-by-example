using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace CashDesk.Server.Entities
{
    public class CashDeskTransactionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ITransaction));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(CashDeskTransaction));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value, typeof(CashDeskTransaction));
        }
    }
}
