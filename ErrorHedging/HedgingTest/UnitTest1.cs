using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorHedging;
using PricingLibrary;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HedgingTest
{
    [TestClass]
    public class TrackingErrorTest
    {
        
        [TestMethod]
        public void RebalancementVanillaCallS8()
        {
            double ratio = 0;
            double compteur = 0;

            for (int i = 0; i < 200; i++)
            {
                DateTime date = DateTime.Now;
                PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");
                PricingLibrary.FinancialProducts.Share[] tabAction = { Action };

                PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test", tabAction, date, 8.0);
                DateTime dateStart = new DateTime(2014, 9, 9, 0, 0, 0);

                OptionManager optionCompute = new OptionManager(Call,dateStart, date, 30, true);
                ComputeResults.computeResults(optionCompute);

              

                double firstValue = optionCompute.HedgingPortfolioValue[0];
                double lastValue = optionCompute.HedgingPortfolioValue[optionCompute.HedgingPortfolioValue.Count-1];
                double payoff = optionCompute.Payoff[optionCompute.Payoff.Count-1];
                
                double ratioTmp = Math.Abs(payoff - lastValue) / firstValue;
                ratio += ratioTmp;
                compteur += 1;
            }

            ratio = ratio / compteur;
            Console.WriteLine(ratio);
        }

        [TestMethod]
        public void RebalancementVanillaCallATM()
        {
            double ratio = 0;
            double compteur = 0;

            for (int i = 0; i < 200; i++)
            {
                DateTime date = DateTime.Now;
                PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");
                PricingLibrary.FinancialProducts.Share[] tabAction = { Action };

                PricingLibrary.FinancialProducts.VanillaCall Call = new PricingLibrary.FinancialProducts.VanillaCall("test", tabAction, date, 10.0);
                DateTime dateStart = new DateTime(2014, 9, 9, 0, 0, 0);

                OptionManager optionCompute = new OptionManager(Call, dateStart, date, 30, true);
                ComputeResults.computeResults(optionCompute);



                double firstValue = optionCompute.HedgingPortfolioValue[0];
                double lastValue = optionCompute.HedgingPortfolioValue[optionCompute.HedgingPortfolioValue.Count - 1];
                double payoff = optionCompute.Payoff[optionCompute.Payoff.Count - 1];

                double ratioTmp = Math.Abs(payoff - lastValue) / firstValue;
                ratio += ratioTmp;
                compteur += 1;
            }

            ratio = ratio / compteur;
            Console.WriteLine(ratio);
        }
        
        [TestMethod]
        public void RebalancementBasket1SJ_S8()
        {
            double ratio = 0;
            double compteur = 0;

            for (int i = 0; i < 200; i++)
            {
                DateTime date = DateTime.Now;
                PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("ALO FP", "ALO FP");

                
                PricingLibrary.FinancialProducts.Share[] tabAction = { Action };
                double[] weightTab = new double[] {1.0};

                PricingLibrary.FinancialProducts.BasketOption Basket = new PricingLibrary.FinancialProducts.BasketOption("BASKET", tabAction, weightTab, date, 9.0);

                DateTime dateStart = new DateTime(2014, 10, 9, 0, 0, 0);

                OptionManager optionCompute = new OptionManager(Basket, dateStart, date, 30, true);
                ComputeResults.computeResults(optionCompute);



                double firstValue = optionCompute.HedgingPortfolioValue[0];
                double lastValue = optionCompute.HedgingPortfolioValue[optionCompute.HedgingPortfolioValue.Count - 1];
                double payoff = optionCompute.Payoff[optionCompute.Payoff.Count - 1];

                double ratioTmp = Math.Abs(payoff - lastValue) / firstValue;

                
                ratio += ratioTmp;
                compteur += 1;
            }

            ratio = ratio / compteur;
            Console.WriteLine(ratio);

        }


        [TestMethod]
        public void RebalancementBasket3Sj_S8()
        {
            double ratio = 0;
            double compteur = 0;

            for (int i = 0; i < 200; i++)
            {
                DateTime date = DateTime.Now;
                PricingLibrary.FinancialProducts.Share Action = new PricingLibrary.FinancialProducts.Share("test", "01");

                PricingLibrary.FinancialProducts.Share Action1 = new PricingLibrary.FinancialProducts.Share("test1", "02");

                PricingLibrary.FinancialProducts.Share Action2 = new PricingLibrary.FinancialProducts.Share("test1", "03");

                PricingLibrary.FinancialProducts.Share[] tabAction = { Action, Action1,Action2};
                double[] weightTab = new double[] { 0.4, 0.3,0.3};

                PricingLibrary.FinancialProducts.BasketOption Basket = new PricingLibrary.FinancialProducts.BasketOption("basket", tabAction, weightTab, date, 8);

                DateTime dateStart = new DateTime(2014, 9, 10, 0, 0, 0);

                OptionManager optionCompute = new OptionManager(Basket, dateStart, date, 30, true);
                ComputeResults.computeResults(optionCompute);


                double firstValue = optionCompute.HedgingPortfolioValue[0];
                double lastValue = optionCompute.HedgingPortfolioValue[optionCompute.HedgingPortfolioValue.Count - 1];
                double payoff = optionCompute.Payoff[optionCompute.Payoff.Count - 1];

                double ratioTmp = Math.Abs(payoff - lastValue) / firstValue;
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

                PricingLibrary.FinancialProducts.BasketOption Basket = new PricingLibrary.FinancialProducts.BasketOption("basket", tabAction, weightTab, date, 8);


                DateTime dateStart = new DateTime(2015, 2, 10, 0, 0, 0);
                OptionManager optionCompute = new OptionManager(Basket, dateStart, date, 30, true);
                ComputeResults.computeResults(optionCompute);



                double firstValue = optionCompute.HedgingPortfolioValue[0];
                double lastValue = optionCompute.HedgingPortfolioValue[optionCompute.HedgingPortfolioValue.Count - 1];
                double payoff = optionCompute.Payoff[optionCompute.Payoff.Count - 1];

                double ratioTmp = Math.Abs(payoff - lastValue) / firstValue;

                ratio += ratioTmp;
                compteur += 1;
            }

            ratio = ratio / compteur;
            Console.WriteLine(ratio);
        }
    }
}
