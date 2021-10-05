using System.Linq;
using System.Threading.Tasks;
using OpenMoney.InterviewExercise.Models.Quotes;
using OpenMoney.InterviewExercise.ThirdParties;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public class MortgageQuoteClient : IMortgageQuoteClient
    {
        private readonly IThirdPartyMortgageApi _api;

        public MortgageQuoteClient(IThirdPartyMortgageApi api)
        {
            _api = api;
        }
        
        public async Task<MortgageQuote> GetQuote(decimal houseValue, decimal deposit)
        {
            var loanToValueFraction = deposit / houseValue;
            if (loanToValueFraction < 0.1m)
            {
                return null;
            }
            
            var mortgageAmount = houseValue - deposit;
            
            var request = new ThirdPartyMortgageRequest
            {
                MortgageAmount = mortgageAmount
            };

            var response = (await _api.GetQuotes(request)).ToList();

            ThirdPartyMortgageResponse cheapestQuote = response
                .OrderBy(a => a.MonthlyPayment)
                .First();
            
            return new MortgageQuote
            {
                MonthlyPayment = (float) cheapestQuote.MonthlyPayment
            };
        }
    }
}