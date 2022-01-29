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
        private readonly MortgageQuoteClient _mortgageClient;

        public MortgageQuoteClientFixture()
        {
            _mortgageClient = new MortgageQuoteClient(_apiMock.Object);
        }

        [Fact]
        public void GetQuote_ShouldReturnNull_If_LTV_Under_10percent()
        {
            const float deposit = 99_999;
            const float houseValue = 1_000_000;
            
            var quote = _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });
            
            Assert.Null(quote);
        }

        [Fact]
        public void GetQuote_ShouldReturnQuote_If_LTV_Exactly_10percent()
        {
            const float deposit = 100_000;
            const float houseValue = 1_000_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m }
                });

            var quote = _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.NotNull(quote);
        }

        [Fact]
        public void GetQuote_ShouldReturnMonthlyPayment_When_Only_One_Returned()
        {
            const float deposit = 10_000;
            const float houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m }
                });
            
            var quote = _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });
            
            Assert.Equal(300m, (decimal)quote.MonthlyPayment);
        }
        
        [Fact]
        public void GetQuote_ShouldReturnLowestMonthlyPayment_When_Multiple_Quotes_Returned()
        {
            const float deposit = 10_000;
            const float houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m },
                    new ThirdPartyMortgageResponse { MonthlyPayment = 100m },
                    new ThirdPartyMortgageResponse { MonthlyPayment = 900m },
                });

            var quote = _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.Equal(100m, (decimal)quote.MonthlyPayment);
        }
    }
}