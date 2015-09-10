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
        [TestMethod]
        public void TestVolatilityAndSpotPrice()
        {
            DateTime date = new DateTime(2014, 6, 1, 0, 0, 0);
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");
            PricingLibrary.FinancialProducts.Share[] tabAction = { Action };
            PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test", tabAction, date, 30.0);

            // updatePortfolioValue(double spot, System.DateTime date, double volatility)
            DateTime date1 = new DateTime(2012, 6, 1, 0, 0, 0);

            Results myResults = new Results(Call, date1, date, 100, true);
            for (DateTime d = date1.AddDays(100); d<date; d=d.AddDays(1))
            {
                //Console.WriteLine(myResults.getSpotPrice(d));
                Console.WriteLine("VOL:" + myResults.getVolatility(d));
            }

            //Console.WriteLine("Volatility: " + myResults.getVolatility(date1.AddDays(30)));
        }
    }
}