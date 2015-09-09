using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary;

namespace ErrorHedging
{
    // Classe abstraite qui implemente le portefeuille de couverture, permet de calculer la valeur du portefeuille
    abstract class HedgingPortfolio
    {
        // Valeur du portefeuille
        protected double _portfolioValue;
        protected double hedgeRatio;
        protected PricingLibrary.Computations.Pricer pricer;
        protected PricingLibrary.FinancialProducts.IOption _Product;

        // On initialise le portefeuille de couverture
        public HedgingPortfolio(PricingLibrary.FinancialProducts.IOption Product, System.DateTime date)
        {
            this._Product = Product;
            this._portfolioValue = 0;
            this.hedgeRatio = 0;
            this.pricer = new PricingLibrary.Computations.Pricer();
        }

        // Getter pour le produit
        public PricingLibrary.FinancialProducts.IOption Product
        {
            get
            {
                return this._Product;
            }
        }

        // Getter pour la valeur du portefeuille
        public double portfolioValue
        {
            get
            {
                return this._portfolioValue;
            }
        }

        
    }

    // Classe qui hérite 
    class HedgingPortfolioVanillaCall : HedgingPortfolio
    {

        public HedgingPortfolioVanillaCall(PricingLibrary.FinancialProducts.VanillaCall Call, System.DateTime date, double initialSpot ,double initialVol) :  base(Call, date)
        {
            // On calcule en plus la valeur p0 du portefeuille  
            PricingLibrary.Computations.PricingResults resultPricer = this.pricer.PriceCall(Call, date, 365, initialSpot, initialVol);

            this._portfolioValue = resultPricer.Price;
            this.hedgeRatio = resultPricer.Deltas[0];
        }

        // Methode qui met à jour la valeur du portefeuille de couverture ainsi que le delta
        //  @spot : Prix spot de l'action sous jacente
        //  @date : date correspondante au prix spot
        //  @volatility : vol correspondante à la date t, cette valeur est estimee en amont
        //
        //  @return : met à jour les attributs portfolioValue et hedgeRatio
        public void updatePortfolioValue(double spot, System.DateTime date, double volatility)
        {
            // On calcule le nouveau delta
            PricingLibrary.Computations.PricingResults resultPricer = this.pricer.PriceCall((PricingLibrary.FinancialProducts.VanillaCall)this.Product, date, 365, spot, volatility);

            System.TimeSpan diff = this.Product.Maturity.Subtract(date) ;
            int nbDays = diff.Days;

            double dateDouble = PricingLibrary.Utilities.DayToDoubleConverter.Convert(nbDays, 365);
            double riskFree = PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(dateDouble);

            this._portfolioValue = this.hedgeRatio * spot + (this.portfolioValue - this.hedgeRatio * spot) * riskFree ;
            this.hedgeRatio = resultPricer.Deltas[0];
        }

    }

}
