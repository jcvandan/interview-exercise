using Moq;
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
            const decimal houseValue = 10_000_001m;
            
            var homeInsuranceClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = homeInsuranceClient.GetQuote(houseValue);
            
            Assert.Null(quote);
        }

        [Fact]
        public void GetQuote_ShouldCallWithCorrectRequest()
        {
            const decimal houseValue = 100_000m;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyHomeInsuranceRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 }
                });

            var homeInsuranceClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = homeInsuranceClient.GetQuote(houseValue);

            _apiMock.Verify(s => s.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r => r.ContentsValue == 50_000m && r.HouseValue == 100_000m)));
        }

        [Fact]
        public void GetQuote_ShouldReturn_AQuote()
        {
            const decimal houseValue = 100_000m;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r =>
                    r.ContentsValue == HomeInsuranceQuoteClient.ContentsValue && r.HouseValue == houseValue)))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 }
                });
            
            var homeInsuranceClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = homeInsuranceClient.GetQuote(houseValue);
            
            Assert.Equal(30m, (decimal)quote.MonthlyPayment);
        }

        [Fact]
        public void GetQuote_ShouldReturnCheapestQuote()
        {
            const decimal houseValue = 100_000m;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r =>
                    r.ContentsValue == HomeInsuranceQuoteClient.ContentsValue && r.HouseValue == houseValue)))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 }, new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 20 }, new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 40 }
                });

            var homeInsuranceClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = homeInsuranceClient.GetQuote(houseValue);

            Assert.Equal(20m, (decimal)quote.MonthlyPayment);
        }
    }
}