using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Example
{
    // Einfacher Feind: bewegt sich feldweise (Raster) und besitzt eine Ellipse zur Darstellung.
    internal class Enemy
    {
        public int Row { get; private set; }
        public int Col { get; private set; }
        private readonly Ellipse ellipse;

        public Enemy(int row, int col, int cellSize)
        {
            Row = row;
            Col = col;

            ellipse = new Ellipse
            {
                Width = cellSize * 0.7,
                Height = cellSize * 0.7,
                Fill = Brushes.Purple
            };

            // Zentriert in der Zelle platzieren
            double left = Col * cellSize + (cellSize - ellipse.Width) / 2.0;
            double top = Row * cellSize + (cellSize - ellipse.Height) / 2.0;
            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);
        }

        // Rückgabe des Shapes, damit MainWindow es ins Canvas einfügen/entfernen kann
        public Ellipse GetEllipse()
        {
            return ellipse;
        }

        // Setzt die neue Rasterposition und verschiebt die sichtbare Ellipse
        public void MoveTo(int newRow, int newCol, int cellSize)
        {
            Row = newRow;
            Col = newCol;
            double left = Col * cellSize + (cellSize - ellipse.Width) / 2.0;
            double top = Row * cellSize + (cellSize - ellipse.Height) / 2.0;
            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);
        }
    }
}