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

    public partial class Form3 : Form
    {
        public Form3(int conteo,List<Puntajes> jugadoresTop)
        {
            InitializeComponent();
            button1.Click += delegate (object sender, EventArgs e) { button1_Click(sender, e, jugadoresTop); };
            button2.Click += delegate (object sender, EventArgs e) { button2_Click(sender, e, jugadoresTop); };
            lblValor.Text = conteo.ToString();
            LoadListView(jugadoresTop);
            listView1.Sorting = SortOrder.Ascending;

        }

        private void button1_Click(object sender, EventArgs e, List<Puntajes> jugadoresTop)
        {
            this.Hide();
            Form1 f1 = new Form1(jugadoresTop);
            f1.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e, List<Puntajes> jugadoresTop)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
                return;
            ListViewItem item = new ListViewItem(lblValor.Text);
            item.SubItems.Add(textBox1.Text); 
            jugadoresTop.Add(new Puntajes() { Tiempo = lblValor.Text, Jugador = textBox1.Text });
            listView1.Items.Add(item);
            listView1.Sorting = SortOrder.Ascending;

        }

        private void LoadListView(List<Puntajes> jugadoresTop)
        {
            foreach (var i in jugadoresTop)
            {
                string[] rowi = { i.Tiempo, i.Jugador };
                var listViewItemi = new ListViewItem(rowi);
                listView1.Items.Add(listViewItemi);
            }
        }


    }
}
