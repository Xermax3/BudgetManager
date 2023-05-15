using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Budget;

namespace AppDevGUI_bhm
{
    /// <summary>
    /// Interaction logic for CategoryForm.xaml
    /// </summary>
    public partial class CategoryForm : Window
    {
        private HomeBudget _budget;
        private Category.CategoryType _typeOfTheLastAddedCat;
        private bool _isSaved;      

        /// <summary>
        /// Initializes a new category form window that allows a user to add a new 
        /// <see cref="Category"/> to the categories table of the database.
        /// </summary>
        /// <param name="budget">The current home budget being used.</param>
        public CategoryForm(HomeBudget budget)
        {
            _budget = budget;
            InitializeComponent();

            cmbBox_catType.ItemsSource = Enum.GetValues(typeof(Category.CategoryType));
            _isSaved = false;

            // Set CategoryType to default value (Expense)
            cmbBox_catType.SelectedItem = Category.CategoryType.Expense;
        }

        /// <summary>
        /// Gets or sets the <see cref="Category.CategoryType"/> of the most recently added <see cref="Category"/>
        /// </summary>
        public Category.CategoryType TypeOfTheLastAddedCategory
        {
            get
            {
                return _typeOfTheLastAddedCat;
            }
            private set
            {
                _typeOfTheLastAddedCat = value;
            }
        }

        private void txtBox_desc_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }

        private async void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateEntry())
                return;

            AddCategory();
            ResetFields();

            // display temporary message
            txtBlock_msg.Text = "Category added successfully";
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

            if (!UniqueCategoryDescription())
            {
                MessageBox.Show(string.Format("A category with the description '{0}' already exists.", txtBox_desc.Text.Trim()), "Duplicate Category Description", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbBox_catType.SelectedItem == null)
            {
                MessageBox.Show("Please enter a category", "Empty field", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool UniqueCategoryDescription()
        {
            foreach(Category category in _budget.Categories.GetList())
            {
                if (category.Description.Trim().ToLower() == txtBox_desc.Text.Trim().ToLower())
                    return false;
            }
            return true;
        }

        private void AddCategory()
        {
            try
            {
                Category.CategoryType selectedType = (Category.CategoryType)cmbBox_catType.SelectedItem;

                // add the category to the database
                _budget.Categories.Add(txtBox_desc.Text, selectedType);

                // save the type of the recently added cat
                TypeOfTheLastAddedCategory = selectedType;
            }
            catch (Exception error)
            {
                _isSaved = false;
                MessageBox.Show(error.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            // mark adding new cat successful
            _isSaved = true;

            // reset fields after entry
            ResetFields();
        }

        private void ResetFields()
        {
            txtBox_desc.Text = "New Category";
            cmbBox_catType.SelectedItem = Category.CategoryType.Expense;
        }

        private void btn_switchToExpense_Click(object sender, RoutedEventArgs e)
        {
            ExpenseForm expenseForm = new ExpenseForm(_budget);
            Close();
            expenseForm.ShowDialog();
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
