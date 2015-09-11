using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHedging
{
    class ShareHisto
    {
        private System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> _Data; // just one List that contains loaded or simulated data
        // beginning of test date
        private System.DateTime startDate;
        // end of test date = option maturity date
        private System.DateTime maturityDate;
        // option
        private PricingLibrary.FinancialProducts.IOption _product;

        // On initialise la classe 
        public ShareHisto(System.DateTime startDate, System.DateTime maturityDate, PricingLibrary.FinancialProducts.IOption product)
        {
            this._Data = new List<PricingLibrary.Utilities.MarketDataFeed.DataFeed>();
            this.startDate = startDate;
            this.maturityDate = maturityDate;
            this._product = product;
        }

        public System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> Data
        {
            get {
                return this._Data;
            }
        }


        /*** loadingSimulated ***/
        /* Function that initialise data with simulated data
         */
        public void loadingSimulated()
        {
            PricingLibrary.Utilities.MarketDataFeed.SimulatedDataFeedProvider import = new PricingLibrary.Utilities.MarketDataFeed.SimulatedDataFeedProvider();
            this._Data = import.GetDataFeed(this._product, this.startDate);
        }


        /*** loadingSQL ***/
        /* Function that initialise data with simulated data
         */
        public void loadingSQL(){

            for (DateTime date = startDate; date <= maturityDate; date = date.AddDays(1))
            {
                using (MyLocalDBDataContext mdc = new MyLocalDBDataContext())
                {
                    List<String> res1 = mdc.HistoricalShareValues.Where(x => (x.date == date)).Select(el => el.id).Distinct().ToList();
                    System.Collections.Generic.Dictionary<string, decimal> res2 = new Dictionary<string,decimal>();
                    foreach (var c in res1)//in this._product.UnderlyingShareIds.ToList())
                    {
                        if(res1.Contains(c)){
                            decimal temp = mdc.HistoricalShareValues.Where(x => (x.date == date && x.id == c)).Select(x => x.value).Distinct().First();
                            res2.Add((string)c, (decimal)temp);
                        }
                    }
                    this._Data.Add(new PricingLibrary.Utilities.MarketDataFeed.DataFeed(date, res2));
                }
                Console.WriteLine("on est bon");

            }
            Console.WriteLine("on est bon");
        }

        public void loading()
        {
            using (MyLocalDBDataContext mdc = new MyLocalDBDataContext())
            {
                var res1 = mdc.HistoricalShareValues.Select(el => el.id.Trim()).Distinct().ToList();
                foreach (var c in res1)
                {
                    Console.WriteLine("Action : " + c);
                }
            }

        }
    }
}