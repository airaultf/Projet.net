using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHedging
{
    class Results
    {
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


        public Results(PricingLibrary.FinancialProducts.IOption option, System.DateTime startDate, System.DateTime maturityDate, int testWindow, bool simulated)
        {
     
            // On initialise le portefeuille à la première journée, gerer cas simu ou non
            myHisto = new ShareHisto(startDate, maturityDate, option);
            myHisto.loading();

            //Calculer initialVol
            double firstSpotPrice = myHisto.Find();
            double initialVol;

            if (myPortfolio is HedgingPortfolioVanillaCall) {
                this.myPortfolio = new HedgingPortfolioVanillaCall(option, startDate, firstSpotPrice, initialVol); // spot a aller chercher, volatilité à calculer
            } else {
                this.myPortfolio = null;
            }
            this.startDate = startDate;
            this.maturityDate = maturityDate;
            this.testWindow = testWindow;
            this.payoff = 0;
            this.hedgingPortfolioValue = 0;
            this.simulated = simulated;
            this.myHisto = new ShareHisto(startDate, maturityDate,myPortfolio.Product); // mettre getter a product
        }

        public void computeResults()
        {
            double spotPrice;
            
            if (simulated){
                spotPrice = this.getSpotPriceFromSimulatedData();
            } else {
                //spotPrice = this.getSpotPriceFromFetchedData();
            }
        }

        
    }
}
