using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHedging
{
    class ShareHisto
    {
        private System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> _Data; // just one List that contains loaded or simulated data
        // beginning of test date
        private System.DateTime startDate;
        // end of test date = option maturity date
        private System.DateTime maturityDate;
        // option
        private PricingLibrary.FinancialProducts.IOption _product;

        // On initialise la classe 
        public ShareHisto(System.DateTime startDate, System.DateTime maturityDate, PricingLibrary.FinancialProducts.IOption product)
        {
            this._Data = new List<PricingLibrary.Utilities.MarketDataFeed.DataFeed>();
            this.startDate = startDate;
            this.maturityDate = maturityDate;
            this._product = product;
        }

        public System.Collections.Generic.List<PricingLibrary.Utilities.MarketDataFeed.DataFeed> Data
        {
            get {
                return this._Data;
            }
        }


        /*** loadingSimulated ***/
        /* Function that initialise data with simulated data
         */
        public void loadingSimulated()
        {
            PricingLibrary.Utilities.MarketDataFeed.SimulatedDataFeedProvider import = new PricingLibrary.Utilities.MarketDataFeed.SimulatedDataFeedProvider();
            this._Data = import.GetDataFeed(this._product, this.startDate);
        }


        /*** loadingSQL ***/
        /* Function that initialise data with simulated data
         */
        public void loadingSQL() 
        {
            string connectionString = "Data Source=(local);Initial Catalog=Northwind;" + "Integrated Security=true";
            string queryString = "SELECT data "
                + "WHERE Action = @idAction ";
            int idAction = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.AddWithValue("@pricePoint", idAction);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}\t{1}\t{2}",
                            reader[0], reader[1], reader[2]);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();

            }
        }
    }
}