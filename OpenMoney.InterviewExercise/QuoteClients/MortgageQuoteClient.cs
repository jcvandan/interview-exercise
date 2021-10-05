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
        private readonly decimal _depositMinPercent = 0.1m;

        public MortgageQuoteClient(IThirdPartyMortgageApi api)
        {
            _api = api;
        }
        
        public MortgageQuote GetQuote(decimal houseValue, decimal deposit)
        {
            // check if mortgage request is eligible
            if (!DepositIsWithinThreshold(houseValue, deposit)) { return null; };
            
            var mortgageAmount = houseValue - deposit;
            
            var request = new ThirdPartyMortgageRequest
            {
                MortgageAmount = mortgageAmount
            };

            var response = _api.GetQuotes(request).Result.ToArray();

            var cheapestQuote = response.Where(quote => quote.MonthlyPayment == response.Min(quote => quote.MonthlyPayment))
                                        .First();
            
            return new MortgageQuote
            {
                MonthlyPayment = (float) cheapestQuote.MonthlyPayment
            };
        }

        protected bool DepositIsWithinThreshold(decimal houseValue, decimal deposit)
        {
            var loanToValueFraction = deposit / houseValue;
            return (loanToValueFraction >= _depositMinPercent);
        }
    }
}