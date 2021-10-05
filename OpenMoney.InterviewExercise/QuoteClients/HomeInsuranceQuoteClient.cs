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
        
        public static decimal ContentsValue = 50_000;

        public HomeInsuranceQuoteClient(IThirdPartyHomeInsuranceApi api)
        {
            _api = api;
        }

        public HomeInsuranceQuote GetQuote(GetQuotesRequest getQuotesRequest)
        {
            // check if request is eligible
            if (getQuotesRequest.HouseValue > 10_000_000d)
            {
                return null;
            }
            
            var request = new ThirdPartyHomeInsuranceRequest
            {
                HouseValue = (decimal) getQuotesRequest.HouseValue,
                ContentsValue = ContentsValue
            };

            var response = _api.GetQuotes(request).GetAwaiter().GetResult().ToArray();

            ThirdPartyHomeInsuranceResponse cheapestQuote = null;
            
            for (var i = 0; i < response.Length; i++)
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
            
            return new HomeInsuranceQuote
            {
                MonthlyPayment = (float) cheapestQuote.MonthlyPayment
            };
        }
    }
}