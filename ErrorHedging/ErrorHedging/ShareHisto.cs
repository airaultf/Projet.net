using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHedging
{
    class ShareHisto
    {
        private System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> _chargeData;
        private System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> _simulData;
        // beginning of test date
        private System.DateTime startDate;
        // end of test date = option maturity date
        private System.DateTime maturityDate;

        public ShareHisto(System.DateTime startDate, System.DateTime maturityDate)
        {
            this._chargeData = new List<PricingLibrary.Utilities.MarketDataFeed.DataFeed>();
            this._simulData = new List<PricingLibrary.Utilities.MarketDataFeed.DataFeed>();
            this.startDate = startDate;
            this.maturityDate = maturityDate;
        }

        public System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> chargeData
        {
            get{
                return this.chargeData;
            }
            set{
                this.chargeData = value; 
            }
        }
        public System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> simulData
        {
            get
            {
                return this.simulData;
            }
            set
            {
                for (System.DateTime date = this.startDate; date <= this.maturityDate; date.AddDays(1))
                {
                    //this.simulData.Add(  PricingLibrary.Utilities.MarketDataFeed.SimulatedDataFeedProvider.GetDataFeed(PricingLibrary.FinancialProducts.IOption, System.DateTime)
                } 
            }
        }

    }
}
