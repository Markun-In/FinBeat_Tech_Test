using FinBeat_Tech_Test.Models;

namespace FinBeat_Tech_Test.Services
{
    public interface IValuesService
    {
        Task<List<Values>> GetFilteredValuesAsync(int page = 1, int pageLim = 10, ValuesFilters? filters = null);
        Task SetNewValuesForSaveAsync(Dictionary<int, string> values);
    }
}
