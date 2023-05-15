using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Budget
{
    /// <summary>
    /// Represents a list of <see cref="Category"/> items.
    /// </summary>
    public class Categories
    {
        private readonly SQLiteConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Categories"/> class.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="isNewDB">Indicates whether the database is newly created or not.</param>
        /// <param name="defaultCats">Indicates whether to create default categories or not.</param>
        public Categories(SQLiteConnection connection, bool isNewDB, bool defaultCats)
        {
            _connection = connection;

            if (isNewDB)
            {
                CreateTables();

                if (defaultCats)
                {
                    SetToDefault();
                }
            }
        }

        private void CreateTables()
        {
            SQLiteCommand command = new SQLiteCommand(_connection);

            // delete the tables
            command.CommandText = "PRAGMA foreign_keys = OFF;";
            command.ExecuteNonQuery();
            command.CommandText = "DROP TABLE IF EXISTS categories";
            command.ExecuteNonQuery();
            command.CommandText = "DROP TABLE IF EXISTS categoryTypes";
            command.ExecuteNonQuery();
            command.CommandText = "PRAGMA foreign_keys = ON;";
            command.ExecuteNonQuery();

            // create tables
            command.CommandText = "CREATE TABLE categoryTypes" +
               "(" +
               "Id             INTEGER         NOT NULL PRIMARY KEY AUTOINCREMENT," +
               "Description    TEXT            NOT NULL" +
               ")";
            command.ExecuteNonQuery();

            // add category types
            foreach (Category.CategoryType categoryType in Enum.GetValues(typeof(Category.CategoryType)))
            {
                string sql = "INSERT INTO categoryTypes (Description) VALUES ('@categoryType')";
                command.Parameters.AddWithValue("@categoryType", categoryType.ToString());
                command.Prepare();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }

            command.CommandText = "CREATE TABLE categories" +
                "(" +
                "Id             INTEGER         NOT NULL PRIMARY KEY AUTOINCREMENT," +
                "Description    TEXT            NOT NULL," +
                "TypeId         INTEGER         NOT NULL," +
                "FOREIGN KEY (TypeId) REFERENCES categoryTypes(Id)" +
                ")";
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Creates tables for <see cref="Categories"/> and <see cref="Category.CategoryType"/> in the database, then inserts default data for each.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void SetToDefault()
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                // add default categories
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Utilities', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Rent', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Food', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Entertainment', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Education', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Miscellaneous', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Medical Expenses', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Vacation', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Credit Card', 3)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Clothes', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Gifts', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Insurance', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Transportation', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Eating Out', 2)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Savings', 4)";
                command.ExecuteNonQuery();
                command.CommandText = "INSERT INTO categories (Description, TypeId) VALUES ('Income', 1)";
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Returns the <see cref="Category"/> in the database with the given id.
        /// </summary>
        /// <param name="id">The id to find.</param>
        /// <returns>The <see cref="Category"/> with the given id.</returns>
        /// <exception cref="Exception"></exception>
        public Category GetCategoryById(int id)
        { 
            Category foundCategory = null;
            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                // Get all the categories stored in the database
                string sql = "SELECT Description, TypeId FROM categories WHERE id = @id";
                command.Parameters.AddWithValue("@id", id);
                command.Prepare();
                command.CommandText = sql;

                // Execute the sql
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    // if there's something to read
                    if (reader.Read())
                    {
                        // ONLY add the category to the obj list, not to the database
                        foundCategory = new Category(id, reader.GetString(1), (Category.CategoryType)reader.GetInt32(2));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return foundCategory;
        }

        /// <summary>
        /// Returns a <see cref="List{T}"/> of <see cref="Category"/> of the same <see cref="Category.CategoryType"/> in the database.
        /// </summary>
        /// <param name="typeId">The id of the target type.</param>
        /// <returns>The <see cref="List{T}"/> of <see cref="Category"/> of the same <see cref="Category.CategoryType"/>.</returns>
        public List<Category> GetCategoriesByType(int typeId)
        {
            List<Category> foundCategories = new List<Category>();
            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                // Get all the categories stored in the database
                string sql = "SELECT Id, Description FROM categories WHERE TypeId = @TypeId";
                command.Parameters.AddWithValue("@TypeId", typeId);
                command.Prepare();
                command.CommandText = sql;

                // Execute the sql
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    // while there's something to read
                    while (reader.Read())
                    {
                        // ONLY add the category to the obj list, not to the database
                        foundCategories.Add(new Category(reader.GetInt32(0), reader.GetString(1), (Category.CategoryType)typeId));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return foundCategories;
        }

        /// <summary>
        /// Adds a <see cref="Category"/> to the database with the given information.
        /// </summary>
        /// <param name="description">The description of the <see cref="Category"/>.</param>
        /// <param name="type">The type of the <see cref="Category"/>.</param>
        /// <exception cref="Exception"></exception>
        public void Add(string description, Category.CategoryType type)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                string sql = "INSERT INTO categories (Description, TypeID) VALUES (@desc, @type)";
                command.Parameters.AddWithValue("@desc", description);
                command.Parameters.AddWithValue("@type", (int)type);
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
        /// Updates a <see cref="Category"/> in the database with the given information.
        /// </summary>
        /// <param name="id">The id of the <see cref="Category"/> to be updated.</param>
        /// <param name="description">The new category description.</param>
        /// <param name="type">The new category type.</param>
        /// <exception cref="Exception"></exception>
        public void Update(int id, string description, Category.CategoryType type)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                string sql = "UPDATE categories SET Description = @desc, TypeId = @type WHERE id = @id";
                command.Parameters.AddWithValue("@desc", description);
                command.Parameters.AddWithValue("@type", (int)type);
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
        /// Removes a <see cref="Category"/> from the database with the given id.
        /// </summary>
        /// <param name="id">The id of the <see cref="Category"/> to be deleted.</param>
        /// <exception cref="Exception"></exception>
        public void Delete(int id)
        {
            // delete the category from the database
            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                string sql = "DELETE FROM categories WHERE id = @id";
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
        /// Returns a <see cref="List{T}"/> of all the <see cref="Category"/> items currently stored in the database.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of all the <see cref="Category"/> items.</returns>
        /// <exception cref="Exception"></exception>
        public List<Category> GetList()
        {
            List<Category> list;

            try
            {
                SQLiteCommand command = new SQLiteCommand(_connection);

                // Get all the categories stored in the database
                string sql = "SELECT Id, Description, TypeId FROM categories ORDER BY id";
                command.CommandText = sql;

                list = new List<Category>();

                // Execute the sql and populate the list with the retrieved categories
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Category(reader.GetInt32(0), reader.GetString(1), (Category.CategoryType)reader.GetInt32(2)));
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

