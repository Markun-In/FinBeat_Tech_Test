using FinBeat_Tech_Test.Models;
using Newtonsoft.Json;

namespace FinBeat_Tech_Test.Services
{
    public class ValuesService : IValuesService
    {
        private readonly ILogger<ValuesService> _logger;
        private readonly IDatabaseService _databaseService;
        private Dictionary<int, Values> _cacheValues = new Dictionary<int, Values>();

        public ValuesService(ILogger<ValuesService> logger, IDatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        public async Task<List<Values>> GetFilteredValuesAsync(int page = 1, int pageLim = 10, ValuesFilters? filters = null)
        {
            if (_cacheValues.Count == 0)
                _cacheValues = await _databaseService.GetAllValuesAsync();

            var filteredValues = FilterValues(filters);
            var result = filteredValues.Skip((page - 1) * pageLim).Take(pageLim).Select(x => x.Value).ToList(); //Преобразую в List, чтобы дальше фреймворк преобразовал это в JSON

           return result;
        }

        public async Task SetNewValuesForSaveAsync(Dictionary<int, string> values)
        {
            var valuesDictionary = SortValuesByCode(values);
            UpdateCacheValues(valuesDictionary);
            await _databaseService.SaveValuesAsync(_cacheValues);
        }

        private Dictionary<int, Values> SortValuesByCode(Dictionary<int, string> values)
        {
            try
            {
                int idCounter = 1;
                var sortedValues = values.Select(x => new Values { Id = idCounter++, Code = x.Key, Value = x.Value })
                                         .OrderBy(x => x.Code) //Сортировка по Code перед сохранением, как указано в задаче
                                         .ToDictionary(x => x.Id, t => t);
                return sortedValues;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        private void UpdateCacheValues(Dictionary<int, Values> values)
        {
            _cacheValues.Clear();
            _cacheValues = values;
            _logger.LogInformation("Cache values is updated");
        }

        private Dictionary<int, Values> FilterValues(ValuesFilters? filters)
        {
            if (filters == null)
                return _cacheValues;

            var filteredValues = _cacheValues;
            try
            {
                if (filters.CodeFrom != null)
                    filteredValues = filteredValues.Where(x => x.Value.Code >= filters.CodeFrom).ToDictionary(x => x.Key, x => x.Value);

                if (filters.CodeTo != null)
                    filteredValues = filteredValues.Where(x => x.Value.Code <= filters.CodeTo).ToDictionary(x => x.Key, x => x.Value);

                //Сортировка по Code
                if (filters.OrderByCode != null)
                {
                    if (filters.OrderByCode == OrderBy.asc)
                        filteredValues = filteredValues.OrderBy(x => x.Value.Code).ToDictionary(x => x.Key, x => x.Value);

                    if (filters.OrderByCode == OrderBy.desc)
                        filteredValues = filteredValues.OrderByDescending(x => x.Value.Code).ToDictionary(x => x.Key, x => x.Value);
                }

                //Сортировка по Id
                if (filters.OrderById != null)
                {
                    if (filters.OrderById == OrderBy.asc)
                        filteredValues = filteredValues.OrderBy(x => x.Value.Id).ToDictionary(x => x.Key, x => x.Value);

                    if (filters.OrderById == OrderBy.desc)
                        filteredValues = filteredValues.OrderByDescending(x => x.Value.Id).ToDictionary(x => x.Key, x => x.Value);
                }
                return filteredValues;
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Error during srting values {ex.Message}");
                throw;
            }
        }
    }
}
