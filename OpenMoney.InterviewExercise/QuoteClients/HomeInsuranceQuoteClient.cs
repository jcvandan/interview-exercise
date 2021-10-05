using System.Linq;
using OpenMoney.InterviewExercise.Models.Quotes;
using OpenMoney.InterviewExercise.ThirdParties;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public interface IHomeInsuranceQuoteClient
    {
        HomeInsuranceQuote GetQuote(decimal houseValue);
    }

    public class HomeInsuranceQuoteClient : IHomeInsuranceQuoteClient
    {
        private IThirdPartyHomeInsuranceApi _api;
        
        public static decimal ContentsValue = 50_000;

        public HomeInsuranceQuoteClient(IThirdPartyHomeInsuranceApi api)
        {
            _api = api;
        }

        public HomeInsuranceQuote GetQuote(decimal houseValue)
        {
            // check if request is eligible
            if (houseValue > 10_000_000m)
            {
                return null;
            }
            
            var request = new ThirdPartyHomeInsuranceRequest
            {
                HouseValue = houseValue,
                ContentsValue = ContentsValue
            };

            var response = _api.GetQuotes(request).Result.ToArray();

            ThirdPartyHomeInsuranceResponse cheapestQuote = response.Min();
            
            return new HomeInsuranceQuote
            {
                MonthlyPayment = (float) cheapestQuote.MonthlyPayment
            };
        }
    }
}