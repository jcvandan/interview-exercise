using OpenMoney.InterviewExercise.Models.Quotes;

namespace OpenMoney.InterviewExercise.Models
{
    public class GetQuotesResponse
    {
        public MortgageQuote MortgageQuote { get; set; }
        public HomeInsuranceQuote HomeInsuranceQuote { get; set; }
        public bool FailedMortgage { get; set; }
        public bool FailedHomeInsurance { get; set; }
    }
}