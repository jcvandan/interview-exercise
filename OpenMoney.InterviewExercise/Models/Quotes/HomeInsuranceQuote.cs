namespace OpenMoney.InterviewExercise.Models.Quotes
{
    public class HomeInsuranceQuote
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }

        public decimal MonthlyPayment { get; set; }

        public static HomeInsuranceQuote Failure(string message)
        {
            return new HomeInsuranceQuote { Succeeded = false, ErrorMessage = message };
        }

        public static HomeInsuranceQuote Success(decimal monthlyPayment)
        {
            return new HomeInsuranceQuote { Succeeded = true, MonthlyPayment = monthlyPayment };
        }
    }
}