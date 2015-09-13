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
            double[] spotPrice = null;
            double[] volatility = null;
            double[,] matriceCorrelation = null;
            System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> histo = option.MyHisto.Data.Where(data => (data.Date >= option.StartDate && data.Date <= option.MaturityDate)).ToList();
            option.HedgingPortfolioValue.Clear();
            option.Payoff.Clear();
            option.dateTime.Clear();
            option.OptionPrice.Clear();

            foreach (PricingLibrary.Utilities.MarketDataFeed.DataFeed data in histo)
            {
                spotPrice = Estimators.getSpotPrices(data.Date, option);
                volatility = Estimators.getVolatilities(data.Date, option);


                if (option.MyPortfolio.Product is PricingLibrary.FinancialProducts.VanillaCall)
                {
                    option.MyPortfolio.updatePortfolioValue(spotPrice, data.Date, volatility,option.Simulated);
                }
                else if (option.MyPortfolio.Product is PricingLibrary.FinancialProducts.BasketOption)
                {
                    matriceCorrelation = Estimators.getCorrelationMatrix(data.Date, option);
                    option.MyPortfolio.updatePortfolioValue(spotPrice, data.Date, volatility,option.Simulated, matriceCorrelation);
                }
                else
                {
                    throw new NotImplementedException();
                }

                option.HedgingPortfolioValue.Add(option.MyPortfolio.portfolioValue);
                option.Payoff.Add(option.MyPortfolio.Product.GetPayoff(data.PriceList));
                option.dateTime.Add(data.Date);
                option.OptionPrice.Add(option.MyPortfolio.ComputeAttribut.priceProduct(option.MyPortfolio.Product, data.Date, spotPrice, volatility, matriceCorrelation).Price);
            }
        }
    }
}