using System;

namespace UnitTesting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(" Console App is Running...");

            var calc = new Calculator();
            Console.WriteLine($"3 + 2 = {calc.Add(3, 2)}");

            Console.WriteLine($"3 - 2 = {calc.Subtract(3, 2)}");

            Console.WriteLine($"3 * 2 = {calc.Multiply(3, 2)}");

            Console.WriteLine($"3 / 1 = {calc.Divide(3, 1)}");

            Console.WriteLine($"3  = {calc.Square(3)}");
        }
    }
}
