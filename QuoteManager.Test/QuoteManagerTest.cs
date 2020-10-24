using QuoteManager.Interface;
using QuoteManager.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using QuoteManagerClass = QuoteManager.Service.QuoteManager;

namespace QuoteManager.Test
{
    [TestClass]
    public class QuoteManagerTest
    {
        private IQuote quote1;
        private IQuote quote2;
        private IQuote quote3;
        private IList<IQuote> quotes;

        [TestInitialize]
        public void Setup()
        {
            quote1 = new Quote
            {
                Id = Guid.NewGuid(),
                AvailableVolume = 1500,
                ExpirationDate = DateTime.Now.AddDays(2),
                Price = 9.5,
                Symbol = "DEF"
            };

            quote2 = new Quote
            {
                Id = Guid.NewGuid(),
                AvailableVolume = 1700,
                ExpirationDate = DateTime.Now.AddDays(2),
                Price = 10,
                Symbol = "DEF"
            };

            quote3 = new Quote
            {
                Id = Guid.NewGuid(),
                AvailableVolume = 1000,
                ExpirationDate = DateTime.Now.AddDays(3),
                Price = 9,
                Symbol = "DEF"
            };

            quotes = new List<IQuote>();
            quotes.Add(quote1);
            quotes.Add(quote2);
            quotes.Add(quote3);
        }

        [TestMethod]
        public void AddOrUpdateQuote_QuoteExists()
        {
            var repository = new Mock<IRepository>();
            var quote = new Mock<IQuote>();
            repository.Setup(x => x.Contains(It.IsAny<Guid>())).Returns(true);

            var sut = new QuoteManagerClass(repository.Object);
            sut.AddOrUpdateQuote(quote.Object);
            repository.Verify(x => x.Contains(It.IsAny<Guid>()));
            repository.Verify(x => x.Update(It.IsAny<IQuote>()));
        }

        [TestMethod]
        public void AddOrUpdateQuote_QuoteDoesNotExist()
        {
            var repository = new Mock<IRepository>();
            var quote = new Mock<IQuote>();

            var sut = new QuoteManagerClass(repository.Object);
            sut.AddOrUpdateQuote(quote.Object);
            repository.Verify(x => x.Contains(It.IsAny<Guid>()));
            repository.Verify(x => x.Add(It.IsAny<IQuote>()));
        }

        [TestMethod]
        public void GetBestQuoteWithAvailableVolume_NoQuotesFound()
        {
            var repository = new Mock<IRepository>();
            repository.Setup(x => x.GetAllAvailableBySymbol(It.IsAny<string>())).Returns(new List<IQuote>());

            var sut = new QuoteManagerClass(repository.Object);
            var res = sut.GetBestQuoteWithAvailableVolume("ABC");

            Assert.AreEqual(null, res);
        }

        [TestMethod]
        public void GetBestQuoteWithAvailableVolume_ReturnLowestPrice()
        {
            var repository = new Mock<IRepository>();
            repository.Setup(x => x.GetAllAvailableBySymbol(It.IsAny<string>())).Returns(quotes);

            var sut = new QuoteManagerClass(repository.Object);
            var res = sut.GetBestQuoteWithAvailableVolume("ABC");

            Assert.AreEqual(quote3.Symbol, res.Symbol);
            Assert.AreEqual(quote3.Price, res.Price);
            Assert.AreEqual(quote3.Id, res.Id);
        }

        [TestMethod]
        public void ExecuteTrade_NoAvailableQuotes()
        {
            var repository = new Mock<IRepository>();
            repository.Setup(x => x.GetAllAvailableBySymbol(It.IsAny<string>())).Returns(new List<IQuote>());

            var sut = new QuoteManagerClass(repository.Object);
            var res = sut.ExecuteTrade("DEF", 1000);

            Assert.AreEqual(null, res);
        }

        [TestMethod]
        public void ExecuteTrade_BoughtOneQuote()
        {
            var repository = new Mock<IRepository>();
            repository.Setup(x => x.GetAllAvailableBySymbol(It.IsAny<string>())).Returns(quotes);

            var sut = new QuoteManagerClass(repository.Object);
            var res = sut.ExecuteTrade("DEF", 1000);

            Assert.AreEqual(res.VolumeExecuted, (uint)1000);
            Assert.AreEqual(res.VolumeRequested, (uint)1000);
            Assert.AreEqual(res.Symbol, "DEF");
            Assert.AreEqual(res.VolumeWeightedAveragePrice, 9.0);

        }

        [TestMethod]
        public void ExecuteTrade_BoughtTwoQuotes()
        {
            var repository = new Mock<IRepository>();
            repository.Setup(x => x.GetAllAvailableBySymbol(It.IsAny<string>())).Returns(quotes);

            var sut = new QuoteManagerClass(repository.Object);
            var res = sut.ExecuteTrade("DEF", 2400);

            Assert.AreEqual(res.VolumeExecuted, (uint)2400);
            Assert.AreEqual(res.VolumeRequested, (uint)2400);
            Assert.AreEqual(res.Symbol, "DEF");
            Assert.AreEqual(res.VolumeWeightedAveragePrice, 9.291666666666666);

        }

        [TestMethod]
        public void ExecuteTrade_BoughtThreeQuotes()
        {
            var repository = new Mock<IRepository>();
            repository.Setup(x => x.GetAllAvailableBySymbol(It.IsAny<string>())).Returns(quotes);

            var sut = new QuoteManagerClass(repository.Object);
            var res = sut.ExecuteTrade("DEF", 3000);

            Assert.AreEqual(res.VolumeExecuted, (uint)3000);
            Assert.AreEqual(res.VolumeRequested, (uint)3000);
            Assert.AreEqual(res.Symbol, "DEF");
            Assert.AreEqual(res.VolumeWeightedAveragePrice, 9.416666666666666);

        }

        [TestMethod]
        public void ExecuteTrade_DidNotReachRequestedVolume()
        {
            var repository = new Mock<IRepository>();
            repository.Setup(x => x.GetAllAvailableBySymbol(It.IsAny<string>())).Returns(quotes);

            var sut = new QuoteManagerClass(repository.Object);
            var res = sut.ExecuteTrade("DEF", 5000);

            Assert.AreEqual(res.VolumeExecuted, (uint)4200);
            Assert.AreEqual(res.VolumeRequested, (uint)5000);
            Assert.AreEqual(res.Symbol, "DEF");
            Assert.AreEqual(res.VolumeWeightedAveragePrice, 9.583333333333334);

        }
    }
}
