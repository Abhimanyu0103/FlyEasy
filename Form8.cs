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
    public partial class Form8 : Form
    {
        OracleConnection conn;
        public Form8()
        {
            InitializeComponent();
        }
        public void ConnectDB()
        {
            conn = new OracleConnection("Data Source=AbhimanyuSingh;User ID=system;Password=abhi0103");
            conn.Open();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure that a row is actually selected
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                // Retrieve data from the selected row
                string pnr = selectedRow.Cells["pnr"].Value.ToString();
                string flightNo = selectedRow.Cells["flight_no"].Value.ToString();

                DialogResult result = MessageBox.Show("Are you sure you want to cancel the booking?", "Cancel Booking", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if(result == DialogResult.No)
                {
                    return;
                }
                if(result == DialogResult.Yes)
                {   
                    try
                    {
                        ConnectDB();

                        using(OracleCommand cmd = new OracleCommand("delete from baggage where pnr = :pnr", conn))
                        {
                            cmd.Parameters.Add("pnr", OracleDbType.Varchar2).Value = pnr;
                            cmd.ExecuteNonQuery();
                        }
                        using (OracleCommand cmd = new OracleCommand("delete from payment where pnr = :pnr", conn))
                        {
                            cmd.Parameters.Add("pnr", OracleDbType.Varchar2).Value = pnr;
                            cmd.ExecuteNonQuery();
                        }
                        using (OracleCommand cmd = new OracleCommand("delete from bookings where pnr = :pnr", conn))
                        {
                            cmd.Parameters.Add("pnr", OracleDbType.Varchar2).Value = pnr;
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Booking Cancelled!\n50% refund will be initiated soon.");
                        }
                        this.Hide();
                        Form8 frm = new Form8();
                        frm.Show();

                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

        private void Form8_Load(object sender, EventArgs e)
        {
            SessionInfo session = SessionInfo.GetInstance();
            label2.Text = "Welcome " + session.Username + " !";
            ConnectDB();

            using (OracleCommand cmd = new OracleCommand("select b.pnr, b.flight_no, a.airline_name, da.city as departure_airport, aa.city as arrival_airport, b.booking_date, TO_CHAR(f.departure_time, 'HH24:MI:SS') as departure_time, TO_CHAR(f.arrival_time, 'HH24:MI:SS') as arrival_time, bg.baggage_id as baggage_id, bg.weight_kg as weight from bookings b join flight f on b.flight_no = f.flight_no join airlines a on f.airline_code = a.airline_code join route r on f.route_id = r.route_id join airport da on r.dep_airport_code = da.airport_code join airport aa on r.arr_airport_code = aa.airport_code left join baggage bg on b.pnr = bg.pnr where b.username = :name", conn))
            {
                cmd.Parameters.Add("name", OracleDbType.Varchar2).Value = session.Username;

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

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
