using System.Threading.Tasks;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.QuoteClients;

namespace OpenMoney.InterviewExercise
{
    public class QuoteOrchestrator
    {
        private readonly IHomeInsuranceQuoteClient _homeInsuranceQuoteClient;
        private readonly IMortgageQuoteClient _mortgageQuoteClient;

        public QuoteOrchestrator(
            IHomeInsuranceQuoteClient homeInsuranceQuoteClient,
            IMortgageQuoteClient mortgageQuoteClient)
        {
            _homeInsuranceQuoteClient = homeInsuranceQuoteClient;
            _mortgageQuoteClient = mortgageQuoteClient;
        }

        public async Task<GetQuotesResponse> GetQuotes(GetQuotesRequest request)
        {
            var mortgageQuote = _mortgageQuoteClient.GetQuote(request);
            var homeInsuranceQuote = _homeInsuranceQuoteClient.GetQuote(request);

            await Task.WhenAll(mortgageQuote, homeInsuranceQuote);

            return new GetQuotesResponse
            {
                MortgageQuote = mortgageQuote.Result,
                HomeInsuranceQuote = homeInsuranceQuote.Result
            };
        }
    }
}