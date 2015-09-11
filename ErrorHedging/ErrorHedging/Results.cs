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
        [DllImport(@"C:\Users\ensimag\Source\Repos\Projet.net2\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREanalysisExpostVolatility")]
        // declare external function
        public static extern int WREanalysisExpostVolatility(
            ref int nbValues,
            double[] portfolioReturns,
            ref double expostVolatility,
            ref int info
            );

        [DllImport(@"C:\Users\ensimag\Source\Repos\Projet.net2\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingLogReturns")]
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
            for (int i = 0; i<portfolioReturns.GetLength(0); i++){
                portfolioReturn1D[i] = portfolioReturns[i,1];
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
        public static double[,] logReturn(double[,] assetsValues, int horizon)
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
            return assetsReturns;
        }

        /*** TEST PARAMETERS ***/


        // options to test
        private HedgingPortfolio myPortfolio;

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
                this.myPortfolio = new HedgingPortfolio((PricingLibrary.FinancialProducts.VanillaCall)option, this.startDate, firstSpotPrice, initialVol); // spot a aller chercher, volatilité à calculer
            }
            else if (option is PricingLibrary.FinancialProducts.BasketOption)
            {
                matriceCorrelation = getMatriceCorrelation(this.startDate);
                this.myPortfolio = new HedgingPortfolio((PricingLibrary.FinancialProducts.BasketOption)option, this.startDate, firstSpotPrice, initialVol, matriceCorrelation); // spot a aller chercher, volatilité à calculer
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
            double[] spotPrice;
            double[] volatility;
            double[,] matriceCorrelation;
            double _hedgingPortfolioValue = 0; // Valeur intermediaire
            double _payoff = 0;                // Valeur intermediaire
            for (DateTime date = startDate; date <= maturityDate; date=date.AddDays(1)) // can be better done with foreach (faster)
            {
                spotPrice = getSpotPrices(date);                 // !!!!!!!!!!!!!!!!!!!! implementé mais à tester
                volatility = getVolatilities(date);              // !!!!!!!!!!!!!!!!!!!! Pas implementé
                if (myPortfolio.Product is PricingLibrary.FinancialProducts.VanillaCall){
                    myPortfolio.updatePortfolioValue(spotPrice, date, volatility);
                }else if (myPortfolio.Product is PricingLibrary.FinancialProducts.BasketOption){
                    matriceCorrelation = getMatriceCorrelation(this.startDate);
                    myPortfolio.updatePortfolioValue(spotPrice, date, volatility, matriceCorrelation);
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
            double dimTab = ((maturityDate.AddDays(testWindow)-startDate)).TotalDays;
            double[,] shareValuesForVolatilityEstimation = new double[(int)dimTab,1];
            double horizon = (maturityDate-startDate).TotalDays;
            int cpt = 0;
            for (DateTime d = date.AddDays(-testWindow); d <= maturityDate; d=d.AddDays(1))
            {
                shareValuesForVolatilityEstimation[cpt,0] = getSpotPrice(d);
                cpt++;
            }
            return computeVolatility(logReturn(shareValuesForVolatilityEstimation, (int)horizon));
        }


        public double[] getVolatilities(DateTime date)
        {
            double[] res = new double[1];
            Console.WriteLine("notImplementedExeption");
            return res;
        }

        /*** getMatriceCorrelation ***/
        /* Function that return the correlation matrice for a given date
         * with a fixed estimation window 
        /* @date : date at which we want to get the correlation matrice
         * @Return : correlation matrice at this date
         */ 
        public double[,] getMatriceCorrelation(DateTime date)
        {
            double[,] res = new double[1,1];
            return res;
        }
    }
}
