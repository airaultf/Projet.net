using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PricingLibrary;

namespace ErrorHedging
{
    class HedgingPortfolio
    {
        // Attributs du portefeuille
        protected double _portfolioValue;
        protected double[] hedgeRatio;
        protected double[] formerSpot;
        protected PricingLibrary.FinancialProducts.IOption _Product;
        protected DateTime lastDay;

        // Attribut qui permet de faire les calculs
        protected computePortfolioValue computeAttribut;

        public HedgingPortfolio(PricingLibrary.FinancialProducts.IOption Product, System.DateTime date, double[] initialSpot, double[] initialVol, double[,] correlationMatrix = null)
        {
            this.formerSpot = initialSpot;
            this.lastDay = date;
            this.computeAttribut = new computePortfolioValue();

            PricingLibrary.Computations.PricingResults result = computeAttribut.priceProduct(Product, date, initialSpot, initialVol, correlationMatrix);

            this._portfolioValue = result.Price;
            this.hedgeRatio = result.Deltas;
            this._Product = Product;
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
        public computePortfolioValue ComputeAttribut
        {
            get
            {
                return this.computeAttribut;
            }
        }

        // Methode qui met à jour la valeur du portefeuille
        public void updatePortfolioValue(double[] tabSpot, System.DateTime date, double[] tabVolatility, bool simulated,double[,] correlationMatrix = null)
        {
            // On price notre call à la date et au prix spot donnés
            PricingLibrary.Computations.PricingResults resultPricer = this.computeAttribut.priceProduct(this._Product, date, tabSpot, tabVolatility, correlationMatrix);

            // On calcule le nombre de jour entre le dernier rebalancement et le rabalancement actuel, on convertit ce nombre en double
            int nbDays = 0;
            double dateDouble = 0;
            if (simulated)
            {
                System.TimeSpan diff = date.Subtract(this.lastDay);
                nbDays = diff.Days;
                dateDouble = PricingLibrary.Utilities.DayCount.ConvertToDouble(nbDays, 365);
            } else
            {
                nbDays = PricingLibrary.Utilities.DayCount.CountBusinessDays(this.lastDay, date);
                dateDouble = PricingLibrary.Utilities.DayCount.ConvertToDouble(nbDays, 250);
            }

            // On calcule la part d'actif sans risque
            double riskFree = PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(dateDouble);

            // On calcule la valeur du portfolio grace à la methode computeValuePortfolio
            double lastValue = this._portfolioValue;
            this._portfolioValue = this.computeAttribut.computeValuePortfolio(tabSpot, this.hedgeRatio, this.formerSpot , riskFree, lastValue);

            //Sauvegarde des differents paramètres pour le prochain rebalancement
            this.formerSpot = tabSpot;
            this.lastDay = date;
            this.hedgeRatio = resultPricer.Deltas;
        }

    }
}
