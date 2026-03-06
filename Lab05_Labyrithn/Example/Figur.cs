using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Example
{
    // Repräsentiert die Spielerfigur als einfache Ellipse, die pixelgenau bewegt werden kann.
    internal class Figur
    {
        // Größe der Figur in Pixeln (Breite/Höhe)
        int hoehe = 14;
        int breite = 14;

        // Aktuelle Position in Pixeln (links/oben)
        int x;
        int y;

        // Die gezeichnete Ellipse, die auf dem Canvas platziert wird
        Ellipse geometrie;

        public Figur(int x, int y)
        {
            // Startkoordinaten übernehmen
            this.x = x;
            this.y = y;

            // Ellipse erzeugen und konfigurieren
            geometrie = new Ellipse();
            geometrie.Width = breite;
            geometrie.Height = hoehe;
            geometrie.Fill = Brushes.Red;

            // Initiale Position setzen (Canvas-Koordinaten)
            Canvas.SetLeft(geometrie, x);
            Canvas.SetTop(geometrie, y);
        }

        // Öffentliche ReadOnly-Zugriffe auf die aktuelle Pixel-Position
        public int X { get { return x; } }
        public int Y { get { return y; } }

        // Bewegt die Figur um dx/dy Pixel und aktualisiert die Canvas-Position
        public void Bewegen(int dx, int dy)
        {
            x += dx;
            y += dy;
            Canvas.SetLeft(geometrie, x);
            Canvas.SetTop(geometrie, y);
        }

        // Rückgabe der Ellipse, damit sie ins Canvas eingefügt oder entfernt werden kann
        public Ellipse GetEllipse()
        {
            return geometrie;
        }
    }
}
/*namespace Example
{
    // Repräsentiert die Spielerfigur als einfache Ellipse, die pixelgenau bewegt werden kann.
    internal class Figur
    {
        // Größe der Figur in Pixeln (Breite/Höhe)
        int hoehe = 14;
        int breite = 14;

        // Aktuelle Position in Pixeln (links/oben)
        int x;
        int y;

        // Die gezeichnete Ellipse, die auf dem Canvas platziert wird
        Ellipse geometrie;

        public Figur(int x, int y)
        {
            // Startkoordinaten übernehmen
            this.x = x;
            this.y = y;

            // Ellipse erzeugen und konfigurieren
            geometrie = new Ellipse();
            geometrie.Width = breite;
            geometrie.Height = hoehe;
            geometrie.Fill = Brushes.Red;

            // Initiale Position setzen (Canvas-Koordinaten)
            Canvas.SetLeft(geometrie, x);
            Canvas.SetTop(geometrie, y);
        }

        // Öffentliche ReadOnly-Zugriffe auf die aktuelle Pixel-Position
        public int X { get { return x; } }
        public int Y { get { return y; } }

        // Bewegt die Figur um dx/dy Pixel und aktualisiert die Canvas-Position
        public void Bewegen(int dx, int dy)
        {
            x += dx;
            y += dy;
            Canvas.SetLeft(geometrie, x);
            Canvas.SetTop(geometrie, y);
        }

        // Rückgabe der Ellipse, damit sie ins Canvas eingefügt oder entfernt werden kann
        public Ellipse GetEllipse()
        {
            return geometrie;
        }
    }
}*/