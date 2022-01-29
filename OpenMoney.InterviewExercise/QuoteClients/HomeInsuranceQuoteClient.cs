using System.Linq;
using System.Threading.Tasks;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.Models.Quotes;
using OpenMoney.InterviewExercise.ThirdParties;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public interface IHomeInsuranceQuoteClient
    {
        Task<HomeInsuranceQuote> GetQuote(GetQuotesRequest getQuotesRequest);
    }

    public class HomeInsuranceQuoteClient : IHomeInsuranceQuoteClient
    {
        private readonly IThirdPartyHomeInsuranceApi _api;
        
        public decimal contentsValue = 50_000;

        public HomeInsuranceQuoteClient(IThirdPartyHomeInsuranceApi api)
        {
            _api = api;
        }

        public async Task<HomeInsuranceQuote> GetQuote(GetQuotesRequest getQuotesRequest)
        {
            // check if request is eligible
            if (getQuotesRequest.HouseValue > 10_000_000m)
            {
                return null;
            }
            
            var request = new ThirdPartyHomeInsuranceRequest
            {
                HouseValue = (decimal) getQuotesRequest.HouseValue,
                ContentsValue = contentsValue
            };

            var response = (await _api.GetQuotes(request)).ToList();

            ThirdPartyHomeInsuranceResponse cheapestQuote = null;
            
            for (var i = 0; i < response.Count; i++)
            {
                var quote = response[i];

                if (cheapestQuote == null)
                {
                    cheapestQuote = quote;
                }
                else if (cheapestQuote.MonthlyPayment > quote.MonthlyPayment)
                {
                    cheapestQuote = quote;
                }
            }

            if (cheapestQuote is null)
                return null;
            
            return new HomeInsuranceQuote
            {
                MonthlyPayment = (decimal)cheapestQuote.MonthlyPayment
            };
        }
    }
}