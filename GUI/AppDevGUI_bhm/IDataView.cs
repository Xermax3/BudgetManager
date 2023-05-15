using System;
using System.Collections.Generic;
using Budget;

namespace AppDevGUI_bhm
{
    public interface IDataView
    {
        DataPresenter Presenter { get; set; }
        List<object> DataSource { get; set; }

        void InitializeMainWindow(HomeBudget budget);

        // get the display states
        bool ExpensesAreBeingDisplayed();
        bool IsStandardDisplay();
        bool IsByMonthDisplay();
        bool IsByCategoryDisplay();
        bool IsByCategoryAndMonthDisplay();
        bool IsShowingAll();
        int GetCurrentTargetCategoryId();
        DateTime? GetCurrentDisplayedStartDate();
        DateTime? GetCurrentDisplayedEndDate();
        DateTime? GetSelectedDate();
        bool DisplayCategoriesMenuItemIsChecked();
        bool DisplayExpensesMenuItemIsChecked();
        bool ItemListIsEmpty();

        // set display states
        void SetCurrentDisplayedMonth(int month);

        // pre-display
        void TurnOffCurrentDisplayMode();
        void CheckDisplayCategoriesMenuItem();
        void UncheckDisplayCategoriesMenuItem();
        void ShowDisplayExpensesOptions();
        void HideDisplayExpensesOptions();
        void ResetKeyword();
        void ShowSearchBox();
        void HideSearchBox();
        void ProcessChosenDisplayExpensesOption();
        void DataClear();

        // display
        void ReloadItemList();

        void SetupStandardDisplay();
        void InitializeStandardDisplay();

        void SetupByMonthDisplay();
        void InitializeByMonthDisplay();

        void SetupByCategoryDisplay();
        void InitializeByCategoryDisplay();

        void SetupByCategoryAndMonthDisplay();
        void InitializeByCategoryAndMonthDisplay(List<string> usedCategoryList);

        void ChangeDisplayedDateInDatePicker(DateTime? firstDate, DateTime? lastDate);

        void ShowNewCategoryForm(HomeBudget budget);
        void ShowNewExpenseForm(HomeBudget budget);

        void ShowExpenseFullDetails(int expId, HomeBudget budget);

        bool ShowDeleteWarning(string expDescription);

        void ResetFocusAfterUpdate(int itemIndex);
    }
}
