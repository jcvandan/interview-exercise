using System.Linq;
using OpenMoney.InterviewExercise.Models.Quotes;
using OpenMoney.InterviewExercise.ThirdParties;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public interface IMortgageQuoteClient
    {
        MortgageQuote GetQuote(decimal houseValue, decimal deposit);
    }

    public class MortgageQuoteClient : IMortgageQuoteClient
    {
        private readonly IThirdPartyMortgageApi _api;

        public MortgageQuoteClient(IThirdPartyMortgageApi api)
        {
            _api = api;
        }
        
        public MortgageQuote GetQuote(decimal houseValue, decimal deposit)
        {
            // check if mortgage request is eligible
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

            var response = _api.GetQuotes(request).Result.ToArray();

            ThirdPartyMortgageResponse cheapestQuote = response.Min();
            
            return new MortgageQuote
            {
                MonthlyPayment = (float) cheapestQuote.MonthlyPayment
            };
        }
    }
}