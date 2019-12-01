using System;

namespace DataMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            var migration = new Migration();
            
            Console.WriteLine("Do you want to migrate users? [Y/N]");
            var k = Console.ReadKey();
            if (k.KeyChar.ToString().ToLower().Equals("y"))
            {
                migration.Start();
            }
            Console.WriteLine("Do you want migrate units? [Y/N]");
            k = Console.ReadKey();
            if (k.KeyChar.ToString().ToLower().Equals("y"))
            {
                migration.MigrateUnits();
            }
            Console.WriteLine("Do you want migrate user association to units? [Y/N]");
            k = Console.ReadKey();
            if (k.KeyChar.ToString().ToLower().Equals("y"))
            {
                migration.MigrateAssociation();
            }
        }
    }
}
