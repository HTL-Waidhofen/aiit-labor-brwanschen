using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Example
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Figur figur = null;

        public MainWindow()
        {
            InitializeComponent();

            StreamReader reader = new StreamReader("maze_10x10.txt");
            string inhalt = reader.ReadToEnd();
            string[] zeilen = inhalt.Split('\n');


            Spielfeld.Background = Brushes.Black;

            

            for (int i = 0; i < zeilen.Length; i++)
            {
                string zeile = zeilen[i];
                for (int a = 0; a < zeile.Length; a++)
                {
                    if (zeile[a] == '#')
                    {
                        Rectangle kansten = new Rectangle
                        {
                            Width = 20,
                            Height = 20,
                            Fill = Brushes.Green
                        };
                        Canvas.SetLeft(kansten, i * 20);
                        Canvas.SetTop(kansten, a * 20);
                        Spielfeld.Children.Add(kansten);
                    }
                    else if (zeile[a] == 'X')
                    {
                        figur = new Figur(a * 20, i * 20);
                        Spielfeld.Children.Add(figur.GetEllipse());
                    }
                }
            }



        }

        public void Window_KeyDown(object obj, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                figur.Bewegen(1, 0);
            }
            if (e.Key == Key.Left)
            {
                figur.Bewegen(-1, 0);
            }
            if (e.Key == Key.Up)
            {
                figur.Bewegen(0, -1);
            }
            if (e.Key == Key.Down)
            {
                figur.Bewegen(0, 
                    1);
            }
        }
    }
}
