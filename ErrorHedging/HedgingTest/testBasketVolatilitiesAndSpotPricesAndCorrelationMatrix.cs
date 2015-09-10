using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHedging
{
    class testBasketVolatilitiesAndSpotPricesAndCorrelationMatrix
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
            Results myResults = new Results(myBasketOption, date1, date, 100, true);
            for (DateTime d = date1.AddDays(100); d < date; d = d.AddDays(1))
            {
                //Console.WriteLine(myResults.getSpotPrice(d));
                double[] mesVols = new double[3];
                mesVols =  myResults.getVolatilities(d);
                Console.WriteLine("VOL:" + mesVols[0] + mesVols[1] + mesVols[2]);
            };
        }

    }
}
