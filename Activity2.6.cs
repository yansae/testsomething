using System;

namespace Activity6
{
    class Program
    {
        static void Main()
        {
            #newchangehaha
            Console.WriteLine("==================================");
            Console.WriteLine("    STUDENT SCHOLARSHIP SYSTEM    ");
            Console.WriteLine("==================================");

            Console.Write("Enter student name: ");
            string name = Console.ReadLine();
            Console.Write("Enter student grade: ");
            int grade = int.Parse(Console.ReadLine());
            Console.Write("Enter number of failed subjects: ");
            int failedSubjects = int.Parse(Console.ReadLine());

            Console.WriteLine("\nStudent Name: " + name);
            if (grade >= 90 && failedSubjects == 0)
            {
                Console.WriteLine("Scholarship Status:");
                Console.WriteLine("FULL SCHOLARSHIP");
            }
            else if (grade >= 85 && failedSubjects == 0)
            {
                Console.WriteLine("Scholarship Status:");
                Console.WriteLine("PARTIAL SCHOLARSHIP");
            }
            else
            {
                Console.WriteLine("Scholarship Status:");
                Console.WriteLine("NOT ELIGIBLE");
            }
            Console.ReadKey();
        }
    }
}