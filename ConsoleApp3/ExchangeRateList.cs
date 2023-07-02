using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp3
{
    public class ExchangeRateList
    {
        public string BaseCurrency { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }

        // Other properties and methods of the ExchangeRateList class

        public override string ToString()
        {
            return $"ExchangeRateList (BaseCurrency: {BaseCurrency}, Rates: {Rates.Count})";
        }
    }
}
