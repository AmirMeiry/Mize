using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeRate
{
    public class ExchangeRateList
    {
        public string disclaimer { get; set; }
        public string license { get; set; }
        public long timestamp { get; set; }
        public string Base { get; set; }
        public Dictionary<string, decimal> rates { get; set; }

        public override string ToString()
        {
            return $"ExchangeRateList (BaseCurrency: {Base}, Rates: {rates.Count})";
        }
    }
}
