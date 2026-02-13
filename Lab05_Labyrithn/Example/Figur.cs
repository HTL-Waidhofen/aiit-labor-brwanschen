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
    internal class Figur
    {
        int hoehe = 14;
        int breite = 14;
        int x;
        int y;
        Ellipse geometrie;
        public Figur(int x, int y)
        {
            this.x = x;
            this.y = y;
            geometrie = new Ellipse();
            geometrie.Width = breite;
            geometrie.Height = hoehe;
            geometrie.Fill = Brushes.Red;
            Canvas.SetLeft(geometrie, x);
            Canvas.SetTop(geometrie, y);
        }

        public void Bewegen(int dx, int dy)
        {
            x += dx;
            y += dy;
        }
        public Ellipse GetEllipse()
        {
            return geometrie;
        }
    }
}
