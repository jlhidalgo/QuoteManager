using System;
using System.Collections.Generic;

namespace QuoteManager.Interface
{
    public interface IRepository
    {
        bool Contains(Guid id);
        void Update(IQuote quote);
        void Add(IQuote quote);
        void Remove(Guid id);
        void RemoveAllBySymbol(string Symbol);
        IList<IQuote> GetAllAvailableBySymbol(string symbol);
    }
}
