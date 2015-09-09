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
        private PricingLibrary.FinancialProducts.IOption product;

        public ShareHisto(System.DateTime startDate, System.DateTime maturityDate, PricingLibrary.FinancialProducts.IOption product)
        {
            this._chargeData = new List<PricingLibrary.Utilities.MarketDataFeed.DataFeed>();
            this._simulData = new List<PricingLibrary.Utilities.MarketDataFeed.DataFeed>();
            this.startDate = startDate.AddDays(30); // Ajuster ici pour selon fenetre pour premiere volatilité
            this.maturityDate = maturityDate;
            this.product = product;
        }

        public System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> chargeData
        {
            get {
                return this.chargeData;
            }
        }

        public System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> simulData
        {
            get
            {
                return this.simulData;
            }
        }

        public void loading() // charger aussi dans bdd ? 
        {
            this._simulData = PricingLibrary.Utilities.MarketDataFeed.SimulatedDataFeedProvider.GetDataFeed(this.product, this.startDate);
        }
    }
}