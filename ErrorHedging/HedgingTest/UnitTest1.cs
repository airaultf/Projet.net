using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorHedging;
using PricingLibrary;

namespace HedgingTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DateTime date = DateTime.Now;
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test","01");
            PricingLibrary.FinancialProducts.Share[] tabAction = {Action};
            PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test",tabAction,date,8.0);

            DateTime date1 = new DateTime(2014, 6, 1, 0, 0, 0);
            ErrorHedging.HedgingPortfolioVanillaCall couvPort = new ErrorHedging.HedgingPortfolioVanillaCall(Call, date1, 8.0 ,0.4);

            double initialValue = couvPort.portfolioValue;
            // updatePortfolioValue(double spot, System.DateTime date, double volatility)
           
            for (int i = 0; i < 50; i++)
            {
                couvPort.updatePortfolioValue(8.0, date1, 0.4);
                Console.WriteLine("Valeur portfolio "+couvPort.portfolioValue);
                date1 = date1.AddDays(1.0);
            }
            double finalValue  = couvPort.portfolioValue;

            Assert.AreEqual(initialValue, finalValue, 0.001, "Valeur initiale");
        }

    }
}
