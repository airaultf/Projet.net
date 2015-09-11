using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorHedging;
using PricingLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace HedgingTest
{
    [TestClass]
    /*** Test for data simulation & loaded datas ***/
    public class testShareHisto
    {
        //[TestMethod]
        /*** Test for data simulation ***/
        /* Print all simulated values 
         * for one share between two dates */
        /*public void TestDataSimulation()
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
        }*/

        [TestMethod]
        /*** Test for data simulation ***/
        /* Print all loaded values 
         * for one share between two dates */
        public void TestLoadedData()
        {
            DateTime date = DateTime.Now;
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("ALO FP", "ALO FP");
            PricingLibrary.FinancialProducts.Share[] tabAction = { Action };
            PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("ALO FP", tabAction, date, 30.0);

            DateTime date1 = new DateTime(2015, 6, 1, 0, 0, 0);

            ShareHisto myShareHisto = new ShareHisto(date1, date, Call);

            myShareHisto.loadingSQL();

            foreach (PricingLibrary.Utilities.MarketDataFeed.DataFeed d in myShareHisto.Data)
            {
                foreach (KeyValuePair<string, decimal> t in d.PriceList)
                {
                    Console.WriteLine("Date : " + d.Date);
                    Console.WriteLine("    "  + t.Key + "   " + t.Value);
                }
            }

        }

        [TestMethod]
        /*** Test for data simulation ***/
        /* Print all loaded values 
         * for one share between two dates */
        public void TestLoadedDataBasket()
        {
            DateTime date = DateTime.Now;
            PricingLibrary.FinancialProducts.Share Action1 = new PricingLibrary.FinancialProducts.Share("JJ", "ALO FP");
            PricingLibrary.FinancialProducts.Share Action2 = new PricingLibrary.FinancialProducts.Share("JJ", "BNP FP");

            PricingLibrary.FinancialProducts.Share[] tabAction = { Action1, Action2};
            double[] poids = new double[2] {0.4, 0.6};
            PricingLibrary.FinancialProducts.BasketOption Call = new PricingLibrary.FinancialProducts.BasketOption("JJ", tabAction, poids, date, 30.0);

            DateTime date1 = new DateTime(2015, 6, 1, 0, 0, 0);

            ShareHisto myShareHisto = new ShareHisto(date1, date, Call);

            myShareHisto.loadingSQL();

            foreach (PricingLibrary.Utilities.MarketDataFeed.DataFeed d in myShareHisto.Data)
            {
                foreach (KeyValuePair<string, decimal> t in d.PriceList)
                {
                    Console.WriteLine("Date : " + d.Date);
                    Console.WriteLine("" + t.Key + "   " + t.Value);
                }
            }

        }

    }
}
