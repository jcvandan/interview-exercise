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
    public class MortgageQuoteClientFixture
    {
        private readonly Mock<IThirdPartyMortgageApi> _apiMock = new();
        private readonly MortgageQuoteClient _mortgageClient;

        public MortgageQuoteClientFixture()
        {
            _mortgageClient = new MortgageQuoteClient(_apiMock.Object);
        }

        [Fact]
        public async Task GetQuote_ShouldReturnFailure_If_LTV_Under_10percent()
        {
            const decimal deposit = 99_999;
            const decimal houseValue = 1_000_000;
            
            var quote = await _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });
            
            Assert.False(quote.Succeeded);
            Assert.Equal("Loan-to-value must be over 10%", quote.ErrorMessage);
        }

        [Fact]
        public async Task GetQuote_ShouldReturnSuccess_If_LTV_Exactly_10percent()
        {
            const decimal deposit = 100_000;
            const decimal houseValue = 1_000_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m }
                });

            var quote = await _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.True(quote.Succeeded);
        }

        [Fact]
        public async Task GetQuote_ShouldReturnMonthlyPayment_When_Only_One_Returned()
        {
            const decimal deposit = 10_000;
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m }
                });
            
            var quote = await _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.True(quote.Succeeded);
            Assert.Equal(300m, quote.MonthlyPayment);
        }
        
        [Fact]
        public async Task GetQuote_ShouldReturnLowestMonthlyPayment_When_Multiple_Quotes_Returned()
        {
            const decimal deposit = 10_000;
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m },
                    new ThirdPartyMortgageResponse { MonthlyPayment = 100m },
                    new ThirdPartyMortgageResponse { MonthlyPayment = 900m },
                });

            var quote = await _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.True(quote.Succeeded);
            Assert.Equal(100m, quote.MonthlyPayment);
        }

        [Fact]
        public async Task GetQuote_Should_Pass_Correct_Mortgage_Amount_To_ThirdParty()
        {
            const decimal deposit = 10_000;
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(
                    It.Is<ThirdPartyMortgageRequest>(r => r.MortgageAmount == 90_000)
                ))
                .ReturnsAsync(new[]
                {
                    new ThirdPartyMortgageResponse {MonthlyPayment = 100m},
                });

            var quote = await _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.True(quote.Succeeded);
            Assert.Equal(100m, quote.MonthlyPayment);
        }

        [Fact]
        public async Task GetQuote_ShouldReturnFailure_When_No_Quotes_Returned()
        {
            const decimal deposit = 10_000;
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(Enumerable.Empty<ThirdPartyMortgageResponse>());

            var quote = await _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.False(quote.Succeeded);
            Assert.Equal("No quotes were returned", quote.ErrorMessage);
        }

        [Fact]
        public async Task GetQuote_ShouldReturnFailure_When_Third_Party_Throws()
        {
            const decimal deposit = 10_000;
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .Throws(new Exception("Test exception message"));

            var quote = await _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.False(quote.Succeeded);
            Assert.Equal("Test exception message", quote.ErrorMessage);
        }

        [Fact]
        public async Task GetQuote_ShouldReturnFailure_If_Mortgage_Amount_Is_Negative()
        {
            const decimal deposit = 1_000_000;
            const decimal houseValue = 100_000;
            
            var quote = await _mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.False(quote.Succeeded);
            Assert.Equal("Mortgage amount cannot be negative", quote.ErrorMessage);
        }
    }
}