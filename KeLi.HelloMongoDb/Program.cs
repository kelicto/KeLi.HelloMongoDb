using System;

using KeLi.HelloMongoDb.Models;
using KeLi.HelloMongoDb.Utils;

namespace KeLi.HelloMongoDb
{
    internal class Program
    {
        private static void Main()
        {
            // Add data.
            {
                MongoUtil.Insert(new Student { Name = "Tom" });
                MongoUtil.Insert(new Student { Name = "Jack" });
                MongoUtil.Insert(new Student { Name = "Tony" });

                Console.WriteLine("After Added data:");

                foreach (var item in MongoUtil.QueryList<Student>())
                    Console.WriteLine(item.Name);
            }

            Console.WriteLine();

            // Delete data.
            {
                MongoUtil.Delete<Student>(f => f.Name.Contains("Tom"));

                Console.WriteLine("After Deleted data:");

                foreach (var item in MongoUtil.QueryList<Student>())
                    Console.WriteLine(item.Name);
            }

            Console.WriteLine();

            // Update data.
            {
                MongoUtil.Update<Student>(u => u.Name.Contains("Jack"), u => u.Name = "Alice");

                Console.WriteLine("After Updated data:");

                foreach (var item in MongoUtil.QueryList<Student>())
                    Console.WriteLine(item.Name);
            }

            Console.WriteLine();

            // Query data.
            {
                var students = MongoUtil.QueryList<Student>(w => w.Name.Contains("T"));

                Console.WriteLine("Query data:");

                foreach (var item in students)
                    Console.WriteLine(item.Name);
            }

            Console.ReadKey();
        }
    }
}