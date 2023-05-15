using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Budget;

namespace AppDevGUI_bhm
{
    /// <summary>
    /// Interaction logic for ExpenseForm.xaml
    /// </summary>
    public partial class ExpenseForm : Window
    {
        private HomeBudget _budget;
        private int _monthOfLastAddedExp;
        private int _lastEnteredCatId;
        private int _lastEnteredExpId;
        private DateTime _lastEnteredDate;
        private bool _isSaved;
        private bool _txtBox_amt_TextChangedByUser = false;

        /// <summary>
        /// Initializes a new expense form window that allows a user to add new 
        /// <see cref="Expense"/> to the expenses table of the provided database.
        /// </summary>
        /// <param name="budget">The current home budget being used.</param>
        public ExpenseForm(HomeBudget budget)
        {
            _budget = budget;

            InitializeComponent();
            FillCategoriesCmbBox();

            DataObject.AddPastingHandler(txtBox_amt, OnPaste);
            txtBox_amt.Text = GUIConstants.DefaultExpenseAmount;
            datePicker_date.SelectedDate = DateTime.Today;
            cmbBox_cat.SelectedIndex = 0;
        }

        /// <summary>
        /// Initializes a new expense form window that allows a user to add new 
        /// <see cref="Expense"/> to the expenses table of the provided database.
        /// </summary>
        /// <param name="budget">Teh current home budget being used.</param>
        /// <param name="lastEnteredCatId">The <see cref="Category"/> of the last entered expense</param>
        /// <param name="lastEnteredDate">The <see cref="DateTime"/> of the last entered expense</param>
        public ExpenseForm(HomeBudget budget, int lastEnteredCatId, DateTime lastEnteredDate)
        {
            _budget = budget;

            InitializeComponent();
            FillCategoriesCmbBox();

            DataObject.AddPastingHandler(txtBox_amt, OnPaste);
            txtBox_amt.Text = GUIConstants.DefaultExpenseAmount;
            cmbBox_cat.SelectedIndex = lastEnteredCatId;
            datePicker_date.SelectedDate = lastEnteredDate;
        }

        /// <summary>
        /// Gets or sets the numerical value of the most recently added <see cref="Expense"/>
        /// </summary>
        public int MonthOfLastAddedExpense
        {
            get
            {
                return _monthOfLastAddedExp;
            }
            private set
            {
                _monthOfLastAddedExp = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Category"/> of the last entered <see cref="Expense"/>
        /// </summary>
        public int LastEnteredCatId
        {
            get
            {
                return _lastEnteredCatId;
            }
            private set
            {
                _lastEnteredCatId = value;
            }
        }

        public int LastEnteredExpenseId
        {
            get
            {
                return _lastEnteredExpId;
            }
            private set
            {
                _lastEnteredExpId = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> of the last entered <see cref="Expense"/>
        /// </summary>
        public DateTime LastEnteredDate
        {
            get
            {
                return _lastEnteredDate;
            }
            set
            {
                _lastEnteredDate = value;
            }
        }

        private void txtBox_desc_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }

        private void txtBox_amt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_txtBox_amt_TextChangedByUser) return;

            _txtBox_amt_TextChangedByUser = false;

            // if the text is the same as the default
            if (txtBox_amt.Text == GUIConstants.DefaultExpenseAmount)
            {
                return;
            }

            // if user manually erases until there's only currency symbol left or nothing left 
            // (by highlighting part of the text and delete)
            if (txtBox_amt.Text.Length <= GUIConstants.CurrencySymbol.Length)
            {
                txtBox_amt.Text = GUIConstants.DefaultExpenseAmount;

                // always place cursor at the end of the amount
                txtBox_amt.SelectionStart = txtBox_amt.Text.Length;
                return;
            }

            // push numbers to the left as they are entered
            string[] values = txtBox_amt.Text.TrimStart(GUIConstants.CurrencySymbol.ToCharArray()).Split(GUIConstants.NumberDecimalSeparator.ToCharArray());
            double multiplier = Math.Pow(10, values[values.Length - 1].Length);
            double amount = (Convert.ToDouble(values[values.Length - 2] + GUIConstants.NumberDecimalSeparator + values[values.Length - 1]) * multiplier) / 100;

            txtBox_amt.Text = CurrencyFormat(amount);

            // always place cursor at the end of the amount
            txtBox_amt.SelectionStart = txtBox_amt.Text.Length;
        }

        private string CurrencyFormat(double amount)
        {
            if (amount < 0)
            {
                amount *= -1;
                return string.Format($"-{GUIConstants.CurrencySymbol}{{0:N2}}", amount);
            }
            else
            {
                return string.Format($"{GUIConstants.CurrencySymbol}{{0:N2}}", amount);
            }
        }

        private void txtBox_amt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // mark the text is changed by the user
            _txtBox_amt_TextChangedByUser = true;

            // prevent non-digits from being entered in the amount field
            char c = e.Text.ToCharArray()[0];
            e.Handled = !(char.IsDigit(c));
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (isText) _txtBox_amt_TextChangedByUser = true;
        }

        private void txtBox_amt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
                // mark the text is changed by the user
                _txtBox_amt_TextChangedByUser = true;
        }

        private void FillCategoriesCmbBox()
        {
            cmbBox_cat.ItemsSource = _budget.Categories.GetList();
            cmbBox_cat.DisplayMemberPath = "Description";
        }

        private void cmbBox_cat_SelectionChange(object sender, RoutedEventArgs e)
        {
            if (cmbBox_cat.SelectedItem == null || ((Category)cmbBox_cat.SelectedItem).Type == Category.CategoryType.Expense)
            {
                chkBox_credit.IsEnabled = true;
            }
            else
            {
                chkBox_credit.IsEnabled = false;
                chkBox_credit.IsChecked = false;
            }
        }

        private async void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateEntry() || DuplicateEntry())
                return;

            // if the user typed in a category that isn't in the database, add it
            if (cmbBox_cat.SelectedItem == null)
                AddCategory();

            AddExpense();
            ResetFields();
            _isSaved = true;

            // display temporary message
            txtBlock_msg.Text = "Expense added successfully";
            txtBlock_msg.Visibility = Visibility.Visible;
            await Task.Delay(1500);
            txtBlock_msg.Visibility = Visibility.Hidden;
        }


        private bool ValidateEntry()
        {
            // check for empty fields
            if (string.IsNullOrWhiteSpace(txtBox_desc.Text))
            {
                MessageBox.Show("Please enter a description", "Empty field", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtBox_amt.Text))
            {
                MessageBox.Show("Please enter an amount", "Empty field", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (double.Parse(txtBox_amt.Text.TrimStart(GUIConstants.CurrencySymbol.ToCharArray())) == 0)
            {
                MessageBox.Show($"Amount must be above {CurrencyFormat(0)}", "Invalid amount", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbBox_cat.SelectedItem == null && cmbBox_cat.Text == "")
            {
                MessageBox.Show("Please enter a category", "Empty field", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool DuplicateEntry()
        {
            List<Expense> expList = _budget.Expenses.GetList();

            // if there is no records in the database
            if (expList.Count == 0)
            {
                return false;
            }

            // get the last entered expense from the database
            Expense lastEnteredExpense = expList.Last();


            // check if the current expense is the same as the last entered expense, if so, warn the user
            if (Math.Abs(lastEnteredExpense.Amount) == Convert.ToDouble(txtBox_amt.Text.TrimStart(GUIConstants.CurrencySymbol.ToCharArray())) &&
                lastEnteredExpense.Date == (DateTime)datePicker_date.SelectedDate &&
                lastEnteredExpense.Description == txtBox_desc.Text &&
                lastEnteredExpense.CategoryId == ((Category)cmbBox_cat.SelectedItem).Id)
            {
                MessageBoxResult result = MessageBox.Show("This expense is the same as the last entered expense, would you like to proceed?",
                    "Duplicate Entry", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                return result == MessageBoxResult.Yes ? false : true;
            }

            return false;
        }


        private void AddCategory()
        {
            // add the category entered in the combo box to both the database and the combo box
            _budget.Categories.Add(cmbBox_cat.Text, Category.CategoryType.Expense);
            FillCategoriesCmbBox();
            cmbBox_cat.SelectedIndex = cmbBox_cat.Items.Count - 1;
        }

        private void AddExpense()
        {
            try
            {
                DateTime selectedDate = (DateTime)datePicker_date.SelectedDate;

                /* 
                 * Add the expense to the database
                 * If the expenses category type is either 'Expense' or 'Savings' add
                 * the inputted amount as a negative, otherwise add it as a positive
                 */
                if (((Category)cmbBox_cat.SelectedItem).Type == Category.CategoryType.Expense ||
                    ((Category)cmbBox_cat.SelectedItem).Type == Category.CategoryType.Savings)
                {
                    _budget.Expenses.Add(
                        selectedDate,
                        ((Category)cmbBox_cat.SelectedItem).Id,
                        Convert.ToDouble("-" + txtBox_amt.Text.TrimStart(GUIConstants.CurrencySymbol.ToCharArray())),
                        txtBox_desc.Text);

                    // add an additional expense using the credit card category if the credit checkbox is checked
                    if (((Category)cmbBox_cat.SelectedItem).Type == Category.CategoryType.Expense && (bool)chkBox_credit.IsChecked)
                    {
                        // if there is no "Credit Card" category in the database (when the user chose not using default categories)
                        if (_budget.Categories.GetList().Find(cat => cat.Description == "Credit Card") == null)
                            _budget.Categories.Add("Credit Card", Category.CategoryType.Credit);

                        _budget.Expenses.Add(
                            selectedDate,
                            _budget.Categories.GetList().Find(cat => cat.Description == "Credit Card").Id,
                            Convert.ToDouble(txtBox_amt.Text.TrimStart(GUIConstants.CurrencySymbol.ToCharArray())),
                            "Credit Card Transfer for " + txtBox_desc.Text);
                    }
                }
                else
                {
                    _budget.Expenses.Add(
                        selectedDate,
                        ((Category)cmbBox_cat.SelectedItem).Id,
                        Convert.ToDouble(txtBox_amt.Text.TrimStart(GUIConstants.CurrencySymbol.ToCharArray())),
                        txtBox_desc.Text);
                }

                // save the date and category of the recently added expense
                LastEnteredCatId = cmbBox_cat.SelectedIndex;
                LastEnteredExpenseId = _budget.Expenses.GetList().Last().Id;
                LastEnteredDate = selectedDate;
            }
            catch (Exception error)
            {
                _isSaved = false;
                MessageBox.Show(error.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            // mark adding new exp successful
            _isSaved = true;
        }

        private void ResetFields()
        {
            txtBox_desc.Text = "New Expense";
            txtBox_amt.Text = GUIConstants.DefaultExpenseAmount;
            chkBox_credit.IsChecked = false;
        }

        private void btn_switchToCategory_Click(object sender, RoutedEventArgs e)
        {
            CategoryForm categoryForm = new CategoryForm(_budget);
            Close();
            categoryForm.ShowDialog();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isSaved)
            {
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
        }
    }
}
