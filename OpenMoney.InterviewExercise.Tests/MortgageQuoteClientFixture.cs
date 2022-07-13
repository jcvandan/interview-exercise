using Moq;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.QuoteClients;
using OpenMoney.InterviewExercise.ThirdParties;
using System.Collections.Generic;
using Xunit;

namespace OpenMoney.InterviewExercise.Tests
{
    public class MortgageQuoteClientFixture
    {
        private readonly Mock<IThirdPartyMortgageApi> _apiMock = new();

        [Fact]
        public void GetQuote_LoanToValueLessThan10Percent_ShouldReturnQuoteFailAndError()
        {
            decimal houseValue = 100_000M;
            decimal deposit = 9_999M;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new List<ThirdPartyMortgageResponse>
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue,
                Deposit = deposit
            });

            Assert.False(quote.Success);
            Assert.Equal("Loan to value ratio must be 10% or higher", quote.ErrorString);
        }

        [Fact]
        public void GetQuote_LoanToValueEqual10Percent_ShouldReturnQuoteSuccess()
        {
            decimal houseValue = 100_000M;
            decimal deposit = 10_000M;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new List<ThirdPartyMortgageResponse>
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue,
                Deposit = deposit
            });

            Assert.True(quote.Success);
        }

        [Fact]
        public void GetQuote_LoanToValueGreaterThan10Percent_ShouldReturnQuoteSuccess()
        {
            decimal houseValue = 100_000M;
            decimal deposit = 10_001M;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new List<ThirdPartyMortgageResponse>
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue,
                Deposit = deposit
            });

            Assert.True(quote.Success);
        }

        [Fact]
        public void GetQuote_MortgageAmountEqualTo0_ShouldReturnQuoteFailAndError()
        {
            decimal houseValue = 100_000M;
            decimal deposit = 100_000M;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new List<ThirdPartyMortgageResponse>
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue,
                Deposit = deposit
            });

            Assert.False(quote.Success);
            Assert.Equal("Mortgage amount cannot be 0", quote.ErrorString);
        }

        [Fact]
        public void GetQuote_MortgageAmountLessThan0_ShouldReturnQuoteFailAndError()
        {
            decimal houseValue = 100_000M;
            decimal deposit = 100_001M;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new List<ThirdPartyMortgageResponse>
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m }
                });

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest
            {
                HouseValue = houseValue,
                Deposit = deposit
            });

            Assert.False(quote.Success);
            Assert.Equal("Mortgage amount cannot be less than 0", quote.ErrorString);
        }

        [Fact]
        public void GetQuote_MultipleResultsShouldReturn_CheapestQuote()
        {
            const decimal deposit = 10_000;
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new List<ThirdPartyMortgageResponse>
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 300m },
                    new ThirdPartyMortgageResponse { MonthlyPayment = 301m },
                    new ThirdPartyMortgageResponse { MonthlyPayment = 299m }
                });
            
            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });
            
            Assert.Equal(299m, quote.MonthlyPayment);
        }

        [Fact]
        public void GetQuote_NoResultsShouldReturn_QuoteFailAndError()
        {
            const decimal deposit = 10_000;
            const decimal houseValue = 100_000;

            _apiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new List<ThirdPartyMortgageResponse>
                {});

            var mortgageClient = new MortgageQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            });

            Assert.False(quote.Success);
            Assert.Equal("No result returned", quote.ErrorString);
        }
    }
}