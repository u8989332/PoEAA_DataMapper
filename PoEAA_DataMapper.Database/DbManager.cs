using System.Data.SQLite;

namespace PoEAA_DataMapper.Database
{
    public static class DbManager
    {
        public static SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection("Data Source=poeaa_datamapper.db");
        }
    }
}