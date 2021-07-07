using System;
using System.Collections.Generic;
using PoEAA_DataMapper.Database;
using PoEAA_DataMapper.Domain;
using PoEAA_DataMapper.Mapper;

namespace PoEAA_DataMapper
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeData();

            Console.WriteLine("Get persons");
            PersonMapper mapper = new PersonMapper();
            // get all persons
            var people = mapper.FinAll();
            PrintPerson(people);

            Console.WriteLine("Insert a new person");
            mapper.Insert(new Person(0, "Rose", "Jackson", 60));
            people = mapper.FinAll();
            PrintPerson(people);

            Console.WriteLine("Update a person's first name");
            var firstPerson = mapper.Find(1);
            firstPerson.FirstName = "Jack";
            mapper.Update(firstPerson);

            Console.WriteLine("Update a person's number of dependents");
            var secondPerson = mapper.Find(2);
            secondPerson.NumberOfDependents = 0;
            mapper.Update(secondPerson);


            Console.WriteLine("Get persons again");
            people = mapper.FinAll();
            PrintPerson(people);

            Console.WriteLine("Get persons with lastname containing n");
            people = mapper.FindByLastName2("%n%");
            PrintPerson(people);
        }

        private static void PrintPerson(IEnumerable<Person> people)
        {
            foreach (var person in people)
            {
                Console.WriteLine($"ID: {person.Id}, " +
                                  $"last name: {person.LastName}, " +
                                  $"first name: {person.FirstName}, " +
                                  $"number of dependents: {person.NumberOfDependents}, " +
                                  $"exemption: {person.GetExemption().Amount}");
            }
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
