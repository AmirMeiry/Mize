using System;
using System.Text.Json;
using System.Threading.Tasks;
using ChainResource;
using ExchangeRate;

namespace ConsoleTestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var memoryStorage = new MemoryStorage<ExchangeRateList>(TimeSpan.FromHours(1));
            var fileSystemStorage = new FileSystemStorage<ExchangeRateList>(TimeSpan.FromHours(4), "exchange_rate_list.json");
            var webServiceStorage = new WebServiceStorage<ExchangeRateList>("http://openexchangerates.org/api/latest.json");

            var chainResource = new ChainResource<ExchangeRateList>(memoryStorage, fileSystemStorage, webServiceStorage);

            var exchangeRateList = await chainResource.GetValue();

            var json = JsonSerializer.Serialize(exchangeRateList);
            Console.WriteLine(json);
        }
    }
}
