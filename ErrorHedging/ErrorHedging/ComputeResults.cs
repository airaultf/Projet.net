using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHedging
{
    static class ComputeResults
    {
        public static void computeResults(OptionManager option)
        {
            double[] spotPrice;
            double[] volatility;
            double[,] matriceCorrelation;
            double _hedgingPortfolioValue = 0; // Valeur intermediaire
            double _payoff = 0;                // Valeur intermediaire

            foreach (PricingLibrary.Utilities.MarketDataFeed.DataFeed data in option.MyHisto.Data)
            {
                //for (DateTime date = this.startDate; date <= maturityDate; date=date.AddDays(1)) // can be better done with foreach (faster) 
                spotPrice = Estimators.getSpotPrices(data.Date, option);
                volatility = Estimators.getVolatilities(data.Date, option);

                if (option.MyPortfolio.Product is PricingLibrary.FinancialProducts.VanillaCall)
                {
                    option.MyPortfolio.updatePortfolioValue(spotPrice, data.Date, volatility);
                }
                else if (option.MyPortfolio.Product is PricingLibrary.FinancialProducts.BasketOption)
                {
                    matriceCorrelation = Estimators.getCorrelationMatrix(data.Date, option); 
                    option.MyPortfolio.updatePortfolioValue(spotPrice, data.Date, volatility, matriceCorrelation);
                }
                else
                {
                    throw new NotImplementedException();
                }

                _hedgingPortfolioValue = option.MyPortfolio.portfolioValue;
                _payoff = option.MyPortfolio.Product.GetPayoff(data.PriceList);
                option.HedgingPortfolioValue = _hedgingPortfolioValue;
                option.Payoff = _payoff;
            }
        }
    }
}
