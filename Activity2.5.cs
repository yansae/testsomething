using System;

namespace Activity5
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("===== SIMPLE CALCULATOR =====\n");
            Console.Write("Enter first number: ");
            int num1 = int.Parse(Console.ReadLine());
            Console.Write("Enter second number: ");
            int num2 = int.Parse(Console.ReadLine());

            Console.WriteLine("Select an operation:");
            Console.WriteLine("1 - Add");
            Console.WriteLine("2 - Subtract");
            Console.WriteLine("3 - Multiply");
            Console.WriteLine("4 - Divide");
            Console.Write("\nEnter your choice (1-4, 5 to exit): ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.WriteLine($"Result: {num1 + num2}");
                    break;
                case 2:
                    Console.WriteLine($"Result: {num1 - num2}");
                    break;
                case 3:
                    Console.WriteLine($"Result: {num1 * num2}");
                    break;
                case 4:
                    if (num2 != 0)
                        Console.WriteLine($"Result: {num1 / num2}");
                    else
                        Console.WriteLine("Error: Division by zero is not allowed.");
                        break;
                default:
                    Console.WriteLine("Invalid choice. Please select a valid operation.");
                    break;
            }
            Console.ReadKey();
        }
    }
}