using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04_Bruchrechnung
{
    internal partial class Program
    {

        static void Main(string[] args)
        {
            //Eingabe 67/69+4/7
            Console.Write("Bitte Bruchrechnung eingeben: ");

            string line = Console.ReadLine();

            Bruchrechnung b = Bruchrechnung.Parse(line);

            Console.WriteLine(b.getResult());


            Console.ReadKey();
        }
    }
}
