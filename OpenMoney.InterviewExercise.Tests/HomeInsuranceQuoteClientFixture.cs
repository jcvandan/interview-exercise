using System.Linq;
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
        public void GetQuote_ShouldReturnNull_IfHouseValue_Over10Mill()
        {
            const float houseValue = 10_000_001;
            
            var mortgageClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });
            
            Assert.Null(quote);
        }
        
        [Fact]
        public void GetQuote_ShouldReturnQuote_IfHouseValue_Is_Exactly_10Million()
        {
            // The test above proves that 10,000,001 returns null, but not that the threshold 
            // is actually set at 10 million (it could be set to 9 million and that test would still pass).
            // This test asserts that the threshold is set correctly
            
            const float houseValue = 10_000_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyHomeInsuranceRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 }
                });

            var quote = _homeInsuranceClient.GetQuote(new GetQuotesRequest {
                HouseValue = houseValue
            });

            Assert.NotNull(quote);
        }

        [Fact]
        public void GetQuote_Should_Return_Quote_When_Only_One_Available()
        {
            const float houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r =>
                    r.ContentsValue == 50_000 && r.HouseValue == (decimal) houseValue)))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 }
                });
            
            var quote = _homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });
            
            Assert.Equal(30m, (decimal)quote.MonthlyPayment);
        }

        [Fact]
        public void GetQuote_ShouldReturnCheapestMonthlyPayment_When_Multiple_Quotes_Returned()
        {
            const float houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r =>
                    r.ContentsValue == 50_000 && r.HouseValue == (decimal)houseValue)))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 },
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 1000 },
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 12 },
                });

            var quote = _homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });

            Assert.Equal(12m, (decimal)quote.MonthlyPayment);
        }



        [Fact]
        public void GetQuote_ShouldReturnNull_When_No_Quotes_Returned()
        {
            const float houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r =>
                    r.ContentsValue == 50_000 && r.HouseValue == (decimal)houseValue)))
                .ReturnsAsync(Enumerable.Empty<ThirdPartyHomeInsuranceResponse>());

            var quote = _homeInsuranceClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });

            Assert.Null(quote);
        }
    }
}