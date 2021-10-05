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
        public void GetQuote_ShouldReturnAQuote()
        {
            const decimal houseValue = 100_000m;
            const decimal deposit = 10_000m;
            const decimal mortgageAmount = houseValue - deposit;
            const decimal monthlyPayment = 100m;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyMortgageRequest>(r => r.MortgageAmount == mortgageAmount)))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = monthlyPayment }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(houseValue, deposit);

            Assert.Equal(monthlyPayment, (decimal)quote.MonthlyPayment);
        }

        [Fact]
        public void GetQuote_WhenDepositIsLessThanThreshold_ThenShouldReturnNull()
        {
            const decimal houseValue = 100_000m;
            const decimal deposit = 1_000m;

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(houseValue, deposit);

            Assert.Null(quote);
        }

        [Fact]
        public void GetQuote_WhenMultipleQuotesReturned_THenShouldReturnTheCheapest()
        {
            const decimal houseValue = 100_000m;
            const decimal deposit = 10_000m;
            const decimal mortgageAmount = houseValue - deposit;
            const decimal monthlyPayment = 100m;
            const decimal difference = 10m;

            _apiMock
                .Setup(api => api.GetQuotes(It.Is<ThirdPartyMortgageRequest>(r => r.MortgageAmount == mortgageAmount)))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = monthlyPayment - difference },
                    new ThirdPartyMortgageResponse { MonthlyPayment = monthlyPayment },
                    new ThirdPartyMortgageResponse { MonthlyPayment = monthlyPayment + difference }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(houseValue, deposit);

            Assert.Equal(monthlyPayment - difference, (decimal)quote.MonthlyPayment);
        }
    }
}