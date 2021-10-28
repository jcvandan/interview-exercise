using System.Linq;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.Models.Quotes;
using OpenMoney.InterviewExercise.ThirdParties;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public interface IHomeInsuranceQuoteClient
    {
        HomeInsuranceQuote GetQuote(GetQuotesRequest getQuotesRequest);
    }

    public class HomeInsuranceQuoteClient : IHomeInsuranceQuoteClient
    {
        private IThirdPartyHomeInsuranceApi _api;
        
        public decimal contentsValue = 50_000;

        public HomeInsuranceQuoteClient(IThirdPartyHomeInsuranceApi api)
        {
            _api = api;
        }

        public HomeInsuranceQuote GetQuote(GetQuotesRequest getQuotesRequest)
        {
			if(!eligibleRequest(getQuotesRequest)) {
                return null;
			}
            
            var request = new ThirdPartyHomeInsuranceRequest
            {
                HouseValue = getQuotesRequest.HouseValue,
                ContentsValue = contentsValue
            };

            var response = _api.GetQuotes(request).GetAwaiter().GetResult().ToArray();

            float cheapestQuote = response.Select(x => x.MonthlyPayment).Min();

            return new HomeInsuranceQuote {
                MonthlyPayment = cheapestQuote
            };
        }

        private bool eligibleRequest(GetQuotesRequest getQuotesRequest) => getQuotesRequest.HouseValue <= 10_000_000;
    }
}