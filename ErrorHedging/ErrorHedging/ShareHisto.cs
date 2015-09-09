﻿using System;
using System.Data;
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

        public void loadingSimulated() // charger aussi dans bdd ? 
        {
            PricingLibrary.Utilities.MarketDataFeed.SimulatedDataFeedProvider import = new PricingLibrary.Utilities.MarketDataFeed.SimulatedDataFeedProvider();
            this._Data = import.GetDataFeed(this._product, this.startDate);
        }
        public void loadingcharge() // charger aussi dans bdd ? 
        {
            Console.WriteLine("NotImplementedException");
        }
    }
}