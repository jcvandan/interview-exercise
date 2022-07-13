using Moq;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.Models.Quotes;
using OpenMoney.InterviewExercise.QuoteClients;
using OpenMoney.InterviewExercise.ThirdParties;
using System.Collections.Generic;
using Xunit;

namespace OpenMoney.InterviewExercise.Tests
{
    public class QuoteOrchestratorFixture
    {
        private readonly Mock<IThirdPartyMortgageApi> _mortgageClientApiMock = new();
        private readonly Mock<IThirdPartyHomeInsuranceApi> _homeInsuranceApiMock = new();

        [Fact]
        public void GetQuotes_PassCorrectValuesToMortgageClient_ReturnQuoteSuccess()
        {
            const decimal deposit = 10_000M;
            const decimal houseValue = 100_000M;

            var request = new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            };

            _mortgageClientApiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new List<ThirdPartyMortgageResponse>
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 500M }
                });

            var orchestrator = new QuoteOrchestrator(
                new HomeInsuranceQuoteClient(_homeInsuranceApiMock.Object),
                new MortgageQuoteClient(_mortgageClientApiMock.Object)
                );

            var response = orchestrator.GetQuotes(request);

            Assert.Equal(500M, response.MortgageQuote.MonthlyPayment);
        }

        [Fact]
        public void GetQuotes_PassCorrectValuesToHomeInsuranceClient_ReturnQuoteSuccess()
        {
            const decimal deposit = 10_000;
            const decimal houseValue = 100_000;

            var request = new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            };

            _homeInsuranceApiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyHomeInsuranceRequest>()))
                .ReturnsAsync(new List<ThirdPartyHomeInsuranceResponse>
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30F }
                });

            var orchestrator = new QuoteOrchestrator(
                new HomeInsuranceQuoteClient(_homeInsuranceApiMock.Object),
                new MortgageQuoteClient(_mortgageClientApiMock.Object)
                );

            var response = orchestrator.GetQuotes(request);

            Assert.Equal(30F, response.HomeInsuranceQuote.MonthlyPayment);
        }

        [Fact]
        public void GetQuotes_PassCorrectValuesToHomeInsuranceAndMortgageQuoteClient_ReturnBothQuoteSuccess()
        {
            const decimal deposit = 10_000;
            const decimal houseValue = 100_000;

            var request = new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            };

            _mortgageClientApiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new List<ThirdPartyMortgageResponse>
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 500M }
                });

            _homeInsuranceApiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyHomeInsuranceRequest>()))
                .ReturnsAsync(new List<ThirdPartyHomeInsuranceResponse>
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30F }
                });

            var orchestrator = new QuoteOrchestrator(
                new HomeInsuranceQuoteClient(_homeInsuranceApiMock.Object),
                new MortgageQuoteClient(_mortgageClientApiMock.Object)
                );

            var response = orchestrator.GetQuotes(request);

            Assert.Equal(30F, response.HomeInsuranceQuote.MonthlyPayment);
            Assert.Equal(500M, response.MortgageQuote.MonthlyPayment);
        }

        [Fact]
        public void GetQuotes_PassInvalidValuesForMortgageQuote_ShouldReturnMortgageFailAndInsuranceSuccess()
        {
            const decimal deposit = 9_999;
            const decimal houseValue = 100_000;

            var request = new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            };

            _mortgageClientApiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new List<ThirdPartyMortgageResponse>
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 500M }
                });

            _homeInsuranceApiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyHomeInsuranceRequest>()))
                .ReturnsAsync(new List<ThirdPartyHomeInsuranceResponse>
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30F }
                });

            var orchestrator = new QuoteOrchestrator(
                new HomeInsuranceQuoteClient(_homeInsuranceApiMock.Object),
                new MortgageQuoteClient(_mortgageClientApiMock.Object)
                );

            var response = orchestrator.GetQuotes(request);

            Assert.False(response.MortgageQuote.Success);
            Assert.Equal(30F, response.HomeInsuranceQuote.MonthlyPayment);
        }

        [Fact]
        public void GetQuotes_PassInvalidValuesForInsuranceQuote_ShouldReturnInsuranceFailAndMortgageSuccess()
        {
            const decimal deposit = 2_000_000M;
            const decimal houseValue = 10_000_001M;

            var request = new GetQuotesRequest
            {
                Deposit = deposit,
                HouseValue = houseValue
            };

            _mortgageClientApiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyMortgageRequest>()))
                .ReturnsAsync(new List<ThirdPartyMortgageResponse>
                {
                    new ThirdPartyMortgageResponse { MonthlyPayment = 500M }
                });

            _homeInsuranceApiMock
                .Setup(api => api.GetQuotes(It.IsAny<ThirdPartyHomeInsuranceRequest>()))
                .ReturnsAsync(new List<ThirdPartyHomeInsuranceResponse>
                {
                    new ThirdPartyHomeInsuranceResponse { MonthlyPayment = 30F }
                });

            var orchestrator = new QuoteOrchestrator(
                new HomeInsuranceQuoteClient(_homeInsuranceApiMock.Object),
                new MortgageQuoteClient(_mortgageClientApiMock.Object)
                );

            var response = orchestrator.GetQuotes(request);

            Assert.False(response.HomeInsuranceQuote.Success);
            Assert.Equal(500m, response.MortgageQuote.MonthlyPayment);
        }
    }
}