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

        public decimal contentsValue = 50_000;

        public HomeInsuranceQuoteClient(IThirdPartyHomeInsuranceApi api)
        {
            _api = api;
        }

        public async Task<HomeInsuranceQuote> GetQuote(GetQuotesRequest getQuotesRequest)
        {
            // check if request is eligible
            if (getQuotesRequest.HouseValue > 10_000_000m)
            {
                return HomeInsuranceQuote.Failure("House value cannot exceed 10,000,000");
            }

            var request = new ThirdPartyHomeInsuranceRequest
            {
                HouseValue = getQuotesRequest.HouseValue,
                ContentsValue = contentsValue
            };

            try
            {
                var response = (await _api.GetQuotes(request)).ToList();

                ThirdPartyHomeInsuranceResponse cheapestQuote = null;

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
                    return HomeInsuranceQuote.Failure("No quotes returned");

                return HomeInsuranceQuote.Success((decimal)cheapestQuote.MonthlyPayment);
            }
            catch (Exception ex)
            {
                // In practice, we would have more specific catch blocks here
                // depending on what the third party API could throw
                // (e.g. HttpException if it is a HTTP call)

                return HomeInsuranceQuote.Failure(ex.Message);
            }
        }
    }
}