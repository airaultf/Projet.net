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
        // option
        private HedgingPortfolio myPortfolio;

        public ShareHisto(System.DateTime startDate, System.DateTime maturityDate, HedgingPortfolio myPortfolio)
        {
            this._chargeData = new List<PricingLibrary.Utilities.MarketDataFeed.DataFeed>();
            this._simulData = new List<PricingLibrary.Utilities.MarketDataFeed.DataFeed>();
            this.startDate = startDate;
            this.maturityDate = maturityDate;
            this.myPortfolio = myPortfolio;
        }

        public System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> chargeData
        {
            get {
                return this.chargeData;
            }
            set {
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
                this.simulData = PricingLibrary.Utilities.MarketDataFeed.SimulatedDataFeedProvider.GetDataFeed(this.myPortfolio.Option, this.startDate);
            }
        }
    }
}