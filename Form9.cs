using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace FLYEASY
{
    public partial class Form9 : Form
    {
        OracleConnection conn;
        public Form9()
        {
            InitializeComponent();
        }

        public void ConnectDB()
        {
            conn = new OracleConnection("Data Source=AbhimanyuSingh;User ID=system;Password=abhi0103");
            conn.Open();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;
            string confirmPassword = textBox3.Text;
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username!");
                return;
            }
            else if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            try
            {
                ConnectDB();
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM example WHERE username = :username";
                cmd.Parameters.Add("username", OracleDbType.Varchar2).Value = username;

                int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (userCount > 0)
                {
                    // Username exists, check password
                    if (password == confirmPassword)
                    {
                        cmd.CommandText = "UPDATE example SET userpassword = :password WHERE username = :username";
                        cmd.Parameters.Clear(); 
                        cmd.Parameters.Add("password", OracleDbType.Varchar2).Value = password;
                        cmd.Parameters.Add("username", OracleDbType.Varchar2).Value = username;
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Password updated successfully.");
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Password and confirm password do not match.");
                    }
                }
                else
                {
                    MessageBox.Show("Username not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form9_Load(object sender, EventArgs e)
        {

        }
    }
}
