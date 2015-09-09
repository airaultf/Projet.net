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
            ref double portfolioReturns,
            ref double expostVolatility,
            ref int info
            );
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
            this.myHisto = new ShareHisto(startDate, maturityDate,myPortfolio.Product); // mettre getter a product

            // On initialise le portefeuille à la première journée
            myHisto = new ShareHisto(this.startDate, this.maturityDate, option);
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
            myPortfolio.updatePortfolioValue(firstSpotPrice, this.startDate, initialVol);
            this.hedgingPortfolioValue = myPortfolio.portfolioValue;
            this.payoff = myPortfolio.Product.GetPayoff(myHisto.Data.Find(data => data.Date == this.startDate).PriceList);
        }

        // A ETTENDRE POUR BASKET
        // Calcul le résultat. Itère jour par jour pour resortir à la fin le payoff et le hedgingPortfolioValue.
        // Faire ensuite une version qui stocke ces résultats.
        public void computeResults()
        {
            // Pour basket, spot price et volatilité = tableau
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

        public double[] getSpotPrice

        // A ETTENDRE POUR BASKET
        // Renvoie la volatilité d'une action
        public double getVolatility(DateTime date)
        {

            return 0;
        }
    }
}
