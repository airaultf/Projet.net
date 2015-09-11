﻿using System;
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
            DateTime date1 = new DateTime(2012, 6, 1, 0, 0, 0);
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");
            PricingLibrary.FinancialProducts.Share Action2 = new PricingLibrary.FinancialProducts.Share("test2", "02");
            PricingLibrary.FinancialProducts.Share Action3 = new PricingLibrary.FinancialProducts.Share("test3", "03");
            PricingLibrary.FinancialProducts.Share[] mesActions = new PricingLibrary.FinancialProducts.Share[3];
            mesActions[0] = Action;
            mesActions[1] = Action2;
            mesActions[2] = Action3;
            double[] weight = { 0.1, 0.7, 0.2 };
            PricingLibrary.FinancialProducts.BasketOption myBasketOption = new PricingLibrary.FinancialProducts.BasketOption("test", mesActions, weight, date, 30.0);
            Results myResults = new Results(myBasketOption, date1, date, 20, true);
            for (DateTime daa = date1.AddDays(20); daa < date; daa = daa.AddDays(1))
            {
                //Console.WriteLine(myResults.getSpotPrice(d));

                double[] mesVols = new double[3];
                mesVols = myResults.getVolatilities(daa);
                Console.WriteLine(mesVols[0]);
                Console.WriteLine(mesVols[1]);
                Console.WriteLine(mesVols[2]);
                double[,] matriceCov = new double[3, 3];
                matriceCov = myResults.getCovarianceMatrix(daa);
                Console.WriteLine(Math.Sqrt(matriceCov[0, 0]) * Math.Sqrt(365));
                Console.WriteLine(Math.Sqrt(matriceCov[1, 1]) * Math.Sqrt(365));
                Console.WriteLine(Math.Sqrt(matriceCov[2, 2]) * Math.Sqrt(365));
            }
        }
    }
}
