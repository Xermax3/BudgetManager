using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;

namespace AppDevGUI_bhm
{
    public class DataPresenter
    {
        private IDataView DataView { get; set; }
        public HomeBudget Budget { get; set; }
        private List<string> _categoryNames;
        private DateTime? _firstRecordedDate;
        private DateTime? _lastRecordedDate;

        public DataPresenter(IDataView dataView, string dbPath, bool isNewDB, bool defaultCats)
        {
            DataView = dataView;
            Budget = new HomeBudget(dbPath, isNewDB, defaultCats);
            _categoryNames = new List<string>(Budget.Categories.GetList().Count);

            // get category names
            GetCategoryNames();
        }

        private List<string> GetCategoryNames()
        {
            List<Category> categories = Budget.Categories.GetList();
            // if the list was changed
            if (_categoryNames.Count != categories.Count)
            {
                // update the name list
                _categoryNames.Clear();
                foreach (Category cat in categories)
                {
                    _categoryNames.Add(cat.Description);
                }
            }

            return _categoryNames;
        }

        public void OnDatabaseOpened()
        {
            DataView.InitializeMainWindow(Budget);
        }

        public void DisplayExpensesMenuItem_OnChecked()
        {
            if (DataView.ExpensesAreBeingDisplayed())
            {
                return;
            }

            DataView.UncheckDisplayCategoriesMenuItem();
            DataView.ShowDisplayExpensesOptions();
            DataView.ShowSearchBox();
            DataView.ProcessChosenDisplayExpensesOption();
        }

        public void DisplayExpensesMenuItem_OnUnchecked()
        {
            if (!DataView.DisplayCategoriesMenuItemIsChecked())
            {
                DataView.CheckDisplayCategoriesMenuItem();
            }
            else
            {
                DataView.HideDisplayExpensesOptions();
                DataView.HideSearchBox();
            }
        }

        public List<object> GetStandardData(DateTime? start, DateTime? end, int currentTargetCatId)
        {
            return Budget.GetBudgetItems(start, end, currentTargetCatId != -1, currentTargetCatId).Cast<object>().ToList(); ;
        }

        public List<object> GetByMonthData(DateTime? start, DateTime? end, int currentTargetCatId)
        {
            return Budget.GetBudgetItemsByMonth(start, end, currentTargetCatId != -1, currentTargetCatId).Cast<object>().ToList(); ;
        }

        public List<object> GetByCategoryData(DateTime? start, DateTime? end, int currentTargetCatId)
        {
            return Budget.GetBudgetItemsByCategory(start, end, currentTargetCatId != -1, currentTargetCatId).Cast<object>().ToList(); ;
        }

        public List<object> GetByCategoryAndMonthData(int currentTargetCatId)
        {
            return Budget.GetBudgetDictionaryByCategoryAndMonth(null, null, currentTargetCatId != -1, currentTargetCatId).Cast<object>().ToList();
        }

        public List<BudgetItem> ChooseStartEndDatesForShowAll()
        {
            int currentTargetCatId = DataView.GetCurrentTargetCategoryId();
            List<BudgetItem> budgetItems = Budget.GetBudgetItems(null, null, currentTargetCatId != -1, currentTargetCatId);

            if (budgetItems.Count > 0)
            {
                _firstRecordedDate = budgetItems.Min(x => x.Date); // earliest date
                _lastRecordedDate = budgetItems.Max(x => x.Date); // latest date
            }
            else
            {
                _firstRecordedDate = null;
                _lastRecordedDate = null;
            }

            DataView.ChangeDisplayedDateInDatePicker(_firstRecordedDate, _lastRecordedDate);
            
            return budgetItems;
        }

        public void OnReloadExpenseItemList()
        {
            DataView.ResetKeyword();

            if (DataView.IsStandardDisplay())
            {
                SetStandardDataSource();
                DataView.InitializeStandardDisplay();
            }
            else if (DataView.IsByMonthDisplay())
            {
                SetByMonthDataSource();
                DataView.InitializeByMonthDisplay();
            }
            else if (DataView.IsByCategoryDisplay())
            {
                SetByCategoryDataSource();
                DataView.InitializeByCategoryDisplay();
            }
            else
            {
                SetByCategoryAndMonthDataSource();
                DataView.InitializeByCategoryAndMonthDisplay(GetCategoryNames());
            }
        }

        private void SetStandardDataSource()
        {
            if (DataView.IsShowingAll())
            {
                DataView.DataSource = ChooseStartEndDatesForShowAll().Cast<object>().ToList();
            }
            else
            {
                DataView.DataSource = GetStandardData(DataView.GetCurrentDisplayedStartDate(), DataView.GetCurrentDisplayedEndDate(), DataView.GetCurrentTargetCategoryId());
            }
        }

        public void OnStandardDisplay()
        {
            if (DataView.IsStandardDisplay())
            {
                return;
            }

            DataView.TurnOffCurrentDisplayMode();
            DataView.DataClear();

            SetStandardDataSource();

            DataView.SetupStandardDisplay();
            DataView.InitializeStandardDisplay();
        }

        private void SetByMonthDataSource()
        {
            DateTime chosenDate = (DateTime)DataView.GetSelectedDate();
            DateTime firstDateOfChosenMonth = new DateTime(chosenDate.Year, chosenDate.Month, 1);
            DateTime lastDateOfChosenMonth = firstDateOfChosenMonth.AddMonths(1).AddDays(-1);
            DataView.SetCurrentDisplayedMonth(chosenDate.Month);

            DataView.DataSource = GetByMonthData(firstDateOfChosenMonth, lastDateOfChosenMonth, DataView.GetCurrentTargetCategoryId());
        }

        public void OnByMonthDisplay()
        {
            if (DataView.IsByMonthDisplay())
            {
                return;
            }

            DataView.TurnOffCurrentDisplayMode();
            DataView.DataClear();

            SetByMonthDataSource();

            DataView.SetupByMonthDisplay();
            DataView.InitializeByMonthDisplay();
        }

        private void SetByCategoryDataSource()
        {
            if (DataView.IsShowingAll())
            {
                ChooseStartEndDatesForShowAll();

                if (_firstRecordedDate != null || _lastRecordedDate != null)
                {
                    DataView.DataSource = GetByCategoryData(_firstRecordedDate, _lastRecordedDate, DataView.GetCurrentTargetCategoryId());
                }
                else
                {
                    DataView.DataSource = null;
                }
            }
            else
            {
                DataView.DataSource = GetByCategoryData(DataView.GetCurrentDisplayedStartDate(), DataView.GetCurrentDisplayedEndDate(), DataView.GetCurrentTargetCategoryId());
            }
        }

        public void OnByCategoryDisplay()
        {
            if (DataView.IsByCategoryDisplay())
            {
                return;
            }

            DataView.TurnOffCurrentDisplayMode();
            DataView.DataClear();

            SetByCategoryDataSource();

            DataView.SetupByCategoryDisplay();
            DataView.InitializeByCategoryDisplay();
        }

        private void SetByCategoryAndMonthDataSource()
        {
            DataView.DataSource = GetByCategoryAndMonthData(DataView.GetCurrentTargetCategoryId());
        }

        public void OnByCategoryAndMonthDisplay()
        {
            if (DataView.IsByCategoryAndMonthDisplay())
            {
                return;
            }

            DataView.TurnOffCurrentDisplayMode();
            DataView.DataClear();

            SetByCategoryAndMonthDataSource();
            
            DataView.SetupByCategoryAndMonthDisplay();
            DataView.InitializeByCategoryAndMonthDisplay(GetCategoryNames());
        }

        public void AddCategoryButton_OnClicked()
        {
            DataView.ShowNewCategoryForm(Budget);
        }

        public void AddExpenseButton_OnClicked()
        {
            DataView.ShowNewExpenseForm(Budget);
        }

        public void BudgetButton_OnDoubleClicked(int expId)
        {
            DataView.ShowExpenseFullDetails(expId, Budget);
        }

        public void DeleteButton_OnClicked(int expId)
        {
            Expense exp = Budget.Expenses.GetExpenseById(expId);

            // display a warning first
            bool result = DataView.ShowDeleteWarning(exp.Description);

            // only delete if user confirms
            if (result)
            {
                Budget.Expenses.Delete(expId);

                DataView.ReloadItemList();
            }
        }
    }
}
