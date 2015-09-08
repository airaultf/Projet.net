using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHedging
{
    class Test
    {
        /*** TEST PARAMETERS ***/

        // options to test
        ExtendedOption myOption;

        // beginning of test date
        Date startDate;

        // end of test date = option maturity date
        Date maturityDate;

        // number (in days) for parameters estimation
        int testWindow;


        /*** TEST RESULTS ***/

        // option payoff
        double payoff;

        // value of the hedging portfolio
        double hedgingPortfolioValue;
    }
}
