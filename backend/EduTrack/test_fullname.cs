using System;
using EduTrack.Domain.ValueObjects;

class Program 
{
    static void Main()
    {
        try 
        {
            Console.WriteLine("Testing FullName.Create with 'Mahedee Hasan'");
            var fullName = FullName.Create("Mahedee Hasan");
            Console.WriteLine($"Result: '{fullName.Value}'");
            Console.WriteLine($"FirstName: '{fullName.FirstName}'");
            Console.WriteLine($"LastName: '{fullName.LastName}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
