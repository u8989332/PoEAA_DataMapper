using System;

namespace PoEAA_DataMapper
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeData();

            PersonMapper mapper = new PersonMapper();
            mapper.Insert(new Person(4, "QQ", "GG", 4545));
            var person = mapper.Find(4);

            //var person = mapper.Find(1);
            //person.FirstName = "QQ";
            //person.LastName = "KK";
            //person.NumberOfDependents = 4545;
            //mapper.Update(person);
            var persons = mapper.FindByLastName2("%n%");
            Console.WriteLine(person.FirstName);
        }

        private static void InitializeData()
        {
            using (var connection = DbManager.CreateConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        @"
                        DROP TABLE IF EXISTS person;
                    ";
                    command.ExecuteNonQuery();


                    command.CommandText =
                        @"
                        CREATE TABLE person (Id int primary key, lastname TEXT, firstname TEXT, numberOfDependents int);
                    ";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        @"
                       
                    INSERT INTO person
                        VALUES (1, 'Sean', 'Reid', 5);

                    INSERT INTO person
                        VALUES (2, 'Madeleine', 'Lyman', 13);

                    INSERT INTO person
                        VALUES (3, 'Oliver', 'Wright', 66);
                    ";
                    command.ExecuteNonQuery();
                }

            }
        }
    }
}
