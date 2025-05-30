using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace FLYEASY
{
    public partial class Form5 : Form
    {
        OracleConnection conn;
        public Form5()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure that a row is actually selected
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                // Retrieve data from the selected row
                string airlineName = selectedRow.Cells["airline_name"].Value.ToString();
                string flightNo = selectedRow.Cells["flight_no"].Value.ToString();
                string deptTime = selectedRow.Cells["dept_time"].Value.ToString();
                string duration = selectedRow.Cells["duration"].Value.ToString();
                string arrTime = selectedRow.Cells["arr_time"].Value.ToString();
                string price = selectedRow.Cells["price"].Value.ToString();

                // Do something with the extracted data, e.g., display it in MessageBox
                MessageBox.Show($"Selected flight details:\nAirline: {airlineName}\nFlight No: {flightNo}\nDeparture Time: {deptTime}\nDuration: {duration}\nArrival Time: {arrTime}\nPrice: {price}");
            }
        }

        public void ConnectDB()
        {
            conn = new OracleConnection("Data Source=AbhimanyuSingh;User ID=system;Password=abhi0103");
            conn.Open();
        }
       
        private void button2_Click(object sender, EventArgs e)
        {

            SessionInfo session = SessionInfo.GetInstance();
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                // Retrieve data from the selected row
                string airlineName = selectedRow.Cells["airline_name"].Value.ToString();
                string flightNo = selectedRow.Cells["flight_no"].Value.ToString();

                try
                {
                    ConnectDB();
                    OracleCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT no_of_seats FROM aircraft WHERE aircraft_id = (SELECT aircraft_id FROM flight WHERE flight_no = :flight_no)";
                    cmd.Parameters.Add("flight_no", OracleDbType.Varchar2).Value = flightNo;

                    int availableSeats = Convert.ToInt32(cmd.ExecuteScalar());
                    if (availableSeats >= session.no_of_passengers)
                    {
                        // Sufficient seats available, proceed to payment
                        Form7 frm = new Form7(session.Username, session.Dept_Airport, session.Arr_Airport, airlineName, flightNo, session.no_of_passengers, session.price);
                        frm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Not enough seats available for this flight.");
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
            else
            {
                MessageBox.Show("Please select a flight before proceeding to payment.");
            }

        }
        private void Form5_Load(object sender, EventArgs e)
        {
            SessionInfo session = SessionInfo.GetInstance();

            string city1 = session.Dept_Airport;
            string city2 = session.Arr_Airport;
            ConnectDB();
            using (OracleCommand cmd = new OracleCommand("select a.airline_name as airline_name, f.flight_no as flight_no, TO_CHAR(departure_time, 'HH24:MI:SS') AS dept_time,r.duration_min as duration, TO_CHAR(arrival_time, 'HH24:MI:SS') AS arr_time, f.price from flight f join airlines a on f.airline_code=a.airline_code join route r on f.route_id=r.route_id where f.route_id in (select route_id from route where dep_airport_code = (select airport_code from airport where city = :city1) and arr_airport_code = (select airport_code from airport where city = :city2))", conn))
            {
                cmd.Parameters.Add("city1", OracleDbType.Varchar2).Value = city1;
                cmd.Parameters.Add("city2", OracleDbType.Varchar2).Value = city2;

                using (OracleDataAdapter oda = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    oda.Fill(dt);
                    dataGridView1.DataSource = dt;

                }
            }
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
