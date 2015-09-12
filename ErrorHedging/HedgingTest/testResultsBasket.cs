using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorHedging;
using PricingLibrary;

namespace HedgingTest
{

    /*** Test for Results Class ***/
    [TestClass]
    public class testResultsBasket
    {
        [TestMethod]
        public void TestVolatilitiesAndSpotPrices()
        {
            DateTime date = new DateTime(2014, 6, 1, 0, 0, 0);
            DateTime date1 = new DateTime(2014, 4, 1, 0, 0, 0);
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "ALO FP");
            PricingLibrary.FinancialProducts.Share Action2 = new PricingLibrary.FinancialProducts.Share("test2", "BNP FP");
            PricingLibrary.FinancialProducts.Share[] mesActions = new PricingLibrary.FinancialProducts.Share[2];
            mesActions[0] = Action;
            mesActions[1] = Action2;
            double[] weight = { 0.1, 0.9 };
            PricingLibrary.FinancialProducts.BasketOption myBasketOption = new PricingLibrary.FinancialProducts.BasketOption("test", mesActions, weight, date, 30.0);
            Results myResults = new Results(myBasketOption, date1, date, 20, false);

            Console.WriteLine(" Premiers resultats : ");
            myResults.HedgingPortfolioValue.ForEach(data => Console.WriteLine(data));
            Console.WriteLine(" \n ");
            myResults.Payoff.ForEach(data => Console.WriteLine(data));
            Console.WriteLine(" \n ");

            myResults.computeResults();

            Console.WriteLine(" Fin : \n");
            myResults.HedgingPortfolioValue.ForEach(data => Console.WriteLine(data));
            Console.WriteLine(" \n ");
            myResults.Payoff.ForEach(data => Console.WriteLine(data));

        }
    }
}
