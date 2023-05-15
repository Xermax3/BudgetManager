using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Budget;

namespace BudgetCodeTests
{
    [TestClass]
    public class TestExpense
    {
        // ========================================================================

        [TestMethod]
        public void ExpenseObject_New()
        {
            // Arrange
            DateTime now = DateTime.Now;
            double amount = 24.55;
            string descr = "New Sweater";
            int category = 34;
            int id = 42;

            // Act
            Expense expense = new Expense(id, now, category, amount, descr);

            // Assert 
            Assert.IsInstanceOfType(expense, typeof(Expense));

            Assert.AreEqual(id, expense.Id);
            Assert.AreEqual(amount, expense.Amount);
            Assert.AreEqual(descr, expense.Description);
            Assert.AreEqual(category, expense.CategoryId);
            Assert.AreEqual(now, expense.Date);
        }

        // ========================================================================

        [TestMethod]
        public void ExpenseCopyConstructoryIsDeepCopy()
        {
            // Arrange
            DateTime now = DateTime.Now;
            double amount = 24.55;
            string descr = "New Sweater";
            int category = 34;
            int id = 42;
            Expense expense = new Expense(id, now, category, amount, descr);

            // Act
            Expense copy = new Expense(expense);
            copy.Amount = expense.Amount + 15;

            // Assert 
            Assert.AreEqual(id, expense.Id);
            Assert.AreNotEqual(amount, copy.Amount);
            Assert.AreEqual(expense.Amount + 15, copy.Amount);
            Assert.AreEqual(descr, expense.Description);
            Assert.AreEqual(category, expense.CategoryId);
            Assert.AreEqual(now, expense.Date);
        }

        // ========================================================================

        [TestMethod]
        public void ExpenseObjectGetSetProperties()
        {
            // question - why cannot I not change the date of an expense.  What if I got the date wrong?

            // Arrange
            DateTime now = DateTime.Now;
            double amount = 24.55;
            string descr = "New Sweater";
            int category = 34;
            int id = 42;
            double newAmount = 54.55;
            string newDescr = "Angora Sweater";
            int newCategory = 38;

            Expense expense = new Expense(id, now, category, amount, descr);

            // Act
            expense.Amount = newAmount;
            expense.CategoryId = newCategory;
            expense.Description = newDescr;

            // Assert 
            Assert.IsTrue(typeof(Expense).GetProperty("Date").CanWrite == true);
            Assert.IsTrue(typeof(Expense).GetProperty("Id").GetSetMethod(true).IsPublic == false);
            Assert.AreEqual(newAmount, expense.Amount);
            Assert.AreEqual(newDescr, expense.Description);
            Assert.AreEqual(newCategory, expense.CategoryId);
        }

        [TestMethod]
        public void ExpenseObjectDatePropertyModifiable()
        {
            // fixed bug where date was not modifiable

            // Arrange
            DateTime now = new DateTime(2021, 2, 1);  // Feb 1, 2021
            double amount = 24.55;
            string descr = "New Sweater";
            int category = 34;
            int id = 42;

            // Act
            Expense expense = new Expense(id, now, category, amount, descr);

            // Assert
            Assert.AreEqual(new DateTime(2021, 2, 1), expense.Date, "Original date has been set");

            // Act
            expense.Date = new DateTime(2021, 2, 14); // Valentine's Day

            // Assert 
            Assert.AreEqual(new DateTime(2021, 2, 14), expense.Date, "Date was successfully changed");
        }
    }
}
