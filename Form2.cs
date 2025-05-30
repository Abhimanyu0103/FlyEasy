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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace FLYEASY
{
    public partial class Form2 : Form
    {

        OracleConnection conn; 
        public Form2()
        {
            InitializeComponent();
        }

        public void ConnectDB()
        {
            conn = new OracleConnection("Data Source=AbhimanyuSingh;User ID=system;Password=abhi0103");
            conn.Open();
        }



        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox4.Text;
            string confirm = textBox5.Text;
            string age_text = textBox3.Text;
            try
            {
                ConnectDB();
                OracleCommand checkCommand = conn.CreateCommand();
                checkCommand.CommandText = "SELECT COUNT(*) FROM example WHERE username = :username";
                checkCommand.Parameters.Add("username", OracleDbType.Varchar2).Value = username;
                int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                checkCommand.Dispose();

                if (count > 0)
                {
                    MessageBox.Show("Username already exists. Please choose a different username.");
                    return;
                }

                OracleCommand command2 = conn.CreateCommand();
                if(password != confirm)
                {
                    MessageBox.Show("Please make sure the passwords match!");
                    return;
                }
                if(!int.TryParse(age_text, out int age))
                {
                    MessageBox.Show("Please enter valid age");
                    return;
                }
                command2.CommandText = "INSERT INTO example(username,emailid,gender,age,userpassword) VALUES (:name, :emailid, :gender, :age, :password)";
                command2.Parameters.Add("name", OracleDbType.Varchar2).Value = username;
                command2.Parameters.Add("emailid", OracleDbType.Varchar2).Value = textBox2.Text;
                command2.Parameters.Add("gender", OracleDbType.Varchar2).Value = comboBox1.Text;
                command2.Parameters.Add("age", OracleDbType.Int32).Value = age;
                command2.Parameters.Add("password", OracleDbType.Varchar2).Value = password;

                int rowsAffected = command2.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Login created successfully!");

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Error updating data.");
                }
                command2.Dispose();
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

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
