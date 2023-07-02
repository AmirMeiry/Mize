using System;
using ChainResource;
using ExchangeRate;

namespace ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var memoryStorage = new MemoryStorage<ExchangeRateList>(TimeSpan.FromHours(1));
            var fileSystemStorage = new FileSystemStorage<ExchangeRateList>(TimeSpan.FromHours(4), "exchange_rate_list.json");
            var webServiceStorage = new WebServiceStorage<ExchangeRateList>("https://openexchangerates.org/latest.json");

            var chainResource = new ChainResource<ExchangeRateList>(memoryStorage, fileSystemStorage, webServiceStorage);

            Console.WriteLine(chainResource);
        }
    }
}
