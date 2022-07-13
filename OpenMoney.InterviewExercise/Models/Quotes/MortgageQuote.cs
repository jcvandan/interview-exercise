namespace OpenMoney.InterviewExercise.Models.Quotes
{
    public class MortgageQuote
    {
        public decimal MonthlyPayment { get; set; }
        public bool Success { get; set; }
        public string ErrorString { get; set; }
    }
}