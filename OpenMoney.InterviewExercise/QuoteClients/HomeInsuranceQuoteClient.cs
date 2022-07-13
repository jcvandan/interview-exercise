using System.Linq;
using OpenMoney.InterviewExercise.Models;
using OpenMoney.InterviewExercise.Models.Quotes;
using OpenMoney.InterviewExercise.ThirdParties;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public interface IHomeInsuranceQuoteClient
    {
        HomeInsuranceQuote GetQuote(GetQuotesRequest getQuotesRequest);
    }

    public class HomeInsuranceQuoteClient : IHomeInsuranceQuoteClient
    {
        private IThirdPartyHomeInsuranceApi _api;

        //I am assuming contentsValue is the standard we cover and don't change this so will keep this as is
        public decimal contentsValue = 50_000M;

        public HomeInsuranceQuoteClient(IThirdPartyHomeInsuranceApi api)
        {
            _api = api;
        }

        public HomeInsuranceQuote GetQuote(GetQuotesRequest getQuotesRequest)
        {
            HomeInsuranceQuote insuranceQuote = new HomeInsuranceQuote();

            if (ValidateQuote(ref insuranceQuote, getQuotesRequest.HouseValue))
            {
                return insuranceQuote;
            }
            
            var request = new ThirdPartyHomeInsuranceRequest
            {
                HouseValue = getQuotesRequest.HouseValue,
                ContentsValue = contentsValue
            };

            var response = _api.GetQuotes(request).GetAwaiter().GetResult().ToList();

            if (response.Count != 0)
            {
                ThirdPartyHomeInsuranceResponse cheapestQuote = null;

                foreach (var quote in response)
                {
                    if (cheapestQuote == null)
                    {
                        cheapestQuote = quote;
                    }
                    else if (cheapestQuote.MonthlyPayment > quote.MonthlyPayment)
                    {
                        cheapestQuote = quote;
                    }
                }

                insuranceQuote.MonthlyPayment = cheapestQuote.MonthlyPayment;
            }
            else
            {
                insuranceQuote.Success = false;
                insuranceQuote.ErrorString = "No result returned";
            }

            return insuranceQuote;
        }

        private bool ValidateQuote(ref HomeInsuranceQuote insuranceQuote, decimal houseValue)
        {
            decimal maxHouseValue = 10_000_000M;
            if (houseValue > maxHouseValue)
            {
                insuranceQuote.Success = false;
                insuranceQuote.ErrorString = "House value must be less than 10,000,000";
                return false;
            }

            insuranceQuote.Success = true;
            return true;
        }
    }
}