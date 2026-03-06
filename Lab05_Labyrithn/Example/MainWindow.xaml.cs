using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Example;

namespace Example
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// Hauptfenster der Anwendung: lädt das Labyrinth, zeichnet Wände,
    /// erstellt die Spielfigur und verteilt Goodies. Behandelt Tastatureingaben.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Die Spielerfigur (Ellipse auf dem Canvas)
        private Figur figur = null;

        // Raster, das anzeigt, ob eine Zelle eine Wand ist (rows x cols)
        private bool[,] walls;
        private int rows;
        private int cols;

        // Größe einer Zelle in Pixeln: wichtig für Positionierung und Bewegung
        private readonly int cellSize = 20;

        // Liste aller aktuell platzierten Goodies
        private readonly List<Goodie> goodies = new List<Goodie>();

        // Anzahl eingesammelter Goodies (Score)
        private int collectedGoodies = 0;

        // Zufallsgenerator für die Platzierung von Goodies
        private readonly Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();

            // --- Labyrinth-Datei einlesen ---
            // Robustere Dateieinlesung: eine Zeile pro String-Element
            string[] zeilen = File.ReadAllLines("maze_10x10.txt");

            // Anzahl der Zeilen = Anzahl der Reihen im Raster
            rows = zeilen.Length;

            // Spaltenanzahl: die längste Zeile (nach Entfernen von \r)
            cols = zeilen.Select(l => l.TrimEnd('\r')).Max(l => l.Length);

            // walls-Array initialisieren
            walls = new bool[rows, cols];

            // Hintergrund des Spielfeldes setzen (Canvas)
            Spielfeld.Background = Brushes.Black;

            // Datei durchlaufen und Wände / Startposition zeichnen
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

            // Goodies zufällig in den Gängen platzieren (z. B. 10)
            PlaceGoodies(10);

            // UI (Score) initial aktualisieren
            UpdateGoodieUI();

            // Sicherstellen, dass das Fenster beim Start Fokus bekommt, damit KeyDown funktioniert
            this.Loaded += (s, e) => Keyboard.Focus(this);
        }

        /// <summary>
        /// Platziert `count` Goodies zufällig in offenen Zellen.
        /// Berücksichtigt Startposition und vermeidet doppelte Plätze.
        /// </summary>
        private void PlaceGoodies(int count)
        {
            int placed = 0;

            // Startposition der Figur in Zellen (falls gesetzt)
            int startRow = -1, startCol = -1;
            if (figur != null)
            {
                startCol = figur.X / cellSize;
                startRow = figur.Y / cellSize;
            }

            while (placed < count)
            {
                int r = rnd.Next(rows);
                int c = rnd.Next(cols);

                // Nur gültige Rasterkoordinaten
                if (r < 0 || r >= rows || c < 0 || c >= cols)
                    continue;

                // Keine Wand
                if (walls[r, c])
                    continue;

                // Nicht auf der Startzelle platzieren
                if (r == startRow && c == startCol)
                    continue;

                // Nicht doppelt platzieren: prüfe vorhandene Goodies
                if (goodies.Any(goodie => goodie.Row == r && goodie.Col == c))
                    continue;

                // Goodie erzeugen und dem Canvas/Liste hinzufügen
                var g = new Goodie(r, c, cellSize);
                goodies.Add(g);
                Spielfeld.Children.Add(g.GetEllipse());
                placed++;
            }
        }

        /// <summary>
        /// Aktualisiert die Anzeige des Scores. Prüft auf null, falls XAML-Element fehlt.
        /// </summary>
        private void UpdateGoodieUI()
        {
            // Vorsicht: GoodieCountText kann null sein, wenn XAML-Name fehlt
            if (GoodieCountText != null)
                GoodieCountText.Text = collectedGoodies.ToString();
        }

        /// <summary>
        /// KeyDown-Handler: bewegt die Figur in Zellen (Pfeiltasten).
        /// Prüft Kollision mit Wänden und sammelt Goodies ein.
        /// </summary>
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
                return; // andere Tasten ignorieren

            // Zielposition in Pixeln und die zugehörige Zelle berechnen
            int targetX = figur.X + dxCell * cellSize;
            int targetY = figur.Y + dyCell * cellSize;
            int targetCol = targetX / cellSize;
            int targetRow = targetY / cellSize;

            // Grenzen prüfen (außerhalb des Labyrinths -> nicht bewegen)
            if (targetRow < 0 || targetRow >= rows || targetCol < 0 || targetCol >= cols)
                return;

            // Wenn in der Zielzelle keine Wand ist, bewege die Figur
            if (!walls[targetRow, targetCol])
            {
                figur.Bewegen(dxCell * cellSize, dyCell * cellSize);

                // Nach der Bewegung prüfen, ob ein Goodie eingesammelt wurde
                var found = goodies.FirstOrDefault(g => g.Row == targetRow && g.Col == targetCol);
                if (found != null)
                {
                    // Goodie vom Canvas entfernen und aus der Liste löschen
                    Spielfeld.Children.Remove(found.GetEllipse());
                    goodies.Remove(found);

                    // Score erhöhen und UI aktualisieren
                    collectedGoodies++;
                    UpdateGoodieUI();
                }
            }
        }
    }
}
/*public partial class MainWindow : Window
{
    // Die Spielerfigur (Ellipse auf dem Canvas)
    private Figur figur = null;

    // Raster, das anzeigt, ob eine Zelle eine Wand ist (rows x cols)
    private bool[,] walls;
    private int rows;
    private int cols;

    // Größe einer Zelle in Pixeln: wichtig für Positionierung und Bewegung
    private readonly int cellSize = 20;

    // Liste aller aktuell platzierten Goodies
    private readonly List<Goodie> goodies = new List<Goodie>();

    // Anzahl eingesammelter Goodies (Score)
    private int collectedGoodies = 0;

    // Zufallsgenerator für die Platzierung von Goodies
    private readonly Random rnd = new Random();

    public MainWindow()
    {
        InitializeComponent();

        // --- Labyrinth-Datei einlesen ---
        // Robustere Dateieinlesung: eine Zeile pro String-Element
        string[] zeilen = File.ReadAllLines("maze_10x10.txt");

        // Anzahl der Zeilen = Anzahl der Reihen im Raster
        rows = zeilen.Length;

        // Spaltenanzahl: die längste Zeile (nach Entfernen von \r)
        cols = zeilen.Select(l => l.TrimEnd('\r')).Max(l => l.Length);

        // walls-Array initialisieren
        walls = new bool[rows, cols];

        // Hintergrund des Spielfeldes setzen (Canvas)
        Spielfeld.Background = Brushes.Black;

        // Datei durchlaufen und Wände / Startposition zeichnen
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

        // Goodies zufällig in den Gängen platzieren (z. B. 10)
        PlaceGoodies(10);

        // UI (Score) initial aktualisieren
        UpdateGoodieUI();

        // Sicherstellen, dass das Fenster beim Start Fokus bekommt, damit KeyDown funktioniert
        this.Loaded += (s, e) => Keyboard.Focus(this);
    }

    /// <summary>
    /// Platziert `count` Goodies zufällig in offenen Zellen.
    /// Berücksichtigt Startposition und vermeidet doppelte Plätze.
    /// </summary>
    private void PlaceGoodies(int count)
    {
        int placed = 0;

        // Startposition der Figur in Zellen (falls gesetzt)
        int startRow = -1, startCol = -1;
        if (figur != null)
        {
            startCol = figur.X / cellSize;
            startRow = figur.Y / cellSize;
        }

        while (placed < count)
        {
            int r = rnd.Next(rows);
            int c = rnd.Next(cols);

            // Nur gültige Rasterkoordinaten
            if (r < 0 || r >= rows || c < 0 || c >= cols)
                continue;

            // Keine Wand
            if (walls[r, c])
                continue;

            // Nicht auf der Startzelle platzieren
            if (r == startRow && c == startCol)
                continue;

            // Nicht doppelt platzieren: prüfe vorhandene Goodies
            if (goodies.Any(goodie => goodie.Row == r && goodie.Col == c))
                continue;

            // Goodie erzeugen und dem Canvas/Liste hinzufügen
            var g = new Goodie(r, c, cellSize);
            goodies.Add(g);
            Spielfeld.Children.Add(g.GetEllipse());
            placed++;
        }
    }

    /// <summary>
    /// Aktualisiert die Anzeige des Scores. Prüft auf null, falls XAML-Element fehlt.
    /// </summary>
    private void UpdateGoodieUI()
    {
        // Vorsicht: GoodieCountText kann null sein, wenn XAML-Name fehlt
        if (GoodieCountText != null)
            GoodieCountText.Text = collectedGoodies.ToString();
    }

    /// <summary>
    /// KeyDown-Handler: bewegt die Figur in Zellen (Pfeiltasten).
    /// Prüft Kollision mit Wänden und sammelt Goodies ein.
    /// </summary>
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
            return; // andere Tasten ignorieren

        // Zielposition in Pixeln und die zugehörige Zelle berechnen
        int targetX = figur.X + dxCell * cellSize;
        int targetY = figur.Y + dyCell * cellSize;
        int targetCol = targetX / cellSize;
        int targetRow = targetY / cellSize;

        // Grenzen prüfen (außerhalb des Labyrinths -> nicht bewegen)
        if (targetRow < 0 || targetRow >= rows || targetCol < 0 || targetCol >= cols)
            return;

        // Wenn in der Zielzelle keine Wand ist, bewege die Figur
        if (!walls[targetRow, targetCol])
        {
            figur.Bewegen(dxCell * cellSize, dyCell * cellSize);

            // Nach der Bewegung prüfen, ob ein Goodie eingesammelt wurde
            var found = goodies.FirstOrDefault(g => g.Row == targetRow && g.Col == targetCol);
            if (found != null)
            {
                // Goodie vom Canvas entfernen und aus der Liste löschen
                Spielfeld.Children.Remove(found.GetEllipse());
                goodies.Remove(found);

                // Score erhöhen und UI aktualisieren
                collectedGoodies++;
                UpdateGoodieUI();
            }
        }
    }
}
}*/
