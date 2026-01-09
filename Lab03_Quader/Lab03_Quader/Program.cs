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
            ParseValue(teile[0]); // Höhe


            return new Quader(h, b, l);
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

            // Qudaer q1 = new Quader(); 
            // Console.WriteLine(q1.GetHeight()); // Instanzmethode
        }
    }
}
