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
        [DllImport(@"C:\Users\ensimag\Source\Repos\Projet.net2\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREanalysisExpostVolatility", CallingConvention=CallingConvention.Cdecl)]
        // declare external function
        public static extern int WREanalysisExpostVolatility(
            ref int nbValues,
            double[] portfolioReturns,
            ref double expostVolatility,
            ref int info
            );

        [DllImport(@"C:\Users\ensimag\Source\Repos\Projet.net2\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingLogReturns", CallingConvention = CallingConvention.Cdecl)]
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

        public static double[] computeVolatilities(double[,] portfolioReturns)
        {
            double[] volatilities = new double[portfolioReturns.GetLength(1)];
            // on boucle sur les actions
            for (int i = 0; i < portfolioReturns.GetLength(1); i++)
            {
                double[] portfolioReturn1D = new double[portfolioReturns.GetLength(0)];
                for (int j = 0; j < portfolioReturns.GetLength(0); i++)
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
                volatilities[i] = expostVolatility;
            }
            return volatilities;
        }

        public static double[,] logReturn(double[,] assetsValues, int horizon)
        {
            int nbValues = assetsValues.GetLength(0);
            int nbAssets = assetsValues.GetLength(1);
            double[,] assetReturns = new double[nbValues, nbAssets];
            for (int i = 1; i < nbValues; i++)
            {
                for (int action=0; action<nbAssets; action++){
                    assetReturns[i - 1, action] = Math.Log((assetsValues[i, action] / assetsValues[i - 1, action]));
                }
            }
                return assetReturns;
        }
        /*public static double[,] logReturn(double[,] assetsValues, ref int horizon)
        {
            int nbValues = assetsValues.GetLength(0);
            int nbAssets = assetsValues.GetLength(1);
            int info = 0;
            double[,] assetsReturns = new double[nbValues-horizon, nbAssets];
            int res = WREmodelingLogReturns(ref nbValues, ref nbAssets, assetsValues, ref horizon, assetsReturns, ref info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR : WREmodelingLogReturns encountered a problem");
                else
                    throw new Exception("WARNING : WREmodelingLogReturns encountered a problem");
        }
            for (int i = 0; i < assetsReturns.Length; i++)
            {
                double j = assetsReturns[i,0];
                Console.WriteLine(j);
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
        private double payoff;

        // value of the hedging portfolio
        private double hedgingPortfolioValue;


        public double Payoff
        {
            get
            {
                return this.payoff;
            }
        }
        public double HedgingPortfolioValue
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
            
            // On initialise le portefeuille à la première journée
            this.myHisto = new ShareHisto(this.startDate.AddDays(-testWindow-1), this.maturityDate, option);
            if (simulated){
                myHisto.loadingSimulated();
            }else{
                myHisto.loadingcharge();  
            }

            //Contruction de myPortfolio, et calcul des valeurs initiales de hedgingPortfolioValue et payoff
            double firstSpotPrice = getSpotPrice(this.startDate);
            double initialVol = 0.4;
            //double initialVol = 0.4;

            if (option is PricingLibrary.FinancialProducts.VanillaCall){
                this.myPortfolio = new HedgingPortfolioVanillaCall((PricingLibrary.FinancialProducts.VanillaCall)option, this.startDate, firstSpotPrice, initialVol); // spot a aller chercher, volatilité à calculer
            }else{
                System.Console.WriteLine("notImplementedExeption");
            }
         
            //myPortfolio.updatePortfolioValue(firstSpotPrice, this.startDate, initialVol);
            this.hedgingPortfolioValue = myPortfolio.portfolioValue;
            this.payoff = myPortfolio.Product.GetPayoff(myHisto.Data.Find(data => data.Date == this.startDate).PriceList);
        }



        // A ETTENDRE POUR BASKET
        // Calcul le résultat. Itère jour par jour pour resortir à la fin le payoff et le hedgingPortfolioValue.
        // Faire ensuite une version qui stocke ces résultats.
        public void computeResults()
        {
            double spotPrice = 0;
            double volatility = 0;
            double _hedgingPortfolioValue = 0; // Valeur intermediaire
            double _payoff = 0;                // Valeur intermediaire
            for (DateTime date = startDate; date <= maturityDate; date=date.AddDays(1)) // can be better done with foreach (faster) 
            {
                spotPrice = getSpotPrice(date);
                //volatility = getVolatility(date);
                myPortfolio.updatePortfolioValue(spotPrice, date, 0.4);
                _hedgingPortfolioValue = myPortfolio.portfolioValue;
                _payoff = myPortfolio.Product.GetPayoff(myHisto.Data.Find(data => data.Date == date).PriceList);
            }
            this.hedgingPortfolioValue = _hedgingPortfolioValue;
            this.payoff = _payoff;
        }

        // A ETTENDRE POUR BASKET
        // Renvoie le prix spot d'une action
        public double getSpotPrice(DateTime date)
        {
            double spotPrice = 0;
            spotPrice = (double) myHisto.Data.Find(data => data.Date == date).PriceList.First().Value;
            return spotPrice;
        }

        public double[] getSpotPrices(DateTime date)
        {
            int taille = this.myPortfolio.Product.UnderlyingShareIds.Length;
            double[] spotPrices = new double[taille];
            int i = 0;
            myHisto.Data.Find(data => data.Date == date).PriceList.OrderBy(dataFeed => dataFeed.Key);
            foreach(KeyValuePair<string, decimal> data in myHisto.Data.Find(data => data.Date == date).PriceList)
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
        public double getVolatility(DateTime date)
        {
            double dimTab = testWindow +1;
            double[,] shareValuesForVolatilityEstimation = new double[(int)dimTab,1];
            int horizon = (int)((maturityDate-startDate).TotalDays);
            int cpt = 0;
            for (DateTime d = date.AddDays(-testWindow); d <= date; d=d.AddDays(1))
            {
                shareValuesForVolatilityEstimation[cpt,0] = getSpotPrice(d);
                cpt++;
            }
            return Math.Sqrt(365)*computeVolatility(logReturn(shareValuesForVolatilityEstimation, horizon));
        }

        public double getVolatility(DateTime date)
        {
            double dimTab = testWindow + 1;
            double[,] shareValuesForVolatilityEstimation = new double[(int)dimTab, 1];
            int horizon = (int)((maturityDate - startDate).TotalDays);
            int cpt = 0;
            for (DateTime d = date.AddDays(-testWindow); d <= date; d = d.AddDays(1))
            {
                shareValuesForVolatilityEstimation[cpt, 0] = getSpotPrice(d);
                cpt++;
            }
            if (simulated)
                return Math.Sqrt(365) * computeVolatility(logReturn(shareValuesForVolatilityEstimation, horizon));
            else
                return Math.Sqrt(250) * computeVolatility(logReturn(shareValuesForVolatilityEstimation, horizon));
        }

        public double getVolatilities(DateTime date)
        {
            int assetNumber = this.nbShare;
            double dimTab = testWindow + 1;
            double[,] shareValuesForVolatilityEstimation = new double[(int)dimTab, nbShare];
            double[] spotPricesAtDate = new double[nbShare];
            int horizon = (int)((maturityDate - startDate).TotalDays);
            int cpt = 0;
            for (DateTime d = date.AddDays(-testWindow); d <= date; d = d.AddDays(1))
            {
                spotPricesAtDate = getSpotPrices(d);
                for (int i=0; i<shareValuesForVolatilityEstimation.GetLength(1); i++){
                    shareValuesForVolatilityEstimation[cpt, i] = spotPricesAtDate[i];
                    cpt++;
                }
            }
            if (simulated)
                return Math.Sqrt(365) * computeVolatility(logReturn(shareValuesForVolatilityEstimation, horizon));
            else
                return Math.Sqrt(250) * computeVolatility(logReturn(shareValuesForVolatilityEstimation, horizon));
        }
    }
}
