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
        [DllImport(@"C:\Users\ensimag\Documents\GitHub\ProjetPointNet\Projet.net\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREanalysisExpostVolatility", CallingConvention = CallingConvention.Cdecl)]
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
                myHisto.loadingSQL();  
            }

            //Contruction de myPortfolio, et calcul des valeurs initiales de hedgingPortfolioValue et payoff
            double[] firstSpotPrice = getSpotPrices(this.startDate);             // !!!!!!!!!!!!!!!!!!!! implementé mais à tester
            double[] initialVol = getVolatilities(this.startDate);               // !!!!!!!!!!!!!!!!!!!! pas implementé
            double[,] matriceCorrelation = null;

            if (option is PricingLibrary.FinancialProducts.VanillaCall){
                double[] vol = new double[] { 0.4 };
                this.myPortfolio = new HedgingPortfolio((PricingLibrary.FinancialProducts.VanillaCall)option, this.startDate, firstSpotPrice, vol); // spot a aller chercher, volatilité à calculer
            }
            else if (option is PricingLibrary.FinancialProducts.BasketOption)
            {
                double[] vol = new double[] {0.4, 0.4, 0.4, 0.4, 0.4};
                matriceCorrelation = getCorrelationMatrix(this.startDate);
                this.myPortfolio = new HedgingPortfolio((PricingLibrary.FinancialProducts.BasketOption)option, this.startDate, firstSpotPrice, vol, matriceCorrelation); // spot a aller chercher, volatilité à calculer
            }
            else
            {
                Console.WriteLine("Not implemented exeption");
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
            double spotPricetab;
            double[] volatility;
            double[,] matriceCorrelation;
            double _hedgingPortfolioValue = 0; // Valeur intermediaire
            double _payoff = 0;                // Valeur intermediaire

            for (DateTime date = startDate; date <= maturityDate; date=date.AddDays(1)) // can be better done with foreach (faster) 
            {
                spotPricetab = getSpotPrice(date);                 // !!!!!!!!!!!!!!!!!!!! implementé mais à tester
                volatility = getVolatilities(date);             // !!!!!!!!!!!!!!!!!!!! Pas implementé
                double[] spotPrice = new double[] {spotPricetab};

                if (myPortfolio.Product is PricingLibrary.FinancialProducts.VanillaCall){
                    double[] vol = new double[] { 0.4 };
                    myPortfolio.updatePortfolioValue(spotPrice, date, vol);
                }else if (myPortfolio.Product is PricingLibrary.FinancialProducts.BasketOption){
                    double[] spotPrice1 = getSpotPrices(date);
                    matriceCorrelation = getCorrelationMatrix(this.startDate);
                    double[] vol = new double[] { 0.4, 0.4, 0.4, 0.4, 0.4 };
                    //Console.WriteLine("vol " + volatility[0] + "matrice corre " + matriceCorrelation[0,0]);
                    myPortfolio.updatePortfolioValue(spotPrice1, date, vol, matriceCorrelation);
                }else{
                    Console.WriteLine("Not implemented exeption");
                }

                _hedgingPortfolioValue = myPortfolio.portfolioValue;
                _payoff = myPortfolio.Product.GetPayoff(myHisto.Data.Find(data => data.Date == date).PriceList);
            }

            this.hedgingPortfolioValue = _hedgingPortfolioValue;
            this.payoff = _payoff;
        }

        /*** getSpotPrice ***/
        /* Function that return the Spot price for a given date
         * with a fixed estimation window 
        /* @date : date at which we want to get the spot prices
         * @Return : spotPrice at this date
         */
        public double getSpotPrice(DateTime date)
        {
            double spotPrice = 0;
            spotPrice = (double) myHisto.Data.Find(data => data.Date == date).PriceList.First().Value;
            return spotPrice;
        }

        /*** getSpotPrices ***/
        /* Function that return the Spot prices for a given date
         * with a fixed estimation window 
        /* @date : date at which we want to get the spot prices
         * @Return : spotPrices at this date
         */
        public double[] getSpotPrices(DateTime date)
        {
            
            int taille = this.nbShare;
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

        public double[] getVolatilities(DateTime date)
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

                for (int i = 0; i < shareValuesForVolatilityEstimation.GetLength(1); i++)
                {
                    shareValuesForVolatilityEstimation[cpt, i] = spotPricesAtDate[i];
                    
                }
                cpt++;
               
            }
            return computeVolatilities(logReturn(shareValuesForVolatilityEstimation, horizon), simulated);
        }


        public double[,] getCorrelationMatrix(DateTime date)
        {
            // correlation matrix not symetrical and defined positive
            if ( testWindow < nbShare )
            {
                throw new Exception("ERROR : getCorrelationMatrix encountered a problem: Estimation window too small");
            }
            int assetNumber = this.nbShare;
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
            }
            return computeCorrelationMatrix(shareValuesForVolatilityEstimation);
        }
    }
}