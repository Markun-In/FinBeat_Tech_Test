using FinBeat_Tech_Test.Models;

namespace FinBeat_Tech_Test.Services
{
    public interface IDatabaseService
    {
        Task ClearValuesAsync();
        Task SaveValuesAsync(Dictionary<int, Values> values);
        Task<Dictionary<int, Values>> GetAllValuesAsync();
    }
}
