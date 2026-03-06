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
        private bool[,] walls;
        private int rows;
        private int cols;
        private readonly int cellSize = 20;

        public MainWindow()
        {
            InitializeComponent();

            // Robustere Dateieinlesung (Zeilen)
            string[] zeilen = File.ReadAllLines("maze_10x10.txt");
            rows = zeilen.Length;
            cols = zeilen.Select(l => l.TrimEnd('\r')).Max(l => l.Length);

            walls = new bool[rows, cols];

            Spielfeld.Background = Brushes.Black;

            for (int i = 0; i < zeilen.Length; i++)
            {
                string zeile = zeilen[i].TrimEnd('\r');
                for (int a = 0; a < zeile.Length; a++)
                {
                    char ch = zeile[a];
                    if (ch == '#')
                    {
                        // Wände speichern und zeichnen (Spalte = a, Zeile = i)
                        walls[i, a] = true;
                        Rectangle kansten = new Rectangle
                        {
                            Width = cellSize,
                            Height = cellSize,
                            Fill = Brushes.Green
                        };
                        Canvas.SetLeft(kansten, a * cellSize);
                        Canvas.SetTop(kansten, i * cellSize);
                        Spielfeld.Children.Add(kansten);
                    }
                    else if (ch == 'X')
                    {
                        // Startposition für die Figur
                        figur = new Figur(a * cellSize, i * cellSize);
                        Spielfeld.Children.Add(figur.GetEllipse());
                    }
                }
            }
        }

        public void Window_KeyDown(object obj, KeyEventArgs e)
        {
            if (figur == null)
                return;

            // Bewegungsrichtung in Zellen
            int dxCell = 0;
            int dyCell = 0;

            if (e.Key == Key.Right)
                dxCell = 1;
            else if (e.Key == Key.Left)
                dxCell = -1;
            else if (e.Key == Key.Up)
                dyCell = -1;
            else if (e.Key == Key.Down)
                dyCell = 1;
            else
                return;

            // Zielposition in Pixeln und Zelle berechnen
            int targetX = figur.X + dxCell * cellSize;
            int targetY = figur.Y + dyCell * cellSize;
            int targetCol = targetX / cellSize;
            int targetRow = targetY / cellSize;

            // Grenzen prüfen
            if (targetRow < 0 || targetRow >= rows || targetCol < 0 || targetCol >= cols)
                return;

            // Wenn in der Zielzelle keine Wand ist, bewegen (Zellenweise Bewegung)
            if (!walls[targetRow, targetCol])
            {
                figur.Bewegen(dxCell * cellSize, dyCell * cellSize);
            }
        }
    }
}