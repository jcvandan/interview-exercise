using System.Linq;
using System.Threading.Tasks;
using OpenMoney.InterviewExercise.Models.Quotes;
using OpenMoney.InterviewExercise.ThirdParties;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public class HomeInsuranceQuoteClient : IHomeInsuranceQuoteClient
    {
        private IThirdPartyHomeInsuranceApi _api;
        
        public static decimal ContentsValue = 50_000;

        public HomeInsuranceQuoteClient(IThirdPartyHomeInsuranceApi api)
        {
            _api = api;
        }

        public async Task<HomeInsuranceQuote> GetQuote(decimal houseValue)
        {
            if (houseValue > 10_000_000m)
            {
                return null;
            }
            
            var request = new ThirdPartyHomeInsuranceRequest
            {
                HouseValue = houseValue,
                ContentsValue = ContentsValue
            };

            var response = (await _api.GetQuotes(request)).ToList();

            ThirdPartyHomeInsuranceResponse cheapestQuote = response
                .OrderBy(a => a.MonthlyPayment)
                .First();
            
            return new HomeInsuranceQuote
            {
                MonthlyPayment = (float) cheapestQuote.MonthlyPayment
            };
        }
    }
}