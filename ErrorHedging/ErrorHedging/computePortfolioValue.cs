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
        // 1 attribut 
        protected PricingLibrary.Computations.Pricer pricer;

        public computePortfolioValue()
        {
            this.pricer = new PricingLibrary.Computations.Pricer();
        }


        public PricingLibrary.Computations.PricingResults priceProduct(PricingLibrary.FinancialProducts.IOption Product, DateTime date, double[] spot, double[] volatility, double[,] correlationMatrix = null)
        {

            if (Product is PricingLibrary.FinancialProducts.VanillaCall)
            {
                return this.pricer.PriceCall((PricingLibrary.FinancialProducts.VanillaCall)Product, date, 365, spot[0], volatility[0]);
            }
            else 
            {
                return this.pricer.PriceBasket((PricingLibrary.FinancialProducts.BasketOption)Product, date, 365, spot, volatility, correlationMatrix);
            }
        }

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
