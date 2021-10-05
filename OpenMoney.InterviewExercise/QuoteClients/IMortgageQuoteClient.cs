using OpenMoney.InterviewExercise.Models.Quotes;
using System.Threading.Tasks;

namespace OpenMoney.InterviewExercise.QuoteClients
{
    public interface IMortgageQuoteClient
    {
        Task<MortgageQuote> GetQuote(decimal houseValue, decimal deposit);
    }
}
