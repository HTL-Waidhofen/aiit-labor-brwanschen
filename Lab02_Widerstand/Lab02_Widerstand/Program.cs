// See https://aka.ms/new-console-template for more information


using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

class Resistor      // Klasse = Bauplan
{
    // Atribute, Member Variablen
    private string label; // Bezeichung fpr R1,R2;...
    private double value; //Widerstandswert
    private double toleranz; // zb 1 für 1%, 5 für 5%
    private double maxPower; // zb 10 für 10 Watt

    //Methoden
    // Konstruktor
    public Resistor(string label, double value, double toleranz,double maxPower)
    {
        this.label = label;
        this.value = value;
        this.toleranz = toleranz;
        this.maxPower = maxPower;

    }
    //Zugriffmethoden Getter and Setter
    public double GetValue()
    {
        return value; 
    }

    public double CalculateCurrent(double voltage)
    {
        double Current = voltage/this.value;
        return Current;
    }
    public double CalculateVoltage (double current)
    {
        double voltage = this.value * current;
        return voltage;
    }
    public Resistor inSeriemit(Resistor r2)
    {
        Resistor Rges = new Resistor("Rges", this.value + r2.value, Math.Min(this.toleranz, r2.toleranz), Math.Min(this.maxPower, r2.maxPower));
        return Rges;
    }
    public Resistor inParallelemit(Resistor r2)
    {
        double RgesValue = 1 / (1 / this.value + 1 / r2.value);
        Resistor Rges = new Resistor("Rges", RgesValue, Math.Min(this.toleranz, r2.toleranz), Math.Min(this.maxPower, r2.maxPower));
        return Rges;
    }


}
internal class Program
{
    static void Main (string[] args)
    {
        // Objekte sind die Instanzen der Klasse
        Resistor r1 = new Resistor("R1",100,5,10);      // objekt 1 vom typ resisotr
        Resistor r2 = new Resistor("R2",100,5,10);       // objekt 2 vom typ ressitor

        double current = r1.CalculateCurrent(5);

        Resistor Rges = r1.inSeriemit(r2);
        Resistor Rpar = r1.inParallelemit(r2);




        Console.ReadKey();
    }
}
