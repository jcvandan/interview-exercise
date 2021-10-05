using Moq;
using OpenMoney.InterviewExercise.QuoteClients;
using OpenMoney.InterviewExercise.ThirdParties;
using Xunit;

namespace OpenMoney.InterviewExercise.Tests
{
    public class MortgageQuoteClientFixture
    {
        private readonly Mock<IThirdPartyMortgageApi> _apiMock = new();

        [Fact]
        public async void GetQuote_ShouldReturnNullForWrongProportion()
        {
            const decimal houseValue = 100_000m;
            const decimal deposit = 100m;
            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 30 }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var response = await mortgageClient.GetQuote(houseValue, deposit);

            Assert.Null(response);
        }

        [Fact]
        public async void GetQuote_ShouldSendCorrectMortgageAmount()
        {
            const decimal houseValue = 100_000m;
            const decimal deposit = 60_000m;
            decimal mortgageAmount = 0;
            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 30 }
                })
            .Callback<ThirdPartyMortgageRequest>(c => mortgageAmount = c.MortgageAmount) ;

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            await mortgageClient.GetQuote(houseValue, deposit);

            Assert.Equal(40_000m, mortgageAmount);
        }

        [Fact]
        public async void GetQuote_ShouldReturn_AQuote()
        {
            const decimal houseValue = 100_000m;
            const decimal deposit = 60_000m;
            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 30 }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = await mortgageClient.GetQuote(houseValue, deposit);

            Assert.Equal(30m, (decimal)quote.MonthlyPayment);
        }

        [Fact]
        public async void GetQuote_ShouldReturnTheCheapestQuote()
        {
            const decimal houseValue = 100_000m;
            const decimal deposit = 60_000m;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 30 },
                    new ThirdPartyMortgageResponse { MonthlyPayment = 22 },
                    new ThirdPartyMortgageResponse { MonthlyPayment = 60 },
                    new ThirdPartyMortgageResponse { MonthlyPayment = 35 }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = await mortgageClient.GetQuote(houseValue, deposit);

            Assert.Equal(22m, (decimal)quote.MonthlyPayment);
        }
    }
}