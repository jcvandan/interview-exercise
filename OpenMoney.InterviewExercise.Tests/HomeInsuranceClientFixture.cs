using Moq;
using OpenMoney.InterviewExercise.QuoteClients;
using OpenMoney.InterviewExercise.ThirdParties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenMoney.InterviewExercise.Tests
{
    public class HomeInsuranceClientFixture
    {
        private readonly Mock<IThirdPartyHomeInsuranceApi> _apiMock = new();

        [Fact]
        public void GetQuote_ShouldReturnNull_IfHouseValue_Over10Mill()
        {
            const decimal houseValue = 10_000_001m;

            var mortgageClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(houseValue);

            Assert.Null(quote);
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

            var mortgageClient = new HomeInsuranceQuoteClient(_apiMock.Object);
            var quote = mortgageClient.GetQuote(houseValue);

            Assert.Equal(30m, (decimal)quote.MonthlyPayment);
        }
    }
}
