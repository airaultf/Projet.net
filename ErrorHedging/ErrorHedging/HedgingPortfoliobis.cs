/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary;

namespace ErrorHedging
{
    // Classe abstraite qui implemente le portefeuille de couverture, permet de calculer la valeur du portefeuille
    abstract class HedgingPortfoliobis
    {
        // Valeur du portefeuille
        protected double _portfolioValue;
        protected double[] hedgeRatio;
        protected double[] formerSpot;
        protected PricingLibrary.Computations.Pricer pricer;
        protected PricingLibrary.FinancialProducts.IOption _Product;
        protected DateTime lastDay;
        protected computePortfolioValue computeAttribut;

        // On initialise le portefeuille de couverture
        public HedgingPortfolio(PricingLibrary.FinancialProducts.IOption Product, System.DateTime date)
        {
            this._Product = Product;
            this._portfolioValue = 0.0;
            this.hedgeRatio = new double[] { 0.0 };
            this.formerSpot = new double[] { 0.0 };
            this.pricer = new PricingLibrary.Computations.Pricer();
            this.lastDay = date;
            this.computeAttribut = new computePortfolioValue();
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
	
	@"C:\Users\ensimag\Documents\GitHub\ProjetPointNet\Projet.net\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCorr"

    // Classe qui hérite de la classe HedgingPortfolio et qui l'implemente pour un produit de type Vanilla Call
    class HedgingPortfolioVanillaCall : HedgingPortfolio
    {

        public HedgingPortfolioVanillaCall(PricingLibrary.FinancialProducts.VanillaCall Call, System.DateTime date, double initialSpot, double initialVol)
            : base(Call, date)
        {
            // On calcule en plus la valeur p0 du portefeuille  
            PricingLibrary.Computations.PricingResults resultPricer = this.pricer.PriceCall(Call, date, 365, initialSpot, initialVol);

            this._portfolioValue = resultPricer.Price;
            this.hedgeRatio = new double[] { resultPricer.Deltas[0] };
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


            System.TimeSpan diff = date.Subtract(this.lastDay);
            int nbDays = diff.Days;


            double dateDouble = PricingLibrary.Utilities.DayCount.ConvertToDouble(nbDays, 365);

            double riskFree = PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(dateDouble);
            double tmp2 = this._portfolioValue - this.hedgeRatio[0] * this.formerSpot[0];

            this._portfolioValue = computeAttribut.computeVanillaCall(this.hedgeRatio[0], spot, this.formerSpot[0], riskFree, this._portfolioValue);

            this.formerSpot[0] = spot;
            this.lastDay = date;
            this.hedgeRatio[0] = resultPricer.Deltas[0];
        }

    }




    // Classe qui etend la 
    class HedgingPortfolioBasketOption : HedgingPortfolio
    {

        protected double[,] CorrelationMatrix;

        // Constructeur pour cette classe
        public HedgingPortfolioBasketOption(PricingLibrary.FinancialProducts.BasketOption Basket, System.DateTime date, double[] initialSpot, double[] initialVol, double[,] CorrelationMatrix)
            : base(Basket, date)
        {
            // On calcule en plus la valeur p0 du portefeuille  
            PricingLibrary.Computations.PricingResults resultPricer = this.pricer.PriceBasket(Basket, date, 365, initialSpot, initialVol, CorrelationMatrix);

            this._portfolioValue = resultPricer.Price;
            this.hedgeRatio = resultPricer.Deltas;
            this.formerSpot = initialSpot;
            this.CorrelationMatrix = CorrelationMatrix;
        }

        public double computeValuePortfolio(double[] tabSpot, double[] tabDelta, double riskFree)
        {
            if (tabSpot.Length != tabDelta.Length)
            {
                throw new FormatException();
            }

            double deltaSpot = 0.0;
            double deltaFormerSpot = 0.0;

            for (int i = 0; i < tabSpot.Length; i++)
            {
                deltaSpot += tabSpot[i] * tabDelta[i];
                deltaFormerSpot += this.formerSpot[i] * tabDelta[i];
            }

            return deltaSpot + (this._portfolioValue - deltaFormerSpot) * riskFree;
        }

        // Methode qui calcule la nouvelle valeur du portefeuille, les tableaux sont toujours ordonnés de la même manière
        //  @tabSpot : tableau contenant les prix spot des sous jacent
        //  @date : date pour laquelle on rebalance le portefeuille
        //  @tabVolatility : tableau contenant les volatilités
        //
        //  @return : la valeur actualisée de notre portefeuille
        public override void updatePortfolioValue(double[] tabSpot, System.DateTime date, double[] tabVolatility)
        {
            // On price notre call à la date et au prix spot donnés
            PricingLibrary.Computations.PricingResults resultPricer = this.pricer.PriceBasket((PricingLibrary.FinancialProducts.BasketOption)this.Product, date, 365, tabSpot, tabVolatility, this.CorrelationMatrix);

            // On calcule le nombre de jour entre le dernier rebalancement et le rabalancement actuel, on convertit ce nombre en double
            System.TimeSpan diff = date.Subtract(this.lastDay);
            int nbDays = diff.Days;
            double dateDouble = PricingLibrary.Utilities.DayCount.ConvertToDouble(nbDays, 365);

            // On calcule la part d'actif sans risque
            double riskFree = PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(dateDouble);


            // On calcule la valeur du portfolio grace à la methode computeValuePortfolio
            this._portfolioValue = this.computeValuePortfolio(tabSpot, this.hedgeRatio, riskFree);

            // Sauvegarde des differents paramètres pour le prochain rebalancement
            this.formerSpot = tabSpot;
            this.lastDay = date;
            this.hedgeRatio = resultPricer.Deltas;
        }

    }

}
*/