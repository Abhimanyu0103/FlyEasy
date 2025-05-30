using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace FLYEASY
{
    public partial class Form4 : Form
    {
        OracleConnection conn;
        public Form4()
        {
            InitializeComponent();
        }
        public void ConnectDB()
        {
            conn = new OracleConnection("Data Source=AbhimanyuSingh;User ID=system;Password=abhi0103");
            conn.Open();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            SessionInfo session = SessionInfo.GetInstance();
            ConnectDB();

            using (OracleCommand cmd = new OracleCommand("select * from payment where pnr in (select pnr from bookings where username = :name)", conn))
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
