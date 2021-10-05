using OpenMoney.InterviewExercise.Models.Quotes;
using System.Threading.Tasks;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public interface IHomeInsuranceQuoteClient
    {
        Task<HomeInsuranceQuote> GetQuote(decimal houseValue);
    }
}
