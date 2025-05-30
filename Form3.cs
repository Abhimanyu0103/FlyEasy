using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FLYEASY
{
    public partial class Form3 : Form
    {
        OracleConnection conn;
        SessionInfo session = SessionInfo.GetInstance();
        public Form3(string username)
        {
            InitializeComponent();

            label4.Text = "Hello " + username + "!";
        }

        public void ConnectDB()
        {
            conn = new OracleConnection("Data Source=AbhimanyuSingh;User ID=system;Password=abhi0103");
            conn.Open();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string from = comboBox1.Text;
            string to = comboBox2.Text;

            if (string.IsNullOrEmpty(from))
            {
                MessageBox.Show("Please select a departure airport.");
                return;
            }

            // Validate if arrival airport is selected
            if (string.IsNullOrEmpty(to))
            {
                MessageBox.Show("Please select an arrival airport.");
                return;
            }

            // Validate if number of passengers is provided
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please enter the number of passengers.");
                return;
            }

            // Parse the number of passengers
            if (!int.TryParse(textBox1.Text, out int no_of_passengers))
            {
                MessageBox.Show("Please enter a valid number for the number of passengers.");
                return;
            }

            DateTime selectedDateTime = dateTimePicker1.Value;
            DateTime currentDateTime = DateTime.Now;
            DateTime selectedDate = selectedDateTime.Date;
            string dept_date = selectedDate.ToString("dd-MM-yyyy");
            no_of_passengers = Convert.ToInt32(textBox1.Text);

            SessionInfo session = SessionInfo.GetInstance();
            session.Dept_Airport = from;
            session.Arr_Airport = to;
            session.no_of_passengers = no_of_passengers;
            session.bookingdate = selectedDate;

            try
            {
                ConnectDB();
                DateTime currentDate = DateTime.Today;
                if (selectedDate < currentDate)
                {
                    MessageBox.Show("Please select a date in the future.");
                    return;
                }

                OracleCommand command = conn.CreateCommand();
                command.CommandText = "select a.airline_name as airline_name, f.flight_no as flight_no, TO_CHAR(departure_time, 'HH24:MI:SS') AS dept_time,r.duration_min as duration, TO_CHAR(arrival_time, 'HH24:MI:SS') AS arr_time, f.price from flight f join airlines a on f.airline_code=a.airline_code join route r on f.route_id=r.route_id where f.route_id in (select route_id from route where dep_airport_code = (select airport_code from airport where city = :city1) and arr_airport_code = (select airport_code from airport where city = :city2))";
                command.Parameters.Add("city1", OracleDbType.Varchar2).Value = from;
                command.Parameters.Add("city2", OracleDbType.Varchar2).Value = to;
                if (selectedDate == currentDate && selectedDateTime.TimeOfDay >= currentDateTime.TimeOfDay)
                {
                    string currentTimeFormatted = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss");
                    command.CommandText += " AND departure_time > TO_TIMESTAMP(:currentTime, 'DD-MM-YYYY HH24:MI:SS')";
                    command.Parameters.Add("currentTime", OracleDbType.Varchar2).Value = currentTimeFormatted;
                }

                OracleDataReader reader = command.ExecuteReader();
                if (from == to)
                {
                    MessageBox.Show("Please pick different cities for arrival and departure!");
                    return;

                }
                else if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        string airline_name = reader.GetString(0);
                        string flight_no = reader.GetString(1);
                        string dept_time = reader.GetString(2);
                        string dept_airport = from;
                        string duration = reader.GetString(3);
                        string arr_time = reader.GetString(4);
                        string arr_airport = to;
                        int price = Convert.ToInt32(reader.GetString(5));
                        session.price = price;
                        session.airline = airline_name;
                        session.flight_no = flight_no;

                        Form5 form = new Form5();
                        form.Show();
                    }
                }
                else
                {
                    MessageBox.Show("No flights found for the provided cities.");
                }

                // Close the data reader
                reader.Close();
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

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form8 frm = new Form8();
            frm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectDB();
                
                using (OracleCommand command = conn.CreateCommand())
                {

                    command.CommandText = "GetUserInformation";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_username", OracleDbType.Varchar2).Value = session.Username;

                    command.Parameters.Add("output", OracleDbType.Varchar2, 4000).Direction = ParameterDirection.ReturnValue;
                    command.BindByName = true;
                    command.ExecuteNonQuery();

                    string output = Convert.ToString(command.Parameters["output"].Value);

                    MessageBox.Show($"{output}", "User Information");
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

        private void button4_Click(object sender, EventArgs e)
        {
            Form4 frm = new Form4();
            frm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Log Out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return;
            }
            if (result == DialogResult.Yes)
            {
                this.Hide();
                Form1 frm = new Form1();
                frm.Show();
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
