using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Backend
{
    public class Connector
    {
        private static string _conStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        /// <summary>
        /// Gets the a table of entities with the specified id from the specified table.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <param name="table">The table to get the data from.</param>
        /// <param name="idColumn">The column to match the id on.</param>
        /// <returns></returns>
        /*public static DataTable GetById(int id, string table, string idColumn)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                DataTable people = new DataTable();
                try
                {
                    string query = String.Format("SELECT * FROM {0} WHERE {1} = {2};", table, idColumn, id);
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    adapter.Fill(people);

                    return people;
                }
                catch (Exception ex)
                {
                    // Handle the error
                }
            }

            return null;
        }
        */

        /// <summary>
        /// Gets the a table of entities with the specified ids from the specified table.
        /// </summary>
        /// <param name="id">The ids to match.</param>
        /// <param name="table">The table to get the data from.</param>
        /// <param name="idColumn">The column to match the id on.</param>
        /// <returns></returns>
        /*public static DataTable GetByIds(int[] ids, string table, string idColumn)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            {
                DataTable entities = new DataTable();
                bool first = true;
                foreach (int id in ids)
                {
                    DataTable entity = new DataTable();
                    try
                    {
                        string query = String.Format("SELECT * FROM {0} WHERE {1} = {2};", table, idColumn, id);
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.CommandType = CommandType.Text;
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = cmd;
                        adapter.Fill(entity);

                        if (!first)
                        {
                            foreach (DataRow row in entity.Rows)
                            {
                                entities.NewRow();
                                entities.Rows.Add(row);
                            }
                        }
                        else
                        {
                            entities = entity.Copy();
                            first = false;
                        }

                    }
                    catch (Exception ex)
                    {
                        // Handle the error
                    }
                }

                return entities;
            }

            return null;
        }
        */

        /// <summary>
        /// Gets the max id of the given table.
        /// </summary>
        /// <param name="table">The table to get the max id from.</param>
        /// <param name="column">The id column of the table.</param>
        /// <returns>The max id of the table.</returns>
        public static int GetMaxId(string table, string column)
        {
            int id = -1;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_conStr))
                {
                    conn.Open();

                    string query = String.Format("SELECT MAX({0}) FROM {1} LIMIT 1;", column, table);
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.CommandType = CommandType.Text;
                    id = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                // Handle the error
            }

            return id;
        }

        /// <summary>
        /// Creates an insert SQL command.
        /// </summary>
        /// <param name="table">The table to insert into.</param>
        /// <param name="param">A dictionary of the values to insert. The keys should be the column names.</param>
        /// <returns>The insert SQL command.</returns>
        public static MySqlCommand CreateInsertCmd(string table, Dictionary<string, object> param)
        {
            string queryCols = "";
            string queryParams = "";

            bool first = true;
            foreach (KeyValuePair<string, object> entry in param)
            {
                if (!first)
                {
                    queryCols += ", " + entry.Key;
                    queryParams += ", @" + entry.Key;
                }
                else
                {
                    first = false;
                    queryCols += entry.Key;
                    queryParams += "@" + entry.Key;
                }

            }


            MySqlCommand cmd = new MySqlCommand(String.Format("INSERT INTO {0} ({1}) VALUES ({2})", table, queryCols, queryParams));
            cmd.CommandType = CommandType.Text;

            foreach (KeyValuePair<string, object> entry in param)
            {
                cmd.Parameters.AddWithValue("@" + entry.Key, entry.Value);
            }

            return cmd;
        }

        /// <summary>
        /// Creates an update SQL command.
        /// </summary>
        /// <param name="table">The table to update on.</param>
        /// <param name="param">A dictionary of values to update. The keys should be the column names.</param>
        /// <param name="updateOn">The column name and value that should of the row(s) that should be updated.</param>
        /// <returns>The update SQL command.</returns>
        public static MySqlCommand CreateUpdateCmd(string table, Dictionary<string, object> param, Tuple<string, object> updateOn)
        {
            string queryCols = "";

            bool first = true;
            foreach (KeyValuePair<string, object> entry in param)
            {
                if (!first)
                {
                    queryCols += ", " + entry.Key + " = @" + entry.Key;
                }
                else
                {
                    first = false;
                    queryCols += entry.Key + " = @" + entry.Key;
                }

            }

            MySqlCommand cmd = new MySqlCommand(String.Format("UPDATE {0} SET {1} WHERE {2} = {3}", table, queryCols, updateOn.Item1, updateOn.Item2));
            cmd.CommandType = CommandType.Text;

            foreach (KeyValuePair<string, object> entry in param)
            {
                cmd.Parameters.AddWithValue("@" + entry.Key, entry.Value);
            }

            return cmd;
        }
    }
}