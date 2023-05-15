using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingAppDevGUI_bhm
{
    class TestConstants
    {
        public static string TestBudgetDB = "TestBudget.db";
        public static string TestBudgetEmptyDB = "TestBudgetEmpty.db";
        public static int currentTargetCategoryId = 1;
        public static int expenseId = 1;
        public static int numOfCategories = 16;
        public static int numOfExpenses = 4;
        public static DateTime firstDate = new DateTime(2021, 04, 14);
        public static DateTime lastDate = new DateTime(2021, 05, 05);
        static public String GetSolutionDir()
        {
            // this is valid for C# .Net Foundation (not for C# .Net Core)
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"));
        }
    }
}
