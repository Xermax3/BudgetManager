using System;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace AppDevGUI_bhm
{
    /// <summary>
    /// Interaction logic for NewDatabaseForm.xaml
    /// </summary>
    public partial class NewDatabaseForm : Window
    {
        private string _selectedFileName;
        private string _savingLocation;
        private string _defaultSavingLocation;
        private bool _usesDefaultCats;

        public NewDatabaseForm()
        {
            InitializeComponent();
        }

        public string SelectedFileName
        {
            get
            {
                return _selectedFileName;
            }
            private set
            {
                _selectedFileName = value;
            }
        }

        public string SavingLocation
        {
            get
            {
                return _savingLocation;
            }
            private set
            {
                _savingLocation = value;
            }
        }

        public bool UsesDefaultCategories
        {
            get
            {
                return _usesDefaultCats;
            }
            private set
            {
                _usesDefaultCats = value;
            }
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((System.Windows.Controls.Button)sender).Background = GUIConstants.NormalButtonBackgroundOnMouseEnter;
        }

        private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((System.Windows.Controls.Button)sender).Background = GUIConstants.NormalButtonBackgroundOnMouseLeave;
        }

        private void txtBox_dbName_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = (System.Windows.Controls.TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }

        private void btn_ok_newDBName_Click(object sender, RoutedEventArgs e)
        {
            // if the field is empty or all whitespace
            if (string.IsNullOrWhiteSpace(txtBox_dbName.Text))
            {
                System.Windows.MessageBox.Show("Please give it a name!", "Empty field", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // check for illegal characters
            if (txtBox_dbName.Text.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
            {
                System.Windows.MessageBox.Show("Your name contains invalid characters!", "Invalid characters", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // store the entered DB name
            if (txtBox_dbName.Text.EndsWith(".db"))
                SelectedFileName = txtBox_dbName.Text;
            else
                SelectedFileName = txtBox_dbName.Text + ".db";

            // choose saving location
            grid_newDBName.Height = 0;

            // Make the default location whatever is specified in the registry  
            RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\BudgetManager");
            if (softwareKey != null)
            {
                _defaultSavingLocation = softwareKey.GetValue("DefaultFolderNewHomeBudget").ToString();
                txtBox_savingLocation.Text = _defaultSavingLocation;
            }
            softwareKey.Close();

            // check if the file already exists in the chose location
            if (File.Exists(txtBox_savingLocation.Text + "//" + SelectedFileName))
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("There is already a file with the same name in this location, would you like to overwrite it?",
                    "Existing File", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if (result == MessageBoxResult.No)
                    DialogResult = false;
            }
        }

        private void btn_browse_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                folderBrowser.Description = "Choose a saving location for this new database";
                folderBrowser.SelectedPath = _defaultSavingLocation;

                // if the user chose a saving location and pressed OK
                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // get full path to the DB file
                    txtBox_savingLocation.Text = folderBrowser.SelectedPath;
                }
                // if the user canceled or closed the browser
                else
                {
                    return;
                }
            }
        }

        private void btn_ok_savingLocation_Click(object sender, RoutedEventArgs e)
        {
            // if the field is empty or all whitespace
            if (string.IsNullOrWhiteSpace(txtBox_dbName.Text))
            {
                System.Windows.MessageBox.Show("Please choose a saving location!", "Empty field", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // store the saving location 
            SavingLocation = txtBox_savingLocation.Text.Replace('/', '\\');

            RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\BudgetManager", true);
            if (softwareKey != null)
            {
                // the user wants to make their path the default one
                if (chkBox_defaultLocation.IsChecked == true && SavingLocation != _defaultSavingLocation)
                    softwareKey.SetValue("DefaultFolderNewHomeBudget", SavingLocation);
                else
                {
                    // the user is currently selecting their default path but doesn't want to keep it: use the built-in path
                    if (chkBox_defaultLocation.IsChecked == false && SavingLocation == _defaultSavingLocation)
                        softwareKey.SetValue("DefaultFolderNewHomeBudget", MainWindow.DEFAULT_FOLDER_NEW_HOMEBUDGET);
                }
                softwareKey.Close();
            }

            // ask the user if they want to use the default cats
            grid_savingLocation.Height = 0;
        }

        private void btn_yes_useDefaultCategories_Click(object sender, RoutedEventArgs e)
        {
            UsesDefaultCategories = true;
            DialogResult = true;
        }

        private void btn_no_useDefaultCategories_Click(object sender, RoutedEventArgs e)
        {
            UsesDefaultCategories = false;
            DialogResult = true;
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
