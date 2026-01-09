using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab03_Quader
{

    class Quader 
    {
        private double hoehe;
        private double breite;
        private double laenge;

        public Quader() : this(1,1,1)
        {
        }

        public Quader(double hoehe, double breite, double laenge)
        {
            //Masße in mm 
            this.hoehe = hoehe;
            this.breite = breite;
            this.laenge = laenge;
        }
        public static double ParseValue(string text)
        {
            double value = 0;
            if (text.EndsWith("cm"))
            {
                string hoeheStr = text.Replace("cm", "");
                value = double.Parse(hoeheStr) * 10; // cm --> mm
            }
            else if (text.EndsWith("mm"))
            {
                string hoeheStr = text.Replace("mm", "");
                value = double.Parse(hoeheStr); // mm
            }
            return value;
        }
        public static Quader Parse(string text) // static --> Klassenmethode 
        {
            double h = 0;
            double b = 0;
            double l = 0;
            text = text.Replace(" ", ""); // Leerzeichen entfernen --> " 2 cm ; 3 cm ; 5 mm " --> "2cm;3cm;5mm"
            string[] teile = text.Split(';'); // ["2cm","3cm","5mm"]
            h = ParseValue(teile[0]); // 2cm --> 20mm
            b = ParseValue(teile[1]); // 3cm --> 30mm
            l = ParseValue(teile[2]); // 5mm --> 5mm


            return new Quader(h, b, l);
        }
        public double GetVolume()
        {
            return hoehe * breite * laenge; // mm^3
        }
        public void DrawFootprint(int mmPerChar = 1)
        {
            //Zeichne mir die Grundfläche des Quaders
            //Beispiel länge = 5, breite = 10
            //##########
            //#        #
            //#        #
            //#        #
            //##########
            // so soll es aussehen
            if (mmPerChar <= 0) mmPerChar = 10;

            // In deinem Beispiel: länge = 5, breite = 10 -> 10 Spalten, 5 Zeilen
            // Daraus folgt: horizontale Darstellung = breite, vertikale = laenge
            int cols = (int)Math.Round(breite / mmPerChar);
            int rows = (int)Math.Round(laenge / mmPerChar);

            if (cols < 2) cols = 2;
            if (rows < 2) rows = 2;

            Console.WriteLine($"Grundfläche: {breite}mm × {laenge}mm  (1 Zeichen = {mmPerChar} mm)");

            // Zeichnung in Grün; ursprüngliche Farbe danach wiederherstellen
            var originalColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;

                // Oberer Rand
                Console.WriteLine(new string('#', cols));

                // Mittlere Reihen
                for (int r = 0; r < rows - 2; r++)
                {
                    if (cols >= 2)
                    {
                        Console.Write('#');
                        Console.Write(new string(' ', cols - 2));
                        Console.WriteLine('#');
                    }
                    else
                    {
                        Console.WriteLine(new string('#', cols));
                    }
                }

                // Unterer Rand (nur, wenn mehr als 1 Reihe existiert)
                if (rows >= 2)
                    Console.WriteLine(new string('#', cols));
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            // Bitte Quader eingeben : 2cm;3cm;5mm
            Console.WriteLine("Bitte Quader eingeben : ");
            string eingabe = Console.ReadLine();
            

            Quader q = Quader.Parse(eingabe); // Klassenmethode

            Console.WriteLine($"Volumen des Quaders berechnen.{q.GetVolume()}mm³");
            q.DrawFootprint(1);
            Random rnd = new Random();

            List<Quader> quaderListe = new List<Quader>();
            for (int i = 0; i < 10; i++)
            {
                

                quaderListe.Add(new Quader(rnd.Next(10, 20),
                    rnd.Next(10,20),
                    rnd.Next(10,20)));
                quaderListe[i].DrawFootprint();
            }

            Console.ReadKey();
            // Qudaer q1 = new Quader(); 
            // Console.WriteLine(q1.GetHeight()); // Instanzmethode
        }
    }
}
