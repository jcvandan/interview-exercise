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
            MortgageQuote mortgageQuote = new MortgageQuote();
            decimal mortgageAmount = getQuotesRequest.HouseValue - getQuotesRequest.Deposit;

            if (!ValidateQuote(mortgageAmount, ref getQuotesRequest, ref mortgageQuote))
            {
                return mortgageQuote;
            }

            var request = new ThirdPartyMortgageRequest
            {
                MortgageAmount = mortgageAmount
            };

            var response = _api.GetQuotes(request).GetAwaiter().GetResult().ToArray();

            ThirdPartyMortgageResponse cheapestQuote = null;

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

            mortgageQuote.MonthlyPayment = cheapestQuote.MonthlyPayment;

            return mortgageQuote;
        }

        private bool ValidateQuote(decimal mortgageAmount, ref GetQuotesRequest getQuotesRequest, ref MortgageQuote mortgageQuote)
        {
            if (!ValidLoanToValue(getQuotesRequest.Deposit, getQuotesRequest.HouseValue))
            {
                mortgageQuote.Success = false;
                mortgageQuote.ErrorString = "Loan to value ratio must be 10% or higher";
                return false;
            }

            if (mortgageAmount < 0)
            {
                mortgageQuote.Success = false;
                mortgageQuote.ErrorString = "Mortgage amount cannot be less than 0";
                return false;
            }

            mortgageQuote.Success = true;
            return true;
        }

        private bool ValidLoanToValue(decimal deposit, decimal houseValue)
        {
            //return false if houseValue == 0 - this will avoid dividing by 0
            return houseValue == 0 ? false : (deposit / houseValue) < 0.1M;
        }
    }
}