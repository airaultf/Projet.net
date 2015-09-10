using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorHedging;
using PricingLibrary;

namespace HedgingTest
{
    /*** Test for Results Class ***/
    [TestClass]
    public class testResults
    {
        /*** Test for getVolatility and getSpotPrice Methods ***/
        /* Should print out a list of Volatilities and SpotPrices for a result object */
        [TestMethod]
        public void TestVolatilityAndSpotPrice()
        {
            DateTime date = DateTime.Now;
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");
            PricingLibrary.FinancialProducts.Share[] tabAction = { Action };
            PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test", tabAction, date, 30.0);

            ErrorHedging.HedgingPortfolioVanillaCall couvPort = new ErrorHedging.HedgingPortfolioVanillaCall(Call, date, 50.0, 0.2);
            Console.WriteLine("Valeur initiale ", couvPort.portfolioValue);
            // updatePortfolioValue(double spot, System.DateTime date, double volatility)
            DateTime date1 = new DateTime(2012, 6, 1, 0, 0, 0);

            Results myResults = new Results(Call, date1, date, 30, true);

            for (DateTime d = date1.AddDays(30); d < date; d=d.AddDays(1))
            {
                //Console.WriteLine("Spot price: " + myResults.getSpotPrice(d));
                Console.WriteLine("Volatility: " + myResults.getVolatility(d));
            }
        }


        [TestMethod]
        public void TestSpotPrices()
        {
            DateTime date = DateTime.Now;
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");
            PricingLibrary.FinancialProducts.Share[] tabAction = { Action };
            PricingLibrary.FinancialProducts.BasketOption Call = new PricingLibrary.FinancialProducts.BasketOption("test", tabAction, date, 30.0);

            ErrorHedging.HedgingPortfolioBasketOption couvPort = new ErrorHedging.HedgingPortfolioVanillaCall(Call, date, 50.0, 0.2);
            Console.WriteLine("Valeur initiale ", couvPort.portfolioValue);
            // updatePortfolioValue(double spot, System.DateTime date, double volatility)
            DateTime date1 = new DateTime(2012, 6, 1, 0, 0, 0);

            Results myResults = new Results(Call, date1, date, 30, true);

            for (DateTime d = date1.AddDays(30); d < date; d = d.AddDays(1))
            {
                Console.WriteLine("Spot price: " + myResults.getSpotPrices(d)[1]);
            }
        }
    }
}
