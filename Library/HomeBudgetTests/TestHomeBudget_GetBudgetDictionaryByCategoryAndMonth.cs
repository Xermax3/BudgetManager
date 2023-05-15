using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Dynamic;

namespace BudgetCodeTests
{
    [TestClass]
    public class TestHomeBudget_GetBudgetDictionaryByCategoryAndMonth
    {
        string testInputFile = TestConstants.testDBInputFile;

        // ========================================================================
        // Get Expenses By Month Method tests
        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetDictionaryByCategoryAndMonth_NoStartEnd_NoFilter_VerifyNumberOfRecords()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);

            int maxRecords = TestConstants.budgetItemsByCategoryAndMonth_MaxRecords;
            Dictionary<string, object> firstRecord = TestConstants.getBudgetItemsByCategoryAndMonthFirstRecord();

            // Act
            List<Dictionary<string, object>> budgetItemsByCategoryAndMonth = homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 9);

            // Assert
            Assert.AreEqual(maxRecords + 1, budgetItemsByCategoryAndMonth.Count, "All records plus TOTALS are accounted for");
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetDictionaryByCategoryAndMonth_NoStartEnd_NoFilter_VerifyFirstRecord()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);

            int maxRecords = TestConstants.budgetItemsByCategoryAndMonth_MaxRecords;
            Dictionary<string, object> firstRecord = TestConstants.getBudgetItemsByCategoryAndMonthFirstRecord();

            // Act
            List<Dictionary<string, object>> budgetItemsByCategoryAndMonth = homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 9);
            Dictionary<string, object> firstRecordTest = budgetItemsByCategoryAndMonth[0];

            // Assert
            Assert.IsTrue(AssertDictionaryForExpenseByCategoryAndMonthIsOK(firstRecord, firstRecordTest));
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetDictionaryByCategoryAndMonth_NoStartEnd_NoFilter_VerifyTotalsRecord()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);

            int maxRecords = TestConstants.budgetItemsByCategoryAndMonth_MaxRecords;
            Dictionary<string, object> totalsRecord = TestConstants.getBudgetItemsByCategoryAndMonthTotalsRecord();

            // Act
            List<Dictionary<string, object>> budgetItemsByCategoryAndMonth = homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 9);
            Dictionary<string, object> totalsRecordTest = budgetItemsByCategoryAndMonth[budgetItemsByCategoryAndMonth.Count - 1];

            // Assert
            // ... loop over all key/value pairs 
            Assert.IsTrue(AssertDictionaryForExpenseByCategoryAndMonthIsOK(totalsRecord, totalsRecordTest), "Totals Record is Valid");
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetDictionaryByCategoryAndMonth_NoStartEnd_FilterbyCategory()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);
            List<Dictionary<string, object>> expectedResults = TestConstants.getBudgetItemsByCategoryAndMonthCat10();

            // Act
            List<Dictionary<string, object>> gotResults = homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, true, 10);

            // Assert
            Assert.AreEqual(expectedResults.Count, gotResults.Count, "correct number of budget items for cat 10");
            for (int record = 0; record < expectedResults.Count; record++)
            {
                Assert.IsTrue(AssertDictionaryForExpenseByCategoryAndMonthIsOK(expectedResults[record],
                    gotResults[record]), "Record:" + record + " is Valid");
            }
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetDictionaryByCategoryAndMonth_2020()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);
            List<Dictionary<string, object>> expectedResults = TestConstants.getBudgetItemsByCategoryAndMonth2020();

            // Act
            List<Dictionary<string, object>> gotResults = homeBudget.GetBudgetDictionaryByCategoryAndMonth(new DateTime(2020, 1, 1), new DateTime(2020, 12, 31), false, 10);

            // Assert
            Assert.AreEqual(expectedResults.Count, gotResults.Count, "correct number of budget items for cat 10");
            for (int record = 0; record < expectedResults.Count; record++)
            {
                Assert.IsTrue(AssertDictionaryForExpenseByCategoryAndMonthIsOK(expectedResults[record],
                    gotResults[record]), "Record:" + record + " is Valid");
            }
        }

        [TestMethod]
        public void randomTest()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(inFile, false, false);
            List<Dictionary<string, object>> expectedResults = TestConstants.getBudgetItemsByCategoryAndMonthCat10();

            // Act
            List<Dictionary<string, object>> gotResults = homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, -1);
            List<Dictionary<string, object>> gotResults2 = homeBudget.GetBudgetDictionaryByCategoryAndMonthCategoriesFirst(null, null, false, -1);
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

        Boolean AssertDictionaryForExpenseByCategoryAndMonthIsOK(Dictionary<string, object> recordExpeted, Dictionary<string, object> recordGot)
        {
            try
            {
                foreach (var kvp in recordExpeted)
                {
                    String key = kvp.Key as String;
                    Object recordExpectedValue = kvp.Value;
                    Object recordGotValue = recordGot[key];

                    // ... validate the budget items
                    if (recordExpectedValue != null && recordExpectedValue.GetType() == typeof(List<BudgetItem>))
                    {
                        List<BudgetItem> expectedItems = recordExpectedValue as List<BudgetItem>;
                        List<BudgetItem> gotItems = recordGotValue as List<BudgetItem>;
                        for (int budgetItemNumber = 0; budgetItemNumber < expectedItems.Count; budgetItemNumber++)
                        {
                            Assert.AreEqual(expectedItems[budgetItemNumber].Amount, gotItems[budgetItemNumber].Amount,
                                "Item:" + budgetItemNumber + " key:" + kvp.Key + ", Amount ok");
                            Assert.AreEqual(expectedItems[budgetItemNumber].CategoryId, gotItems[budgetItemNumber].CategoryId,
                                "Item:" + budgetItemNumber + " key:" + kvp.Key + ", Category ID ok");
                            Assert.AreEqual(expectedItems[budgetItemNumber].ExpenseId, gotItems[budgetItemNumber].ExpenseId,
                                "Item:" + budgetItemNumber + " key:" + kvp.Key + ", Expense ID ok");
                        }
                    }

                    // else ... validate the value for the specified key
                    else
                    {
                        Assert.AreEqual(recordExpectedValue, recordGotValue, "Key:" + key + " is OK");
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

