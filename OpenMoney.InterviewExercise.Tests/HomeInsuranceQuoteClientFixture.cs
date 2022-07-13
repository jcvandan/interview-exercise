using Moq;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.QuoteClients;
using OpenMoney.InterviewExercise.ThirdParties;
using System.Collections.Generic;
using Xunit;

namespace OpenMoney.InterviewExercise.Tests
{
    public class HomeInsuranceQuoteClientFixture
    {
        private readonly Mock<IThirdPartyHomeInsuranceApi> _apiMock = new();

        [Fact]
        public void GetQuote_HouseValueOver10Mill_ShouldReturnQuoteFailAndError()
        {
            const decimal houseValue = 10_000_001M;
            
            var homeInsuranceClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });
            
            Assert.False(quote.Success);
            Assert.Equal("House value must be less than 10,000,000", quote.ErrorString);
        }

        [Fact]
        public void GetQuote_HouseValueEqualTo10Mill_ShouldReturnQuoteSuccess()
        {
            const decimal houseValue = 10_000_000M;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyHomeInsuranceRequest>()))
                .ReturnsAsync(new List<ThirdPartyHomeInsuranceResponse>
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 }
                });

            var homeInsuranceClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });

            Assert.True(quote.Success);
        }

        [Fact]
        public void GetQuote_HouseValueUnder10Mill_ShouldReturnQuoteSuccess()
        {
            const decimal houseValue = 9_999_999M;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyHomeInsuranceRequest>()))
                .ReturnsAsync(new List<ThirdPartyHomeInsuranceResponse>
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 }
                });

            var homeInsuranceClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });

            Assert.True(quote.Success);
        }

        [Fact]
        public void GetQuote_MultipleResultsShouldReturn_CheapestQuote()
        {
            const decimal houseValue = 100_000M;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyHomeInsuranceRequest>()))
                .ReturnsAsync(new List<ThirdPartyHomeInsuranceResponse>
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 31 },
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 },
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 29 }
                });

            var homeInsuranceClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });
            
            Assert.Equal(29F, quote.MonthlyPayment);
        }
    }
}