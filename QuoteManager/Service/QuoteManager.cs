using QuoteManager.Interface;
using QuoteManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuoteManager.Service
{
    public class QuoteManager : IQuoteManager
    {
        private readonly IRepository _quoteRepository;

        public QuoteManager(IRepository quoteRepository)
        {
            _quoteRepository = quoteRepository;
        }

        public void AddOrUpdateQuote(IQuote quote)
        {
            if (_quoteRepository.Contains(quote.Id))
                _quoteRepository.Update(quote);
            else 
                _quoteRepository.Add(quote);
        }

        public ITradeResult ExecuteTrade(string symbol, uint volumeRequested)
        {
            var quotes = _quoteRepository.GetAllAvailableBySymbol(symbol);
            if (quotes.Count == 0)
                return null;

            var sortedQuotes = new List<IQuote>(quotes.OrderBy(x => x.Price));

            var i = 0;
            uint remainingVolume = volumeRequested;
            double sumPriceVolume = 0;
            while (remainingVolume > 0 && i < sortedQuotes.Count)
            {
                if (remainingVolume <= sortedQuotes[i].AvailableVolume)
                {
                    sortedQuotes[i].AvailableVolume -= remainingVolume;
                    sumPriceVolume += remainingVolume * sortedQuotes[i].Price;
                    remainingVolume = 0;
                }
                else 
                {
                    remainingVolume -= sortedQuotes[i].AvailableVolume;
                    sumPriceVolume += sortedQuotes[i].AvailableVolume * sortedQuotes[i].Price;
                    sortedQuotes[i].AvailableVolume = 0;
                }
                _quoteRepository.Update(sortedQuotes[i++]);
            }

            return new TradeResult
            {
                Id = Guid.NewGuid(),
                Symbol = symbol,
                VolumeExecuted = volumeRequested - remainingVolume,
                VolumeRequested = volumeRequested,
                VolumeWeightedAveragePrice = sumPriceVolume / (volumeRequested - remainingVolume)
            };
            
        }

        public IQuote GetBestQuoteWithAvailableVolume(string symbol)
        {
            var quotes = _quoteRepository.GetAllAvailableBySymbol(symbol);
            if (quotes.Count == 0)
                return null;

            return quotes.OrderBy(x => x.Price).First();
        }

        public void RemoveAllQuotes(string symbol)
        {
            _quoteRepository.RemoveAllBySymbol(symbol);
        }

        public void RemoveQuote(Guid id)
        {
            _quoteRepository.Remove(id);
        }
    }
}
