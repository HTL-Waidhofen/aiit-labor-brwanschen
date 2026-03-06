using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Example
{
    // Repräsentiert ein Goodie, das in einer Rasterzelle angezeigt wird.
    internal class Goodie
    {
        // Position des Goodies in Rasterkoordinaten (Zeile/Spalte)
        public int Row { get; }
        public int Col { get; }

        // Die sichtbare Ellipse des Goodies
        private readonly Ellipse ellipse;

        public Goodie(int row, int col, int cellSize)
        {
            Row = row;
            Col = col;

            // Ellipse passend zur Zelle erstellen (60% der Zellgröße)
            ellipse = new Ellipse
            {
                Width = cellSize * 0.6,
                Height = cellSize * 0.6,
                Fill = Brushes.Gold
            };

            // Ellipse zentriert in der Zelle platzieren
            double left = Col * cellSize + (cellSize - ellipse.Width) / 2.0;
            double top = Row * cellSize + (cellSize - ellipse.Height) / 2.0;
            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);
        }

        // Zugriff auf das Shape, damit das MainWindow es ins Canvas einfügen/entfernen kann
        public Ellipse GetEllipse()
        {
            return ellipse;
        }
    }
}
/*namespace Example
{
    // Repräsentiert ein Goodie, das in einer Rasterzelle angezeigt wird.
    internal class Goodie
    {
        // Position des Goodies in Rasterkoordinaten (Zeile/Spalte)
        public int Row { get; }
        public int Col { get; }

        // Die sichtbare Ellipse des Goodies
        private readonly Ellipse ellipse;

        public Goodie(int row, int col, int cellSize)
        {
            Row = row;
            Col = col;

            // Ellipse passend zur Zelle erstellen (60% der Zellgröße)
            ellipse = new Ellipse
            {
                Width = cellSize * 0.6,
                Height = cellSize * 0.6,
                Fill = Brushes.Gold
            };

            // Ellipse zentriert in der Zelle platzieren
            double left = Col * cellSize + (cellSize - ellipse.Width) / 2.0;
            double top = Row * cellSize + (cellSize - ellipse.Height) / 2.0;
            Canvas.SetLeft(ellipse, left);
            Canvas.SetTop(ellipse, top);
        }

        // Zugriff auf das Shape, damit das MainWindow es ins Canvas einfügen/entfernen kann
        public Ellipse GetEllipse()
        {
            return ellipse;
        }
    }
}*/