using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace metode
{
    
        public static class DB
        {
            static string connection = "";

            public static void pristup_bazi(string query)
            {
                try
                {
                    TextReader tr = new StreamReader("c:\\Program Files\\SM\\connUpis.txt");
                    connection = tr.ReadLine();
                    tr.Close();

                    SqlConnection myconnection = new SqlConnection(connection);

                    myconnection.Open();

                    SqlCommand mycommand = new SqlCommand();
                    mycommand.CommandText = query;
                    mycommand.Connection = myconnection;
                    mycommand.ExecuteNonQuery();
                    myconnection.Close();
                }
                catch (SqlException e)
                {
                    switch (e.Number)
                    {
                        case 2627:
                            MessageBox.Show("Vec je uneto za ozabranog radnika i za izabrani datum");
                            break;
                        default:
                            throw;
                    }
                }

            }

            public static DataTable baza_upit(string query)
            {
                TextReader tr = new StreamReader("c:\\Program Files\\SM\\conn.txt");
                connection = tr.ReadLine();
                tr.Close();

                SqlDataAdapter myAdapterPretraga = new SqlDataAdapter(query, connection);
                DataTable pretraga = new DataTable();
                myAdapterPretraga.Fill(pretraga);

                return pretraga;

            }

            public static DataTable baza_upit(string query, SqlConnection conn)
            {
                SqlDataAdapter st = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                st.Fill(dt);
                return dt;
            }

            public static void pristup_bazi(string query, SqlConnection conn)
            {
                SqlCommand myComm4 = new SqlCommand();
                myComm4.CommandText = query;
                myComm4.Connection = conn;
                myComm4.ExecuteNonQuery();
            }
        }
    
}
