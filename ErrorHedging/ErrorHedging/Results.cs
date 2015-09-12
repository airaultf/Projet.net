using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ErrorHedging
{
    class Results
    {
        // Import the WRE dll for fetching volatility
        // from datas
        [DllImport(@"C:\Users\ensimag\Documents\GitHub\ProjetPointNet\Projet.net\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCorr", CallingConvention = CallingConvention.Cdecl)]
        // declare external function
        public static extern int WREanalysisExpostVolatility(
            ref int nbValues,
            double[] portfolioReturns,
            ref double expostVolatility,
            ref int info
            );

        [DllImport(@"C:\Users\ensimag\Documents\GitHub\ProjetPointNet\Projet.net\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCorr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WREmodelingCorr(
            ref int nbValues,
            ref int nbAssets,
            double[,] assetsReturns,
            double[,] corr,
            ref int info
            );


        [DllImport(@"C:\Users\ensimag\Documents\GitHub\ProjetPointNet\Projet.net\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCorr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WREmodelingCov(
            ref int nbValues,
            ref int nbAssets,
            double[,] assetsReturns,
            double[,] cov,
            ref int info
        );

        [DllImport(@"C:\Users\ensimag\Documents\GitHub\ProjetPointNet\Projet.net\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCorr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WREmodelingLogReturns(
            ref int nbValues,
            ref int nbAssets,
            double[,] assetsValues,
            ref int horizon,
            double[,] assetsReturns,
            ref int info
        );
         /*** function to compute volatility and encapsulate ***/
         /* WRE Function
         * throw exception if WREanalysisExpostVolatility encountered a problem
         * @portfolioReturns : share value at everydate used for
         *                     parameter estimation
         * @Return : computed volatility
         */
        public static double computeVolatility(double[,] portfolioReturns)
        {
            double[] portfolioReturn1D = new double[portfolioReturns.GetLength(0)];
            for (int i=0; i<portfolioReturns.GetLength(0); i++){
                portfolioReturn1D[i] = portfolioReturns[i,0];
            }
            double expostVolatility = 0;
            int nbValues = portfolioReturns.GetLength(0);
            int info = 0;
            int res = 0;
            res = WREanalysisExpostVolatility(ref nbValues, portfolioReturn1D, ref expostVolatility, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR : WREanalysisExpostVolatility encountered a problem");
                else
                    throw new Exception("WARNING : WREanalysisExpostVolatility encountered a problem");
            }
            return expostVolatility;
        }

        public static double[] computeVolatilities(double[,] portfolioReturns, bool simulated)
        {
            double[] volatilities = new double[portfolioReturns.GetLength(1)];
            
            // on boucle sur les actions
            for (int i = 0; i < portfolioReturns.GetLength(1); i++)
            {
                double[] portfolioReturn1D = new double[portfolioReturns.GetLength(0)];

                for (int j = 0; j < portfolioReturns.GetLength(0); j++)
                {
                    portfolioReturn1D[j] = portfolioReturns[j, i];
                }
                double expostVolatility = 0;
                int nbValues = portfolioReturns.GetLength(0);
                int info = 0;
                int res = 0;
                res = WREanalysisExpostVolatility(ref nbValues, portfolioReturn1D, ref expostVolatility, ref info);
                if (res != 0)
                {
                    if (res < 0)
                        throw new Exception("ERROR : WREanalysisExpostVolatility encountered a problem");
                    else
                        throw new Exception("WARNING : WREanalysisExpostVolatility encountered a problem");
                }
                if (simulated)
                    volatilities[i] = Math.Sqrt(365) * expostVolatility;
                else
                    volatilities[i] = Math.Sqrt(250) * expostVolatility;
            }

            return volatilities;
        }

        public static double[,] computeCorrelationMatrix(double[,] assetsReturns)
        {
            int nbValues = assetsReturns.GetLength(0);
            int nbAssets = assetsReturns.GetLength(1);
            int info = 0;
            int res = 0;
            double[,] corr = new double[nbAssets, nbAssets];
            res = WREmodelingCorr(ref nbValues, ref nbAssets, assetsReturns, corr, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR : WREmodelingCorr encountered a problem");
                else
                    throw new Exception("WARNING : WREmodelingCorr encountered a problem");
            }
            return corr;
        }

        public static double[,] computeCovarianceMatrix(double[,] assetsReturns)
        {
            int nbValues = assetsReturns.GetLength(0);
            int nbAssets = assetsReturns.GetLength(1);
            int info = 0;
            int res = 0;
            double[,] cov = new double[nbAssets, nbAssets];
            res = WREmodelingCov(ref nbValues, ref nbAssets, assetsReturns, cov, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR : WREmodelingCorr encountered a problem");
                else
                    throw new Exception("WARNING : WREmodelingCorr encountered a problem");
            }
            return cov;
        }

        public static double[,] logReturn(double[,] assetsValues)
        {
            int nbValues = assetsValues.GetLength(0);
            int nbAssets = assetsValues.GetLength(1);
            double[,] assetReturns = new double[nbValues, nbAssets];
            for (int i = 1; i < nbValues; i++)
            {
                for (int action = 0; action < nbAssets; action++)
                {
                    assetReturns[i - 1, action] = Math.Log((assetsValues[i, action] / assetsValues[i - 1, action]));
                }
            }
                return assetReturns;
        }

        /*public static double[,] logReturn(double[,] assetsValues)
        {
            int nbValues = assetsValues.GetLength(0);
            int nbAssets = assetsValues.GetLength(1);
            int info = 0;
            int horizon = 1;
            double[,] assetsReturns = new double[nbValues-horizon, nbAssets];
            int res = WREmodelingLogReturns(ref nbValues, ref nbAssets, assetsValues, ref horizon, assetsReturns, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR : WREmodelingLogReturns encountered a problem");
                else
                    throw new Exception("WARNING : WREmodelingLogReturns encountered a problem");
        }
            return assetsReturns;
        }*/

        /*** TEST PARAMETERS ***/


        // options to test
        private HedgingPortfolio myPortfolio;

        protected int nbShare;

        // beginning of test date
        private System.DateTime startDate;

        // end of test date = option maturity date
        private System.DateTime maturityDate;

        // number (in days) for parameters estimation
        private int testWindow;

        // simulated = true <=> simulated datas
        private bool simulated;

        // Histo for the data feeds of a stock price 
        private ShareHisto myHisto;


        /*** TEST RESULTS ***/ 

        // option payoff
        protected List<double> payoff;

        // value of the hedging portfolio
        protected List<double> hedgingPortfolioValue;


        public List<double> Payoff
        {
            get
            {
                return this.payoff;
            }
        }
        public List<double> HedgingPortfolioValue
        {
            get
            {
                return this.hedgingPortfolioValue;
            }
        }

        // Constructeur de Results, initialise les TEST PARAMETERS et les TEST RESULTS (aux valeurs en date startDate)
        //  @option : option que l'on va tester
        //  @startDate : Date de départ du test (le test commence à startDate + testWindow)
        //  @MaturityDate : Date de fin du test
        //  @testWindow : Taille de la fenettre qui permet de calculer la volatilité
        //  @simulated : Pour des données simulées ou réelles

        public Results(PricingLibrary.FinancialProducts.IOption option, System.DateTime startDate, System.DateTime maturityDate, int testWindow, bool simulated)
        {
            this.startDate = startDate.AddDays(testWindow);
            this.maturityDate = maturityDate;
            this.testWindow = testWindow;
            this.simulated = simulated;
            this.nbShare = option.UnderlyingShareIds.Length;
            this.hedgingPortfolioValue = new List<double>();
            this.payoff = new List<double>();
            
            // Les données sont chargées
            this.myHisto = new ShareHisto(this.startDate.AddDays(-testWindow), this.maturityDate, option);
            if (simulated){
                myHisto.loadingSimulated();
            }else{
                myHisto.loadingSQL();
                if (myHisto.Data.First().PriceList.Count != this.nbShare)
                {
                    throw new Exception("Mauvais nom donné à une action");
                }
            }
            myHisto.Data.OrderBy(x => x.Date); // on classe les données

            //Contruction de myPortfolio, et calcul des valeurs initiales de hedgingPortfolioValue et payoff
             double[] firstSpotPrice = null;
            double[] initialVol = null;
            double[,] matriceCorrelation = null;

            // La startDate utilisée est la première date avant la startDate pour laquelle ne sont pas vides
            DateTime date = this.startDate;
            while(!myHisto.Data.Where(data => data.Date == date).Any() && date >= this.startDate.AddDays(testWindow)){
                date = date.AddDays(-1);
            }
            if(!myHisto.Data.Where(data => data.Date == date).Any()){
                throw new NotImplementedException();
            }
            this.startDate = date;

            firstSpotPrice = getSpotPrices(this.startDate);
            initialVol = getVolatilities(this.startDate);

            if (option is PricingLibrary.FinancialProducts.VanillaCall)
            {
                this.myPortfolio = new HedgingPortfolio((PricingLibrary.FinancialProducts.VanillaCall)option, this.startDate, firstSpotPrice, initialVol); // spot a aller chercher, volatilité à calculer
            }
            else if (option is PricingLibrary.FinancialProducts.BasketOption)
            {
                matriceCorrelation = getCorrelationMatrix(this.startDate);
                this.myPortfolio = new HedgingPortfolio((PricingLibrary.FinancialProducts.BasketOption)option, this.startDate, firstSpotPrice, initialVol, matriceCorrelation); // spot a aller chercher, volatilité à calculer
            }
            else
            {
                throw new NotSupportedException();
            }

            this.hedgingPortfolioValue.Add(myPortfolio.portfolioValue); 
            this.payoff.Add(myPortfolio.Product.GetPayoff(myHisto.Data.Find(data => data.Date == this.startDate).PriceList));
        }



        // A ETTENDRE POUR BASKET
        // Calcul le résultat. Itère jour par jour pour resortir à la fin le payoff et le hedgingPortfolioValue.
        // Faire ensuite une version qui stocke ces résultats.
        public void computeResults()
        {
            double[] spotPrice;
            double[] volatility;
            double[,] matriceCorrelation;
            System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> histo = myHisto.Data.Where(data => (data.Date >= this.startDate && data.Date <= this.maturityDate)).ToList();
            this.hedgingPortfolioValue.Clear();
            this.payoff.Clear();

            foreach (PricingLibrary.Utilities.MarketDataFeed.DataFeed data in histo)
            {
                spotPrice = getSpotPrices(data.Date);
                volatility = getVolatilities(data.Date);

                if (myPortfolio.Product is PricingLibrary.FinancialProducts.VanillaCall)
                {
                    myPortfolio.updatePortfolioValue(spotPrice, data.Date, volatility);
                }
                else if (myPortfolio.Product is PricingLibrary.FinancialProducts.BasketOption)
                {
                    matriceCorrelation = getCorrelationMatrix(data.Date);
                    myPortfolio.updatePortfolioValue(spotPrice, data.Date, volatility, matriceCorrelation);
                }
                else
                {
                    throw new NotImplementedException();
                }

                this.hedgingPortfolioValue.Add(myPortfolio.portfolioValue);
                this.payoff.Add(myPortfolio.Product.GetPayoff(data.PriceList));
            }
        }


        /*** getSpotPrices ***/
        /* Function that return the Spot prices for a given date
         * with a fixed estimation window 
        /* @date : date at which we want to get the spot prices
         * @Return : spotPrices at this date
         */
        public double[] getSpotPrices(DateTime date)
        {
            // Verification du correct appel de la methode
            if (!myHisto.Data.Where(data => data.Date == date).Any())
            {
                throw new Exception("Erreur dans l'appel de GetSpotPrice avec une date a laquelle il n'y a pas de spotPrice");
            }

            myHisto.Data.Find(data => data.Date == date).PriceList.OrderBy(dataFeed => dataFeed.Key);
            int taille = this.nbShare;
            double[] spotPrices = new double[taille];

            int i = 0; 
            foreach (KeyValuePair<string, decimal> data in myHisto.Data.Find(data => data.Date == date).PriceList)
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
        public double[] getVolatilities(DateTime date)
        {
            System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> histo  = myHisto.Data.Where(data => (data.Date >= date.AddDays(-this.testWindow) && data.Date <= date)).ToList();
            histo.OrderBy(data => data.Date);
            int dimTemps = histo.Count;
            double[,] shareValuesForVolatilityEstimation = new double[dimTemps,this.nbShare];
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
            return new double[nbShare];// computeVolatilities(logReturn(shareValuesForVolatilityEstimation), simulated);
        }


        public double[,] getCorrelationMatrix(DateTime date)
        {
            System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> histo = myHisto.Data.Where(data => (data.Date >= date.AddDays(-this.testWindow) && data.Date <= date)).ToList();
            histo.OrderBy(data => data.Date);
            int dimTemps = histo.Count;

            // correlation matrix not symetrical and defined positive 
            if (dimTemps < nbShare)
            {
                throw new Exception("ERROR : getCorrelationMatrix encountered a problem: Estimation window too small");
            }
            double[,] shareValuesForVolatilityEstimation = new double[dimTemps, this.nbShare];
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
            //$$$$$$$$$$$$$$$$$$$$  Debug $$$$$$$$$$$$$$$$$$$$$$$$$
            return new double[nbShare, nbShare]; //computeCorrelationMatrix(shareValuesForVolatilityEstimation);
        }
                    
    }
}