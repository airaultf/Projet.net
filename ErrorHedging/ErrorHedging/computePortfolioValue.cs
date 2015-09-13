using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHedging
{
    // Classe qui permet de faire les calculs pour le reabalancement du portefeuille
    class computePortfolioValue
    {
        // 1 attribut qui est un pricer
        protected PricingLibrary.Computations.Pricer pricer;

        public computePortfolioValue()
        {
            this.pricer = new PricingLibrary.Computations.Pricer();
        }

        // Permet de price un produit selon les paramètres d'entrée
        public PricingLibrary.Computations.PricingResults priceProduct(PricingLibrary.FinancialProducts.IOption Product, DateTime date, double[] spot, double[] volatility, bool simulated, double[,] correlationMatrix = null)
        {
            int nbDays = 0;
            if (simulated)
            {
                nbDays = 365;
            }
            else
            {
                nbDays = 250;
            }

            if (Product is PricingLibrary.FinancialProducts.VanillaCall)
            {
                return this.pricer.PriceCall((PricingLibrary.FinancialProducts.VanillaCall)Product, date, nbDays, spot[0], volatility[0]);
            }
            else 
            {
                return this.pricer.PriceBasket((PricingLibrary.FinancialProducts.BasketOption)Product, date, nbDays, spot, volatility, correlationMatrix);
            }
        }

        // Calcule la nouvelle valeur du portefeuille selon la formule suivante :
        //      V(ti+1) = sum[delta(ti)*S(ti+1)] + (V(ti) - sum[delta(ti)*S(ti)])*Rf
        // V(t) est la valeur du portefeuille, S(t) la valeur du sous-jacent, delta(t) est le delta du sous-jacent
        // Rf est le taux sans risque sur la periode [ti,ti+1]
        public double computeValuePortfolio(double[] tabSpot, double[] tabDelta, double[] formerSpot, double riskFree, double lastValue)
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
                deltaFormerSpot += formerSpot[i] * tabDelta[i];
            }
             
            return deltaSpot + (lastValue - deltaFormerSpot) * riskFree;
        }
    }
}
