using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.QuoteClients;
using OpenMoney.InterviewExercise.ThirdParties;
using Xunit;

namespace OpenMoney.InterviewExercise.Tests
{
    public class HomeInsuranceQuoteClientFixture
    {
        private readonly Mock<IThirdPartyHomeInsuranceApi> _apiMock = new();
        private readonly HomeInsuranceQuoteClient _homeInsuranceClient;

        public HomeInsuranceQuoteClientFixture()
        {
            _homeInsuranceClient = new HomeInsuranceQuoteClient(_apiMock.Object);
        }

        [Fact]
        public async Task GetQuote_ShouldReturnFailure_IfHouseValue_Over10Mill()
        {
            const decimal houseValue = 10_000_001;
            
            var quote = await _homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });
            
            Assert.False(quote.Succeeded);
            Assert.Equal("House value cannot exceed 10,000,000", quote.ErrorMessage);
        }
        
        [Fact]
        public async Task GetQuote_ShouldReturnQuote_IfHouseValue_Is_Exactly_10Million()
        {
            // The test above proves that 10,000,001 returns null, but not that the threshold 
            // is actually set at 10 million (it could be set to 9 million and that test would still pass).
            // This test asserts that the threshold is set correctly
            
            const decimal houseValue = 10_000_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyHomeInsuranceRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 }
                });

            var quote = await _homeInsuranceClient.GetQuote(new GetQuotesRequest {
                HouseValue = houseValue
            });

            Assert.True(quote.Succeeded);
        }

        [Fact]
        public async Task GetQuote_Should_Return_Quote_When_Only_One_Available()
        {
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r =>
                    r.ContentsValue == 50_000 && r.HouseValue == houseValue)))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 }
                });
            
            var quote = await _homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });

            Assert.True(quote.Succeeded);
            Assert.Equal(30m, quote.MonthlyPayment);
        }

        [Fact]
        public async Task GetQuote_ShouldReturnCheapestMonthlyPayment_When_Multiple_Quotes_Returned()
        {
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r =>
                    r.ContentsValue == 50_000 && r.HouseValue == houseValue)))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 },
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 1000 },
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 12 },
                });

            var quote = await _homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });

            Assert.True(quote.Succeeded);
            Assert.Equal(12m, quote.MonthlyPayment);
        }
        
        [Fact]
        public async Task GetQuote_ShouldReturnFailure_When_No_Quotes_Returned()
        {
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r =>
                    r.ContentsValue == 50_000 && r.HouseValue == houseValue)))
                .ReturnsAsync(Enumerable.Empty<ThirdPartyHomeInsuranceResponse>());

            var quote = await _homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });

            Assert.False(quote.Succeeded);
            Assert.Equal("No quotes returned", quote.ErrorMessage);
        }



        [Fact]
        public async Task GetQuote_ShouldReturnFailure_When_Third_Party_Throws()
        {
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyHomeInsuranceRequest>()))
                .Throws(new Exception("Test exception message"));

            var quote = await _homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });

            Assert.False(quote.Succeeded);
            Assert.Equal("Test exception message", quote.ErrorMessage);
        }
    }
}