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
            }/*
            for (DateTime d = date.AddDays(-testWindow); d <= date; d = d.AddDays(1))
            {
                spotPricesAtDate = getSpotPrices(d);
                if (spotPricesAtDate.Length != 0)
                {
                    temp.Add(getSpotPrices(d));
                }
            }

            shareValuesForVolatilityEstimation = new double[temp.Count, nbShare];

            foreach(double[] t in temp){
                for (int i = 0; i < nbShare; i++)
                {
                    shareValuesForVolatilityEstimation[cpt, i] = t[i];
                    cpt++;
                }
            }*/
            return ComputeEstimators.computeVolatilities(ComputeEstimators.logReturn(shareValuesForVolatilityEstimation), option.Simulated);
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
            double[,] shareValuesForVolatilityEstimation = new double[dimTemps, option.NbShare];
            int temps = 0;
            int asset = 0;

            foreach (PricingLibrary.Utilities.MarketDataFeed.DataFeed data in histo)
            {
                foreach (KeyValuePair<string, decimal> keyValue in data.PriceList)
                {
                    shareValuesForVolatilityEstimation[temps, asset] = (double)keyValue.Value;
                    asset++;
                }
                temps++;
            }
            /*int assetNumber = this.nbShare;
            double dimTab = testWindow + 1;
            double[,] shareValuesForVolatilityEstimation = new double[(int)dimTab, nbShare];
            double[] spotPricesAtDate = new double[nbShare];
            int cpt = 0;

            for (DateTime d = date.AddDays(-testWindow); d <= date; d = d.AddDays(1))
            {
                spotPricesAtDate = getSpotPrices(d);
                for (int i = 0; i < shareValuesForVolatilityEstimation.GetLength(1); i++)
                {
                    shareValuesForVolatilityEstimation[cpt, i] = spotPricesAtDate[i];
                }
                cpt++;
            }*/
            return ComputeEstimators.computeCorrelationMatrix(shareValuesForVolatilityEstimation);
        }

    }
}
