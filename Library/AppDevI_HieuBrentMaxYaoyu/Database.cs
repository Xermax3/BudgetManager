using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Budget
{
    /// <summary>
    /// Represents the <see cref="Database"/>, providing the connection between the database and the models.
    /// </summary>
    public static class Database
    {
        private static SQLiteConnection _connection;

        /// <summary>
        /// Creates a new database and opens a <see cref="SQLiteConnection"/> to it. The given file is created if it does not exist.
        /// </summary>
        /// <param name="databaseFile">The database file to store the new data.</param>
        /// <returns>The <see cref="SQLiteConnection"/> to the database file.</returns>
        /// <exception cref="IOException">If the file is not given.</exception>
        public static SQLiteConnection CreateNewDatabase(string databaseFile)
        {
            if (databaseFile == null || databaseFile == "")
            {
                throw new IOException("Database file is not given.");
            }

            string[] analysedPath = databaseFile.Replace('/', '\\').Split('\\');
            string currentLocation = "";
            bool newFolder = false;
            for (int i = 0; i < analysedPath.Length; i++)
            {
                if (i == analysedPath.Length - 1)
                {
                    if (!newFolder)
                    {
                        // if the given file doesn't exist
                        if (!File.Exists(databaseFile))
                        {
                            // create and prepare the file
                            SQLiteConnection.CreateFile(databaseFile);
                        }
                        break;
                    }

                    // create and prepare the file
                    SQLiteConnection.CreateFile(databaseFile);
                    break;
                }

                currentLocation += analysedPath[i] + "\\";
                if (newFolder || !Directory.Exists(currentLocation))
                {
                    if (!newFolder)
                    {
                        newFolder = true;
                    }

                    Directory.CreateDirectory(currentLocation);
                }
            }

            // open a connection to the file
            _connection = new SQLiteConnection($"Data Source={databaseFile}; Foreign Keys=1");
            _connection.Open();

            return _connection;
        }

        /// <summary>
        /// Opens a <see cref="SQLiteConnection"/> to the given database file.
        /// </summary>
        /// <param name="databaseFile">The path to the database file.</param>
        /// <returns>The <see cref="SQLiteConnection"/> to the database file.</returns>
        /// <exception cref="IOException">If the file is not given or does not exist.</exception>
        public static SQLiteConnection OpenExistingDatabase(string databaseFile)
        {
            if (databaseFile == null || databaseFile == "")
            {
                throw new IOException("Database file is not given");
            }

            if (!File.Exists(databaseFile))
            {
                throw new IOException("Database file does not exist");
            }

            // open a connection to the file
            _connection = new SQLiteConnection($"Data Source={databaseFile}; Foreign Keys=1");
            _connection.Open();

            if (!ValidDatabase())
            {
                CloseConnection();
                throw new IOException("Database file is corrupted");
            }

            return _connection;
        }

        /// <summary>
        /// Closes the current <see cref="SQLiteConnection"/>, then waits for all processes using it to end.
        /// </summary>
        public static void CloseConnection()
        {
            if (_connection != null)
            {
                _connection.Close();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private static bool ValidDatabase()
        {
            string[] neededTables = { "categories", "categoryTypes", "expenses" };
            DataTable dt = _connection.GetSchema("Tables");

            bool tableExists;
            foreach (string table in neededTables)
            {
                tableExists = false;

                foreach (DataRow row in dt.Rows)
                {
                    if (table == (string)row[2])
                    {
                        tableExists = true;
                        break;
                    }

                }

                if (!tableExists)
                    return false;
            }

            return true;
        }
    }
}
