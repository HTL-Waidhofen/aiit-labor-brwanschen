using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab07_Referenzen
{
    internal class Program
    {
        class Person
        {
            private string vorname;     // Attribute / Membervariable
            private string nachname;
            public Adresse Adresse { get; set; } 

            public string Vorname // Property
            {
                get { return vorname; }

                set
                {

                    vorname = value[0].ToString().ToUpper()
                             + value.ToLower().Substring(1);
                }
            }
            public string Nachname
            {
                get { return nachname; }
                set { nachname = value;}
            }

            public Person(string vorname, string nachname)
            {
                this.vorname = vorname;
                this.nachname = nachname;
            }
              
        }
        class Adresse
        {
            public string Strasse { get; set; }
            public Ort Ort { get; set; }
            public int Hausnummer { get; set; }

            public Adresse(string strasse, int hausnummer)
            {
                Strasse = strasse;
                
                Hausnummer = hausnummer;
            }
        }

        class Ort
        {
            public string Name { get; set; }
            public int Postleitzahl { get; set; }
            public Person Buergermeister { get; set; }

            public Ort(string name, int postleitzahl)
                {
                    Name = name;
                    Postleitzahl = postleitzahl;
            }
        }

        static void Main(string[] args)
        {
            int x = 10;

            Ort ort = new Ort("Musterstadt", 1234);

            Person p1 = new Person("Max", "Mustermann");
            Person p2 = new Person("Dagobert", "Duck");
            Person p3 = new Person("Susi", "Beispielfrau");
            Person p4 = new Person("Daisy", "Duck");

            Adresse a1 = new Adresse("Musterstraße", 4);
            Adresse a2 = new Adresse("Musterplatz", 7);

            a1.Ort = ort;
            a2.Ort = ort;

            p1.Adresse = a1;
            p2.Adresse = a2;
            p3.Adresse = a1;
            p4.Adresse = a2;


            Console.WriteLine(p1.Vorname);

            Console.ReadKey();

        }
    }
}
