using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHedging
{
    static class Estimators
    {
        /*** getSpotPrices ***/
        /* Function that return the Spot prices for a given date
         * with a fixed estimation window 
        /* @date : date at which we want to get the spot prices
         * @Return : spotPrices at this date
         */
        public static double[] getSpotPrices(DateTime date, OptionManager option)
        {
            // Verification du correct appel de la methode
            if (!option.MyHisto.Data.Where(data => data.Date == date).Any())
            {
                throw new Exception("Erreur dans l'appel de GetSpotPrice avec une date a laquelle il n'y a pas de spotPrice");
            }
            double[] spotPrices = new double[option.NbShare];
            option.MyHisto.Data.Find(data => data.Date == date).PriceList.OrderBy(dataFeed => dataFeed.Key);

            int i = 0;
            foreach (KeyValuePair<string, decimal> data in option.MyHisto.Data.Find(data => data.Date == date).PriceList)
            {
                spotPrices[i] = (double)data.Value;
                i++;
            }
            return spotPrices;
        }


        /*** getVolatility ***/
        /* Function that computes volatility for a given date
         * with a fixed estimation window 
        /* @date : date at which we want to get volatility
         * @Return : volatility at this date
         */
        public static double[] getVolatilities(DateTime date, OptionManager option)
        {
            System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> histo = option.MyHisto.Data.Where(data => (data.Date >= date.AddDays(-option.TestWindow) && data.Date <= date)).ToList();
            histo.OrderBy(data => data.Date);
            int dimTemps = histo.Count;
            double[,] shareValuesForVolatilityEstimation = new double[dimTemps, option.NbShare];
            int temps = 0;
            int asset = 0;

            foreach (PricingLibrary.Utilities.MarketDataFeed.DataFeed data in histo)
            {
                asset = 0;
                foreach (KeyValuePair<string, decimal> keyValue in data.PriceList)
                {
                    shareValuesForVolatilityEstimation[temps, asset] = (double)keyValue.Value;
                    asset++;
                }
                temps++;
            }
            // $$$$$$$$$$$$$$$$$$ Debug  $$$$$$$$$$$$$$$$$$$$$$$$$
            //return new double[option.NbShare];
            return ComputeEstimators.computeVolatilities(ComputeEstimators.logReturn(shareValuesForVolatilityEstimation), option.Simulated);
            //return new double[1] { 0.4 };
        }

        public static double[,] getCorrelationMatrix(DateTime date, OptionManager option)
        {
            // correlation matrix not symetrical and defined positive   
            if (option.TestWindow < option.NbShare)
            {
                throw new Exception("ERROR : getCorrelationMatrix encountered a problem: Estimation window too small");
            }

            System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> histo = option.MyHisto.Data.Where(data => (data.Date >= date.AddDays(-option.TestWindow) && data.Date <= date)).ToList();
            histo.OrderBy(data => data.Date);
            int dimTemps = histo.Count;

            // correlation matrix not symetrical and defined positive 
            if (dimTemps < option.NbShare)
            {
                throw new Exception("ERROR : getCorrelationMatrix encountered a problem: Estimation window too small");
            }
            double[,] shareValuesForVolatilityEstimation = new double[dimTemps, option.NbShare];            
            int temps = 0;
            int asset = 0;

            foreach (PricingLibrary.Utilities.MarketDataFeed.DataFeed data in histo)
            {
                asset = 0;
                foreach (KeyValuePair<string, decimal> keyValue in data.PriceList)
                {
                    shareValuesForVolatilityEstimation[temps, asset] = (double)keyValue.Value;
                    asset++;
                }
                temps++;
            }
            //return new double[option.NbShare, option.NbShare];
            return ComputeEstimators.computeCorrelationMatrix(shareValuesForVolatilityEstimation);
        }

    }
}
