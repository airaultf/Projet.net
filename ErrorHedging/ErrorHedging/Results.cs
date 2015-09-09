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
        [DllImport("./wre-ensimag-c-4.1.dll", EntryPoint = "WREanalysisExpostVolatility")]
        // declare external function
        public static extern int WREanalysisExpostVolatility(
            ref int nbValues,
            double[] portfolioReturns,
            double[] expostVolatility,
            int[] info
            );

         /*** function to compute volatility and encapsulate ***/
         /* WRE Function
         * throw exception if WREanalysisExpostVolatility encountered a problem
         * @portfolioReturns : share value at everydate used for
         *                     parameter estimation
         * @Return : computed volatility
         */
        public static double computeVolatility(double[] portfolioReturns)
        {
            double[] expostVolatility = new double[1];
            int nbValues = portfolioReturns.GetLength(0);
            int[] info = new int[1];
            int res = 0;
            res = WREanalysisExpostVolatility(ref nbValues, portfolioReturns, expostVolatility, info);
            if (res != 0)
            {
                if (res < 0)
                    throw new Exception("ERROR : WREanalysisExpostVolatility encountered a problem");
                else
                    throw new Exception("WARNING : WREanalysisExpostVolatility encountered a problem");
            }
            return expostVolatility[0];
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
                myHisto.loadingcharge();  
            }

            //Contruction de myPortfolio, et calcul des valeurs initiales de hedgingPortfolioValue et payoff
            double firstSpotPrice = getSpotPrice(this.startDate);
            double initialVol = getVolatility(this.startDate);

            if (myPortfolio is HedgingPortfolioVanillaCall){
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
            for (DateTime date = startDate; date <= maturityDate; date.AddDays(1)) // can be better done with foreach (faster) 
            {
                spotPrice = getSpotPrice(date);
                volatility = getVolatility(date);
                myPortfolio.updatePortfolioValue(spotPrice, date, volatility);
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

        /*** getVolatility ***/
        /* Function that computes volatility for a given date
         * with a fixed estimation window 
        /* @date : date at which we want to get volatility
         * @Return : volatility at this date
         */
        public double getVolatility(DateTime date)
        {
            double[] shareValuesForVolatilityEstimation = new double[testWindow+1];
            int cpt = 0;
            for (DateTime estimationStartDate = date.AddDays(-testWindow); estimationStartDate <= date; estimationStartDate.AddDays(1))
            {
                Console.WriteLine(estimationStartDate);
                shareValuesForVolatilityEstimation[cpt] = getSpotPrice(estimationStartDate);
                cpt++;
            }
            return computeVolatility(shareValuesForVolatilityEstimation);
        }
    }
}
