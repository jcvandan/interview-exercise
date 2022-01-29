using System;
using System.Linq;
using System.Threading.Tasks;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.Models.Quotes;
using OpenMoney.InterviewExercise.ThirdParties;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public interface IHomeInsuranceQuoteClient
    {
        Task<HomeInsuranceQuote> GetQuote(GetQuotesRequest getQuotesRequest);
    }

    public class HomeInsuranceQuoteClient : IHomeInsuranceQuoteClient
    {
        private readonly IThirdPartyHomeInsuranceApi _api;

        public decimal ContentsValue = 50_000;

        public HomeInsuranceQuoteClient(IThirdPartyHomeInsuranceApi api)
        {
            _api = api;
        }

        public async Task<HomeInsuranceQuote> GetQuote(GetQuotesRequest getQuotesRequest)
        {
            if (!HouseValueIsWithinLimits(getQuotesRequest))
                return HomeInsuranceQuote.Failure("House value cannot exceed 10,000,000");

            var request = new ThirdPartyHomeInsuranceRequest
            {
                HouseValue = getQuotesRequest.HouseValue,
                ContentsValue = ContentsValue
            };

            try
            {
                var quotes = (await _api.GetQuotes(request)).ToList();

                return quotes.Any()
                    ? HomeInsuranceQuote.Success((decimal)quotes.Min(q => q.MonthlyPayment))
                    : HomeInsuranceQuote.Failure("No quotes returned");
            }
            catch (Exception ex)
            {
                // In practice, we would have more specific catch blocks here
                // depending on what the third party API could throw
                // (e.g. HttpException if it is a HTTP call)

                return HomeInsuranceQuote.Failure(ex.Message);
            }
        }

        private bool HouseValueIsWithinLimits(GetQuotesRequest quotesRequest)
        {
            return quotesRequest.HouseValue <= 10_000_000m;
        }
    }
}