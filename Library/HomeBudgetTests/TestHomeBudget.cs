using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Budget;

namespace BudgetCodeTests
{
    [TestClass]
    public class TestHomeBudget
    {
        // ========================================================================

        [TestMethod]
        [ExpectedException(typeof(IOException))]
        public void HomeBudgetObject_New_NoFileSpecified()
        {
            // Act
            HomeBudget homeBudget = new HomeBudget(@"C:\Users\Dan Halis\Documents\Budgets\new", true, false);
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetObject_New_FileNotExist()
        {
            // Arrange
            // Make sure that "abc.db" doesn't exist
            if (File.Exists("abc.db"))
            {
                File.Delete("abc.db");
            }

            // Act
            HomeBudget homeBudget = new HomeBudget("abc.db", true, true);

            // Assert
            // Expect that the file is created
            Assert.IsTrue(File.Exists("abc.txt"));
            // Expect that category list is set to default values
            Assert.AreEqual(TestConstants.numberOfCategoriesInFile, homeBudget.Categories.GetList().Count);
            // Expect that there's nothing yet in the expense list
            Assert.AreEqual(0, homeBudget.Expenses.GetList().Count);

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetObject_New_WithFilename()
        {
            // Arrange
            string databasefile = GetSolutionDir() + "\\" + TestConstants.testDBInputFile;
            int numExpenses = TestConstants.numberOfExpensesInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;

            // Act
            HomeBudget homeBudget = new HomeBudget(databasefile, false, false);

            // Assert 
            Assert.IsInstanceOfType(homeBudget, typeof(HomeBudget));
            Assert.AreEqual(numExpenses, homeBudget.Expenses.GetList().Count, "Correct number of expenses read");
            Assert.AreEqual(numCategories, homeBudget.Categories.GetList().Count, "Correct number of categories read");

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgeMethod_ReadFromFile_ReadsCorrectData()
        {
            // Arrange
            string databasefile = GetSolutionDir() + "\\" + TestConstants.testDBInputFile;
            int numExpenses = TestConstants.numberOfExpensesInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;
            Expense firstExpenseInFile = TestConstants.firstExpenseInFile;
            Category firstCategoryInFile = TestConstants.firstCategoryInFile;

            HomeBudget homeBudget = new HomeBudget(databasefile, false, false);

            // Act
            Expense firstExpense = homeBudget.Expenses.GetList()[0];
            Category firstCategory = homeBudget.Categories.GetList()[0];


            // Assert 
            Assert.AreEqual(numExpenses, homeBudget.Expenses.GetList().Count, "Correct number of expenses read");
            Assert.AreEqual(numCategories, homeBudget.Categories.GetList().Count, "Correct number of categories read");
            Assert.AreEqual(firstExpenseInFile.Description, firstExpense.Description, "expense descr is correct");
            Assert.AreEqual(firstCategoryInFile.Description, firstCategory.Description, "category descr is correct");

            Database.CloseConnection();
        }

        // ========================================================================

        // -------------------------------------------------------
        // helpful functions, ... they are not tests
        // -------------------------------------------------------

        private String GetSolutionDir()
        {
            // this is valid for C# .Net Foundation (not for C# .Net Core)
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"));
        }
    }
}

