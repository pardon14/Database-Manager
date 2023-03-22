using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wypozyczalnia_kamperow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connectionString = "Data Source.;Initial Catalog=WypozyczalniaKamperow;Integrated Security=True;";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
