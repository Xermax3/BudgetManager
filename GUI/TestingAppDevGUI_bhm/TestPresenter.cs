using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AppDevGUI_bhm;
using System.Collections.Generic;
using Budget;
using System.IO;

namespace TestingAppDevGUI_bhm
{
    [TestClass]
    public class TestPresenter : IDataView
    {
        bool called_setDataSource;
        bool called_initializeMainWindow;
        bool called_expensesAreBeingDisplayed;
        bool called_isStandardDisplay;
        bool called_isByMonthDisplay;
        bool called_isByCategoryDisplay;
        bool called_isByCategoryAndMonthDisplay;
        bool called_isShowingAll;
        bool called_getCurrentTargetCategoryId;
        bool called_getCurrentDisplayedStartDate;
        bool called_getCurrentDisplayedEndDate;
        bool called_getSelectedDate;
        bool called_displayCategoriesMenuItemIsChecked;
        bool called_setCurrentDisplayedMonth;
        bool called_turnOffCurrentDisplayMode;
        bool called_checkDisplayCategoriesMenuItem;
        bool called_uncheckDisplayCategoriesMenuItem;
        bool called_showDisplayExpensesOptions;
        bool called_hideDisplayExpensesOptions;
        bool called_resetKeyword;
        bool called_showSearchBox;
        bool called_hideSearchBox;
        bool called_processChosenDisplayExpensesOption;
        bool called_dataClear;
        bool called_reloadItemList;
        bool called_setupStandardDisplay;
        bool called_initializeStandardDisplay;
        bool called_setupByMonthDisplay;
        bool called_initializeByMonthDisplay;
        bool called_setupByCategoryDisplay;
        bool called_initializeByCategoryDisplay;
        bool called_setupByCategoryAndMonthDisplay;
        bool called_initializeByCategoryAndMonthDisplay;
        bool called_changeDisplayedDateInDatePicker;
        bool called_showNewCategoryForm;
        bool called_showNewExpenseForm;
        bool called_showExpenseFullDetails;
        bool called_showDeleteWarning;

        bool expensesAreBeingDisplayed;
        bool displayCategoriesMenuItemIsChecked;
        bool isStandardDisplay;
        bool isByMonthDisplay;
        bool isByCategoryDisplay;
        bool isByCategoryAndMonthDisplay;
        bool isShowingAll;
        bool deleteConfirmation;
        int categoryListCount;
        int setMonth;
        string expenseDescription;
        List<object> dataSource;
        DateTime? firstDate;
        DateTime? lastDate;
        Object budget;

        public DataPresenter Presenter { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public List<object> DataSource
        {
            get { throw new NotImplementedException(); }
            set
            {
                called_setDataSource = true;
                dataSource = value;
            }
        }

        public void InitializeMainWindow(HomeBudget budget)
        {
            called_initializeMainWindow = true;
        }

        public bool ExpensesAreBeingDisplayed()
        {
            called_expensesAreBeingDisplayed = true;
            return expensesAreBeingDisplayed;
        }

        public bool IsStandardDisplay()
        {
            called_isStandardDisplay = true;
            return isStandardDisplay;
        }

        public bool IsByMonthDisplay()
        {
            called_isByMonthDisplay = true;
            return isByMonthDisplay;
        }

        public bool IsByCategoryDisplay()
        {
            called_isByCategoryDisplay = true;
            return isByCategoryDisplay;
        }

        public bool IsByCategoryAndMonthDisplay()
        {
            called_isByCategoryAndMonthDisplay = true;
            return isByCategoryAndMonthDisplay;
        }

        public bool IsShowingAll()
        {
            called_isShowingAll = true;
            return isShowingAll;
        }

        public int GetCurrentTargetCategoryId()
        {
            called_getCurrentTargetCategoryId = true;
            return TestConstants.currentTargetCategoryId;
        }

        public DateTime? GetCurrentDisplayedStartDate()
        {
            called_getCurrentDisplayedStartDate = true;
            return TestConstants.firstDate;
        }

        public DateTime? GetCurrentDisplayedEndDate()
        {
            called_getCurrentDisplayedEndDate = true;
            return TestConstants.lastDate;
        }

        public DateTime? GetSelectedDate()
        {
            called_getSelectedDate = true;
            return TestConstants.firstDate;
        }

        public bool DisplayCategoriesMenuItemIsChecked()
        {
            called_displayCategoriesMenuItemIsChecked = true;
            return displayCategoriesMenuItemIsChecked;
        }

        public bool DisplayExpensesMenuItemIsChecked()
        {
            throw new NotImplementedException();
        }

        public bool ItemListIsEmpty()
        {
            throw new NotImplementedException();
        }

        public void SetCurrentDisplayedMonth(int month)
        {
            called_setCurrentDisplayedMonth = true;
            setMonth = month;
        }

        public void TurnOffCurrentDisplayMode()
        {
            called_turnOffCurrentDisplayMode = true;
        }

        public void CheckDisplayCategoriesMenuItem()
        {
            called_checkDisplayCategoriesMenuItem = true;
        }

        public void UncheckDisplayCategoriesMenuItem()
        {
            called_uncheckDisplayCategoriesMenuItem = true;
        }

        public void ShowDisplayExpensesOptions()
        {
            called_showDisplayExpensesOptions = true;
        }

        public void HideDisplayExpensesOptions()
        {
            called_hideDisplayExpensesOptions = true;
        }

        public void ResetKeyword()
        {
            called_resetKeyword = true;
        }

        public void ShowSearchBox()
        {
            called_showSearchBox = true;
        }

        public void HideSearchBox()
        {
            called_hideSearchBox = true;
        }

        public void ProcessChosenDisplayExpensesOption()
        {
            called_processChosenDisplayExpensesOption = true;
        }

        public void DataClear()
        {
            called_dataClear = true;
        }

        public void ReloadItemList()
        {
            called_reloadItemList = true;
        }

        public void SetupStandardDisplay()
        {
            called_setupStandardDisplay = true;
        }

        public void InitializeStandardDisplay()
        {
            called_initializeStandardDisplay = true;
        }

        public void SetupByMonthDisplay()
        {
            called_setupByMonthDisplay = true;
        }

        public void InitializeByMonthDisplay()
        {
            called_initializeByMonthDisplay = true;
        }

        public void SetupByCategoryDisplay()
        {
            called_setupByCategoryDisplay = true;
        }

        public void InitializeByCategoryDisplay()
        {
            called_initializeByCategoryDisplay = true;
        }

        public void SetupByCategoryAndMonthDisplay()
        {
            called_setupByCategoryAndMonthDisplay = true;
        }

        public void InitializeByCategoryAndMonthDisplay(List<string> usedCategoryList)
        {
            called_initializeByCategoryAndMonthDisplay = true;
            categoryListCount = usedCategoryList.Count;
        }

        public void ChangeDisplayedDateInDatePicker(DateTime? firstDate, DateTime? lastDate)
        {
            called_changeDisplayedDateInDatePicker = true;
            this.firstDate = firstDate;
            this.lastDate = lastDate;
        }

        public void ShowNewCategoryForm(HomeBudget budget)
        {
            called_showNewCategoryForm = true;
            this.budget = budget;
        }

        public void ShowNewExpenseForm(HomeBudget budget)
        {
            called_showNewExpenseForm = true;
            this.budget = budget;
        }

        public void ShowExpenseFullDetails(int expId, HomeBudget budget)
        {
            called_showExpenseFullDetails = true;
            this.budget = budget;
        }

        public bool ShowDeleteWarning(string expDescription)
        {
            called_showDeleteWarning = true;
            expenseDescription = expDescription;
            return deleteConfirmation;
        }

        public void ResetFocusAfterUpdate(int itemIndex)
        {
            throw new NotImplementedException();
        }

        [TestInitialize]
        public void ResetAll()
        {
            called_setDataSource = false;
            called_initializeMainWindow = false;
            called_expensesAreBeingDisplayed = false;
            called_isStandardDisplay = false;
            called_isByMonthDisplay = false;
            called_isByCategoryDisplay = false;
            called_isByCategoryAndMonthDisplay = false;
            called_isShowingAll = false;
            called_getCurrentTargetCategoryId = false;
            called_getCurrentDisplayedStartDate = false;
            called_getCurrentDisplayedEndDate = false;
            called_getSelectedDate = false;
            called_displayCategoriesMenuItemIsChecked = false;
            called_setCurrentDisplayedMonth = false;
            called_turnOffCurrentDisplayMode = false;
            called_checkDisplayCategoriesMenuItem = false;
            called_uncheckDisplayCategoriesMenuItem = false;
            called_showDisplayExpensesOptions = false;
            called_hideDisplayExpensesOptions = false;
            called_resetKeyword = false;
            called_showSearchBox = false;
            called_hideSearchBox = false;
            called_processChosenDisplayExpensesOption = false;
            called_dataClear = false;
            called_reloadItemList = false;
            called_setupStandardDisplay = false;
            called_initializeStandardDisplay = false;
            called_setupByMonthDisplay = false;
            called_initializeByMonthDisplay = false;
            called_setupByCategoryDisplay = false;
            called_initializeByCategoryDisplay = false;
            called_setupByCategoryAndMonthDisplay = false;
            called_initializeByCategoryAndMonthDisplay = false;
            called_changeDisplayedDateInDatePicker = false;
            called_showNewCategoryForm = false;
            called_showNewExpenseForm = false;
            called_showExpenseFullDetails = false;
            called_showDeleteWarning = false;
            isStandardDisplay = false;
            isByMonthDisplay = false;
            isByCategoryDisplay = false;
            isByCategoryAndMonthDisplay = false;
            isShowingAll = false;
            deleteConfirmation = false;
        }

        [TestMethod]
        public void InitializeMainWindow_OnDbOpen()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnDatabaseOpened();

            // Assert
            Assert.IsTrue(called_initializeMainWindow, "Main window initialized");
        }

        [TestMethod]
        public void DisplayExpensesMenuItem_OnChecked_ExpensesAreBeingDisplayed()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            expensesAreBeingDisplayed = true;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.DisplayExpensesMenuItem_OnChecked();

            // Assert
            Assert.IsTrue(called_expensesAreBeingDisplayed, "Checked if expenses are being displayed");
            Assert.IsFalse(called_uncheckDisplayCategoriesMenuItem, "Display categories not unchecked");
            Assert.IsFalse(called_showDisplayExpensesOptions, "Display options not shown");
            Assert.IsFalse(called_showSearchBox, "Search box not shown");
            Assert.IsFalse(called_processChosenDisplayExpensesOption, "Chosen display option not processed");
        }

        [TestMethod]
        public void DisplayExpensesMenuItem_OnChecked_ExpensesAreNotBeingDisplayed()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            expensesAreBeingDisplayed = false;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.DisplayExpensesMenuItem_OnChecked();

            // Assert
            Assert.IsTrue(called_expensesAreBeingDisplayed, "Checked if expenses are being displayed");
            Assert.IsTrue(called_uncheckDisplayCategoriesMenuItem, "Display categories unchecked");
            Assert.IsTrue(called_showDisplayExpensesOptions, "Display options shown");
            Assert.IsTrue(called_showSearchBox, "Search box shown");
            Assert.IsTrue(called_processChosenDisplayExpensesOption, "Chosen display option processed");
        }

        [TestMethod]
        public void DisplayExpensesMenuItem_OnUnchecked_isChecked()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            displayCategoriesMenuItemIsChecked = true;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.DisplayExpensesMenuItem_OnUnchecked();

            // Assert
            Assert.IsTrue(called_displayCategoriesMenuItemIsChecked, "Check if display categories is chosen");
            Assert.IsTrue(called_hideDisplayExpensesOptions, "Display options hidden");
            Assert.IsTrue(called_hideSearchBox, "Search box hidden");
            Assert.IsFalse(called_checkDisplayCategoriesMenuItem, "Category menu item not checked");
        }

        [TestMethod]
        public void DisplayExpensesMenuItem_OnUnchecked_isNotChecked()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            displayCategoriesMenuItemIsChecked = false;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.DisplayExpensesMenuItem_OnUnchecked();

            // Assert
            Assert.IsTrue(called_displayCategoriesMenuItemIsChecked, "Check if display categories is chosen");
            Assert.IsFalse(called_hideDisplayExpensesOptions, "Display options not hidden");
            Assert.IsFalse(called_hideSearchBox, "Search box not hidden");
            Assert.IsTrue(called_checkDisplayCategoriesMenuItem, "Category menu item checked");
        }

        [TestMethod]
        public void ShowAllExpensesClicked_DBNotEmpty()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            displayCategoriesMenuItemIsChecked = false;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.ChooseStartEndDatesForShowAll();

            // Assert
            Assert.AreEqual(this.firstDate, TestConstants.firstDate);
            Assert.AreEqual(this.lastDate, TestConstants.lastDate);
            Assert.IsTrue(called_changeDisplayedDateInDatePicker, "Date changed in date picker");
            Assert.IsTrue(called_getCurrentTargetCategoryId, "Retrieved current target category");
        }

        [TestMethod]
        public void ShowAllExpensesClicked_EmptyDB()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetEmptyDB}";
            displayCategoriesMenuItemIsChecked = false;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.ChooseStartEndDatesForShowAll();

            // Assert
            Assert.IsNull(this.firstDate);
            Assert.IsNull(this.lastDate);
            Assert.IsTrue(called_changeDisplayedDateInDatePicker, "Date changed in date picker");
            Assert.IsTrue(called_getCurrentTargetCategoryId, "Retrieved current target category");
        }

        [TestMethod]
        public void OnReloadExpenseItemList_FromStandardDisplay_ShowAllChosen()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isStandardDisplay = true;
            isShowingAll = true;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnReloadExpenseItemList();

            // Assert
            Assert.IsTrue(called_isStandardDisplay, "Check if the chosen display type is standard display");
            Assert.IsFalse(called_isByMonthDisplay, "Check if the chosen display type is by month");
            Assert.IsFalse(called_isByCategoryDisplay, "Check if the chosen display type is by category");
            Assert.IsFalse(called_isByCategoryAndMonthDisplay, "If the chosen display type is by category not checked");
            Assert.IsTrue(called_initializeStandardDisplay, "Standard display initialized");
            Assert.IsFalse(called_initializeByMonthDisplay, "Month display not initialized");
            Assert.IsFalse(called_initializeByCategoryDisplay, "Category display not initialized");
            Assert.IsFalse(called_initializeByCategoryAndMonthDisplay, "Category and month display not initialized");
            Assert.IsTrue(called_resetKeyword, "Keyword reset");
            Assert.IsTrue(called_isShowingAll, "Is showing all retrieved");
            Assert.IsFalse(called_getCurrentDisplayedStartDate, "Start date not retrieved");
            Assert.IsFalse(called_getCurrentDisplayedEndDate, "End date not retrieved");
            Assert.IsTrue(called_getCurrentTargetCategoryId, "Retrieved current category id");
            Assert.IsTrue(called_changeDisplayedDateInDatePicker, "Display date in date picker changed");
            Assert.IsTrue(called_setDataSource, "Data source set");
            Assert.IsNotNull(dataSource, "Set data source value is not null");
        }

        [TestMethod]
        public void OnReloadExpenseItemList_FromStandardDisplay_DatePeriodSpecified()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isStandardDisplay = true;
            isShowingAll = false;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnReloadExpenseItemList();

            // Assert
            Assert.IsTrue(called_isStandardDisplay, "Check if the chosen display type is standard display");
            Assert.IsFalse(called_isByMonthDisplay, "Check if the chosen display type is by month");
            Assert.IsFalse(called_isByCategoryDisplay, "Check if the chosen display type is by category");
            Assert.IsFalse(called_isByCategoryAndMonthDisplay, "If the chosen display type is by category not checked");
            Assert.IsTrue(called_initializeStandardDisplay, "Standard display initialized");
            Assert.IsFalse(called_initializeByMonthDisplay, "Month display not initialized");
            Assert.IsFalse(called_initializeByCategoryDisplay, "Category display not initialized");
            Assert.IsFalse(called_initializeByCategoryAndMonthDisplay, "Category and month display not initialized");
            Assert.IsTrue(called_resetKeyword, "Keyword reset");
            Assert.IsTrue(called_isShowingAll, "Is showing all retrieved");
            Assert.IsTrue(called_getCurrentDisplayedStartDate, "Retrieved start date");
            Assert.IsTrue(called_getCurrentDisplayedEndDate, "Retrieved end date");
            Assert.IsTrue(called_getCurrentTargetCategoryId, "Retrieved current category id");
            Assert.IsFalse(called_changeDisplayedDateInDatePicker, "Display date in date picker not changed");
            Assert.IsTrue(called_setDataSource, "Data source set");
            Assert.IsNotNull(dataSource, "Set data source value is not null");
        }

        [TestMethod]
        public void OnReloadExpenseItemList_FromByMonthDisplay()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isByMonthDisplay = true;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnReloadExpenseItemList();

            // Assert
            Assert.IsTrue(called_isStandardDisplay, "Check if the chosen display type is standard display");
            Assert.IsTrue(called_isByMonthDisplay, "Check if the chosen display type is by month");
            Assert.IsFalse(called_isByCategoryDisplay, "Check if the chosen display type is by category");
            Assert.IsFalse(called_isByCategoryAndMonthDisplay, "If the chosen display type is by category not checked");
            Assert.IsFalse(called_initializeStandardDisplay, "Standard display not initialized");
            Assert.IsTrue(called_initializeByMonthDisplay, "Month display initialized");
            Assert.IsFalse(called_initializeByCategoryDisplay, "Category display not initialized");
            Assert.IsFalse(called_initializeByCategoryAndMonthDisplay, "Category and month display not initialized");
            Assert.IsTrue(called_resetKeyword, "Keyword reset");
            Assert.IsTrue(called_getSelectedDate, "Selected date retrieved");
            Assert.IsTrue(called_setCurrentDisplayedMonth, "Current displayed month set");
            Assert.IsTrue(setMonth > 0 && setMonth <= 12);
            Assert.IsTrue(called_setDataSource, "Data source set");
            Assert.IsNotNull(dataSource, "Set data source value is not null");
        }

        [TestMethod]
        public void OnReloadExpenseItemList_FromByCategoryDisplay_ShowAllChosen()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isByCategoryDisplay = true;
            isShowingAll = true;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnReloadExpenseItemList();

            // Assert
            Assert.IsTrue(called_isStandardDisplay, "Check if the chosen display type is standard display");
            Assert.IsTrue(called_isByMonthDisplay, "Check if the chosen display type is by month");
            Assert.IsTrue(called_isByCategoryDisplay, "Check if the chosen display type is by category");
            Assert.IsFalse(called_isByCategoryAndMonthDisplay, "If the chosen display type is by category not checked");
            Assert.IsFalse(called_initializeStandardDisplay, "Standard display not initialized");
            Assert.IsFalse(called_initializeByMonthDisplay, "Month display not initialized");
            Assert.IsTrue(called_initializeByCategoryDisplay, "Category display initialized");
            Assert.IsFalse(called_initializeByCategoryAndMonthDisplay, "Category and month display not initialized");
            Assert.IsTrue(called_resetKeyword, "Keyword reset");
            Assert.IsTrue(called_isShowingAll, "Is showing all retrieved");
            Assert.IsTrue(called_changeDisplayedDateInDatePicker, "Display date in date picker changed");
            Assert.IsFalse(called_getCurrentDisplayedStartDate, "Start date not retrieved");
            Assert.IsFalse(called_getCurrentDisplayedEndDate, "End date not retrieved");
            Assert.IsTrue(called_getCurrentTargetCategoryId, "Current category id retrieved");
            Assert.IsTrue(called_setDataSource, "Data source set");
            Assert.IsNotNull(dataSource, "Set data source value is not null");
        }

        [TestMethod]
        public void OnReloadExpenseItemList_FromByCategoryDisplay_DatePeriodSpecified()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isByCategoryDisplay = true;
            isShowingAll = false;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnReloadExpenseItemList();

            // Assert
            Assert.IsTrue(called_isStandardDisplay, "Check if the chosen display type is standard display");
            Assert.IsTrue(called_isByMonthDisplay, "Check if the chosen display type is by month");
            Assert.IsTrue(called_isByCategoryDisplay, "Check if the chosen display type is by category");
            Assert.IsFalse(called_isByCategoryAndMonthDisplay, "If the chosen display type is by category not checked");
            Assert.IsFalse(called_initializeStandardDisplay, "Standard display not initialized");
            Assert.IsFalse(called_initializeByMonthDisplay, "Month display not initialized");
            Assert.IsTrue(called_initializeByCategoryDisplay, "Category display initialized");
            Assert.IsFalse(called_initializeByCategoryAndMonthDisplay, "Category and month display not initialized");
            Assert.IsTrue(called_resetKeyword, "Keyword reset");
            Assert.IsTrue(called_isShowingAll, "Is showing all retrieved");
            Assert.IsFalse(called_changeDisplayedDateInDatePicker, "Display date in date picker not changed");
            Assert.IsTrue(called_getCurrentDisplayedStartDate, "Start date retrieved");
            Assert.IsTrue(called_getCurrentDisplayedEndDate, "End date retrieved");
            Assert.IsTrue(called_getCurrentTargetCategoryId, "Current category id retrieved");
            Assert.IsTrue(called_setDataSource, "Data source set");
            Assert.IsNotNull(dataSource, "Set data source value is not null");
        }

        [TestMethod]
        public void OnReloadExpenseItemList_FromByCategoryAndMonthDisplay()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            int categoryCount = TestConstants.numOfCategories;
            isByCategoryAndMonthDisplay = true;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnReloadExpenseItemList();

            // Assert
            Assert.IsTrue(called_isStandardDisplay, "Check if the chosen display type is standard display");
            Assert.IsTrue(called_isByMonthDisplay, "Check if the chosen display type is by month");
            Assert.IsTrue(called_isByCategoryDisplay, "Check if the chosen display type is by category");
            Assert.IsFalse(called_isByCategoryAndMonthDisplay, "If the chosen display type is by category not checked");
            Assert.IsFalse(called_initializeStandardDisplay, "Standard display not initialized");
            Assert.IsFalse(called_initializeByMonthDisplay, "Month display not initialized");
            Assert.IsFalse(called_initializeByCategoryDisplay, "Category display not initialized");
            Assert.IsTrue(called_initializeByCategoryAndMonthDisplay, "Category and month display initialized");
            Assert.IsTrue(called_resetKeyword, "Keyword reset");
            Assert.IsTrue(called_getCurrentTargetCategoryId, "Current category id retrieved");
            Assert.IsTrue(called_setDataSource, "Data source set");
            Assert.IsNotNull(dataSource, "Set data source value is not null");
            Assert.AreEqual(categoryListCount, categoryCount);
        }

        [TestMethod]
        public void StandardDisplayChosen_AlreadyOnStandardDisplay()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isStandardDisplay = true;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnStandardDisplay();

            // Assert
            Assert.IsTrue(called_isStandardDisplay, "Check if on standard display");
            Assert.IsFalse(called_turnOffCurrentDisplayMode, "Current display mode not turned off");
            Assert.IsFalse(called_dataClear, "Data not cleared");
            Assert.IsFalse(called_setupStandardDisplay, "Standard display not setup");
            Assert.IsFalse(called_initializeStandardDisplay, "Standard display not initialized");
            Assert.IsFalse(called_isShowingAll, "Is showing all not retrieved");
            Assert.IsFalse(called_setDataSource, "Data source not set");
        }

        [TestMethod]
        public void StandardDisplayChosen_NotOnStandardDisplay()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isStandardDisplay = false;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnStandardDisplay();

            // Assert
            Assert.IsTrue(called_isStandardDisplay, "Check if on standard display");
            Assert.IsTrue(called_turnOffCurrentDisplayMode, "Current display mode turned off");
            Assert.IsTrue(called_dataClear, "Data cleared");
            Assert.IsTrue(called_setupStandardDisplay, "Standard display setup");
            Assert.IsTrue(called_isShowingAll, "Is showing all retrieved");
            Assert.IsTrue(called_initializeStandardDisplay, "Standard display initialized");
            Assert.IsTrue(called_setDataSource, "Data source set");
        }

        [TestMethod]
        public void ByMonthDisplayChosen_AlreadyOnByMonthDisplay()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isByMonthDisplay = true;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnByMonthDisplay();

            // Assert
            Assert.IsTrue(called_isByMonthDisplay, "Check if on by month display");
            Assert.IsFalse(called_turnOffCurrentDisplayMode, "Current display mode not turned off");
            Assert.IsFalse(called_dataClear, "Data not cleared");
            Assert.IsFalse(called_setupByMonthDisplay, "By month display not setup");
            Assert.IsFalse(called_initializeByMonthDisplay, "By month display not initialized");
            Assert.IsFalse(called_setCurrentDisplayedMonth, "Current displayed month not retrieved");
            Assert.IsFalse(called_setDataSource, "Data source not set");
        }

        [TestMethod]
        public void ByMonthDisplayChosen_NotOnByMonthDisplay()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isByMonthDisplay = false;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnByMonthDisplay();

            // Assert
            Assert.IsTrue(called_isByMonthDisplay, "Check if on by month display");
            Assert.IsTrue(called_turnOffCurrentDisplayMode, "Current display mode turned off");
            Assert.IsTrue(called_dataClear, "Data cleared");
            Assert.IsTrue(called_setupByMonthDisplay, "By month display setup");
            Assert.IsTrue(called_initializeByMonthDisplay, "By month display initialized");
            Assert.IsTrue(called_setDataSource, "Data source set");
        }

        [TestMethod]
        public void ByCategoryDisplayChosen_AlreadyOnByCategoryDisplay()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isByCategoryDisplay = true;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnByCategoryDisplay();

            // Assert
            Assert.IsTrue(called_isByCategoryDisplay, "Check if on by category display");
            Assert.IsFalse(called_turnOffCurrentDisplayMode, "Current display mode not turned off");
            Assert.IsFalse(called_dataClear, "Data not cleared");
            Assert.IsFalse(called_setupByCategoryDisplay, "By category display not setup");
            Assert.IsFalse(called_initializeByCategoryDisplay, "By category display not initialized");
            Assert.IsFalse(called_isShowingAll, "Is showing all not retrieved");
            Assert.IsFalse(called_getCurrentTargetCategoryId, "Current category not retrieved");
            Assert.IsFalse(called_setDataSource, "Data source not set");
        }

        [TestMethod]
        public void ByCategoryDisplayChosen_NotOnByCategoryDisplay()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isByCategoryDisplay = false;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnByCategoryDisplay();

            // Assert
            Assert.IsTrue(called_isByCategoryDisplay, "Check if on by category display");
            Assert.IsTrue(called_turnOffCurrentDisplayMode, "Current display mode turned off");
            Assert.IsTrue(called_dataClear, "Data cleared");
            Assert.IsTrue(called_setupByCategoryDisplay, "By category display setup");
            Assert.IsTrue(called_initializeByCategoryDisplay, "By category display initialized");
            Assert.IsTrue(called_isShowingAll, "Is showing all retrieved");
            Assert.IsTrue(called_getCurrentTargetCategoryId, "Current category retrieved");
            Assert.IsTrue(called_setDataSource, "Data source set");
        }

        [TestMethod]
        public void ByCategoryAndMonthDisplayChosen_AlreadyOnByCategoryAndMonthDisplay()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            isByCategoryAndMonthDisplay = true;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnByCategoryAndMonthDisplay();

            // Assert
            Assert.IsTrue(called_isByCategoryAndMonthDisplay, "Check if on by category and month display");
            Assert.IsFalse(called_turnOffCurrentDisplayMode, "Current display mode not turned off");
            Assert.IsFalse(called_dataClear, "Data not cleared");
            Assert.IsFalse(called_setupByCategoryAndMonthDisplay, "By category and month display not setup");
            Assert.IsFalse(called_initializeByCategoryAndMonthDisplay, "By category and month display not initialized");
            Assert.IsFalse(called_setDataSource, "Data source not set");
        }

        [TestMethod]
        public void ByCategoryAndMonthDisplayChosen_NotOnByCategoryAndMonthDisplay()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            int categoryCount = TestConstants.numOfCategories;
            isByCategoryAndMonthDisplay = false;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.OnByCategoryAndMonthDisplay();

            // Assert
            Assert.IsTrue(called_isByCategoryAndMonthDisplay, "Check if on by category and month display");
            Assert.IsTrue(called_turnOffCurrentDisplayMode, "Current display mode turned off");
            Assert.IsTrue(called_dataClear, "Data cleared");
            Assert.IsTrue(called_setupByCategoryAndMonthDisplay, "By category and month display setup");
            Assert.IsTrue(called_initializeByCategoryAndMonthDisplay, "By category and month display initialized");
            Assert.IsTrue(called_setDataSource, "Data source set");
            Assert.AreEqual(categoryListCount, categoryCount);
        }

        [TestMethod]
        public void AddCategoryButtonClicked()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.AddCategoryButton_OnClicked();

            // Assert
            Assert.IsTrue(called_showNewCategoryForm, "New category form shown");
            Assert.IsNotNull(budget);
            Assert.IsInstanceOfType(budget, typeof(HomeBudget));
        }

        [TestMethod]
        public void AddExpenseButtonClicked()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.AddExpenseButton_OnClicked();

            // Assert
            Assert.IsTrue(called_showNewExpenseForm, "New expense form shown");
            Assert.IsNotNull(budget);
            Assert.IsInstanceOfType(budget, typeof(HomeBudget));
        }

        [TestMethod]
        public void ExpenseDoubleClicked()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            int expId = TestConstants.expenseId;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.BudgetButton_OnDoubleClicked(expId);

            // Assert
            Assert.IsTrue(called_showExpenseFullDetails, "Full details of expense shown");
            Assert.IsNotNull(budget);
            Assert.IsInstanceOfType(budget, typeof(HomeBudget));
        }

        [TestMethod]
        public void DeleteButtonDoubleClicked_UserCancels()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String dbPath = $"{folder}{TestConstants.TestBudgetDB}";
            deleteConfirmation = false;
            int expId = TestConstants.expenseId;
            int expenseCount = TestConstants.numOfExpenses;

            // Act
            var p = new DataPresenter(this, dbPath, false, false);
            p.DeleteButton_OnClicked(expId);

            // Assert
            Assert.IsTrue(called_showDeleteWarning, "Delete warning shown");
            Assert.IsNotNull(expenseDescription);
            Assert.IsFalse(called_reloadItemList, "item's not reloaded");
            Assert.AreEqual(expenseCount, p.Budget.GetBudgetItems(null, null, false, -1).Count);
        }

        [TestMethod]
        public void DeleteButtonDoubleClicked_UserConfirms()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDbPath = $"{folder}{TestConstants.TestBudgetDB}";
            String deleteDbPath = $"{folder}TestDbDeleteExpense.db";
            File.Copy(goodDbPath, deleteDbPath, true);
            deleteConfirmation = true;
            int expId = TestConstants.expenseId;
            int expenseCount = TestConstants.numOfExpenses;

            // Act
            var p = new DataPresenter(this, deleteDbPath, false, false);
            p.DeleteButton_OnClicked(expId);

            // Assert
            Assert.IsTrue(called_showDeleteWarning, "Delete warning shown");
            Assert.IsNotNull(expenseDescription);
            Assert.IsTrue(called_reloadItemList, "item's reloaded");
            Assert.AreEqual(expenseCount - 1, p.Budget.GetBudgetItems(null, null, false, -1).Count);
        }
    }
}
