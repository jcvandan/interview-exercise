using System;
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
            if (!WithinLoanToValueLimit(getQuotesRequest))
                return MortgageQuote.Failure("Loan-to-value must be over 10%");

            var mortgageAmount = getQuotesRequest.HouseValue - getQuotesRequest.Deposit;
            if (mortgageAmount < 0)
                return MortgageQuote.Failure("Mortgage amount cannot be negative");

            var request = new ThirdPartyMortgageRequest
            {
                MortgageAmount = mortgageAmount
            };

            try
            {
                var quotes = (await _api.GetQuotes(request)).ToList();
                
                return quotes.Any()
                    ? MortgageQuote.Success(quotes.Min(q => q.MonthlyPayment)) 
                    : MortgageQuote.Failure("No quotes were returned");
            }
            catch (Exception ex)
            {
                // In practice, we would have more specific catch blocks here
                // depending on what the third party API could throw
                // (e.g. HttpException if it is a HTTP call)

                return MortgageQuote.Failure(ex.Message);
            }
        }

        private static bool WithinLoanToValueLimit(GetQuotesRequest quoteRequest)
        {
            var loanToValueFraction = quoteRequest.Deposit / quoteRequest.HouseValue;
            return loanToValueFraction >= 0.1m;
        }
    }
}