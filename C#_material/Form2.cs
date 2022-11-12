using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace TRABAJO_FINAL_LP
{
    
    public partial class Form2 : Form
    {
        private int conteo;
        private Celda[][] tablero;
        private Label lbMarcador;
        private bool inicializado = false;
        public int CeldasClickeadas = 0;
        private List<Celda> marcados = new List<Celda>();
        public class Celda : Button
        {
            public bool TieneBomba { get; set; }
            public int Columna { get; set; }
            public int Fila { get; set; }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            conteo++;
            lblValor.Text = conteo.ToString();

        }
        public Form2(List<Puntajes> jugadoresTop)
        {
            InitializeComponent();
            conteo = 0;
            button3.Click += delegate (object sender, EventArgs e) { button3_Click(sender, e, jugadoresTop); };
            const int tamañoCelda = 25;
            this.ClientSize = new Size(tamañoCelda * 8 + 200, tamañoCelda * 8);
            this.Text = "Buscaminas simple";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            tablero = (from nFila in Enumerable.Range(0, 8)
                       select (from nCol in Enumerable.Range(0, 8)
                               select new Celda()
                               {
                                   TieneBomba = false,
                                   Columna = nCol,
                                   Fila = nFila,
                                   Top = nFila * tamañoCelda,
                                   Left = nCol * tamañoCelda,
                                   Size = new Size(tamañoCelda, tamañoCelda),
                                   TextAlign = ContentAlignment.MiddleCenter,
                                   BackColor = Color.LightGray
                               }).ToArray()).ToArray();

            foreach (var fila in tablero)
            {
                foreach (var Celda in fila)
                {
                    Celda.Click += CeldaClickeada;
                    Celda.MouseUp += CeldaMarcado;
                    this.Controls.Add(Celda);
                }
            }
            lbMarcador = new Label
            {
                Text = "Bombas restantes: " + (10 - marcados.Count),
                Font = new Font(FontFamily.GenericMonospace, 10),
                Top = tamañoCelda * 2,
                Left = tamañoCelda * 8 + 50,
                Size = new Size(150, 200)
            };
            this.Controls.Add(lbMarcador);
        }

        private void CeldaClickeada(object sender, EventArgs evt)
        {
            Celda c = sender as Celda;
            if (!marcados.Any(Celda => object.ReferenceEquals(Celda, c)))
            {
                SeleccionarCelda(c);
            }
        }

        private void CeldaMarcado(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Celda c = sender as Celda;
                if (marcados.Any(Celda => object.ReferenceEquals(Celda, c)))
                {
                    marcados.Remove(c);
                    c.Text = string.Empty;
                    c.BackColor = Color.LightGray;
                    lbMarcador.Text = "Bombas restantes: " + (10 - marcados.Count());
                }
                else
                {
                    if (marcados.Count < 10)
                    {
                        c.Text = "F";
                        c.BackColor = Color.Orange;
                        marcados.Add(c);

                        lbMarcador.Text = "Bombas restantes: " + (10 - marcados.Count());
                        if (marcados.Count == 10 && marcados.All(Celda => Celda.TieneBomba))
                        {
                            timerF2.Enabled = false;
                            lbMarcador.Text = "¡Felicidades! ¡Has ganado!";
                            DeshabilitarTablero();
                        }
                    }
                }
            }
        }

        public void SeleccionarCelda(Celda c)
        {
            c.Click -= CeldaClickeada;
            c.Enabled = false;
            if (c.TieneBomba == false)
            {
                CeldasClickeadas++;
            }
            c.BackColor = Color.White;
            lbMarcador.Focus();

            if (!inicializado)
            {
                var random = new Random();
                int bombasGeneradas = 0;
                while (bombasGeneradas < 10)
                {
                    foreach (var fila in tablero)
                    {
                        foreach (var Celda in fila)
                        {
                            if (random.Next(1, 10) == 5)
                            {
                                if (!object.ReferenceEquals(c, Celda) && bombasGeneradas < 10)
                                {
                                    bombasGeneradas++;
                                    Celda.TieneBomba = true;
                                }
                            }
                        }
                    }
                }
                inicializado = true;
            }

            c.FlatStyle = FlatStyle.Flat;
            if (c.TieneBomba)
            {
                c.BackColor = Color.Red;
                c.Text = "B";
                timerF2.Enabled = false;
                lbMarcador.Text = "¡Caiste en una bomba! ¡Has perdido!";

                DeshabilitarTablero();
            }
            else
            {
                int bombasAlrededor = ContarBombasAlrededor(c);
                if (ContarBombasAlrededor(c) != 0)
                {
                    c.Text = bombasAlrededor.ToString();
                }
                else
                {
                    SeleccionarVacíosAlrededor(c);
                }
            }

            if (CeldasClickeadas == 54)
            {
                timerF2.Enabled = false;
                lbMarcador.Text = "¡Felicidades! ¡Has ganado!";
                DeshabilitarTablero();
            }
        }

        private void DeshabilitarTablero()
        {
            foreach (var fila in tablero)
            {
                foreach (var Celda in fila)
                {
                    Celda.Enabled = false;
                }
            }
        }

        private int ContarBombasAlrededor(Celda c)
        {
            int bombasAlrededor = 0;

            RevisarBombaAlrededor(c, ref bombasAlrededor, 1, 1);
            RevisarBombaAlrededor(c, ref bombasAlrededor, 1, -1);
            RevisarBombaAlrededor(c, ref bombasAlrededor, 1, 0);
            RevisarBombaAlrededor(c, ref bombasAlrededor, -1, 1);
            RevisarBombaAlrededor(c, ref bombasAlrededor, -1, -1);
            RevisarBombaAlrededor(c, ref bombasAlrededor, -1, 0);
            RevisarBombaAlrededor(c, ref bombasAlrededor, 0, -1);
            RevisarBombaAlrededor(c, ref bombasAlrededor, 0, 1);

            return bombasAlrededor;
        }

        private void SeleccionarVacíosAlrededor(Celda c)
        {
            SeleccionarVacíosAlrededor(c, 1, 1);
            SeleccionarVacíosAlrededor(c, 1, -1);
            SeleccionarVacíosAlrededor(c, 1, 0);
            SeleccionarVacíosAlrededor(c, -1, 1);
            SeleccionarVacíosAlrededor(c, -1, -1);
            SeleccionarVacíosAlrededor(c, -1, 0);
            SeleccionarVacíosAlrededor(c, 0, -1);
            SeleccionarVacíosAlrededor(c, 0, 1);
        }

        private void SeleccionarVacíosAlrededor(Celda c, short incCol, short incFila)
        {
            int fila = c.Fila + incFila;
            int columna = c.Columna + incCol;
            if (fila >= 0 && fila < 8 && columna >= 0 && columna < 8)
            {
                var Celda = tablero[fila][columna];
                if (!Celda.TieneBomba && ContarBombasAlrededor(Celda) == 0 && Celda.Enabled)
                {
                    SeleccionarCelda(Celda);
                }
            }
        }

        private void RevisarBombaAlrededor(Celda c, ref int contador, short incCol, short incFila)
        {
            int fila = c.Fila + incFila;
            int columna = c.Columna + incCol;
            if (fila >= 0 && fila < 8 && columna >= 0 && columna < 8
                && tablero[fila][columna].TieneBomba)
            {
                contador++;
            }
        }


        private void button3_Click(object sender, EventArgs e, List<Puntajes> jugadoresTop)
        {
            if (CeldasClickeadas == 54 || (marcados.Count == 10 && marcados.All(Celda => Celda.TieneBomba)))
            {
                this.Hide();
                Form3 f3 = new Form3(conteo,jugadoresTop) ;
                f3.ShowDialog();
                this.Close();
            }
            else
            {
                this.Hide();
                Form1 f1 = new Form1(jugadoresTop);
                f1.ShowDialog();
                this.Close();
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            timerF2.Enabled = true;
        }

        private void timerF2_Tick(object sender, EventArgs e)
        {
            conteo++;
            label6.Text = conteo.ToString();
        }
    }

  



}