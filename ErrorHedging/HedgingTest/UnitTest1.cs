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
            PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test",tabAction,date,30.0);

            ErrorHedging.HedgingPortfolioVanillaCall couvPort = new ErrorHedging.HedgingPortfolioVanillaCall(Call, date, 50.0 ,0.2);
            Console.WriteLine("Valeur initiale ",couvPort.portfolioValue);
            // updatePortfolioValue(double spot, System.DateTime date, double volatility)
            DateTime date1 = new DateTime(2014, 6, 1, 0, 0, 0);
            for (int i = 0; i < 100; i++)
            {
                couvPort.updatePortfolioValue(50.0, date1, 0.2);
                Console.WriteLine("Valeur portfolio ", couvPort.portfolioValue);
                date1.AddDays(1.0);
            }
        }

    }
}
