using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorHedging;
using PricingLibrary;

namespace HedgingTest
{
    [TestClass]
    public class UnitTest1
    {
        /*
        [TestMethod]
        public void TestMethod1()
        {
            DateTime date = DateTime.Now;
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test","01");
            PricingLibrary.FinancialProducts.Share[] tabAction = {Action};
            PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test",tabAction,date,8.0);

            DateTime date1 = new DateTime(2013, 9, 9, 0, 0, 0);
            ErrorHedging.HedgingPortfolio couvPort = new ErrorHedging.HedgingPortfolio(Call, date1, 10.0, 0.4);
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
        */
        [TestMethod]
        public void TestRebalancement()
        {
            double ratio = 0;
            double compteur = 0;

            for (int i = 0; i < 300; i++)
            {
                DateTime date = DateTime.Now;
                PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");
                PricingLibrary.FinancialProducts.Share[] tabAction = { Action };

                PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test", tabAction, date, 8.0);


                DateTime dateStart = new DateTime(2014, 9, 10, 0, 0, 0);


                ErrorHedging.Results result = new ErrorHedging.Results(Call, dateStart, date, 1, true);

                double firstValue = result.HedgingPortfolioValue;
                result.computeResults();

                double payoff = result.Payoff;
                double lastValue = result.HedgingPortfolioValue;
                double ratioTmp = Math.Abs(payoff - lastValue) / firstValue;

                ratio += ratioTmp;
                compteur += 1;
                
            }

            ratio = ratio / compteur;
            Console.WriteLine(ratio);

        }

    }
}
