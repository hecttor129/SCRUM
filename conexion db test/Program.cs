using DAL;
using ENTITY;
using System;
using System.Linq;
using static DAL.DB_Context;

class Program
{
    static void Main()
    {
        try
        {
            using var context = new DB_Context();

            Console.WriteLine("Intentando conectar...");

            bool conecta = context.Database.CanConnect();

            Console.WriteLine($"¿Conectó? {conecta}");

            var usuarios = context.Usuarios.ToList();

            Console.WriteLine($"Usuarios encontrados: {usuarios.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error:");
            Console.WriteLine(ex.Message);
        }

        Console.ReadKey();
    }
}
