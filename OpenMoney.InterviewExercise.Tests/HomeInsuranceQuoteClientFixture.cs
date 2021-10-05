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
        public async void GetQuote_ShouldReturnNull_IfHouseValue_Over10Mill()
        {
            const decimal houseValue = 10_000_001m;

            var mortgageClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = await mortgageClient.GetQuote(houseValue);

            Assert.Null(quote);
        }

        [Fact]
        public async void GetQuote_ShouldReturn_AQuote()
        {
            const decimal houseValue = 100_000m;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r =>
                    r.ContentsValue == HomeInsuranceQuoteClient.ContentsValue && r.HouseValue == houseValue)))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 }
                });

            var mortgageClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = await mortgageClient.GetQuote(houseValue);

            Assert.Equal(30m, (decimal)quote.MonthlyPayment);
        }

        [Fact]
        public async void GetQuote_ShouldReturnTheCheapestQuote()
        {
            const decimal houseValue = 100_000m;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyHomeInsuranceRequest>(r =>
                    r.ContentsValue == HomeInsuranceQuoteClient.ContentsValue && r.HouseValue == houseValue)))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30 },
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 22 },
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 60 },
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 35 }
                });

            var mortgageClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = await mortgageClient.GetQuote(houseValue);

            Assert.Equal(22m, (decimal)quote.MonthlyPayment);
        }
    }
}