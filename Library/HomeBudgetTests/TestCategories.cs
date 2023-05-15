using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Data.SQLite;
using Budget;

namespace BudgetCodeTests
{
    [TestClass]
    public class TestCategories
    {
        public int numberOfCategoriesInFile = TestConstants.numberOfCategoriesInFile;
        public int maxIDInCategoryInFile = TestConstants.maxIDInCategoryInFile;
        Category firstCategoryInFile = TestConstants.firstCategoryInFile;

        // ========================================================================

        [TestMethod]
        public void CategoriesObject_New()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            SQLiteConnection connection = Database.CreateNewDatabase(newDB);

            // Act
            Categories categories = new Categories(connection, true, true);

            // Assert 
            Assert.IsInstanceOfType(categories, typeof(Categories));

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void CategoriesObject_New_CreatesDefaultCategories()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            SQLiteConnection connection = Database.CreateNewDatabase(newDB);

            // Act
            Categories categories = new Categories(connection, true, true);

            // Assert 
            Assert.IsFalse(categories.GetList().Count == 0, "Non zero categories");

            Database.CloseConnection();
        }

        [TestMethod]
        public void CategoriesObject_New_NoDefaultCategories()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            SQLiteConnection connection = Database.CreateNewDatabase(newDB);

            // Act
            Categories categories = new Categories(connection, true, false);

            // Assert 
            Assert.IsTrue(categories.GetList().Count == 0, "Non zero categories");

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_ReadFromDatabase_ValidateCorrectDataWasRead()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            SQLiteConnection connection = Database.OpenExistingDatabase(existingDB);

            // Act
            Categories categories = new Categories(connection, false, false);
            List<Category> list = categories.GetList();
            Category firstCategory = list[0];

            // Assert
            Assert.AreEqual(numberOfCategoriesInFile, list.Count, "Number of list elements are correct");
            Assert.AreEqual(firstCategoryInFile.Id, firstCategory.Id, "ID of first element");
            Assert.AreEqual(firstCategoryInFile.Description, firstCategory.Description, "Description of first Element");

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_List_ReturnsListOfCategories()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            SQLiteConnection connection = Database.OpenExistingDatabase(newDB);

            Categories categories = new Categories(connection, false, false);

            // Act
            List<Category> list = categories.GetList();

            // Assert
            Assert.AreEqual(numberOfCategoriesInFile, list.Count);

            Database.CloseConnection();
        }


        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_Add()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            File.Copy(goodDB, messyDB, true);
            SQLiteConnection connection = Database.OpenExistingDatabase(messyDB);

            Categories categories = new Categories(connection, false, false);
            string descr = "New Category";
            Category.CategoryType type = Category.CategoryType.Income;

            // Act
            categories.Add(descr, type);
            List<Category> categoriesList = categories.GetList();
            int sizeOfList = categoriesList.Count;

            // Assert
            Assert.AreEqual(numberOfCategoriesInFile + 1, sizeOfList, "List size incremented");
            Assert.AreEqual(descr, categoriesList[sizeOfList - 1].Description, "Description property set correctly");

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_Delete()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            File.Copy(goodDB, messyDB, true);
            SQLiteConnection connection = Database.OpenExistingDatabase(messyDB);

            Categories categories = new Categories(connection, false, false);
            int IdToDelete = 3;

            // Act
            categories.Delete(IdToDelete);
            List<Category> categoriesList = categories.GetList();
            int sizeOfList = categoriesList.Count;

            // Assert
            Assert.AreEqual(numberOfCategoriesInFile - 1, sizeOfList, "List size decremented");
            Assert.IsFalse(categoriesList.Exists(e => e.Id == IdToDelete), "correct Category item deleted");

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            File.Copy(goodDB, messyDB, true);
            SQLiteConnection connection = Database.OpenExistingDatabase(messyDB);

            Categories categories = new Categories(connection, false, false);
            int IdToDelete = 9999;
            int sizeOfList = categories.GetList().Count;

            // Act
            try
            {
                categories.Delete(IdToDelete);
                Assert.AreEqual(sizeOfList, categories.GetList().Count, "No Category was removed from list");
            }

            // Assert
            catch
            {
                Assert.IsTrue(false, "Invalid ID causes Delete to break");
            }

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_GetCategoryFromId()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            SQLiteConnection connection = Database.OpenExistingDatabase(newDB);

            Categories categories = new Categories(connection, false, false);
            int catID = 15;

            // Act
            Category category = categories.GetCategoryById(catID);

            // Assert
            Assert.AreEqual(catID, category.Id);

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_SetCategoriesToDefaults()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            SQLiteConnection connection = Database.CreateNewDatabase(newDB);

            // Act
            Categories categories = new Categories(connection, true, true);
            int initialCategoryCount = categories.GetList().Count;

            // modify list of categories
            categories.Delete(1);
            categories.Delete(2);
            categories.Delete(3);
            categories.Add("Another one ", Category.CategoryType.Credit);

            //"just double check that initial conditions are correct");
            Assert.AreNotEqual(initialCategoryCount, categories.GetList().Count, "unequal list sizes");

            // Act
            categories.SetToDefault();

            // Assert
            Assert.AreEqual(initialCategoryCount, categories.GetList().Count);

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_UpdateCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            SQLiteConnection connection = Database.CreateNewDatabase(newDB);

            Categories categories = new Categories(connection, true, true);
            String newDescr = "Presents";
            int id = 11;

            // Act
            categories.Update(id, newDescr, Category.CategoryType.Income);
            Category category = categories.GetCategoryById(id);

            // Assert 
            Assert.AreEqual(newDescr, category.Description);
            Assert.AreEqual(Category.CategoryType.Income, category.Type);

            Database.CloseConnection();
        }
    }
}

