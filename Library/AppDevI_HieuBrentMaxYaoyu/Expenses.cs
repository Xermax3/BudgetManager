using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Budget
{
    /// <summary>
    /// Represents a collection of <see cref="Expense"/> items.
    /// </summary>
    public class Expenses
    {
        private readonly SQLiteConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Expenses"/> class.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="isNewDB">Indicates whether the database is newly created or not.</param>
        public Expenses(SQLiteConnection connection, bool isNewDB)
        {
            _connection = connection;

            if (isNewDB)
            {
                CreateTable();
            }
        }

        private void CreateTable()
        {
            SQLiteCommand command = new SQLiteCommand(_connection)
            {
                // delete the tables
                CommandText = "DROP TABLE IF EXISTS expenses"
            };
            
            command.ExecuteNonQuery();

            // create tables
            command.CommandText = "CREATE TABLE expenses" +
                "(" +
                "Id             INTEGER         NOT NULL PRIMARY KEY AUTOINCREMENT," +
                "Date           TEXT            NOT NULL," +
                "Amount         FLOAT           NOT NULL," +
                "Description    TEXT            NOT NULL," +
                "CategoryId     INTEGER         NOT NULL," +
                "FOREIGN KEY (CategoryId) REFERENCES categories(Id)" +
                ")";
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Adds an <see cref="Expense"/> to the database with the given information.
        /// </summary>
        /// <param name="date">The date of the <see cref="Expense"/></param>
        /// <param name="categoryId">The category of the <see cref="Expense"/></param>
        /// <param name="amount">The amount of the <see cref="Expense"/></param>
        /// <param name="description">The description of the <see cref="Expense"/></param>
        /// <exception cref="Exception"></exception>
        public void Add(DateTime date, int categoryId, double amount, string description)
        {
            // add the new expense to the database
            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                string sql = "INSERT INTO expenses (Date, Description, Amount, CategoryId) VALUES (@date, @desc, @amt, @catId)";
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@desc", description);
                command.Parameters.AddWithValue("@amt", amount);
                command.Parameters.AddWithValue("@catId", categoryId);
                command.Prepare();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Updates an <see cref="Expense"/> in the database with the given information.
        /// </summary>
        /// <param name="id">The id of the <see cref="Expense"/> to be updated</param>
        /// <param name="date">The date of the <see cref="Expense"/></param>
        /// <param name="categoryId">The category of the <see cref="Expense"/></param>
        /// <param name="amount">The amount of the <see cref="Expense"/></param>
        /// <param name="description">The description of the <see cref="Expense"/></param>
        /// <exception cref="Exception"></exception>
        public void Update(int id, DateTime date, int categoryId, double amount, string description)
        {
            // update the expense in the database
            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                string sql = "UPDATE expenses SET Date = @date, Description = @desc, Amount = @amt, CategoryId = @catId WHERE id = @id";
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@desc", description);
                command.Parameters.AddWithValue("@amt", amount);
                command.Parameters.AddWithValue("@catId", categoryId);
                command.Parameters.AddWithValue("@id", id);
                command.Prepare();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Removes an <see cref="Expense"/> from the database with the given id.
        /// </summary>
        /// <param name="id">The id of the <see cref="Expense"/> to be deleted.</param>
        /// <exception cref="Exception"></exception>
        public void Delete(int id)
        {
            // delete the expense from the database
            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                string sql = "DELETE FROM expenses WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);
                command.Prepare();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Returns the <see cref="Expense"/> in the database with the given id.
        /// </summary>
        /// <param name="id">The id to find.</param>
        /// <returns>The <see cref="Expense"/> with the given id.</returns>
        /// <exception cref="Exception"></exception>
        public Expense GetExpenseById(int id)
        {
            Expense foundExp = null;
            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                // Get all the expenses stored in the database
                string sql = "SELECT Date, CategoryId, Amount, Description FROM expenses WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                command.Prepare();
                command.CommandText = sql;

                // Execute the sql and populate the list with the retrieved expenses
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        foundExp = new Expense(id, reader.GetDateTime(0), reader.GetInt32(1), reader.GetDouble(2), reader.GetString(3));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return foundExp;
        }

        /// <summary>
        /// Retrieves a <see cref="List{T}"/> of all the <see cref="Expense"/> items currently stored in the database.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of all the <see cref="Expense"/> items.</returns>
        /// <exception cref="Exception"></exception>
        public List<Expense> GetList()
        {
            List<Expense> list;

            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                // Get all the expenses stored in the database
                string sql = "SELECT Id, Date, CategoryId, Amount, Description FROM expenses ORDER BY id";
                command.CommandText = sql;

                list = new List<Expense>();

                // Execute the sql and populate the list with the retrieved expenses
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Expense(reader.GetInt32(0), reader.GetDateTime(1), reader.GetInt32(2), reader.GetDouble(3), reader.GetString(4)));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return list;
        }
    }
}