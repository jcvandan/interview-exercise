using System.Linq;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.Models.Quotes;
using OpenMoney.InterviewExercise.ThirdParties;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public interface IMortgageQuoteClient
    {
        MortgageQuote GetQuote(GetQuotesRequest getQuotesRequest);
    }

    public class MortgageQuoteClient : IMortgageQuoteClient
    {
        private readonly IThirdPartyMortgageApi _api;

        public MortgageQuoteClient(IThirdPartyMortgageApi api)
        {
            _api = api;
        }
        
        public MortgageQuote GetQuote(GetQuotesRequest getQuotesRequest)
        {
            // check if mortgage request is eligible
            var loanToValueFraction = getQuotesRequest.Deposit / getQuotesRequest.HouseValue;
            if (loanToValueFraction < 0.1m)
            {
                return null;
            }

            if(getQuotesRequest.HouseValue > 10_000_000) {
                return null;
			}
            
            var request = new ThirdPartyMortgageRequest
            {
                MortgageAmount = getQuotesRequest.HouseValue - getQuotesRequest.Deposit
            };

            var response = _api.GetQuotes(request).GetAwaiter().GetResult().ToArray();

            decimal cheapestQuote = response.Select(x => x.MonthlyPayment).Min();

            return new MortgageQuote {
                MonthlyPayment = cheapestQuote
            };
        }
    }
}