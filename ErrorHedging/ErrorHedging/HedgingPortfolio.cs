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
        protected double[] hedgeRatio;
        protected double formerSpot;
        protected PricingLibrary.Computations.Pricer pricer;
        protected PricingLibrary.FinancialProducts.IOption _Product;
        protected DateTime firstDay;

        // On initialise le portefeuille de couverture
        public HedgingPortfolio(PricingLibrary.FinancialProducts.IOption Product, System.DateTime date)
        {
            this._Product = Product;
            this._portfolioValue = 0;
            this.hedgeRatio = new double[] {0.0};
            this.formerSpot = 0;
            this.pricer = new PricingLibrary.Computations.Pricer();
            this.firstDay = date;
        }

        abstract public void updatePortfolioValue(double spot, System.DateTime date, double volatility);

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

    // Classe qui hérite de la classe HedgingPortfolio et qui l'implemente pour un produit de type Vanilla Call
    class HedgingPortfolioVanillaCall : HedgingPortfolio
    {

        public HedgingPortfolioVanillaCall(PricingLibrary.FinancialProducts.VanillaCall Call, System.DateTime date, double initialSpot ,double initialVol) :  base(Call, date)
        {
            // On calcule en plus la valeur p0 du portefeuille  
            PricingLibrary.Computations.PricingResults resultPricer = this.pricer.PriceCall(Call, date, 365, initialSpot, initialVol);

            this._portfolioValue = resultPricer.Price;
            this.hedgeRatio = new double[] {resultPricer.Deltas[0]};
            this.formerSpot = initialSpot;
        }

        // Methode qui met à jour la valeur du portefeuille de couverture ainsi que le delta
        //  @spot : Prix spot de l'action sous jacente
        //  @date : date correspondante au prix spot
        //  @volatility : vol correspondante à la date t, cette valeur est estimee en amont
        //
        //  @return : met à jour les attributs portfolioValue et hedgeRatio
        public override void updatePortfolioValue(double spot, System.DateTime date, double volatility)
        {
            // On price notre call à la date et au prix spot donnés
            PricingLibrary.Computations.PricingResults resultPricer = this.pricer.PriceCall((PricingLibrary.FinancialProducts.VanillaCall)this.Product, date, 365, spot, volatility);

            Console.WriteLine("Result pricer " + resultPricer.Price + " Delta " + resultPricer.Deltas[0]);

            System.TimeSpan diff = date.Subtract(this.firstDay) ;
            int nbDays = diff.Days;

            double dateDouble = PricingLibrary.Utilities.DayToDoubleConverter.Convert(nbDays, 365);

            double riskFree = PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(dateDouble);
            double tmp2 = this._portfolioValue - this.hedgeRatio[0] * this.formerSpot;

            Console.WriteLine("Hedge ratio " + this.hedgeRatio[0] + "Hedge Ratio après " + resultPricer.Deltas[0]);
            Console.WriteLine("Quantite 2 : " + tmp2 + " RiskFree " +riskFree);

            //  Math.Exp((riskFree-1.0)*dateDouble)
            this._portfolioValue = this.hedgeRatio[0] * spot + (this._portfolioValue - this.hedgeRatio[0] * this.formerSpot) * riskFree;

            this.formerSpot = spot;

            this.hedgeRatio[0] = resultPricer.Deltas[0];
        }
    }
}
