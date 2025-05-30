using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace FLYEASY
{
    public partial class Form7 : Form
    {
        OracleConnection conn;
        SessionInfo session = SessionInfo.GetInstance();
        public Form7(string Username, string Dept_Airport, string Arr_Airport, string airline, string flight_no, int no_of_passengers, int price)
        {
            InitializeComponent();

            label4.Text = Username;
            label7.Text = Dept_Airport;
            label9.Text = Arr_Airport;
            label11.Text = airline;
            label13.Text = flight_no;
            label15.Text = no_of_passengers.ToString();
            label18.Text = Convert.ToString(no_of_passengers * price);

            
        }
        public void ConnectDB()
        {
            conn = new OracleConnection("Data Source=AbhimanyuSingh;User ID=system;Password=abhi0103");
            conn.Open();
        }
        public string GenerateRandomPNR()
        {
            // Define the characters from which the PNR will be composed
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            // Create a random number generator
            Random random = new Random();

            // Generate a random PNR of length 6
            string pnr = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return pnr;
        }

        public string GenerateRandomBaggageID()
        {
            // Define the characters from which the PNR will be composed
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            // Create a random number generator
            Random random = new Random();

            // Generate a random PNR of length 6
            string bagg = new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return bagg;
        }

        public string GenerateRandomPaymentID()
        {
            // Define the characters from which the PNR will be composed
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

            // Create a random number generator
            Random random = new Random();

            // Generate a random PNR of length 6
            string pay = new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return pay;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string baggage_weight = textBox1.Text;
            string acc_no = textBox2.Text;
            string pin_no = textBox3.Text;
            string randompnr = GenerateRandomPNR();
            string randombag_id = GenerateRandomBaggageID();
            string payment_id = GenerateRandomPaymentID();
            try
            {
                ConnectDB();
                if (string.IsNullOrEmpty(baggage_weight) || string.IsNullOrEmpty(acc_no) || string.IsNullOrEmpty(pin_no))
                {
                    MessageBox.Show("Please enter all fields before proceeding!");
                    return;
                }
                else if (!int.TryParse(acc_no, out int account_no) || !int.TryParse(pin_no, out int pin))
                {
                    MessageBox.Show("Enter valid characters for account number and pin!");
                    return;
                }

                if (!int.TryParse(baggage_weight, out int weight))
                {
                    MessageBox.Show("Enter valid number for weight!");
                    return;
                }

                int payment = session.price * session.no_of_passengers;
                int total_weight_allowed = 15 * session.no_of_passengers;
                if (weight > total_weight_allowed)
                {
                    int overweight = weight - total_weight_allowed;
                    int excessBaggageFee = 1000 * overweight;
                   

                    DialogResult result = MessageBox.Show($"Baggage overweight! \nIncreasing price at the rate of 1000/kg\n Total additional payment: {excessBaggageFee}.\n\nDo you wish to proceed with the booking?", "Baggage Overweight", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        // User chose not to proceed with booking
                        return;
                    }

                    // Update label with new payment
                    if(result == DialogResult.Yes)
                    {
                        payment += excessBaggageFee; // Add excess baggage fee to the price
                        label18.Text = payment.ToString();
                        session.total = payment;

                        using (OracleCommand cmd = new OracleCommand("INSERT INTO bookings VALUES (:pnr, :username, :flight_no, :no_of_passengers, TO_DATE(:bookingdate, 'dd-mm-yyyy HH24:MI:SS'))", conn))
                        {
                            cmd.Parameters.Add("pnr", OracleDbType.Varchar2).Value = randompnr;
                            cmd.Parameters.Add("username", OracleDbType.Varchar2).Value = session.Username;
                            cmd.Parameters.Add("flight_no", OracleDbType.Varchar2).Value = session.flight_no;
                            cmd.Parameters.Add("no_of_passengers", OracleDbType.Int32).Value = session.no_of_passengers;
                            cmd.Parameters.Add("bookingdate", OracleDbType.Date).Value = session.bookingdate;
                            
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Flight booked successfully!");
                            this.Hide();
                        }
                        using (OracleCommand cmd = new OracleCommand("insert into baggage values (:id, :pnr, :flight_no, :weight)", conn))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("id", OracleDbType.Varchar2).Value = randombag_id;
                            cmd.Parameters.Add("pnr", OracleDbType.Varchar2).Value = randompnr;
                            cmd.Parameters.Add("flight_no", OracleDbType.Varchar2).Value = session.flight_no;
                            cmd.Parameters.Add("weight", OracleDbType.Int32).Value = Convert.ToInt32(textBox1.Text);
                            cmd.ExecuteNonQuery();
                        }
                        using (OracleCommand cmd = new OracleCommand("insert into payment values (:id, :pnr, :amount, :flight_no)", conn))
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("id", OracleDbType.Varchar2).Value = payment_id;
                            cmd.Parameters.Add("pnr", OracleDbType.Varchar2).Value = randompnr;
                            cmd.Parameters.Add("amount", OracleDbType.Int32).Value = session.total;
                            cmd.Parameters.Add("flight_no", OracleDbType.Varchar2).Value = session.flight_no;
                            
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    session.total = payment;
                    using (OracleCommand cmd = new OracleCommand("INSERT INTO bookings VALUES (:pnr, :username, :flight_no, :no_of_passengers, TO_DATE(:bookingdate, 'dd-mm-yyyy HH24:MI:SS'))", conn))
                    {
                        cmd.Parameters.Add("pnr", OracleDbType.Varchar2).Value = randompnr;
                        cmd.Parameters.Add("username", OracleDbType.Varchar2).Value = session.Username;
                        cmd.Parameters.Add("flight_no", OracleDbType.Varchar2).Value = session.flight_no;
                        cmd.Parameters.Add("no_of_passengers", OracleDbType.Int32).Value = session.no_of_passengers;
                        cmd.Parameters.Add("bookingdate", OracleDbType.Date).Value = session.bookingdate;
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Flight booked successfully!");
                        this.Hide();
                    }
                    using (OracleCommand cmd = new OracleCommand("insert into baggage values (:id, :pnr, :flight_no, :weight)", conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("id", OracleDbType.Varchar2).Value = randombag_id;
                        cmd.Parameters.Add("pnr", OracleDbType.Varchar2).Value = randompnr;
                        cmd.Parameters.Add("flight_no", OracleDbType.Varchar2).Value = session.flight_no;
                        cmd.Parameters.Add("weight", OracleDbType.Int32).Value = Convert.ToInt32(textBox1.Text);
                        cmd.ExecuteNonQuery();
                    }
                    using (OracleCommand cmd = new OracleCommand("insert into payment values (:id, :pnr, :amount, :flight_no)", conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("id", OracleDbType.Varchar2).Value = payment_id;
                        cmd.Parameters.Add("pnr", OracleDbType.Varchar2).Value = randompnr;
                        cmd.Parameters.Add("amount", OracleDbType.Int32).Value = session.total;
                        cmd.Parameters.Add("flight_no", OracleDbType.Varchar2).Value = session.flight_no;
                        
                        cmd.ExecuteNonQuery();
                    }

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

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }
    }
}
