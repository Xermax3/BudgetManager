using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Budget;

namespace AppDevGUI_bhm
{
    /// <summary>
    /// Interaction logic for ExpenseInfoWindow.xaml
    /// </summary>
    public partial class ExpenseInfoWindow : Window
    {
        private HomeBudget _budget;
        private Expense _selectedExpense;
        private Category _category;
        private int _lastEditedExpenseId;
        private bool _editModeOn = false;
        private bool _isSaved = false;
        private bool _txtBox_amt_TextChangedByUser = false;

        public ExpenseInfoWindow(HomeBudget budget, Expense selectedExp, Category category)
        {
            _budget = budget;
            _selectedExpense = selectedExp;
            _category = category;

            InitializeComponent();

            txtBlock_desc.Text = _selectedExpense.Description;
            txtBox_desc.Text = _selectedExpense.Description;
            DataObject.AddPastingHandler(txtBox_amt, OnPaste);
            txtBlock_amt.Text = CurrencyFormat(_selectedExpense.Amount);
            txtBox_amt.Text = CurrencyFormat(_selectedExpense.Amount);
            txtBlock_cat.Text = _category.ToString();
            FillCategoriesCmbBox();
            cmbBox_cat.SelectedValuePath = "Description";
            cmbBox_cat.SelectedValue = _category.Description;
            txtBlock_date.Text = _selectedExpense.Date.ToString("dddd, dd MMMM yyyy");
            datePicker_date.SelectedDate = _selectedExpense.Date;
        }

        public int LastEditedExpenseId
        {
            get
            {
                return _lastEditedExpenseId;
            }
            private set
            {
                _lastEditedExpenseId = value;
            }
        }

        private void btn_changeMode_Click(object sender, RoutedEventArgs e)
        {
            SwitchMode();
        }

        private void SwitchMode()
        {
            _editModeOn = !_editModeOn;

            if (_editModeOn)
            {
                txtBlock_desc_container.Height = 0;
                txtBlock_amt.Height = 0;
                txtBlock_cat.Height = 0;
                txtBlock_date.Height = 0;
                btn_save.Width = 100;
                btn_close.Margin = new Thickness { Left = 20 };
            }
            else
            {
                txtBlock_desc_container.Height = 25;
                txtBlock_amt.Height = 25;
                txtBlock_cat.Height = 25;
                txtBlock_date.Height = 25;
                btn_save.Width = 0;
                btn_close.Margin = new Thickness { Left = 0 };
            }
        }

        // Retrieved from ExpenseForm
        private void txtBox_desc_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }

        // Retrieved from ExpenseForm
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
            string[] values = txtBox_amt.Text.TrimStart('-').TrimStart(GUIConstants.CurrencySymbol.ToCharArray()).Split(GUIConstants.NumberDecimalSeparator.ToCharArray());
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

        // Retrieved from ExpenseForm
        private void FillCategoriesCmbBox()
        {
            cmbBox_cat.ItemsSource = _budget.Categories.GetList();
            cmbBox_cat.DisplayMemberPath = "Description";
        }

        // Mostly retrieved from ExpenseForm
        private async void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateEntry())
                return;

            // if the user typed in a category that isn't in the database, add it
            if (cmbBox_cat.SelectedItem == null)
                AddCategory();

            UpdateExpense();

            _isSaved = true;

            // display temporary message
            txtBlock_msg.Text = "Expense updated successfully";
            txtBlock_msg.Visibility = Visibility.Visible;
            await Task.Delay(1500);
            txtBlock_msg.Visibility = Visibility.Hidden;
        }

        // Mostly retrived from ExpenseForm (now checking if the values are the same as the original)
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

            if (double.Parse(txtBox_amt.Text.TrimStart('-').TrimStart(GUIConstants.CurrencySymbol.ToCharArray())) == 0)
            {
                MessageBox.Show($"Amount must be above {CurrencyFormat(0)}.", "Invalid amount", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbBox_cat.SelectedItem == null && cmbBox_cat.Text == "")
            {
                MessageBox.Show("Please enter a category", "Empty field", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (txtBox_desc.Text == _selectedExpense.Description &&
                Convert.ToDouble(txtBox_amt.Text.TrimStart('-').TrimStart(GUIConstants.CurrencySymbol.ToCharArray())) == Math.Abs(_selectedExpense.Amount) &&
                (((Category)cmbBox_cat.SelectedItem) != null && ((Category)cmbBox_cat.SelectedItem).Id == _category.Id) &&
                (DateTime)datePicker_date.SelectedDate == _selectedExpense.Date)
            {
                MessageBox.Show("No values were modified!", "No changes", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // Retrieved from ExpenseForm
        private void AddCategory()
        {
            // add the category entered in the combo box to both the database and the combo box
            _budget.Categories.Add(cmbBox_cat.Text, Category.CategoryType.Expense);
            FillCategoriesCmbBox();
            cmbBox_cat.SelectedIndex = cmbBox_cat.Items.Count - 1;
        }

        // Mostly retrieved from ExpenseForm
        private void UpdateExpense()
        {
            try
            {
                DateTime selectedDate = (DateTime)datePicker_date.SelectedDate;

                // add the expense to the database
                if (((Category)cmbBox_cat.SelectedItem).Type == Category.CategoryType.Expense || ((Category)cmbBox_cat.SelectedItem).Type == Category.CategoryType.Savings)
                {
                    _budget.Expenses.Update(_selectedExpense.Id,
                        selectedDate,
                        ((Category)cmbBox_cat.SelectedItem).Id,
                        Convert.ToDouble("-" + txtBox_amt.Text.TrimStart('-').TrimStart(GUIConstants.CurrencySymbol.ToCharArray())),
                        txtBox_desc.Text);
                }
                else
                {
                    _budget.Expenses.Update(_selectedExpense.Id,
                        selectedDate,
                        ((Category)cmbBox_cat.SelectedItem).Id,
                        Convert.ToDouble(txtBox_amt.Text.TrimStart('-').TrimStart(GUIConstants.CurrencySymbol.ToCharArray())),
                        txtBox_desc.Text);
                }

                LastEditedExpenseId = _selectedExpense.Id;
            }
            catch (Exception error)
            {
                _isSaved = false;
                MessageBox.Show(error.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            // mark adding new exp successful
            _isSaved = true;

            // get a new copy of expense with updated information, update textboxes, go back to non-edit mode
            _selectedExpense = _budget.Expenses.GetExpenseById(_selectedExpense.Id);
            txtBlock_desc.Text = _selectedExpense.Description;
            txtBlock_amt.Text = CurrencyFormat(_selectedExpense.Amount);
            txtBlock_cat.Text = _category.ToString();
            txtBlock_date.Text = _selectedExpense.Date.ToString("dddd, dd MMMM yyyy");
            SwitchMode();
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
