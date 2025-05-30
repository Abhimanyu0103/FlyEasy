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

    public partial class Form1 : Form
    {
        OracleConnection conn;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        public void ConnectDB()
        {
            conn = new OracleConnection("Data Source=AbhimanyuSingh;User ID=system;Password=abhi0103");
            conn.Open();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            try
            {
                ConnectDB();

                // Create a command to select the user with the provided username and password
                OracleCommand command = conn.CreateCommand();
                command.CommandText = "SELECT count(*) FROM example WHERE username = :username AND userpassword = :password";
                command.Parameters.Add("username", OracleDbType.Varchar2).Value = username;
                command.Parameters.Add("password", OracleDbType.Varchar2).Value = password;

                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    MessageBox.Show("Login successful!");
                    SessionInfo session = SessionInfo.GetInstance();
                    session.Username = username;

                    Form3 frm = new Form3(username);
                    frm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password. Please try again.");
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form9 frm = new Form9();    
            frm.Show();
        }
    }
}
