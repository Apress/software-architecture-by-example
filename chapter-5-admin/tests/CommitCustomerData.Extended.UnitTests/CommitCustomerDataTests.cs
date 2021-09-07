using System;
using Xunit;

namespace CommitCustomerData.Extended.UnitTests
{    
    public class CommitCustomerDataTests
    {
        [Fact]
        public void CommitCustomerData_After_ParseJson()
        {
            // Arrange
            string json = "[{\"Name\":\"Customer bcc51794-dff1-4be0-a29d-9708a5e09ce7\",\"Address\":\"Customer Address\",\"Email\":\"customer1454@domain.com\",\"CreditLimit\":393},{\"Name\":\"Customer b7f84bc4-5c12-46cc-b68f-9aa9744f2fec\",\"Address\":\"Customer Address\",\"Email\":\"customer8013@domain.com\",\"CreditLimit\":385},{\"Name\":\"Customer ae86d250-feb8-44ba-9d78-7219e36cbd1f\",\"Address\":\"Customer Address\",\"Email\":\"customer8280@domain.com\",\"CreditLimit\":808},{\"Name\":\"Customer 26014c6f-99ac-43be-bedb-109c3ed1b6e5\",\"Address\":\"Customer Address\",\"Email\":\"customer8125@domain.com\",\"CreditLimit\":262},{\"Name\":\"Customer a422cc2b-e18e-4d02-a7ec-5b74f42cdaa5\",\"Address\":\"Customer Address\",\"Email\":\"customer7497@domain.com\",\"CreditLimit\":917},{\"Name\":\"Customer 3e1c0455-b5d7-44b6-a2e5-939d7a54ca42\",\"Address\":\"Customer Address\",\"Email\":\"customer9088@domain.com\",\"CreditLimit\":654},{\"Name\":\"Customer 2a0c65ca-0757-4f40-ac5d-767e06100f38\",\"Address\":\"Customer Address\",\"Email\":\"customer5048 @domain.com\",\"CreditLimit\":836}]";
            var commitCustomerData = new CommitCustomerDataExtended.CommitCustomerData();

            // Act            
            commitCustomerData.After(json);

            // Assert
        }
    }
}
