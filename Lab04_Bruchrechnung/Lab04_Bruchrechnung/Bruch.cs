using System;

namespace Lab04_Bruchrechnung
{
    internal partial class Program
    {
        class Bruch
        {
            private int zaehler;
            private int nenner;

            public Bruch(int zaehler, int nenner)
            {
                this.zaehler = zaehler;
                this.nenner = nenner;
            }
            public int getZaehler()
            {
                return zaehler;
            }
            public override string ToString()
            {
                return zaehler + "/" + nenner;
            }
            public int getNenner()
            {
                return nenner;
            }
            public void setZaehler(int zaehler)
            {
                this.zaehler = zaehler;

            }
            public void setNenner(int nenner)
            {
                if (nenner == 0)
                {
                    throw new ArgumentException("Nenner darf nicht 0 sein.");
                }

                this.nenner = nenner;

            }
            public static Bruch Parse(string str)
            {
                string[] parts = str.Split('/');
                int zaehler = int.Parse(parts[0]);
                int nenner = int.Parse(parts[1]);
                return new Bruch(zaehler, nenner);

            }
            public void kuerzen()
            {
                //28 - 35
                int kleinster = Math.Min(zaehler, nenner);
                for (int i = kleinster; i > 1; i--)
                {
                    if (zaehler % i == 0 && nenner % i == 0)
                    {
                        zaehler /= i;
                        nenner /= i;
                    }
                }

            }
            public void add(Bruch b)
            {
                int neuerNenner = this.nenner * b.getNenner();
                int neuerZaehler = this.zaehler * b.getNenner() + b.getZaehler() * this.nenner;

                this.nenner = neuerNenner;
                this.zaehler = neuerZaehler;
                kuerzen();
            }
            public void sub(Bruch b)
            {
                int neuerNenner = this.nenner * b.getNenner();
                int neuerZaehler = this.zaehler * b.getNenner() - b.getZaehler() * this.nenner;
                this.nenner = neuerNenner;
                this.zaehler = neuerZaehler;
                kuerzen();
            }
            public void mul(Bruch b)
            {
                int neuerNenner = this.nenner * b.getNenner();
                int neuerZaehler = this.zaehler * b.getZaehler();
                this.nenner = neuerNenner;
                this.zaehler = neuerZaehler;
                kuerzen();
            }
            public void div(Bruch b)
            {
                int neuerNenner = this.nenner * b.getZaehler();
                int neuerZaehler = this.zaehler * b.getNenner();
                this.nenner = neuerNenner;
                this.zaehler = neuerZaehler;
                kuerzen();
            }

            static void Main(string[] args)
            {
                //Eingabe 67/69
                Console.Write("Bitte Bruch eingeben: ");
                
                string line = Console.ReadLine();
                Console.Write("Bitte Bruch eingeben: ");
                string line2 = Console.ReadLine();

                Bruch b = Bruch.Parse(line);
                Bruch b2 = Bruch.Parse(line2);
                
                b.add(b2);
                Console.WriteLine(b);

                b.sub(b2);
                b.mul(b2);
                b.div(b2);



                Console.ReadKey();
            }
        }
    }
}
