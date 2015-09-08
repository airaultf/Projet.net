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
        private ExtendedOption myOption;

        // beginning of test date
        private System.DateTime startDate;

        // end of test date = option maturity date
        private System.DateTime maturityDate;

        // number (in days) for parameters estimation
        private int testWindow;


        /*** TEST RESULTS ***/ 

        // option payoff
        private double payoff;

        // value of the hedging portfolio
        private double hedgingPortfolioValue;


        public Results(ExtendedOption myOption, System.DateTime startDate, System.DateTime maturityDate, int testWindow)
        {
            this.myOption = myOption;
            this.startDate = startDate;
            this.maturityDate = maturityDate;
            this.testWindow = testWindow;
            this.payoff = 0;
            this.hedgingPortfolioValue = 0;
        }

        public void computeResults()
        {

        }

    }
}
