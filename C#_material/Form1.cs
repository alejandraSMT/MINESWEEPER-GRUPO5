using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TRABAJO_FINAL_LP
{

    public partial class Form1 : Form
    {
        public Form1(List<Puntajes> jugadoresTop)
        {
            InitializeComponent();
            button1.Click += delegate (object sender, EventArgs e) { button1_Click(sender, e, jugadoresTop); };

        }


        private void button1_Click(object sender, EventArgs e, List<Puntajes> jugadoresTop)
        {
            this.Hide();
            Form2 f2 = new Form2(jugadoresTop);
            f2.ShowDialog();
            this.Close();
        }

    }
}
