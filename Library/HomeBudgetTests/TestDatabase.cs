using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Budget;
using System.Data.SQLite;
using System.IO;

namespace BudgetCodeTests
{
    [TestClass]
    public class TestDatabase
    {
        [TestMethod]
        public void SQLite_TestNewDatabase_NewFolderAndNewFileCreated()
        {
            // Arrange
            string filename = "newdb.db";
            string fullpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Budgets", filename);
            // Act
            Database.CreateNewDatabase(fullpath);

            Assert.IsTrue(File.Exists(fullpath));

            Database.CloseConnection();
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_TablesCreated()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> tables = new List<String>() { "categoryTypes", "expenses", "categories" };

            // Act
            Database.CreateNewDatabase(path + "\\" + filename);

            // Assert
            string cmd = " .tables";
            List<String> databaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (databaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no tables created in new database ");
            }

            String table_string = databaseOutput[0];
            foreach (String table in tables)
            {
                Assert.IsTrue(table_string.Contains(table), $"table {table} in database");
            }

            Database.CloseConnection();
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_ForeignKeyConstraintsEnabled()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";

            // Act
            SQLiteConnection connection = Database.CreateNewDatabase(path + "\\" + filename);

            // Assert
            String connectionString = connection.ConnectionString;
            Assert.IsTrue(connectionString.Contains("Foreign Keys=1"), "FK Constraints enabled");

            Database.CloseConnection();
        }

        [TestMethod]
        public void SQLite_TestExistingDatabase_RequiredForeignKeysExpenses()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);

            // Act
            SQLiteConnection connection = Database.OpenExistingDatabase(messyDB);

            // Assert
            String connectionString = connection.ConnectionString;
            Assert.IsTrue(connectionString.Contains("Foreign Keys=1"),
                           "FK Constraints enabled");

            // Cleanup
            Database.CloseConnection();
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_ColumnsInTableExpenses()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> columns = new List<string>() { "Id", "CategoryId", "Amount", "Date", "Description" };

            // Act
            Database.CreateNewDatabase(path + "\\" + filename);

            // Assert
            string cmd = " \".mode list\" \"pragma table_info(expenses)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no columns in table expenses ");
            }

            // Assert
            foreach (String column in columns)
            {
                int index = DatabaseOutput.FindIndex(s => s.Contains($"|{column}|"));
                Assert.AreNotEqual(-1, index, $"column {column} found in table expenses");
            }

            Database.CloseConnection();
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_ColumnsInTableCategory()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> columns = new List<string>() { "Id", "Description", "TypeId" };

            // Act
            Database.CreateNewDatabase(path + "\\" + filename);

            // Assert
            string cmd = " \".mode list\" \"pragma table_info(categories)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no columns in table categories ");
            }

            // Assert
            foreach (String column in columns)
            {
                int index = DatabaseOutput.FindIndex(s => s.Contains($"|{column}|"));
                Assert.AreNotEqual(-1, index, $"column {column} found in table categories");
            }

            Database.CloseConnection();
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_ColumnsInTableCategoryTypes()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> columns = new List<string>() { "Id", "Description" };

            // Act
            Database.CreateNewDatabase(path + "\\" + filename);

            // Assert
            string cmd = " \".mode list\" \"pragma table_info(CategoryTypes)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no columns in table types ");
            }

            // Assert
            foreach (String column in columns)
            {
                int index = DatabaseOutput.FindIndex(s => s.Contains($"|{column}|"));
                Assert.AreNotEqual(-1, index, $"column {column} found in table CategoryTypes");
            }

            Database.CloseConnection();
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_RequiredForeignKeysCategories()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            Dictionary<String, String> FKtable = new Dictionary<String, String>()
            {
                {"table", "categoryTypes"},
                { "from", "TypeId" },
                {"to", "Id" },
            };

            // Act
            Database.CreateNewDatabase(path + "\\" + filename);

            // Assert
            string cmd = " \".mode line\" \"pragma foreign_key_list(categories)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no foreign in table categories ");
            }

            // Assert
            foreach (KeyValuePair<string, string> kvp in FKtable)
            {
                String FKProperty = $"{kvp.Key} = {kvp.Value}";
                int index = DatabaseOutput.FindIndex(s => s.Contains(FKProperty));
                Assert.AreNotEqual(-1, index, $"{FKProperty} in table categories");
            }

            Database.CloseConnection();
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_RequiredForeignKeysExpenses()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            Dictionary<String, String> FKtable = new Dictionary<String, String>()
            {
                {"table", "categories"},
                { "from", "CategoryId" },
                {"to", "Id" },
            };

            // Act
            Database.CreateNewDatabase(path + "\\" + filename);

            // Assert
            string cmd = " \".mode line\" \"pragma foreign_key_list(expenses)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no foreign in table expenses ");
            }

            // Assert
            foreach (KeyValuePair<string, string> kvp in FKtable)
            {
                String FKProperty = $"{kvp.Key} = {kvp.Value}";
                int index = DatabaseOutput.FindIndex(s => s.Contains(FKProperty));
                Assert.AreNotEqual(-1, index, $"{FKProperty} in table expenses");
            }

            Database.CloseConnection();
        }

        //// Below are new tests

        [TestMethod]
        public void SQLite_TestExistingDatabase_ThrowsExceptionIfDoesNotExist()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "dbThatDoesNotExist.db";

            // Act / Assert
            Assert.ThrowsException<IOException>(() => Database.OpenExistingDatabase(path + "\\" + filename));

            Database.CloseConnection();
        }

        [TestMethod]
        public void SQLite_TestExistingDatabase_TablesExist()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "testDBInput.db";
            List<String> tables = new List<String>() { "categoryTypes", "expenses", "categories" };

            // Act
            Database.OpenExistingDatabase(path + "\\" + filename);

            // Assert
            string cmd = " .tables";
            List<String> databaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput(path + "\\" + filename + cmd);
            if (databaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There are no tables in the test database ");
            }

            String table_string = databaseOutput[0];
            foreach (String table in tables)
            {
                Assert.IsTrue(table_string.Contains(table), $"table {table} in database");
            }

            Database.CloseConnection();
        }
    }

    public class DatabaseCommandLine
    {
        static public List<String> ExecuteAndReturnOutput(string DatabaseCmd)
        {
            // https://stackoverflow.com/questions/206323/how-to-execute-command-line-in-c-get-std-out-results

            //Create process
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();

            //strCommand is path and file name of command to run
            pProcess.StartInfo.FileName = "sqlite3";

            //strCommandParameters are parameters to pass to program
            pProcess.StartInfo.Arguments = DatabaseCmd;

            pProcess.StartInfo.UseShellExecute = false;

            //Set output of program to be written to process output stream
            pProcess.StartInfo.RedirectStandardOutput = true;

            //Start the process
            pProcess.Start();

            //Wait for process to finish
            pProcess.WaitForExit();

            //Get program output
            string strOutput = pProcess.StandardOutput.ReadToEnd();

            // Convert the output to a list of strings
            List<String> output = new List<string>();
            using (System.IO.StringReader reader = new System.IO.StringReader(strOutput))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    output.Add(line);
                }
            }

            return output;
        }
    }
}
