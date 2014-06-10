using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace dsm
{
    class Data
    {
        DataTable dataTable;
        SqlConnection connection;

        public Data()
        {
            dataTable = new DataTable();
            connection = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\PlayGroundCase.mdf;Integrated Security=True");  
        }

        public SqlConnection getConnection()
        {
            return connection;
        }

        public List<DataRow> executeQuery(string sql)
        {
            using (SqlCommand showresult = new SqlCommand(sql, connection)) {
                using (SqlDataReader dr = showresult.ExecuteReader())
                {
                    dataTable.Load(dr);
                }
            }
            return getLatestRows(dataTable);
        }

        public DataTable getDatatable()
        {
            return dataTable;
        }

        /// <summary>
        /// Establish a database connection
        /// </summary>
        public bool databaseConnection()
        {
            // Start connection
            try
            {
                connection.Open();
                return true;
            }

            // If connection failed then display text and change color to red
            catch (Exception e)
            {
                Console.WriteLine("Database Connection failed | " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets a list with the lastest row for each tag
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private List<DataRow> getLatestRows(DataTable dataTable)
        {
            List<string> checkedRows = new List<string>();
            List<DataRow> latestRows = new List<DataRow>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (!checkedRows.Contains(dataTable.Rows[i].ItemArray[0].ToString()))
                {
                    List<DataRow> sameRows = new List<DataRow>();
                    for (int j = 0; j < dataTable.Rows.Count; j++)
                    {
                        if (dataTable.Rows[i].ItemArray[0].ToString().Equals(dataTable.Rows[j].ItemArray[0].ToString()))
                        {
                            sameRows.Add(dataTable.Rows[j]);
                        }
                    }
                    latestRows.Add(getLatestRow(sameRows));
                    checkedRows.Add(dataTable.Rows[i].ItemArray[0].ToString());
                }
            }
            return latestRows;
        }

        /// <summary>
        /// Gets the lastest row from a list with datarows. Searches at a colum with the name time
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private DataRow getLatestRow(List<DataRow> rows)
        {
            DataRow latestRow = null;
            foreach (DataRow row in rows)
            {
                if (latestRow == null || DateTime.Compare((DateTime)row["time"], (DateTime)latestRow["time"]) > 0)
                {
                    latestRow = row;
                }
            }
            return latestRow;
        }
    }
}
