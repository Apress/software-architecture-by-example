using CashDesk.Server.Persistence;
using CashDesk.UnitTesks.Mocks;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CashDesk.UnitTesks.Integration
{
    public class Persistence
    {

        [Fact]
        public void PersistData_LoadStream_CanReadSaveStream()
        {
            // Arrange
            string streamName = "EventStream1";

            var cashDeskStream = new CashDesk.Server.CashDeskEventStream()
            {
                StreamName = "EventStream1"
            };

            var dataEventsPersistence = new DataEventsPersistenceMock();

            cashDeskStream.AddTransaction(new Server.Entities.CashDeskTransaction(
                new DateTime(2021, 05, 05), "HAMMER", "Test", 20.12m, "1"));

            var dataPersistence = new PersistData<CashDesk.Server.CashDeskEventStream>(dataEventsPersistence);

            // Act            
            dataPersistence.Save(cashDeskStream);
            var result = dataPersistence.Load(streamName);

            // Assert
            Assert.Equal(streamName, result.StreamName);
            Assert.Equal(20.12m, result.CashDeskBalance["1"]);
        }

        [Fact]
        public void PersistData_LoadStream_SaveStream_MultipleTimes()
        {
            // Arrange
            string streamName = "EventStream1";

            var cashDeskStream = new CashDesk.Server.CashDeskEventStream()
            {
                StreamName = "EventStream1"
            };

            var dataEventsPersistence = new DataEventsPersistenceMock();

            cashDeskStream.AddTransaction(new Server.Entities.CashDeskTransaction(
                new DateTime(2021, 05, 05), "HAMMER", "Test", 20.12m, "1"));

            var dataPersistence = new PersistData<CashDesk.Server.CashDeskEventStream>(dataEventsPersistence);

            // Act            
            dataPersistence.Save(cashDeskStream);
            var result = dataPersistence.Load(streamName);

            cashDeskStream.AddTransaction(new Server.Entities.CashDeskTransaction(
                new DateTime(2021, 05, 05), "NAIL", "Test", 5.12m, "1"));

            dataPersistence.Save(cashDeskStream);
            result = dataPersistence.Load(streamName);

            cashDeskStream.AddTransaction(new Server.Entities.CashDeskTransaction(
                new DateTime(2021, 05, 05), "SCREWDRIVER", "Test", 8.55m, "1"));

            dataPersistence.Save(cashDeskStream);
            result = dataPersistence.Load(streamName);

            // Assert
            Assert.Equal(streamName, result.StreamName);
            Assert.Equal(33.79m, result.CashDeskBalance["1"]);
        }

    }
}
