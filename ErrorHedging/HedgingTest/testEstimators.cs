using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorHedging;
using PricingLibrary;

namespace HedgingTest
{

    /*** Test for Results Class ***/
    [TestClass]
    public class testEstimators
    {

            DateTime date = new DateTime(2014, 6, 1, 0, 0, 0);
            DateTime date1 = new DateTime(2012, 6, 1, 0, 0, 0);
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");
            PricingLibrary.FinancialProducts.Share Action2 = new PricingLibrary.FinancialProducts.Share("test2", "02");
            PricingLibrary.FinancialProducts.Share Action3 = new PricingLibrary.FinancialProducts.Share("test3", "03");
            PricingLibrary.FinancialProducts.Share[] mesActions = new PricingLibrary.FinancialProducts.Share[3];

        [TestMethod]
        public void TestVolatitilies()
        {
            mesActions[0] = Action;
            mesActions[1] = Action2;
            mesActions[2] = Action3;
            double[] weight = { 0.1, 0.7, 0.2 };
            PricingLibrary.FinancialProducts.BasketOption myBasketOption = new PricingLibrary.FinancialProducts.BasketOption("test", mesActions, weight, date, 30.0);
            OptionManager option = new OptionManager(myBasketOption, date1, date, 20, true);
            for (DateTime daa = date1.AddDays(20); daa < date; daa = daa.AddDays(1))
            {
                //Console.WriteLine(myResults.getSpotPrice(d));

                double[] mesVols = new double[3];
                mesVols = Estimators.getVolatilities(daa, option);
                Console.WriteLine(mesVols[0]);
                Console.WriteLine(mesVols[1]);
                Console.WriteLine(mesVols[2]);
            }
        }

        [TestMethod]
        public void TestCorrelationMatrix()
        {
            mesActions[0] = Action;
            mesActions[1] = Action2;
            mesActions[2] = Action3;
            double[] weight = { 0.1, 0.7, 0.2 };
            PricingLibrary.FinancialProducts.BasketOption myBasketOption = new PricingLibrary.FinancialProducts.BasketOption("test", mesActions, weight, date, 30.0);
            OptionManager option = new OptionManager(myBasketOption, date1, date, 20, true);
            for (DateTime daa = date1.AddDays(20); daa < date; daa = daa.AddDays(1))
            {
                //Console.WriteLine(myResults.getSpotPrice(d));

                double[,] mesVols = new double[3,3];
                mesVols = Estimators.getCorrelationMatrix(daa, option);
                Console.WriteLine(mesVols[0,0]);
                Console.WriteLine(mesVols[1,0]);
                Console.WriteLine(mesVols[2,0]);
                Console.WriteLine(mesVols[0, 1]);
                Console.WriteLine(mesVols[1, 1]);
                Console.WriteLine(mesVols[2, 1]);
                Console.WriteLine(mesVols[0, 2]);
                Console.WriteLine(mesVols[1, 2]);
                Console.WriteLine(mesVols[2, 2]);
            }
        }

        [TestMethod]
        public void TestSpotPrices()
        {
            mesActions[0] = Action;
            mesActions[1] = Action2;
            mesActions[2] = Action3;
            double[] weight = { 0.1, 0.7, 0.2 };
            PricingLibrary.FinancialProducts.BasketOption myBasketOption = new PricingLibrary.FinancialProducts.BasketOption("test", mesActions, weight, date, 30.0);
            OptionManager option = new OptionManager(myBasketOption, date1, date, 20, true);
            for (DateTime daa = date1.AddDays(20); daa < date; daa = daa.AddDays(1))
            {
                //Console.WriteLine(myResults.getSpotPrice(d));

                double[] spotPrices = new double[3];
                spotPrices = Estimators.getSpotPrices(daa, option);
                Console.WriteLine(spotPrices[0]);
                Console.WriteLine(spotPrices[1]);
                Console.WriteLine(spotPrices[2]);
            }
        }
    }
}
