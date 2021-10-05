using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.QuoteClients;
using System.Threading.Tasks;
using OpenMoney.InterviewExercise.Models.Quotes;
using System;

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
            var response = new GetQuotesResponse();
            var mortgageQuote = _mortgageQuoteClient.GetQuote(
                new decimal(request.HouseValue),
                new decimal(request.Deposit)
            );
            var homeInsuranceQuote = _homeInsuranceQuoteClient.GetQuote(
                new decimal(request.HouseValue)
            );

            try
            {
                response.MortgageQuote = await mortgageQuote;
            }
            catch
            {
                response.FailedMortgage = true;
            }

            try
            {
                response.HomeInsuranceQuote = await homeInsuranceQuote;
            }
            catch 
            {
                response.FailedHomeInsurance = true;
            }

            return response;
        }
    }
}