using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorHedging;
using PricingLibrary;

namespace HedgingTest
{
    [TestClass]
    public class UnitTest1
    {
        /*
        [TestMethod]
        public void TestMethod1()
        {
            DateTime date = DateTime.Now;
            PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test","01");
            PricingLibrary.FinancialProducts.Share[] tabAction = {Action};
            PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test",tabAction,date,8.0);

            DateTime date1 = new DateTime(2013, 9, 9, 0, 0, 0);
            ErrorHedging.HedgingPortfolio couvPort = new ErrorHedging.HedgingPortfolio(Call, date1, 10.0, 0.4);
            Console.WriteLine("Valeur portfolio " + couvPort.portfolioValue);
            // updatePortfolioValue(double spot, System.DateTime date, double volatility)
            
            for (int i = 0; i < 365; i++)
            {
                couvPort.updatePortfolioValue(10.0, date1, 0.4);
                Console.WriteLine("Valeur portfolio "+ couvPort.portfolioValue);
                Console.WriteLine("\n");
                date1 = date1.AddDays(1.0);
            }
        }
        */
        [TestMethod]
        public void TestRebalancementVanillaCall()
        {
            double ratio = 0;
            double compteur = 0;

            for (int i = 0; i < 300; i++)
            {
                DateTime date = DateTime.Now;
                PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");
                PricingLibrary.FinancialProducts.Share[] tabAction = { Action };

                PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test", tabAction, date, 9.0);


                DateTime dateStart = new DateTime(2014, 9, 9, 0, 0, 0);


                ErrorHedging.Results result = new ErrorHedging.Results(Call, dateStart, date, 20, true);

                double firstValue = result.HedgingPortfolioValue;

                result.computeResults();

                double payoff = result.Payoff;
                double lastValue = result.HedgingPortfolioValue;
                double ratioTmp = Math.Abs(payoff - lastValue) / firstValue;

                ratio += ratioTmp;
                compteur += 1;
            }

            ratio = ratio / compteur;
            Console.WriteLine(ratio);
        }

        [TestMethod]
        public void RebalancementBasket()
        {
            double ratio = 0;
            double compteur = 0;

            for (int i = 0; i < 300; i++)
            {
                DateTime date = DateTime.Now;
                PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");

                //PricingLibrary.FinancialProducts.Share Action1 = new PricingLibrary.FinancialProducts.Share("test1", "02");

                //PricingLibrary.FinancialProducts.Share[] tabAction = { Action, Action1 };
                PricingLibrary.FinancialProducts.Share[] tabAction = { Action };
                double[] weightTab = new double[] {1.0};

                PricingLibrary.FinancialProducts.BasketOption Basket = new PricingLibrary.FinancialProducts.BasketOption("basket", tabAction, weightTab, date, 9.0);

                DateTime dateStart = new DateTime(2014, 10, 9, 0, 0, 0);


                ErrorHedging.Results result = new ErrorHedging.Results(Basket, dateStart, date, 30, true);

                double firstValue = result.HedgingPortfolioValue;

                result.computeResults();

                double payoff = result.Payoff;
                double lastValue = result.HedgingPortfolioValue;
                double ratioTmp = Math.Abs(payoff - lastValue) / firstValue;

                ratio += ratioTmp;
                compteur += 1;
            }

            ratio = ratio / compteur;
            Console.WriteLine(ratio);

        }


        [TestMethod]
        public void RebalancementBasket3Sj()
        {
            double ratio = 0;
            double compteur = 0;

            for (int i = 0; i < 1; i++)
            {
                DateTime date = DateTime.Now;
                PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");

                PricingLibrary.FinancialProducts.Share Action1 = new PricingLibrary.FinancialProducts.Share("test1", "02");

                //PricingLibrary.FinancialProducts.Share Action2 = new PricingLibrary.FinancialProducts.Share("test1", "03");

                PricingLibrary.FinancialProducts.Share[] tabAction = { Action, Action1};
                double[] weightTab = new double[] { 0.4, 0.6};

                PricingLibrary.FinancialProducts.BasketOption Basket = new PricingLibrary.FinancialProducts.BasketOption("basket", tabAction, weightTab, date, 1);


                DateTime dateStart = new DateTime(2015, 2, 10, 0, 0, 0);


                ErrorHedging.Results result = new ErrorHedging.Results(Basket, dateStart, date, 20, true);

                double firstValue = result.HedgingPortfolioValue;

                result.computeResults();

                double payoff = result.Payoff;
                double lastValue = result.HedgingPortfolioValue;
                double ratioTmp = Math.Abs(payoff - lastValue) / firstValue;

                Console.WriteLine("payoff " + payoff +" lastvalue "+lastValue+" firstValue "+ firstValue);
                ratio += ratioTmp;
                compteur += 1;
            }

            ratio = ratio / compteur;
            Console.WriteLine(ratio);
        }

        [TestMethod]
        public void RebalancementBasket5Sj()
        {
            double ratio = 0;
            double compteur = 0;

            for (int i = 0; i < 200; i++)
            {
                DateTime date = DateTime.Now;
                PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");

                PricingLibrary.FinancialProducts.Share Action1 = new PricingLibrary.FinancialProducts.Share("test1", "02");

                PricingLibrary.FinancialProducts.Share Action2 = new PricingLibrary.FinancialProducts.Share("test1", "03");

                PricingLibrary.FinancialProducts.Share Action3 = new PricingLibrary.FinancialProducts.Share("test1", "04");

                PricingLibrary.FinancialProducts.Share Action4 = new PricingLibrary.FinancialProducts.Share("test1", "05");

                PricingLibrary.FinancialProducts.Share[] tabAction = { Action, Action1, Action2, Action3,Action4 };
                double[] weightTab = new double[] {0.2,0.2,0.2,0.2,0.2};

                PricingLibrary.FinancialProducts.BasketOption Basket = new PricingLibrary.FinancialProducts.BasketOption("basket", tabAction, weightTab, date, 9);


                DateTime dateStart = new DateTime(2015, 2, 10, 0, 0, 0);


                ErrorHedging.Results result = new ErrorHedging.Results(Basket, dateStart, date, 30, false);

                double firstValue = result.HedgingPortfolioValue;

                result.computeResults();

                double payoff = result.Payoff;
                double lastValue = result.HedgingPortfolioValue;
                double ratioTmp = Math.Abs(payoff - lastValue) / firstValue;

                ratio += ratioTmp;
                compteur += 1;
            }

            ratio = ratio / compteur;
            Console.WriteLine(ratio);
        }

    }
}
