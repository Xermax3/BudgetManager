using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Budget;

namespace BudgetCodeTests
{
    [TestClass]
    public class TestCategory
    {
        // ========================================================================

        [TestMethod]
        public void CategoryObject_New()
        {
            // Arrange
            string descr = "Clothing";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.Credit;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.IsInstanceOfType(category, typeof(Category));
            Assert.AreEqual(id, category.Id);
            Assert.AreEqual(descr, category.Description);
            Assert.AreEqual(type, category.Type);
        }

        // ========================================================================

        [TestMethod]
        public void CategoryObjectGetSetProperties()
        {
            // Arrange
            string descr = "Clothing";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.Credit;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.IsInstanceOfType(category, typeof(Category));
            Assert.IsTrue(typeof(Category).GetProperty("Id").GetSetMethod(true).IsPublic == false);
            Assert.IsTrue(typeof(Category).GetProperty("Description").CanWrite == true);
            Assert.IsTrue(typeof(Category).GetProperty("Type").CanWrite == true);
        }

        // ========================================================================

        [TestMethod]
        public void CategoryObject_New_WithDefaultType()
        {

            // Arrange
            string descr = "Clothing";
            int id = 42;
            Category.CategoryType defaultType = Category.CategoryType.Expense;

            // Act
            Category category = new Category(id, descr);

            // Assert 
            Assert.AreEqual(defaultType, category.Type);
        }

        // ========================================================================

        [TestMethod]
        public void CategoryObject_New_TypeIncome()
        {

            // Arrange
            string descr = "Work";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.Income;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.AreEqual(type, category.Type);

        }

        // ========================================================================

        [TestMethod]
        public void CategoryObjectType_New_Expense()
        {

            // Arrange
            string descr = "Eating Out";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.Expense;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.AreEqual(type, category.Type);

        }

        // ========================================================================

        [TestMethod]
        public void CategoryObject_New_TypeCredit()
        {
            // Arrange
            string descr = "MasterCard";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.Credit;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.AreEqual(type, category.Type);
        }

        // ========================================================================

        [TestMethod]
        public void CategoryObject_New_TypeSavings()
        {
            // Arrange
            string descr = "For House";
            int id = 42;
            Category.CategoryType type = Category.CategoryType.Savings;

            // Act
            Category category = new Category(id, descr, type);

            // Assert 
            Assert.AreEqual(type, category.Type);

        }

        // ========================================================================

        [TestMethod]
        public void CategoryObject_ToString()
        {
            // Arrange
            string descr = "Eating Out";
            int id = 1;

            // Act
            Category category = new Category(id, descr);

            // Assert 
            Assert.AreEqual("Income: " + descr, category.ToString());
        }
    }
}

