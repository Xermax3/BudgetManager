using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace AppDevGUI_bhm
{
    /// <summary>
    /// Interaction logic for StartUpWindow.xaml
    /// </summary>
    public partial class StartUpWindow : Window
    {
        private string _dbFullPath;
        private bool _usesDefaultCats;

        public StartUpWindow(bool firstTime = false)
        {
            InitializeComponent();
            LoadRecentDatabases();
            if (firstTime)
            {
                txtBlock_StartUpMessage.Text = "Welcome to the Budget Manager App! To create your first database, click 'Create a new database'.";
            }
        }

        public string FullPathToDatabase
        {
            get
            {
                return _dbFullPath;
            }
            private set
            {
                _dbFullPath = value;
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

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = GUIConstants.NormalButtonBackgroundOnMouseEnter;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = GUIConstants.NormalButtonBackgroundOnMouseLeave;
        }

        // CREATE NEW DATABASE
        private void btn_newDb_Click(object sender, RoutedEventArgs e)
        {
            // choose name for new DB file
            NewDatabaseForm newDBForm = new NewDatabaseForm();

            // if the user canceled or closed the window
            if (newDBForm.ShowDialog() == false)
            {
                return;
            }
            // if the user filled all the neccessary info and pressed OK
            else
            {
                FullPathToDatabase = System.IO.Path.Combine(newDBForm.SavingLocation, newDBForm.SelectedFileName);
                UsesDefaultCategories = newDBForm.UsesDefaultCategories;
            }

            // mark this is a new DB
            DialogResult = true;
        }

        // OPEN EXISTING DATABASE
        private void btn_existingDb_Click(object sender, RoutedEventArgs e)
        {
            // choose existing DB file
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Open an existing database";
            openFileDialog.Filter = "Database Files|*.db";

            // if the user canceled
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }

            // get full path to the DB file
            FullPathToDatabase = openFileDialog.FileName;

            // mark this is not a new DB
            DialogResult = false;
        }

        // RECENT DATABASES
        private void LoadRecentDatabases()
        {
            RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\BudgetManager");
            if (softwareKey != null)
            {
                object recentDBVal = softwareKey.GetValue("RecentDatabases");
                if (recentDBVal == null)
                {
                    TextBlock emptyMessage = new TextBlock
                    {
                        Text = "You have not used any database so far.",
                        FontWeight = FontWeights.Light
                    };
                    stckPanel_recentDb.Children.Add(emptyMessage);
                }
                else
                {
                    string[] recentDB = recentDBVal.ToString().Split(MainWindow.RECENTDB_SEPARATOR);
                    for (int i = 0; i < recentDB.Length; i++)
                    {
                        // Generate button
                        System.Windows.Controls.Button dbButton = new System.Windows.Controls.Button
                        {
                            Uid = recentDB[i],
                            ToolTip = recentDB[i],
                            Style = (Style)FindResource("style_btn_normal"),
                            Height = 55,
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left
                        };
                        dbButton.Click += dbButton_Click;

                        // Generate wrapper for button content
                        StackPanel dbButtonContentWrapper = new StackPanel
                        {
                            Margin = new Thickness(5)
                        };

                        // Generate file name textblock
                        int fileNameLastSeparator = recentDB[i].LastIndexOf('\\');
                        TextBlock dbButtonName = new TextBlock
                        {
                            Text = string.Format(recentDB[i].Substring(fileNameLastSeparator + 1)),
                            FontWeight = FontWeights.Regular,
                            TextWrapping = TextWrapping.NoWrap,
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            Margin = new Thickness { Bottom = 2 }
                        };

                        // Generate file path textblock
                        TextBlock dbButtonPath = new TextBlock
                        {
                            Text = recentDB[i],
                            TextWrapping = TextWrapping.NoWrap,
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            FontSize = 10
                        };

                        // Link GUI elements
                        dbButtonContentWrapper.Children.Add(dbButtonName);
                        dbButtonContentWrapper.Children.Add(dbButtonPath);
                        dbButton.Content = dbButtonContentWrapper;
                        stckPanel_recentDb.Children.Add(dbButton);
                    }
                }
                softwareKey.Close();
            }
        }

        private void dbButton_Click(object sender, RoutedEventArgs e)
        {
            // set full path property
            FullPathToDatabase = ((System.Windows.Controls.Button)sender).Uid;

            // mark this is not a new DB
            DialogResult = false;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
