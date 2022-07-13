namespace OpenMoney.InterviewExercise.Models.Quotes
{
    public class HomeInsuranceQuote
    {
        public float MonthlyPayment { get; set; }
        public float BuildingsCover { get; set; }
        public float ContentsCover { get; set; }
        public string ErrorString { get; set; }
        public bool Success { get; set; }
    }
}