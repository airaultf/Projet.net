using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ErrorHedging
{
    static class ComputeEstimators
    {
        // Import the WRE dll for volatility computation
        [DllImport(@"C:\Users\alexandre\Source\Repos\Projet.net2\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREanalysisExpostVolatility", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WREanalysisExpostVolatility(
            ref int nbValues,
            double[] portfolioReturns,
            ref double expostVolatility,
            ref int info
            );

        // Import the WRE dll for correlation matrix computation
        [DllImport(@"C:\Users\alexandre\Source\Repos\Projet.net2\ErrorHedging\ErrorHedging\wre-ensimag-c-4.1.dll", EntryPoint = "WREmodelingCorr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WREmodelingCorr(
            ref int nbValues,
            ref int nbAssets,
            double[,] assetsReturns,
            double[,] corr,
            ref int info
            );

        public static double[] computeVolatilities(double[,] portfolioReturns, bool simulated)
        {
            double[] volatilities = new double[portfolioReturns.GetLength(1)];

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
            
            if (nbAssets == 1)
            {
                return new double[1, 1] { {1} };
            }

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

        public static double[,] logReturn(double[,] assetsValues)
        {
            int nbValues = assetsValues.GetLength(0);
            int nbAssets = assetsValues.GetLength(1);
            double[,] assetReturns = new double[nbValues, nbAssets];
            for (int i = 1; i < nbValues; i++)
            {
                for (int action = 0; action < nbAssets; action++)
                {
                    assetReturns[i - 1, action] = Math.Log((assetsValues[i, action] / assetsValues[i - 1, action]));
                }
            }
            return assetReturns;
        }
    }
}
