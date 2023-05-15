using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;

namespace Budget
{
    /// <summary>
    /// Represents the <see cref="HomeBudget"/>, including a list of <see cref="Budget.Categories"/> and a list of <see cref="Budget.Expenses"/>.
    /// </summary>
    public class HomeBudget
    {
        private readonly SQLiteConnection _connection;
        private readonly Categories _categories;
        private readonly Expenses _expenses;

        /// <summary>
        /// Initializes a new instance of <see cref="HomeBudget"/> class with a given database file.
        /// </summary>
        /// <param name="databaseFile">The database file.</param>
        /// <param name="isNewDB">Indicates whether to create a new database (with default data) or to open an existing database.</param>
        /// <param name="defaultCats">Indicates whether to create default categories or not.</param>
        public HomeBudget(string databaseFile, bool isNewDB, bool defaultCats)
        {
            if (isNewDB)
            {
                _connection = Database.CreateNewDatabase(databaseFile);
            }
            else
            {
                _connection = Database.OpenExistingDatabase(databaseFile);
            }

            _categories = new Categories(_connection, isNewDB, defaultCats);
            _expenses = new Expenses(_connection, isNewDB);
        }

        /// <summary>
        /// Returns the list of <see cref="Budget.Categories"/>.
        /// </summary>
        public Categories Categories
        {
            get
            {
                return _categories;
            }
        }

        /// <summary>
        /// Returns the list of <see cref="Budget.Expenses"/>.
        /// </summary>
        public Expenses Expenses
        {
            get
            {
                return _expenses;
            }
        }

        #region GetList
        /// <summary>
        /// Returns a <see cref="List{T}"/> of <see cref="BudgetItem"/> within a time period.
        /// </summary>
        /// <param name="start">Starting date.</param>
        /// <param name="end">Ending date.</param>
        /// <param name="filterFlag">Indicates whether unwanted categories are filtered out.</param>
        /// <param name="targetCategoryId">The id of the wanted category.</param>
        /// <returns>The <see cref="List{T}"/> of <see cref="BudgetItem"/>.</returns>
        /// <exception cref="Exception"></exception>
        public List<BudgetItem> GetBudgetItems(DateTime? start, DateTime? end, bool filterFlag, int targetCategoryId)
        {
            start = start ?? new DateTime(1900, 1, 1);
            end = end ?? new DateTime(2500, 1, 1);

            try
            {
                string sql;
                SQLiteCommand command = new SQLiteCommand(_connection);

                if (filterFlag)
                {
                    sql = "SELECT CategoryId, e.Id, Date, c.Description, e.Description, e.Amount FROM categories as c, expenses as e WHERE c.Id = @targetCatId AND c.Id = CategoryId AND Date >= @start AND Date <= @end ORDER BY Date";
                    command.Parameters.AddWithValue("@targetCatId", targetCategoryId);
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                }
                else
                {
                    sql = "SELECT CategoryId, e.Id, Date, c.Description, e.Description, e.Amount FROM categories as c, expenses as e WHERE c.Id = CategoryId AND Date >= @start AND Date <= @end ORDER BY Date";
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                }

                command.Prepare();
                command.CommandText = sql;

                List<BudgetItem> list = new List<BudgetItem>();

                double total = 0;
                // Execute the sql and populate the list with the retrieved expenses
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        total += reader.GetDouble(5);
                        list.Add(new BudgetItem
                        {
                            CategoryId = reader.GetInt32(0),
                            ExpenseId = reader.GetInt32(1),
                            Date = reader.GetDateTime(2),
                            CategoryDescription = reader.GetString(3),
                            ExpenseDescription = reader.GetString(4),
                            Amount = reader.GetDouble(5),
                            Balance = total
                        });
                    }
                }

                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Returns a <see cref="List{T}"/> of <see cref="BudgetItemsByMonth"/> (budget items grouped by month) within a time period.
        /// </summary>
        /// <param name="start">Starting date.</param>
        /// <param name="end">Ending date.</param>
        /// <param name="filterFlag">Indicates whether unwanted categories are filtered out.</param>
        /// <param name="targetCategoryId">The id of the wanted category.</param>
        /// <returns>The <see cref="List{T}"/> of <see cref="BudgetItemsByMonth"/>.</returns>
        /// <exception cref="Exception"></exception>
        public List<BudgetItemsByMonth> GetBudgetItemsByMonth(DateTime? start, DateTime? end, bool filterFlag, int targetCategoryId)
        {
            start = start ?? new DateTime(1900, 1, 1);
            end = end ?? new DateTime(2500, 1, 1);

            try
            {
                string sql;
                SQLiteCommand command = new SQLiteCommand(_connection);

                if (filterFlag)
                {
                    sql = "SELECT strftime('%Y', e.Date) as 'year', strftime('%m', e.Date) as 'month', SUM(e.Amount) FROM categories as c, expenses as e WHERE c.Id = @targetCatId AND c.Id = e.CategoryId AND e.Date >= @start AND e.Date <= @end " +
                    "GROUP BY `month` ORDER BY `year`, `month`";
                    command.Parameters.AddWithValue("@targetCatId", targetCategoryId);
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                }
                else
                {
                    sql = "SELECT strftime('%Y', e.Date) as 'year', strftime('%m', e.Date) as 'month', SUM(e.Amount) FROM categories as c, expenses as e WHERE c.Id = e.CategoryId AND e.Date >= @start AND e.Date <= @end " +
                    "GROUP BY `month` ORDER BY `year`, `month`";
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                }

                command.Prepare();
                command.CommandText = sql;

                List<BudgetItemsByMonth> list = new List<BudgetItemsByMonth>();

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int year = int.Parse(reader.GetString(0));
                        int month = int.Parse(reader.GetString(1));
                        DateTime startDate = new DateTime(year, month, 1);
                        int lastDay = DateTime.DaysInMonth(year, month);
                        DateTime endDate = new DateTime(year, month, lastDay);

                        list.Add(new BudgetItemsByMonth
                        {
                            Month = reader.GetString(1) + '/' + reader.GetString(0),
                            Details = GetBudgetItems(startDate, endDate, filterFlag, targetCategoryId),
                            Total = reader.GetDouble(2)
                        });
                    }
                }

                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Returns a <see cref="List{T}"/> of <see cref="BudgetItemsByCategory"/> (budget items grouped by category) within a time period.
        /// </summary>
        /// <param name="start">Starting date.</param>
        /// <param name="end">Ending date.</param>
        /// <param name="filterFlag">Indicates whether unwanted categories are filtered out.</param>
        /// <param name="targetCategoryId">The id of the wanted category.</param>
        /// <returns>The <see cref="List{T}"/> of <see cref="BudgetItemsByCategory"/>.</returns>
        /// <exception cref="Exception"></exception>
        public List<BudgetItemsByCategory> GetBudgetItemsByCategory(DateTime? start, DateTime? end, bool filterFlag, int targetCategoryId)
        {
            start = start ?? new DateTime(1900, 1, 1);
            end = end ?? new DateTime(2500, 1, 1);

            try
            {
                string sql;
                SQLiteCommand command = new SQLiteCommand(_connection);

                if (filterFlag)
                {
                    sql = "SELECT c.Description, c.Id, SUM(e.Amount) FROM categories as c, expenses as e WHERE c.Id = @targetCatId AND c.Id = e.CategoryId AND e.Date >= @start AND e.Date <= @end " +
                    "GROUP BY c.Description ORDER BY c.Description";
                    command.Parameters.AddWithValue("@targetCatId", targetCategoryId);
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                }
                else
                {
                    sql = "SELECT c.Description, c.Id, SUM(e.Amount) FROM categories as c, expenses as e WHERE c.Id = e.CategoryId AND e.Date >= @start AND e.Date <= @end " +
                    "GROUP BY c.Description ORDER BY c.Description, e.Date";
                    command.Parameters.AddWithValue("@start", start);
                    command.Parameters.AddWithValue("@end", end);
                }

                command.Prepare();
                command.CommandText = sql;

                List<BudgetItemsByCategory> list = new List<BudgetItemsByCategory>();

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BudgetItemsByCategory
                        {
                            CategoryDescription = reader.GetString(0),
                            Details = GetBudgetItems(start, end, true, reader.GetInt32(1)),
                            Total = reader.GetDouble(2)
                        });
                    }
                }

                return list;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Gets a list of budget items grouped by month and category.
        /// </summary>
        /// <param name="Start">Starting date</param>
        /// <param name="End">Ending date</param>
        /// <param name="FilterFlag">If filter is on, unwanted categories are ignored</param>
        /// <param name="CategoryID">ID of the wanted category</param>
        /// <returns> The list of expenses grouped by month and category. </returns>
        public List<Dictionary<string, object>> GetBudgetDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<BudgetItemsByMonth> GroupedByMonth = GetBudgetItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalsPerCategory = new Dictionary<string, double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>
                {
                    ["Month"] = MonthGroup.Month,
                    ["Total"] = MonthGroup.Total
                };

                // break up the month details into categories
                var GroupedByCategory = MonthGroup.Details.GroupBy(c => c.CategoryDescription);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of details
                    double total = 0;
                    var details = new List<BudgetItem>();

                    foreach (var item in CategoryGroup)
                    {
                        total += item.Amount;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["details:" + CategoryGroup.Key] = details;
                    record[CategoryGroup.Key] = total;

                    // keep track of totals for each category
                    if (totalsPerCategory.TryGetValue(CategoryGroup.Key, out double CurrentCatTotal))
                    {
                        totalsPerCategory[CategoryGroup.Key] = CurrentCatTotal + total;
                    }
                    else
                    {
                        totalsPerCategory[CategoryGroup.Key] = total;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>
            {
                ["Month"] = "ALL MONTHS"
            };

            double grandTotal = 0;
            foreach (var cat in _categories.GetList())
            {
                if (totalsPerCategory.ContainsKey(cat.Description))
                {
                    grandTotal += totalsPerCategory[cat.Description];
                    totalsRecord.Add(cat.Description, totalsPerCategory[cat.Description]);
                }
                else
                {
                    totalsRecord.Add(cat.Description, 0);
                }
            }
            totalsRecord.Add("Total", grandTotal);

            summary.Add(totalsRecord);

            return summary;
        }

        /// <summary>
        /// Behaves like GetBudgetDictionaryByCategoryAndMonth, but instead months are grouped within categories.
        /// </summary>
        /// <param name="Start">Starting date</param>
        /// <param name="End">Ending date</param>
        /// <param name="FilterFlag">If filter is on, unwanted categories are ignored</param>
        /// <param name="CategoryID">ID of the wanted category</param>
        /// <returns> The list of expenses grouped by category and month. </returns>
        public List<Dictionary<string, object>> GetBudgetDictionaryByCategoryAndMonthCategoriesFirst(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by categories 
            // -----------------------------------------------------------------------
            List<BudgetItemsByCategory> GroupedByCategories = GetBudgetItemsByCategory(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each category
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalsPerMonth = new Dictionary<string, double>();

            foreach (var CategoryGroup in GroupedByCategories)
            {
                // create record object for this category
                Dictionary<string, object> record = new Dictionary<string, object>
                {
                    ["Category"] = CategoryGroup.CategoryDescription,
                    ["Total"] = CategoryGroup.Total
                };

                // break up the categories details into months
                var GroupedByMonth = CategoryGroup.Details.GroupBy(c => c.Date.Year + "/" + c.Date.Month);

                // -----------------------------------------------------------------------
                // loop over each month
                // -----------------------------------------------------------------------
                foreach (var MonthGroup in GroupedByMonth.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of details
                    double total = 0;
                    var details = new List<BudgetItem>();

                    foreach (var item in MonthGroup)
                    {
                        total += item.Amount;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["details:" + MonthGroup.Key] = details;
                    record[MonthGroup.Key.ToString()] = total;

                    // keep track of totals for each category
                    if (totalsPerMonth.TryGetValue(MonthGroup.Key.ToString(), out double CurrentMonthTotal))
                    {
                        totalsPerMonth[MonthGroup.Key.ToString()] = CurrentMonthTotal + total;
                    }
                    else
                    {
                        totalsPerMonth[MonthGroup.Key.ToString()] = total;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each month
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>
            {
                ["Category"] = "ALL CATEGORIES"
            };

            double grandTotal = 0;
            foreach (var yearmonth in GetBudgetItems(null, null, false, -1).Select(x => x.Date.Year + "/" + x.Date.Month).ToList())
            {
                if (totalsPerMonth.ContainsKey(yearmonth))
                {
                    grandTotal += totalsPerMonth[yearmonth];
                    totalsRecord.Add(yearmonth, totalsPerMonth[yearmonth]); // causes error because duplicate...
                }
                else
                {
                    totalsRecord.Add(yearmonth, 0);
                }
            }
            totalsRecord.Add("Total", grandTotal);

            summary.Add(totalsRecord);

            return summary;
        }
        #endregion GetList
    }
}