using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Budget;

namespace BudgetCodeTests
{
    [TestClass]
    public class TestHomeBudget_GetBudgetItems
    {
        string testInputFile = TestConstants.testDBInputFile;

        // ========================================================================
        // Get Expenses Method tests
        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_NoStartEnd_NoFilter()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);
            List<Expense> listExpenses = homeBudget.Expenses.GetList();
            List<Category> listCategories = homeBudget.Categories.GetList();

            // Act
            List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(null, null, false, 9);

            // Assert
            Assert.AreEqual(listExpenses.Count, budgetItems.Count, "correct number of budget items");
            foreach (Expense expense in listExpenses)
            {
                BudgetItem budgetItem = budgetItems.Find(b => b.ExpenseId == expense.Id);
                Category category = listCategories.Find(c => c.Id == expense.CategoryId);
                Assert.AreEqual(budgetItem.CategoryDescription, category.Description, "Category description ok");
                Assert.AreEqual(budgetItem.CategoryId, expense.CategoryId, "Category id is ok");
                Assert.AreEqual(budgetItem.Amount, expense.Amount, "Amount is ok");
                Assert.AreEqual(budgetItem.ExpenseDescription, expense.Description, "Expense description ok");
            }
        }

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_NoStartEnd_NoFilter_VerifyBalanceProperty()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);

            // Act
            List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(null, null, false, 9);

            // Assert
            double balance = 0;
            foreach (BudgetItem budgetItem in budgetItems)
            {
                balance = balance + budgetItem.Amount;
                Assert.AreEqual(balance, budgetItem.Balance, "Balance for expense id ", budgetItem.ExpenseId, " is good");
            }
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_NoStartEnd_FilterbyCategory()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);
            int filterCategory = 9;
            List<Expense> listExpenses = TestConstants.filteredbyCat9();
            List<Category> listCategories = homeBudget.Categories.GetList();

            // Act
            List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(null, null, true, filterCategory);

            // Assert
            Assert.AreEqual(listExpenses.Count, budgetItems.Count, "correct number of budget items");
            foreach (Expense expense in listExpenses)
            {
                BudgetItem budgetItem = budgetItems.Find(b => b.ExpenseId == expense.Id);
                Category category = listCategories.Find(c => c.Id == expense.CategoryId);
                Assert.AreEqual(budgetItem.CategoryDescription, category.Description, "Category description ok");
                Assert.AreEqual(budgetItem.CategoryId, expense.CategoryId, "Category id is ok");
                Assert.AreEqual(budgetItem.Amount, expense.Amount, "Amount is ok");
                Assert.AreEqual(budgetItem.ExpenseDescription, expense.Description, "Expense description ok");
            }
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_2018_filterDate()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);
            List<Expense> listExpenses = TestConstants.filteredbyYear2018();
            List<Category> listCategories = homeBudget.Categories.GetList();

            // Act
            List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), false, 0);

            // Assert
            Assert.AreEqual(listExpenses.Count, budgetItems.Count, "correct number of budget items");
            foreach (Expense expense in listExpenses)
            {
                BudgetItem budgetItem = budgetItems.Find(b => b.ExpenseId == expense.Id);
                Category category = listCategories.Find(c => c.Id == expense.CategoryId);
                Assert.AreEqual(budgetItem.CategoryDescription, category.Description, "Category description ok");
                Assert.AreEqual(budgetItem.CategoryId, expense.CategoryId, "Category id is ok");
                Assert.AreEqual(budgetItem.Amount, expense.Amount, "Amount is ok");
                Assert.AreEqual(budgetItem.ExpenseDescription, expense.Description, "Expense description ok");
            }
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_2018_filterDate_verifyBalance()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);
            List<Expense> listExpenses = TestConstants.filteredbyCat9();
            List<Category> listCategories = homeBudget.Categories.GetList();

            // Act
            List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(null, null, true, 9);
            double total = budgetItems[budgetItems.Count - 1].Balance;


            // Assert
            Assert.AreEqual(TestConstants.filteredbyCat9Total, total, "budgetitem balance is correct");
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_2018_filterDateAndCat10()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);
            List<Expense> listExpenses = TestConstants.filteredbyYear2018AndCategory10();
            List<Category> listCategories = homeBudget.Categories.GetList();

            // Act
            List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), true, 10);

            // Assert
            Assert.AreEqual(listExpenses.Count, budgetItems.Count, "correct number of budget items");
            foreach (Expense expense in listExpenses)
            {
                BudgetItem budgetItem = budgetItems.Find(b => b.ExpenseId == expense.Id);
                Category category = listCategories.Find(c => c.Id == expense.CategoryId);
                Assert.AreEqual(budgetItem.CategoryDescription, category.Description, "Category description ok");
                Assert.AreEqual(budgetItem.CategoryId, expense.CategoryId, "Category id is ok");
                Assert.AreEqual(budgetItem.Amount, expense.Amount, "Amount is ok");
                Assert.AreEqual(budgetItem.ExpenseDescription, expense.Description, "Expense description ok");
            }
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

