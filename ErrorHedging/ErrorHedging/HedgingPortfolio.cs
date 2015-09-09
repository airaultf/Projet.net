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
        protected double[] formerSpot;
        protected PricingLibrary.Computations.Pricer pricer;
        protected PricingLibrary.FinancialProducts.IOption _Product;
        protected DateTime lastDay;

        // On initialise le portefeuille de couverture
        public HedgingPortfolio(PricingLibrary.FinancialProducts.IOption Product, System.DateTime date)
        {
            this._Product = Product;
            this._portfolioValue = 0.0;
            this.hedgeRatio = new double[] {0.0};
            this.formerSpot = new double[] {0.0};
            this.pricer = new PricingLibrary.Computations.Pricer();
            this.lastDay = date;
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

            this._portfolioValue  = resultPricer.Price;
            this.hedgeRatio = new double[] {resultPricer.Deltas[0]};
            this.formerSpot[0] = initialSpot;
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


            System.TimeSpan diff = date.Subtract(this.lastDay) ;
            int nbDays = diff.Days;

            double dateDouble = PricingLibrary.Utilities.DayToDoubleConverter.Convert(nbDays, 365);

            double riskFree = PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(dateDouble);
            double tmp2 = this._portfolioValue  - this.hedgeRatio[0] * this.formerSpot[0] ;


            //  Math.Exp((riskFree-1.0)*dateDouble)
            this._portfolioValue = this.hedgeRatio[0] * spot + (this._portfolioValue - this.hedgeRatio[0] * this.formerSpot[0]) * riskFree;

            this.formerSpot[0]  = spot;
            this.lastDay = date;
            this.hedgeRatio[0] = resultPricer.Deltas[0];
        }
    }
    /*
    class HedgingPortfolioBasketOption : HedgingPortfolio
    {

        protected double[,] CorrelationMatrix;

        public double computeValuePortfolio(double lastValue, double[] tabSpot, double[] tabDelta, double riskFree)
        {
            if (tabSpot.Length != tabDelta.Length) {
                throw new FormatException();
            }

            double deltaSpot = 0.0;
            double sum1 = 0.0;
            double sum2 = 0.0;

            for (int i = 0; i < tabSpot.Length; i++)
            {
                deltaSpot = tabSpot[i] * tabDelta[i];



            }

                return 0.0;
        }

        public HedgingPortfolioBasketOption(PricingLibrary.FinancialProducts.BasketOption Basket, System.DateTime date, double[] initialSpot, double[] initialVol,double[,] CorrelationMatrix) : base(Basket, date)
        {
            // On calcule en plus la valeur p0 du portefeuille  
            PricingLibrary.Computations.PricingResults resultPricer = this.pricer.PriceBasket(Basket, date, 365, initialSpot, initialVol,CorrelationMatrix);

            this._portfolioValue = resultPricer.Price;
            this.hedgeRatio = resultPricer.Deltas;
            this.formerSpot = initialSpot;
            this.CorrelationMatrix = CorrelationMatrix;
        }

        public override void updatePortfolioValue(double[] spot, System.DateTime date, double[] volatility)
        {
            // On price notre call à la date et au prix spot donnés
            PricingLibrary.Computations.PricingResults resultPricer = this.pricer.PriceBasket((PricingLibrary.FinancialProducts.BasketOption)this.Product, date, 365, spot, volatility, this.CorrelationMatrix);


            System.TimeSpan diff = date.Subtract(this.lastDay);
            int nbDays = diff.Days;

            double dateDouble = PricingLibrary.Utilities.DayToDoubleConverter.Convert(nbDays, 365);

            double riskFree = PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(dateDouble);
            double tmp2 = this._portfolioValue - this.hedgeRatio[0] * this.formerSpot[0];


            //  Math.Exp((riskFree-1.0)*dateDouble)
            
            this._portfolioValue = this.hedgeRatio[0] * spot + (this._portfolioValue - this.hedgeRatio[0] * this.formerSpot[0]) * riskFree;

            this.formerSpot[0] = spot;
            this.lastDay = date;
            this.hedgeRatio[0] = resultPricer.Deltas[0];
        }

    }
  */
}

