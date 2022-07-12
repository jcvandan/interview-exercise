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
            decimal loanToValueFraction = getQuotesRequest.Deposit / getQuotesRequest.HouseValue;
            if (loanToValueFraction < 0.1M)
            {
                return null;
            }
            
            decimal mortgageAmount = getQuotesRequest.HouseValue - getQuotesRequest.Deposit;

            if (mortgageAmount < 0)
            {
                //error handling
            }

            var request = new ThirdPartyMortgageRequest
            {
                MortgageAmount = (decimal) mortgageAmount
            };

            var response = _api.GetQuotes(request).GetAwaiter().GetResult().ToArray();

            ThirdPartyMortgageResponse cheapestQuote = null;
            
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
            
            return new MortgageQuote
            {
                MonthlyPayment = cheapestQuote.MonthlyPayment
            };
        }
    }
}