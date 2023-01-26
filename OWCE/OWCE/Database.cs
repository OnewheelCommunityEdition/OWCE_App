using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;

namespace OWCE
{
    public static class Database
    {
        public static SQLiteConnection Connection { get; private set; }

        static Database()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "owce.db");

            Connection = new SQLiteConnection(databasePath);
        }

        public static void Init()
        {
            Connection.CreateTable<Ride>();
        }
    }
}
