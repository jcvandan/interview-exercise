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
        public void GetQuote_ShouldReturn_AQuote()
        {
            const float houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r =>
                    r.ContentsValue == HomeInsuranceQuoteClient.ContentsValue && r.HouseValue == (decimal) houseValue)))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 }
                });
            
            var mortgageClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue
            });
            
            Assert.Equal(30m, (decimal)quote.MonthlyPayment);
        }
    }
}