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
        // [DllImport('wre-ensimag-c-4.1.dll')]


        // Contient un dictionnaire d'action avec les delta correspondant
        Dictionary<PricingLibrary.FinancialProducts.Share, double> StockDelta;
        double[] freeRate;
        int typeProduct;

        // Valeur du portefeuille
        double portfolioValue;


        // On initialise le portefeuille de couverture avec un Vanilla Call
        public HedgingPortfolio(PricingLibrary.FinancialProducts.VanillaCall Call)
        {
            int typeProduct = 0;
            double[,] freeRate = new double[0, 0];
            Dictionary<PricingLibrary.FinancialProducts.Share, double> StockDelta = new Dictionary<PricingLibrary.FinancialProducts.Share, double>();
            StockDelta.Add(Call.UnderlyingShare, 0);
        }

        public double updateHedgeRatioVanillaCall(System.DateTime date, double spot, PricingLibrary.FinancialProducts.VanillaCall Call, double vol)
        {
            PricingLibrary.Computations.Pricer price = new PricingLibrary.Computations.Pricer();
            PricingLibrary.Computations.PricingResults result = price.PriceCall(Call, date, 365, spot, vol);


            return 0;
            
        }



    }
}
