using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Example
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// Hauptfenster der Anwendung: lädt das Labyrinth, zeichnet Wände,
    /// erstellt die Spielfigur, verteilt Goodies und steuert Feinde.
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
        private int collectedGoodies = 0; // Anzahl gesammelter Goodies
        private int playerPoints = 10;    // Startpunkte des Spielers

        // Zufallsgenerator für die Platzierung von Goodies
        private readonly Random rnd = new Random();

        // Feinde und Timer, der ihre Bewegung steuert
        private readonly List<Enemy> enemies = new List<Enemy>();
        private readonly DispatcherTimer enemyTimer = new DispatcherTimer();

        private TextBlock gameOverText;
        private TextBlock pointsText;

        public MainWindow()
        {
            InitializeComponent();

            // Tastatur und Fokus sicherstellen
            this.Focusable = true;
            this.KeyDown += Window_KeyDown;
            this.Loaded += (s, e) => Keyboard.Focus(this);

            // Datei einlesen
            string[] zeilen = File.ReadAllLines("maze_10x10.txt");
            rows = zeilen.Length;
            cols = zeilen.Select(l => l.TrimEnd('\r')).Max(l => l.Length);
            walls = new bool[rows, cols];

            // Canvas Hintergrund
            Spielfeld.Background = Brushes.Black;

            // Labyrinth zeichnen und Startposition finden
            for (int r = 0; r < rows; r++)
            {
                string zeile = zeilen[r].TrimEnd('\r');
                for (int c = 0; c < zeile.Length; c++)
                {
                    char ch = zeile[c];
                    if (ch == '#')
                    {
                        walls[r, c] = true;
                        var rect = new Rectangle { Width = cellSize, Height = cellSize, Fill = Brushes.Green };
                        Canvas.SetLeft(rect, c * cellSize);
                        Canvas.SetTop(rect, r * cellSize);
                        Spielfeld.Children.Add(rect);
                    }
                    else if (ch == 'X')
                    {
                        figur = new Figur(c * cellSize, r * cellSize);
                        Spielfeld.Children.Add(figur.GetEllipse());
                    }
                }
            }

            // UI: Punkte links oben (Goodie-Anzeige entfernt)
            pointsText = new TextBlock { Foreground = Brushes.White, FontSize = 16 };
            Canvas.SetLeft(pointsText, 10);
            Canvas.SetTop(pointsText, 10);

            Spielfeld.Children.Add(pointsText);
            UpdateGoodieUI();

            // Goodies und Feinde
            PlaceGoodies(10);
            SetupEnemies(3);

            enemyTimer.Interval = TimeSpan.FromMilliseconds(500);
            enemyTimer.Tick += EnemyTimer_Tick;
            enemyTimer.Start();
        }

        private void SetupEnemies(int count)
        {
            int placed = 0;
            while (placed < count)
            {
                int r = rnd.Next(rows);
                int c = rnd.Next(cols);
                if (r < 0 || r >= rows || c < 0 || c >= cols) continue;
                if (walls[r, c]) continue;
                if (figur != null && r == figur.Y / cellSize && c == figur.X / cellSize) continue;
                if (enemies.Any(e => e.Row == r && e.Col == c)) continue;

                var enemy = new Enemy(r, c, cellSize);
                enemies.Add(enemy);
                Spielfeld.Children.Add(enemy.GetEllipse());
                placed++;
            }
        }

        private void EnemyTimer_Tick(object sender, EventArgs e)
        {
            foreach (var enemy in enemies.ToList())
            {
                var next = GetNextStepTowardsRandomTarget(enemy);
                if (next != null)
                {
                    enemy.MoveTo(next.Value.Item1, next.Value.Item2, cellSize);
                }

                // sofort Prüfen ob Feind auf Spieler steht
                if (figur != null)
                {
                    int playerRow = figur.Y / cellSize;
                    int playerCol = figur.X / cellSize;
                    if (enemy.Row == playerRow && enemy.Col == playerCol)
                        ApplyPlayerHit();
                }
            }
        }

        // BFS: finde einen zufälligen erreichbaren Zielpunkt und gib den nächsten Schritt dorthin zurück
        private Tuple<int, int> GetNextStepTowardsRandomTarget(Enemy enemy)
        {
            int sr = enemy.Row;
            int sc = enemy.Col;

            bool[,] visited = new bool[rows, cols];
            int[] dr = { 1, -1, 0, 0 };
            int[] dc = { 0, 0, 1, -1 };

            var q = new Queue<Tuple<int, int>>();
            var pred = new Dictionary<int, int>(); // key = r*cols + c, value = predKey
            visited[sr, sc] = true;
            q.Enqueue(Tuple.Create(sr, sc));

            var reachable = new List<Tuple<int, int>>();

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                reachable.Add(cur);
                for (int i = 0; i < 4; i++)
                {
                    int nr = cur.Item1 + dr[i];
                    int nc = cur.Item2 + dc[i];
                    if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;
                    if (visited[nr, nc]) continue;
                    if (walls[nr, nc]) continue;
                    // blockiere Zellen, die aktuell von anderen Feinden belegt sind
                    if (enemies.Any(e => e != enemy && e.Row == nr && e.Col == nc)) continue;

                    visited[nr, nc] = true;
                    int key = nr * cols + nc;
                    int curKey = cur.Item1 * cols + cur.Item2;
                    pred[key] = curKey;
                    q.Enqueue(Tuple.Create(nr, nc));
                }
            }

            // Entferne Startzelle aus Auswahl
            reachable.RemoveAll(t => t.Item1 == sr && t.Item2 == sc);
            if (reachable.Count == 0) return null;

            var target = reachable[rnd.Next(reachable.Count)];
            int tkey = target.Item1 * cols + target.Item2;

            // Rückwärts: bestimme direkten Nachschritt vom Start
            int curk = tkey;
            int startk = sr * cols + sc;
            int prevk;
            while (pred.TryGetValue(curk, out prevk))
            {
                if (prevk == startk)
                {
                    int nextR = curk / cols;
                    int nextC = curk % cols;
                    return Tuple.Create(nextR, nextC);
                }
                curk = prevk;
            }

            return null;
        }

        private void ApplyPlayerHit()
        {
            // kurzes visuelles Feedback
            FlashPlayer();

            playerPoints = Math.Max(0, playerPoints - 1);
            UpdateGoodieUI();

            if (playerPoints <= 0) GameOver();
        }

        private async void FlashPlayer()
        {
            try
            {
                if (figur == null) return;
                var el = figur.GetEllipse();
                if (el == null) return;
                var orig = el.Fill;
                el.Fill = Brushes.Red;
                await Task.Delay(200);
                el.Fill = orig;
            }
            catch { }
        }

        private void GameOver()
        {
            enemyTimer.Stop();
            if (gameOverText == null)
            {
                gameOverText = new TextBlock
                {
                    Text = "Game Over",
                    Foreground = Brushes.Red,
                    FontSize = 32
                };
                Canvas.SetLeft(gameOverText, (cols * cellSize) / 2 - 80);
                Canvas.SetTop(gameOverText, (rows * cellSize) / 2 - 32);
                Spielfeld.Children.Add(gameOverText);
            }
            gameOverText.Visibility = Visibility.Visible;
        }

        private void PlaceGoodies(int count)
        {
            int placed = 0;
            int startRow = -1, startCol = -1;
            if (figur != null) { startCol = figur.X / cellSize; startRow = figur.Y / cellSize; }

            while (placed < count)
            {
                int r = rnd.Next(rows);
                int c = rnd.Next(cols);
                if (r < 0 || r >= rows || c < 0 || c >= cols) continue;
                if (walls[r, c]) continue;
                if (r == startRow && c == startCol) continue;
                // keine Goodies in der rechten Spalte
                if (c == cols - 1) continue;
                if (goodies.Any(goodie => goodie.Row == r && goodie.Col == c)) continue;

                var g = new Goodie(r, c, cellSize);
                goodies.Add(g);
                Spielfeld.Children.Add(g.GetEllipse());
                placed++;
            }
        }

        private void UpdateGoodieUI()
        {
            if (pointsText != null) pointsText.Text = $"Punkte: {playerPoints}";
            // Goodie-Anzeige entfernt (keine rechte Statusleiste mehr)
        }

        /// <summary>
        /// KeyDown-Handler: bewegt die Figur in Zellen (Pfeiltasten).
        /// Prüft Kollision mit Wänden und sammelt Goodies ein.
        /// </summary>
        public void Window_KeyDown(object obj, KeyEventArgs e)
        {
            if (figur == null || playerPoints <= 0) return;

            int dx = 0, dy = 0;
            if (e.Key == Key.Right) dx = 1;
            else if (e.Key == Key.Left) dx = -1;
            else if (e.Key == Key.Up) dy = -1;
            else if (e.Key == Key.Down) dy = 1;
            else return;

            int targetX = figur.X + dx * cellSize;
            int targetY = figur.Y + dy * cellSize;
            int targetCol = targetX / cellSize;
            int targetRow = targetY / cellSize;

            if (targetRow < 0 || targetRow >= rows || targetCol < 0 || targetCol >= cols) return;

            if (!walls[targetRow, targetCol])
            {
                figur.Bewegen(dx * cellSize, dy * cellSize);

                var found = goodies.FirstOrDefault(g => g.Row == targetRow && g.Col == targetCol);
                if (found != null)
                {
                    Spielfeld.Children.Remove(found.GetEllipse());
                    goodies.Remove(found);
                    collectedGoodies++;
                    playerPoints++;
                    UpdateGoodieUI();
                }

                var enemyHere = enemies.FirstOrDefault(en => en.Row == targetRow && en.Col == targetCol);
                if (enemyHere != null) ApplyPlayerHit();
            }
        }
    }
}
