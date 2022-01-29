namespace OpenMoney.InterviewExercise.Models.Quotes
{
    public class MortgageQuote
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }

        public decimal MonthlyPayment { get; set; }

        public static MortgageQuote Failure(string message)
        {
            return new MortgageQuote {Succeeded = false, ErrorMessage = message};
        }

        public static MortgageQuote Success(decimal monthlyPayment)
        {
            return new MortgageQuote { Succeeded = true, MonthlyPayment = monthlyPayment };
        }
    }
}