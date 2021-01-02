using CashDesk.Server.EventTypes;
using CashDesk.Server.Persistence;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CashDesk.UnitTesks
{
    public class EventSourcingTests
    {
        [Fact]
        public void AddSingleEventSingleTill()
        {
            // Arrange
            var cashDesk = new CashDesk.Server.CashDeskEventStream();
            var transaction = new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "HAMMER", "Claw Hammer", 20.43m, "1");

            // Act
            cashDesk.AddTransaction(transaction);

            // Assert
            Assert.Equal(20.43m, cashDesk.CashDeskBalance["1"]);
        }

        [Fact]
        public void AddMultipleSingleTill()
        {
            // Arrange
            var cashDesk = new CashDesk.Server.CashDeskEventStream();
            
            // Act
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "HAMMER", "Claw Hammer", 20.43m, "1"));
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL2_5", "2.5mm NAILS (100)", 0.94m, "1"));
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL1_8", "1.8mm NAILS (250)", 2.01m, "1"));

            // Assert
            Assert.Equal(23.38m, cashDesk.CashDeskBalance["1"]);
        }

        [Fact]
        public void AddMultipleWithCorrectionsSingleTill()
        {
            // Arrange
            var cashDesk = new CashDesk.Server.CashDeskEventStream();

            // Act
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "HAMMER", "Claw Hammer", 20.43m, "1"));
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL2_5", "2.5mm NAILS (100)", 0.94m, "1"));
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL1_8", "1.8mm NAILS (250)", 2.01m, "1"));
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL1_8", "1.8mm NAILS (250)", -2.01m, "1"));

            // Assert
            Assert.Equal(21.37m, cashDesk.CashDeskBalance["1"]);
        }

        [Fact]
        public void AddMultipleTransactionsMultipleTills()
        {
            // Arrange
            var cashDesk = new CashDesk.Server.CashDeskEventStream();

            // Act
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "HAMMER", "Claw Hammer", 20.43m, "1"));
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL2_5", "2.5mm NAILS (100)", 0.94m, "1"));
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "HAMMER", "Claw Hammer", 20.43m, "2"));
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL1_8", "1.8mm NAILS (250)", 2.01m, "1"));
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL2_5", "2.5mm NAILS (100)", 0.94m, "2"));

            // Assert
            Assert.Equal(23.38m, cashDesk.CashDeskBalance["1"]);
            Assert.Equal(21.37m, cashDesk.CashDeskBalance["2"]);
        }

        [Fact]
        public void PersistData_Save()
        {
            // Arrange
            string streamName = "EntityStream1";
            var dataEventsPersistence = Substitute.For<DataPersistence.IDataEventsPersistence>();            

            var cashDesk = new CashDesk.Server.CashDeskEventStream()
            {
                StreamName = streamName
            };
            var dataPersistence = new PersistData<CashDesk.Server.CashDeskEventStream>(dataEventsPersistence);

            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "HAMMER", "Claw Hammer", 20.43m, "1"));
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL2_5", "2.5mm NAILS (100)", 0.94m, "1"));
            cashDesk.AddTransaction(new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL1_8", "1.8mm NAILS (250)", 2.01m, "1"));

            // Act
            dataPersistence.Save<DataCashDeskTransactionAddedEvent, CashDeskTransactionAddedEvent>(cashDesk);

            // Assert
            dataEventsPersistence.Received(1).Append(Arg.Is<string>(streamName), Arg.Any<string>());
        }

        [Fact]
        public void PersistData_Load()
        {
            // Arrange
            string streamName = "EntityStream1";
            string eventType = typeof(DataCashDeskTransactionAddedEvent).AssemblyQualifiedName;

            var changes = new List<DataCashDeskTransactionAddedEvent>()
            {
                new DataCashDeskTransactionAddedEvent()
                {
                    EventType = eventType,
                    Transaction = new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "HAMMER", "Claw Hammer", 20.43m, "1")
                },
                new DataCashDeskTransactionAddedEvent()
                {
                    EventType = eventType,
                    Transaction = new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL2_5", "2.5mm NAILS (100)", 0.94m, "1")
                },
                new DataCashDeskTransactionAddedEvent()
                {
                    EventType = eventType,
                    Transaction = new Server.Entities.CashDeskTransaction(new DateTime(2021, 02, 05), "NAIL1_8", "1.8mm NAILS (250)", 2.01m, "1")
                }
            };
            var cashDeskReturnStream = new CashDesk.Server.CashDeskEventStream()
            {
                Changes = changes.Select(a => (object)a).ToList(),
                StreamName = streamName
            };
            var returnStreamSerialised = JsonConvert.SerializeObject(cashDeskReturnStream);
            
            var dataEventsPersistence = Substitute.For<DataPersistence.IDataEventsPersistence>();
            dataEventsPersistence.Read(Arg.Is<string>(streamName)).Returns(new[] { returnStreamSerialised });  
            
            var dataPersistence = new PersistData<CashDesk.Server.CashDeskEventStream>(dataEventsPersistence);

            // Act
            var result = dataPersistence.Load(streamName);

            // Assert
            Assert.Equal(streamName, result.StreamName);
            Assert.Equal(23.38m, result.CashDeskBalance["1"]);
        }


    }
}
