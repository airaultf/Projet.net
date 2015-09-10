using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorHedging;
using PricingLibrary;

namespace HedgingTest
{
    [TestClass]
    /*** Test for data simulation & loaded datas ***/
    public class testShareHisto
    {
        [TestMethod]
        /*** Test for data simulation ***/
        /* Print all simulated values 
         * for one share between two dates */
        public void TestDataSimulation()
        {
            DateTime date = DateTime.Now;
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");
            PricingLibrary.FinancialProducts.Share[] tabAction = { Action };
            PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test", tabAction, date, 30.0);

            DateTime date1 = new DateTime(2014, 6, 1, 0, 0, 0);

            ShareHisto myShareHisto = new ShareHisto(date1, date, Call);

            myShareHisto.loadingSimulated();
            decimal value=0;
            

            myShareHisto.Data.ForEach(delegate(PricingLibrary.Utilities.MarketDataFeed.DataFeed data)
            {
                data.PriceList.TryGetValue("01", out value);
                Console.WriteLine(data.Date + "       " + value.ToString());
            }
            );
        }

        [TestMethod]
        /*** Test for data simulation ***/
        /* Print all loaded values 
         * for one share between two dates */
        public void TestLoadedData()
        {
            DateTime date = DateTime.Now;
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");
            PricingLibrary.FinancialProducts.Share[] tabAction = { Action };
            PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test", tabAction, date, 30.0);

            DateTime date1 = new DateTime(2014, 6, 1, 0, 0, 0);

            ShareHisto myShareHisto = new ShareHisto(date1, date, Call);

            //myShareHisto.loadingcharge();
            decimal value = 0;


            myShareHisto.Data.ForEach(delegate(PricingLibrary.Utilities.MarketDataFeed.DataFeed data)
            {
                data.PriceList.TryGetValue("01", out value);
                Console.WriteLine(data.Date + "       " + value.ToString());
            }
            );
        }
    }
}
