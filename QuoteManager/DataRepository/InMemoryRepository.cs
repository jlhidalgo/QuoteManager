using QuoteManager.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuoteManager.DataRepository
{
    public class InMemoryRepository : IRepository
    {
        private IDictionary<Guid, IQuote> _book;

        public InMemoryRepository()
        {
            _book = new Dictionary<Guid, IQuote>();
        }
        public void Add(IQuote quote)
        {
            _book.Add(quote.Id, quote);
        }

        public bool Contains(Guid id)
        {
            return _book.ContainsKey(id);
        }

        public IList<IQuote> GetAllAvailableBySymbol(string symbol)
        {
            var currentDate = DateTime.Now;
            var quotes = _book.Where(x => x.Value.Symbol == symbol && x.Value.AvailableVolume > 0 && x.Value.ExpirationDate > currentDate).Select(x => x.Value);
            return new List<IQuote>(quotes);
        }

        public void Remove(Guid id)
        {
            if (_book.ContainsKey(id))
            {
                _book.Remove(id);
            }
        }

        public void RemoveAllBySymbol(string symbol)
        {
            var keys = _book.Where(x => x.Value.Symbol == symbol).Select(x => x.Key);
            foreach (Guid key in keys)
                _book.Remove(key);
            
        }

        public void Update(IQuote quote)
        {
            _book[quote.Id].Price = quote.Price;
            _book[quote.Id].AvailableVolume = quote.AvailableVolume;
            _book[quote.Id].Symbol = quote.Symbol;
        }
    }
}
