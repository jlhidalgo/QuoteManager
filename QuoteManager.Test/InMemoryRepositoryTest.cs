using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuoteManager.DataRepository;
using QuoteManager.Interface;
using QuoteManager.Model;
using System;

namespace QuoteManager.Test
{
    [TestClass]
    public class InMemoryRepositoryTest
    {
        private IRepository sut;
        private IQuote quote1;
        private IQuote quote2;
        private IQuote quote3;
        private IQuote quote4;

        [TestInitialize]
        public void Setup()
        {
            sut = new InMemoryRepository();

            quote1 = new Quote
            {
                Id = Guid.NewGuid(),
                AvailableVolume = 10000,
                ExpirationDate = DateTime.Now.AddDays(1),
                Price = 8.55,
                Symbol = "ABC"
            };

            quote2 = new Quote
            {
                Id = Guid.NewGuid(),
                AvailableVolume = 2500,
                ExpirationDate = DateTime.Now.AddDays(2),
                Price = 20.21,
                Symbol = "DEF"
            };

            quote3 = new Quote
            {
                Id = Guid.NewGuid(),
                AvailableVolume = 2800,
                ExpirationDate = DateTime.Now.AddDays(3),
                Price = 21.21,
                Symbol = "DEF"
            };

            quote4 = new Quote
            {
                Id = Guid.NewGuid(),
                AvailableVolume = 5000,
                ExpirationDate = DateTime.Now.AddDays(2),
                Price = 10.20,
                Symbol = "GHI"
            };
        }

        [TestMethod]
        public void AddQuote_QuoteIsContained()
        {
            sut.Add(quote1);
            sut.Add(quote2);
            Assert.IsTrue(sut.Contains(quote1.Id));
            Assert.IsTrue(sut.Contains(quote2.Id));
        }

        [TestMethod]
        public void Contains_QuoteIsNotContained()
        {
            sut.Add(quote1);
            sut.Add(quote2);
            Assert.IsFalse(sut.Contains(Guid.NewGuid()));
        }

        [TestMethod]
        public void Remove_QuoteExists()
        {
            sut.Add(quote1);
            sut.Add(quote2);
            sut.Remove(quote1.Id);
            Assert.IsFalse(sut.Contains(quote1.Id));
            Assert.IsTrue(sut.Contains(quote2.Id));
        }

        [TestMethod]
        public void Remove_QuoteDoesNotExist()
        {
            sut.Add(quote1);
            sut.Add(quote2);
            sut.Remove(Guid.NewGuid());
            Assert.IsTrue(sut.Contains(quote1.Id));
            Assert.IsTrue(sut.Contains(quote2.Id));
        }

        [TestMethod]
        public void RemoveAllBySymbol_SymbolExists()
        {
            sut.Add(quote1);
            sut.Add(quote2);
            sut.Add(quote3);
            sut.Add(quote4);
            sut.RemoveAllBySymbol("DEF");
            Assert.IsTrue(sut.Contains(quote1.Id));
            Assert.IsFalse(sut.Contains(quote2.Id));
            Assert.IsFalse(sut.Contains(quote3.Id));
            Assert.IsTrue(sut.Contains(quote4.Id));
        }

        [TestMethod]
        public void RemoveAllBySymbol_SymbolDoesNotExist()
        {
            sut.Add(quote1);
            sut.Add(quote2);
            sut.Add(quote3);
            sut.Add(quote4);
            sut.RemoveAllBySymbol("ZZZ");
            Assert.IsTrue(sut.Contains(quote1.Id));
            Assert.IsTrue(sut.Contains(quote2.Id));
            Assert.IsTrue(sut.Contains(quote3.Id));
            Assert.IsTrue(sut.Contains(quote4.Id));
        }

        [TestMethod]
        public void GetAllAvailableBySymbol_SymbolDoesNotExists()
        {
            sut.Add(quote1);
            sut.Add(quote2);
            sut.Add(quote3);
            sut.Add(quote4);
            var res = sut.GetAllAvailableBySymbol("ZZZ");
            Assert.AreEqual(0, res.Count);
        }

        [TestMethod]
        public void GetAllAvailableBySymbol_AllExpired()
        {
            quote2.ExpirationDate = DateTime.Now.AddDays(-1);
            quote3.ExpirationDate = DateTime.Now.AddDays(-2);
            sut.Add(quote1);
            sut.Add(quote2);
            sut.Add(quote3);
            sut.Add(quote4);
            var res = sut.GetAllAvailableBySymbol("DEF");
            Assert.AreEqual(0, res.Count);
        }

        [TestMethod]
        public void GetAllAvailableBySymbol_NotAvailableVolume()
        {
            quote2.AvailableVolume = 0;
            quote3.AvailableVolume = 0;
            sut.Add(quote1);
            sut.Add(quote2);
            sut.Add(quote3);
            sut.Add(quote4);
            var res = sut.GetAllAvailableBySymbol("DEF");
            Assert.AreEqual(0, res.Count);
        }

        [TestMethod]
        public void GetAllAvailableBySymbol_AllAvailable()
        {
            sut.Add(quote1);
            sut.Add(quote2);
            sut.Add(quote3);
            sut.Add(quote4);
            var res = sut.GetAllAvailableBySymbol("DEF");
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(res[0].Symbol, "DEF");
            Assert.AreEqual(res[1].Symbol, "DEF");
            Assert.AreNotEqual(res[1].AvailableVolume, res[0].AvailableVolume);
        }
    }
}
