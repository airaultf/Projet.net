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
            DateTime date1 = new DateTime(2014, 1, 1, 0, 0, 0);
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "ALO FP");
            PricingLibrary.FinancialProducts.Share Action2 = new PricingLibrary.FinancialProducts.Share("test2", "BNP FP");
            PricingLibrary.FinancialProducts.Share[] mesActions = new PricingLibrary.FinancialProducts.Share[2];
            mesActions[0] = Action;
            mesActions[1] = Action2;
            double[] weight = { 0.1, 0.9 };
            PricingLibrary.FinancialProducts.BasketOption myBasketOption = new PricingLibrary.FinancialProducts.BasketOption("test", mesActions, weight, date, 30.0);
            OptionManager myOptionManager = new OptionManager(myBasketOption, date1, date, 20, false); 

            Console.WriteLine(" Premiers resultats : ");
            myOptionManager.HedgingPortfolioValue.ForEach(data => Console.WriteLine(data));
            Console.WriteLine(" \n ");
            myOptionManager.Payoff.ForEach(data => Console.WriteLine(data));
            Console.WriteLine(" \n ");


           ComputeResults.computeResults(myOptionManager);

            Console.WriteLine(" Fin : \n");
            Console.WriteLine("\n");
            myOptionManager.HedgingPortfolioValue.ForEach(data => Console.WriteLine(data));
            Console.WriteLine("\n");
            //myResults.OptionPrice.ForEach(data => Console.WriteLine(data));
            Console.WriteLine(" Option price \n");
            myOptionManager.OptionPrice.ForEach(data => Console.WriteLine(data));
            Console.WriteLine(" \n ");
            Console.WriteLine(" \n ");
            myOptionManager.dateTime.ForEach(data => Console.WriteLine(data));
        }
    }
}
