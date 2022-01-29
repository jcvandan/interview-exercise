using System.Linq;
using System.Threading.Tasks;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.Models.Quotes;
using OpenMoney.InterviewExercise.ThirdParties;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public interface IMortgageQuoteClient
    {
        Task<MortgageQuote> GetQuote(GetQuotesRequest getQuotesRequest);
    }

    public class MortgageQuoteClient : IMortgageQuoteClient
    {
        private readonly IThirdPartyMortgageApi _api;

        public MortgageQuoteClient(IThirdPartyMortgageApi api)
        {
            _api = api;
        }
        
        public async Task<MortgageQuote> GetQuote(GetQuotesRequest getQuotesRequest)
        {
            // check if mortgage request is eligible
            var loanToValueFraction = getQuotesRequest.Deposit / getQuotesRequest.HouseValue;
            if (loanToValueFraction < 0.1m)
            {
                return null;
            }
            
            var mortgageAmount = getQuotesRequest.HouseValue - getQuotesRequest.Deposit;
            
            var request = new ThirdPartyMortgageRequest
            {
                MortgageAmount = (decimal) mortgageAmount
            };

            var response = (await _api.GetQuotes(request)).ToList();

            ThirdPartyMortgageResponse cheapestQuote = null;
            
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
            
            return new MortgageQuote
            {
                MonthlyPayment = cheapestQuote.MonthlyPayment
            };
        }
    }
}