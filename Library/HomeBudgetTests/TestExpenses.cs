using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Data.SQLite;
using Budget;

namespace BudgetCodeTests
{
    [TestClass]
    public class TestExpenses
    {
        int numberOfExpensesInFile = TestConstants.numberOfExpensesInFile;
        int maxIDInExpenseFile = TestConstants.maxIDInExpenseFile;
        Expense firstExpenseInFile = new Expense(1, new DateTime(2021, 1, 10), 10, 12, "hat (on credit)");


        // ========================================================================

        [TestMethod]
        public void ExpensesObject_New()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            SQLiteConnection connection = Database.CreateNewDatabase(newDB);

            // Act
            Expenses expenses = new Expenses(connection, true);

            // Assert 
            Assert.IsInstanceOfType(expenses, typeof(Expenses));

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_ReadFromFile_ValidateCorrectDataWasRead()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            SQLiteConnection connection = Database.OpenExistingDatabase(existingDB);

            // Act
            Expenses expenses = new Expenses(connection, false);
            List<Expense> list = expenses.GetList();
            Expense firstExpense = list[0];

            // Assert
            Assert.AreEqual(numberOfExpensesInFile, list.Count, "Number of list elements are correct");
            Assert.AreEqual(firstExpenseInFile.Id, firstExpense.Id, "ID of first element");
            Assert.AreEqual(firstExpenseInFile.Amount, firstExpense.Amount, "Amount of first element");
            Assert.AreEqual(firstExpenseInFile.Description, firstExpense.Description, "Description of first Element");
            Assert.AreEqual(firstExpenseInFile.CategoryId, firstExpense.CategoryId, "Category of First Element");

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_List_ReturnsListOfExpenses()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            SQLiteConnection connection = Database.OpenExistingDatabase(existingDB);

            Expenses expenses = new Expenses(connection, false);

            // Act
            List<Expense> list = expenses.GetList();

            // Assert
            Assert.AreEqual(numberOfExpensesInFile, list.Count);

            Database.CloseConnection();
        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_List_ModifyListDoesNotModifyExpensesInstance()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            SQLiteConnection connection = Database.OpenExistingDatabase(existingDB);

            Expenses expenses = new Expenses(connection, false);

            // Act
            List<Expense> list = expenses.GetList();
            list[0].Amount = list[0].Amount + 21.03;

            // Assert
            Assert.AreNotEqual(list[0].Amount, expenses.GetList()[0].Amount, "Modifying list should not modify Expense Object");

        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_Add()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            File.Copy(goodDB, messyDB, true);
            SQLiteConnection connection = Database.OpenExistingDatabase(messyDB);

            Expenses expenses = new Expenses(connection, false);

            int category = 17;
            //int category = 57;
            double amount = 98.1;

            // Act
            expenses.Add(DateTime.Now, category, amount, "new expense");
            List<Expense> expensesList = expenses.GetList();
            int sizeOfList = expensesList.Count;

            // Assert
            Assert.AreEqual(numberOfExpensesInFile + 1, sizeOfList, "List size incremented");
            Assert.AreEqual(maxIDInExpenseFile + 1, expensesList[sizeOfList - 1].Id, "Id set to max + 1");
            Assert.AreEqual(amount, expensesList[sizeOfList - 1].Amount, "Amount property set correctly");

        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_Delete()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            File.Copy(goodDB, messyDB, true);
            SQLiteConnection connection = Database.OpenExistingDatabase(messyDB);

            Expenses expenses = new Expenses(connection, false);
            int IdToDelete = 3;

            // Act
            expenses.Delete(IdToDelete);
            List<Expense> expensesList = expenses.GetList();
            int sizeOfList = expensesList.Count;

            // Assert
            Assert.AreEqual(numberOfExpensesInFile - 1, sizeOfList, "List size decremented");
            Assert.IsFalse(expensesList.Exists(e => e.Id == IdToDelete), "correct expense item deleted");

        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            File.Copy(goodDB, messyDB, true);
            SQLiteConnection connection = Database.OpenExistingDatabase(messyDB);

            Expenses expenses = new Expenses(connection, false);
            int IdToDelete = 1006;
            int sizeOfList = expenses.GetList().Count;

            // Act
            try
            {
                expenses.Delete(IdToDelete);
                Assert.AreEqual(sizeOfList, expenses.GetList().Count, "No Expense was removed from list");
            }

            // Assert
            catch
            {
                Assert.IsTrue(false, "Invalid ID causes Delete to break");
            }
        }
    }
}

