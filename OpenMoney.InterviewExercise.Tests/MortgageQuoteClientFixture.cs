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
        public void GetQuote_ShouldCallWithCorrectMortgageAmount()
        {
            const decimal houseValue = 100_000m;
            const decimal deposit = 25_000m;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 700 }
                });
            
            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(houseValue, deposit);

            _apiMock.Verify(s => s.GetQuotes(It.Is<ThirdPartyMortgageRequest>(r => r.MortgageAmount == 75_000m)));
        }

        [Fact]
        public void GetQuote_IfLtvLessThan10Percent_ShouldReturnNull_ShouldNotSendRequest()
        {
            const decimal houseValue = 100_000m;
            const decimal deposit = 9_999m;

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(houseValue, deposit);

            _apiMock.Verify(s => s.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()), Times.Never);
            Assert.Null(quote);
        }

        [Fact]
        public void GetQuote_ShouldReturnCheapestQuote()
        {
            const decimal houseValue = 100_000m;
            const decimal deposit = 25_000m;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 700 }, new ThirdPartyMortgageResponse { MonthlyPayment = 500 }, new ThirdPartyMortgageResponse { MonthlyPayment = 600 }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(houseValue, deposit);

            Assert.Equal(500m, (decimal)quote.MonthlyPayment);
        }
    }
}