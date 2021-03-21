using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.HotelApi;
using Xunit;

namespace TravelAgent.ApiTests
{
    public class TestHotelApi
    {
        [Fact]
        public async Task HotelApi_GetAvailability_Returns200OK()
        {
            // Arrange
            using var server = new WebApplicationFactory<Startup>();
            using var client = server.CreateClient();

            var dateTime = new DateTime(2021, 01, 02);
            string dateParam = dateTime.ToString("yyyy-MM-dd");

            // Act
            var result = await client.GetAsync($"https://localhost:44375/Availability?date={dateParam}");

            // Assert
            Assert.True(result.IsSuccessStatusCode);
        }

        [Fact]
        public async Task HotelApi_BookRoom_Returns200OK()
        {
            // Arrange
            using var server = new WebApplicationFactory<Startup>();
            using var client = server.CreateClient();

            var date = new DateTime(2021, 01, 02);

            string roomRequestJson = "{ \"date\": \"" + date.ToString("yyyy-MM-ddTHH:mm:ssZ") + "\", \"roomType\": 1 }";

            var requestContent = new StringContent(
                roomRequestJson,
                Encoding.UTF8,
                "application/json");

            // Act
            var result = await client.PostAsync("https://localhost:44375/book", requestContent);

            // Assert
            Assert.True(result.IsSuccessStatusCode);
        }

    }
}
