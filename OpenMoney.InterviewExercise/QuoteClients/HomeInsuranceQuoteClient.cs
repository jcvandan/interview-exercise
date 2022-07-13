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
        
        public decimal contentsValue = 50_000M;

        public HomeInsuranceQuoteClient(IThirdPartyHomeInsuranceApi api)
        {
            _api = api;
        }

        public HomeInsuranceQuote GetQuote(GetQuotesRequest getQuotesRequest)
        {
            HomeInsuranceQuote insuranceQuote = new HomeInsuranceQuote();

            if (ValidateQuote(ref insuranceQuote, getQuotesRequest.HouseValue))
            {
                return insuranceQuote;
            }
            
            var request = new ThirdPartyHomeInsuranceRequest
            {
                HouseValue = getQuotesRequest.HouseValue,
                ContentsValue = contentsValue
            };

            var response = _api.GetQuotes(request).GetAwaiter().GetResult().ToList();

            ThirdPartyHomeInsuranceResponse cheapestQuote = null;

            foreach (var quote in response)
            {
                if (cheapestQuote == null)
                {
                    cheapestQuote = quote;
                }
                else if (cheapestQuote.MonthlyPayment > quote.MonthlyPayment)
                {
                    cheapestQuote = quote;
                }
            }

            insuranceQuote.MonthlyPayment = cheapestQuote.MonthlyPayment;

            return insuranceQuote;
        }

        private bool ValidateQuote(ref HomeInsuranceQuote insuranceQuote, decimal houseValue)
        {
            if (houseValue > 10_000_000M)
            {
                insuranceQuote.Success = false;
                insuranceQuote.Error = "House valse must be less than "
                return false;
            }

            insuranceQuote.Success = true;
            return true;
        }
    }
}