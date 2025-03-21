using FinBeat_Tech_Test.Services;

namespace FinBeat_Tech_Test
{
    public class ConfigurationServices
    {
        public static void SetDIConfiguration(IServiceCollection service)
        {
            service.AddSingleton<IDatabaseService, DatabaseService>();
            service.AddSingleton<IValuesService, ValuesService>();
        }
    }
}
