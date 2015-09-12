using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHedging
{
    class OptionManager
    {
        // options to test
        private HedgingPortfolio myPortfolio;

        public HedgingPortfolio MyPortfolio
        {
            get { return myPortfolio; }
            set { myPortfolio = value; }
        }

        private int nbShare;

        public int NbShare
        {
            get { return nbShare; }
            set { nbShare = value; }
        }

        // beginning of test date
        private System.DateTime startDate;

        public System.DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        // end of test date = option maturity date
        private System.DateTime maturityDate;

        public System.DateTime MaturityDate
        {
            get { return maturityDate; }
            set { maturityDate = value; }
        }

        // number (in days) for parameters estimation
        private int testWindow;

        public int TestWindow
        {
            get { return testWindow; }
            set { testWindow = value; }
        }

        // simulated = true <=> simulated datas
        private bool simulated;

        public bool Simulated
        {
            get { return simulated; }
            set { simulated = value; }
        }

        // Histo for the data feeds of a stock price 
        private ShareHisto myHisto;

        public ShareHisto MyHisto1
        {
            get { return myHisto; }
            set { myHisto = value; }
        }

        public ShareHisto MyHisto
        {
            get { return myHisto; }
            set { myHisto = value; }
        }


        /*** TEST RESULTS ***/

        // option payoff
        private double payoff;

        public double Payoff
        {
            get { return payoff; }
            set { payoff = value; }
        }

        // value of the hedging portfolio
        private double hedgingPortfolioValue;

        public double HedgingPortfolioValue
        {
            get { return hedgingPortfolioValue; }
            set { hedgingPortfolioValue = value; }
        }

        public OptionManager(PricingLibrary.FinancialProducts.IOption option, System.DateTime startDate, System.DateTime maturityDate, int testWindow, bool simulated)
        {
            this.startDate = startDate.AddDays(testWindow);
            this.maturityDate = maturityDate;
            this.testWindow = testWindow;
            this.simulated = simulated;
            this.nbShare = option.UnderlyingShareIds.Length;
            
            // Les données sont chargées
            this.myHisto = new ShareHisto(this.startDate.AddDays(-testWindow), this.maturityDate, option);
            if (simulated){
                myHisto.loadingSimulated();
            }else{
                myHisto.loadingSQL();  
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

            firstSpotPrice = Estimators.getSpotPrices(this.startDate, this);
            initialVol = Estimators.getVolatilities(this.startDate, this);

            if (option is PricingLibrary.FinancialProducts.VanillaCall)
            {
                this.myPortfolio = new HedgingPortfolio((PricingLibrary.FinancialProducts.VanillaCall)option, this.startDate, firstSpotPrice, initialVol); // spot a aller chercher, volatilité à calculer
            }
            else if (option is PricingLibrary.FinancialProducts.BasketOption)
            {
                matriceCorrelation = Estimators.getCorrelationMatrix(date, this);
                this.myPortfolio = new HedgingPortfolio((PricingLibrary.FinancialProducts.BasketOption)option, this.startDate, firstSpotPrice, initialVol, matriceCorrelation); // spot a aller chercher, volatilité à calculer
            }
            else
            {
                throw new NotSupportedException();
            }

            this.hedgingPortfolioValue = myPortfolio.portfolioValue;
            this.payoff = myPortfolio.Product.GetPayoff(myHisto.Data.Find(data => data.Date == this.startDate).PriceList);
        }
    }
}
