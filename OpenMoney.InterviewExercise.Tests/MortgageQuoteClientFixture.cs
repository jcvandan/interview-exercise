using Moq;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.QuoteClients;
using OpenMoney.InterviewExercise.ThirdParties;
using Xunit;

namespace OpenMoney.InterviewExercise.Tests
{
    public class MortgageQuoteClientFixture
    {
        private readonly Mock<IThirdPartyMortgageApi> _apiMock = new();

        [Fact]
        public void GetQuote_ShouldReturnNull_IfDepositUnder10Percent()
        {
            const decimal deposit = 9_000;
            const decimal houseValue = 100_000;
            
            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });
            
            Assert.Null(quote);
        }

        [Fact]
        public void GetQuote_ShouldReturnNull_IfHouseValueOver10Mil() {
            const decimal deposit = 2_000_000;
            const decimal houseValue = 11_000_000;

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.Null(quote);
        }

        [Fact]
        public void GetQuote_ShouldReturn_AQuote()
        {
            const decimal deposit = 10_000;
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m }
                });
            
            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });
            
            Assert.Equal(300m, quote.MonthlyPayment);
        }

        [Fact]
        public void GetQuote_ShouldReturn_SmallestQuote() {
            const decimal deposit = 10_000;
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 200m },
                    new ThirdPartyMortgageResponse { MonthlyPayment = 500m }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.Equal(200m, quote.MonthlyPayment);
        }
    }
}