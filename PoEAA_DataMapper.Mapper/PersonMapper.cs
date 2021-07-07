using PoEAA_DataMapper.Database;
using PoEAA_DataMapper.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace PoEAA_DataMapper.Mapper
{
    public class PersonMapper : AbstractMapper, IPersonFinder
    {
        private const string Columns = " id, lastname, firstname, numberOfDependents ";

        private const string UpdateStatementString =
            "UPDATE person SET lastname = ?, firstname = ?, numberOfDependents = ? WHERE id = ?";
        protected override string FindStatement()
        {
            return "SELECT " + Columns + " FROM person WHERE id = $id";
        }

        protected override string InsertStatement()
        {
            return "INSERT INTO person VALUES (?, ?, ?, ?)";
        }

        protected override int FindNextDatabaseId()
        {
            string sql = "SELECT max(id) as curId from person";
            using var conn = DbManager.CreateConnection();
            conn.Open();
            using IDbCommand comm = new SQLiteCommand(sql, conn);
            using IDataReader reader = comm.ExecuteReader();
            bool hasResult = reader.Read();
            if (hasResult)
            {
                return ((int)((long)reader["curId"] + 1));
            }
            else
            {
                return 1;
            }
        }

        protected override DomainObject DoLoad(int id, IDataReader reader)
        {
            object[] resultSet = new object[reader.FieldCount];
            reader.GetValues(resultSet);
            string lastName = resultSet[1].ToString();
            string firstName = resultSet[2].ToString();
            int numberOfDependents = (int)resultSet[3];
            return new Person(id, lastName, firstName, numberOfDependents);
        }

        protected override void DoInsert(DomainObject subject, IDbCommand insertStatement)
        {
            Person person = (Person) subject;
            var p1 = insertStatement.CreateParameter();
            p1.DbType = DbType.String;
            p1.Value = person.LastName;

            var p2 = insertStatement.CreateParameter();
            p2.DbType = DbType.String;
            p2.Value = person.FirstName;

            var p3 = insertStatement.CreateParameter();
            p3.DbType = DbType.Int32;
            p3.Value = person.NumberOfDependents;
            insertStatement.Parameters.Add(p1);
            insertStatement.Parameters.Add(p2);
            insertStatement.Parameters.Add(p3);
        }

        public Person Find(int id)
        {
            return (Person) AbstractFind(id);
        }

        public IList<Person> FindByLastName2(string pattern)
        {
            return FindMany(new FindByLastName(pattern))
                .Cast<Person>().ToList();
        }

        public IList<Person> FinAll()
        {
            return FindMany(new FindAllStatement())
                .Cast<Person>().ToList();
        }

        public void Update(Person subject)
        {
            try
            {
                using var conn = DbManager.CreateConnection();
                conn.Open();
                using IDbCommand comm = new SQLiteCommand(UpdateStatementString, conn);
                var p1 = comm.CreateParameter();
                p1.DbType = DbType.String;
                p1.Value = subject.LastName;

                var p2 = comm.CreateParameter();
                p2.DbType = DbType.String;
                p2.Value = subject.FirstName;

                var p3 = comm.CreateParameter();
                p3.DbType = DbType.Int32;
                p3.Value = subject.NumberOfDependents;

                var p4 = comm.CreateParameter();
                p4.DbType = DbType.Int32;
                p4.Value = subject.Id;

                comm.Parameters.Add(p1);
                comm.Parameters.Add(p2);
                comm.Parameters.Add(p3);
                comm.Parameters.Add(p4);

                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        private class FindByLastName : IStatementSource
        {
            private readonly string _lastName;

            public string Sql { get; } =
                "SELECT " + Columns + " FROM person WHERE UPPER(lastname) like UPPER(?) ORDER BY lastName";

            public object[] Parameters
            {
                get
                {
                    return new object[] {_lastName};
                }
            }

            public FindByLastName(string lastName)
            {
                _lastName = lastName;
            }
        }

        private class FindAllStatement : IStatementSource
        {
            private readonly string _lastName;

            public string Sql { get; } =
                "SELECT * FROM person";

            public object[] Parameters
            {
                get
                {
                    return new object[] {};
                }
            }
        }
    }
}
