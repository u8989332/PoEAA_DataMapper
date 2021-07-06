using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace PoEAA_DataMapper
{
    abstract class AbstractMapper
    {
        protected Dictionary<int, DomainObject> LoadedMap = new Dictionary<int, DomainObject>();
        protected abstract string FindStatement();
        protected abstract string InsertStatement();

        protected abstract int FindNextDatabaseId();

        protected DomainObject AbstractFind(int id)
        {
            bool findResult = LoadedMap.TryGetValue(id, out DomainObject result);
            if (findResult)
            {
                return result;
            }

            try
            {
                using var conn = DbManager.CreateConnection();
                conn.Open();
                using IDbCommand comm = new SQLiteCommand(FindStatement(), conn);
                comm.Parameters.Add(new SQLiteParameter("$id", id));
                using IDataReader reader = comm.ExecuteReader();
                reader.Read();
                result = Load(reader);
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        protected DomainObject Load(IDataReader reader)
        {
            object[] resultSet = new object[reader.FieldCount];
            reader.GetValues(resultSet);

            int id = (int)resultSet[0];
            if (LoadedMap.ContainsKey(id))
            {
                return LoadedMap[id];
            }

            DomainObject result = DoLoad(id, reader);
            LoadedMap.Add(id, result);
            return result;
        }

        protected List<DomainObject> LoadAll(IDataReader reader)
        {
            List<DomainObject> result = new List<DomainObject>();
            while (reader.Read())
            {
                result.Add(Load(reader));
            }

            return result;
        }

        protected abstract DomainObject DoLoad(int id, IDataReader reader);

        public int Insert(DomainObject subject)
        {
            try
            {
                using var conn = DbManager.CreateConnection();
                conn.Open();
                using IDbCommand comm = new SQLiteCommand(InsertStatement(), conn);
                subject.Id = FindNextDatabaseId();
                var parameter = comm.CreateParameter();
                parameter.DbType = DbType.Int32;
                parameter.Value = subject.Id;
                comm.Parameters.Add(parameter);
                DoInsert(subject, comm);
                comm.ExecuteNonQuery();
                LoadedMap.Add(subject.Id, subject);
                return subject.Id;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        protected abstract void DoInsert(DomainObject subject, IDbCommand insertStatement);

        public List<DomainObject> FindMany(IStatementSource source)
        {
            try
            {
                using var conn = DbManager.CreateConnection();
                conn.Open();
                using IDbCommand comm = new SQLiteCommand(source.Sql, conn);
                foreach (var p in source.Parameters)
                {
                    var parameter = comm.CreateParameter();
                    parameter.DbType = DbType.Object;
                    parameter.Value = p;
                    comm.Parameters.Add(parameter);
                }
                using IDataReader reader = comm.ExecuteReader();
                return LoadAll(reader);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

    }
}
