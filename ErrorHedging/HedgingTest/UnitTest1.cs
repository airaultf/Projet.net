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

            DateTime date1 = new DateTime(2013, 9, 9, 0, 0, 0);
            ErrorHedging.HedgingPortfolioVanillaCall couvPort = new ErrorHedging.HedgingPortfolioVanillaCall(Call, date1, 10.0, 0.4);
            Console.WriteLine("Valeur portfolio " + couvPort.portfolioValue);
            // updatePortfolioValue(double spot, System.DateTime date, double volatility)
            
            for (int i = 0; i < 365; i++)
            {
                couvPort.updatePortfolioValue(10.0, date1, 0.4);
                Console.WriteLine("Valeur portfolio "+ couvPort.portfolioValue);
                Console.WriteLine("\n");
                date1 = date1.AddDays(1.0);
            }
        }

        [TestMethod]
        public void TestRebalancement()
        {
            DateTime date = DateTime.Now;
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test","01");
            PricingLibrary.FinancialProducts.Share[] tabAction = {Action};

            PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test",tabAction,date,8.0);


            DateTime dateStart = new DateTime(2015, 8, 9, 0, 0, 0);


            ErrorHedging.Results result = new ErrorHedging.Results(Call, dateStart, date, 30, true);
           

            Console.WriteLine("Valeur portefeuille : " + result.HedgingPortfolioValue);

        }

    }
}
