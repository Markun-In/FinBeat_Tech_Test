using FinBeat_Tech_Test.Models;
using FinBeat_Tech_Test.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

namespace FinBeat_Tech_Test.Controllers
{
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IValuesService _valueService;
        private readonly ILogger<ValuesController> _logger;
        private static readonly Logger RequestLogger = LogManager.GetLogger("RequestLogger");

        public ValuesController(IValuesService valueService, ILogger<ValuesController> logger)
        {
            _valueService = valueService;
            _logger = logger;
        }

        /// <summary>
        /// Получение данных на сохранение
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        public async Task<IActionResult> SaveValues([FromBody] Dictionary<int, string> values)
        {
            RequestLogger.Info("Request to save data: {@Values}", values);
            try
            {
                if (values == null || values.Count == 0)
                {
                    _logger.LogError("Recieved empty data in post request!");
                    return BadRequest("Invalid data");
                }

                await _valueService.SetNewValuesForSaveAsync(values);

                _logger.LogInformation("Values is saved");
                RequestLogger.Info("Values is saved");

                return Ok(new { message = "Values is saved" }); // TODO Вернуть нормальный ответ пользователю
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Error during saving data: {ex.Message}", ex);
                RequestLogger.Info($"Values is not saved: {ex.Message}", ex);
                return BadRequest(ex.Message);
            }
            
        }

        /// <summary>
        /// Отправка пользователю отфильтрованных и отсортированных данных 
        /// </summary>
        /// <param name="page">default = 1</param>
        /// <param name="pageLim">default = 10</param>
        /// <param name="codeFrom"></param>
        /// <param name="codeTo"></param>
        /// <param name="orderById"> 0 - asc || 1 - desc </param>
        /// <param name="orderByCode">0 - asc || 1 - desc </param>
        /// <returns>JSON</returns>
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetValues([FromQuery] int page = 1, [FromQuery] int pageLim = 10, [FromQuery] int? codeFrom = null, 
                                                   [FromQuery] int? codeTo = null, [FromQuery] OrderBy? orderById = null, 
                                                   [FromQuery] OrderBy? orderByCode = null)
        {
            RequestLogger.Info("Request to get data: Page = {page}, PageLim = {pageLim}, CodeFrom={CodeFrom}, " +
                "CodeTo={CodeTo}, orderById = {orderById}, orderByCode = {orderByCode}", page, pageLim, codeFrom, codeTo, orderById, orderByCode);
            try
            {
                var filters = new ValuesFilters(codeFrom, codeTo, orderById, orderByCode);
                _logger.LogInformation($"Filters: {JsonConvert.SerializeObject(filters)}");

                var values = await _valueService.GetFilteredValuesAsync(page, pageLim, filters);
                var result = JsonConvert.SerializeObject(values);

                _logger.LogInformation("Values have been sent");
                RequestLogger.Info("Values have been sent [Count:{Count}]: Response:{Values}", values.Count(), result);
                
                return Ok(values);// values, чтобы удобнее было читать json))
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during getting data: {ex.Message}", ex);
                _logger.LogError($"Values were not sent: {ex.Message}", ex);
                return BadRequest(ex.Message);
            }
        }
    }
}
