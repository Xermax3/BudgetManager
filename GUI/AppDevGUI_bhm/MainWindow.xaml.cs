using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Budget;
using TextBlockHighlighterCS;

namespace AppDevGUI_bhm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDataView
    {
        public static string DEFAULT_FOLDER_NEW_HOMEBUDGET = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Budgets");
        public static char RECENTDB_SEPARATOR = '|';

        private string _dbFullPath;
       // private HomeBudget _budget;

        // displaying
        private int _currentTargetCatId = -1;
        private DateTime? _selectedDate;
        private DateTime? _currentDisplayedStartDate;
        private DateTime? _currentDisplayedEndDate;
        private int _currentDisplayedCatTypeIndex = -1;
        private bool _expsDisplayedWithinSelectedDays;
        private int _currentDisplayedMonth = -1;
        private bool _expsDisplayedByCategory;
        private bool _expsSummaryDisplayed;
        private bool _showAllExps;
        private bool _dateChangedByProgram = false;
        private bool _calendarOpenedByUser = false;
        private bool _firstSelectedDateChangedCall = true;
        private Button _currentSelectedExpBtn;
        private int _numLoadedItems = 0;

        // entering data
        private int _lastEnteredCatId;
        private int _lastEnteredOrEditedExpId;
        private DateTime _lastEnteredDate;

        // searching
        private bool _searchModeTurnedBackOn = false;
        private bool _focusChangedByUserDuringTraversal = false;
        private string _currentKeyword;
        private int _traversalStartIndex;
        private bool _reachedTraversalStart = false;
        private List<Button> _matchedBudgetBtns;
        private List<IDictionary<string, object>> _matchedSummaryRows;
        private int _currentMatchedIndex;

        public MainWindow()
        {
            StartProgram();
        }

        public void StartProgram()
        {
            bool validDb = false;
            bool firstTime = true;

            do
            {
                // If the registry key for this application does not exist, create it
                RegistryKey softwareKeyStart = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\BudgetManager");
                if (softwareKeyStart == null)
                {
                    softwareKeyStart = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\BudgetManager");
                    softwareKeyStart.SetValue("DefaultFolderNewHomeBudget", DEFAULT_FOLDER_NEW_HOMEBUDGET);
                    softwareKeyStart.Close();
                }
                else
                {
                    // For testing purposes. Uncomment the code below to delete the application's key.
                    //Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\BudgetManager");
                    //Close();
                    firstTime = softwareKeyStart.GetValue("RecentDatabases") == null;
                    softwareKeyStart.Close();
                }

                StartUpWindow startUpWindow = new StartUpWindow(firstTime);

                bool? isNewDB = startUpWindow.ShowDialog();

                // if the user closed the window
                if (startUpWindow.FullPathToDatabase == null)
                {
                    Close();
                }
                else
                {
                    // get the full path to the DB file
                    FullPathToDatabase = startUpWindow.FullPathToDatabase;

                    // initialize HomeBudget
                    try
                    {
                        Presenter = new DataPresenter(this, FullPathToDatabase, (bool)isNewDB, startUpWindow.UsesDefaultCategories);                      
                    }
                    catch
                    {
                        MessageBox.Show("Unusable file, please choose a budget database file.", "Unusable File", MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

                    validDb = true;

                    // push this database to the top of the recent db list in the registry
                    RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\BudgetManager", true);
                    if (softwareKey != null)
                    {
                        object recentDBVal = softwareKey.GetValue("RecentDatabases");
                        if (recentDBVal == null)
                        {
                            softwareKey.SetValue("RecentDatabases", FullPathToDatabase);
                        }
                        else
                        {
                            List<string> recentDB = recentDBVal.ToString().Split(RECENTDB_SEPARATOR).ToList();
                            if (recentDB.Exists(x => x == FullPathToDatabase))
                                recentDB.Remove(FullPathToDatabase);
                            recentDB.Insert(0, FullPathToDatabase);
                            softwareKey.SetValue("RecentDatabases", string.Join(RECENTDB_SEPARATOR.ToString(), recentDB.ToArray()));
                        }
                        softwareKey.Close();
                    }

                    Presenter.OnDatabaseOpened();
                }
            } while (!validDb);
        }

        // FROM INTERFACE
        public void InitializeMainWindow(HomeBudget budget)
        {
            InitializeComponent();
            ReloadCategoryFilterCombobox(budget);

            // show current file name and its immediate parent
            int fileNameLastSeparator = FullPathToDatabase.LastIndexOf('\\');
            int fileNameSecondToLastSeparator = fileNameLastSeparator > 0 ? FullPathToDatabase.LastIndexOf('\\', fileNameLastSeparator - 1) : -1;
            txt_currentFile.Text = string.Format(FullPathToDatabase.Substring(fileNameSecondToLastSeparator + 1));

            // display expenses
            menuItem_displayExps.IsChecked = true;
        }

        // FROM INTERFACE
        public DataPresenter Presenter { get; set; }
        // FROM INTERFACE
        public List<object> DataSource { get; set; }

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

        // -----------------------------
        // Change File
        // -----------------------------

        private void btn_newDB_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void btn_openDB_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void btn_cloneDB_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        // -----------------------------
        // Add New Item
        // -----------------------------

        private void btn_addExpense_Click(object sender, RoutedEventArgs e)
        {
            Presenter.AddExpenseButton_OnClicked();
        }

        // FROM INTERFACE
        public void ShowNewExpenseForm(HomeBudget budget)
        {
            ExpenseForm ef = _lastEnteredCatId == 0 && _lastEnteredDate == DateTime.MinValue ?
                new ExpenseForm(budget) : new ExpenseForm(budget, _lastEnteredCatId, _lastEnteredDate);

            if (ef.ShowDialog() == true)
            {
                _lastEnteredCatId = ef.LastEnteredCatId;
                _lastEnteredOrEditedExpId = ef.LastEnteredExpenseId;
                _lastEnteredDate = ef.LastEnteredDate;

                ReloadCategoryFilterCombobox(budget);
                ReloadItemList();
            }
        }

        private void btn_addCategory_Click(object sender, RoutedEventArgs e)
        {
            Presenter.AddCategoryButton_OnClicked();
        }

        // FROM INTERFACE
        public void ShowNewCategoryForm(HomeBudget budget)
        {
            CategoryForm cf = new CategoryForm(budget);

            if (cf.ShowDialog() == true)
            {
                ReloadCategoryFilterCombobox(budget);

                // if the type of the last added cat is being displayed
                if (_currentDisplayedCatTypeIndex == (int)cf.TypeOfTheLastAddedCategory - 1)
                {
                    // reload the cat list
                    ReloadItemList();
                }
            }
        }

        private void ReloadCategoryFilterCombobox(HomeBudget budget)
        {
            int currentFilterOption = control_dataGridView.cmb_categoryFilter.SelectedIndex;

            control_dataGridView.cmb_categoryFilter.Items.Clear();
            control_dataGridView.cmb_categoryFilter.Items.Add("All Categories");
            foreach (Category cat in budget.Categories.GetList())
            {
                control_dataGridView.cmb_categoryFilter.Items.Add(cat);
            }

            if (currentFilterOption == -1)
            {
                control_dataGridView.cmb_categoryFilter.SelectedIndex = 0;
            }
            else
            {
                control_dataGridView.cmb_categoryFilter.SelectedIndex = currentFilterOption;
            }
        }

        // -----------------------------
        // Process Display Options
        // -----------------------------

        // FROM INTERFACE
        public void TurnOffCurrentDisplayMode()
        {
            // if current display mode is other than exps by selected days and exps by cat
            // or if selected display mode is other than exps by selected days and exps by cat
            if (cmb_displayExpOptions.Visibility == Visibility.Hidden ||
                (!_expsDisplayedWithinSelectedDays && !_expsDisplayedByCategory) ||
                (cmb_displayExpOptions.SelectedIndex != 0 && cmb_displayExpOptions.SelectedIndex != 2))
            {
                control_dataGridView.stckPanel_filterWrapper.Children.RemoveRange(1, control_dataGridView.stckPanel_filterWrapper.Children.Count - 1);
            }

            control_dataGridView.stckPanel_itemList.Children.Clear();

            // if cats are being displayed
            if (_currentDisplayedCatTypeIndex > -1)
            {
                // remove catType cursor
                control_dataGridView.grid_displayFilter.Children.RemoveAt(1);
                _currentDisplayedCatTypeIndex = -1;
                return;
            }

            // if exps are being displayed

            _currentSelectedExpBtn = null;

            // if exps are being displayed within selected days or by category
            if (_currentDisplayedStartDate != null)
            {
                if (cmb_displayExpOptions.Visibility == Visibility.Hidden || 
                   (cmb_displayExpOptions.SelectedIndex != 0 && cmb_displayExpOptions.SelectedIndex != 2))
                {
                    _currentDisplayedStartDate = null;
                    _currentDisplayedEndDate = null;
                }

                // if exps are being displayed within selected days
                if (_expsDisplayedWithinSelectedDays)
                {
                    _expsDisplayedWithinSelectedDays = false;
                }
                // if exps are being displayed by cat
                else
                {
                    _expsDisplayedByCategory = false;
                }

                return;
            }

            // if exps are being displayed by month
            if (_currentDisplayedMonth > -1)
            {
                _currentDisplayedMonth = -1;
                return;
            }

            // if exps summary is being displayed
            if (_expsSummaryDisplayed)
            {
                _expsSummaryDisplayed = false;
            }
        }

        // FROM INTERFACE
        public void ResetKeyword()
        {
            // erase memory about the current keyword
            _currentKeyword = null;
        }

        // FROM INTERFACE
        public void ReloadItemList()
        {
            ResetKeyword();

            if (_currentDisplayedCatTypeIndex > -1)
            {
                LoadCategoriesByChosenType();
            }
            else
            {
                Presenter.OnReloadExpenseItemList();
            }
        }

        private void menuItem_displayCats_Checked(object sender, RoutedEventArgs e)
        {
            if (_currentDisplayedCatTypeIndex > -1)
            {
                return;
            }

            if (menuItem_displayExps.IsChecked)
            {
                menuItem_displayExps.IsChecked = false;
            }

            DisplayCategories();
        }

        private void menuItem_displayCats_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!menuItem_displayExps.IsChecked)
            {
                menuItem_displayCats.IsChecked = true;
            }
        }

        public void cmb_categoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selectedItem = ((ComboBox)sender).SelectedItem;

            if (selectedItem == null)
            {
                return;
            }

            if (selectedItem.ToString() == "All Categories")
            {
                if (_currentTargetCatId == -1)
                {
                    return;
                }

                _currentTargetCatId = -1;
            }
            else
            {
                int selectedCatId = ((Category)selectedItem).Id;

                if (_currentTargetCatId == selectedCatId)
                {
                    return;
                }

                _currentTargetCatId = selectedCatId;
            }

            ReloadItemList();
        }

        private void menuItem_displayExps_Checked(object sender, RoutedEventArgs e)
        {
            Presenter.DisplayExpensesMenuItem_OnChecked();
        }
        
        // FROM INTERFACE
        public void UncheckDisplayCategoriesMenuItem()
        {
            menuItem_displayCats.IsChecked = false;
        }

        // FROM INTERFACE
        public void ShowDisplayExpensesOptions()
        {
            // show exp display options
            cmb_displayExpOptions.Visibility = Visibility.Visible;
        }

        // FROM INTERFACE
        public void ShowSearchBox()
        {
            // show search box
            txtBox_find.Visibility = Visibility.Visible;
            btn_find.Visibility = Visibility.Visible;
        }

        // FROM INTERFACE
        public void ProcessChosenDisplayExpensesOption()
        {
            ProcessChosenDisplayExpensesOption(cmb_displayExpOptions.SelectedIndex);
        }

        private void cmb_displayExpOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExpensesAreBeingDisplayed())
            {
                ProcessChosenDisplayExpensesOption(cmb_displayExpOptions.SelectedIndex);
            }
        }

        private void ProcessChosenDisplayExpensesOption(int selection)
        {
            control_dataGridView.cmb_categoryFilter.Width = double.NaN;
            control_dataGridView.cmb_categoryFilter.Margin = new Thickness(10, 0, 0, 0);

            switch (selection)
            {
                case 0:
                    Presenter.OnStandardDisplay();
                    break;
                case 1:
                    Presenter.OnByMonthDisplay();
                    break;
                case 2:
                    Presenter.OnByCategoryDisplay();
                    break;
                case 3:
                    Presenter.OnByCategoryAndMonthDisplay();
                    break;
            }
        }

        private void menuItem_displayExps_Unchecked(object sender, RoutedEventArgs e)
        {
            Presenter.DisplayExpensesMenuItem_OnUnchecked();
        }

        // FROM INTERFACE
        public void CheckDisplayCategoriesMenuItem()
        {
            menuItem_displayExps.IsChecked = true;
        }

        // FROM INTERFACE
        public void HideDisplayExpensesOptions()
        {
            // hide exp display options
            cmb_displayExpOptions.Visibility = Visibility.Hidden;
        }

        // FROM INTERFACE
        public void HideSearchBox()
        {
            // hide search box
            txtBox_find.Visibility = Visibility.Hidden;
            btn_find.Visibility = Visibility.Hidden;
        }

        // FROM INTERFACE
        public bool DisplayCategoriesMenuItemIsChecked()
        {
            return menuItem_displayCats.IsChecked;
        }

        // FROM INTERFACE
        public bool DisplayExpensesMenuItemIsChecked()
        {
            return menuItem_displayExps.IsChecked;
        }

        // FROM INTERFACE
        public bool ExpensesAreBeingDisplayed()
        {
            return _currentDisplayedStartDate != null || _currentDisplayedMonth > -1 || _expsSummaryDisplayed;
        }

        // -----------------------------

        // -----------------------------
        // Display Categories
        // -----------------------------

        private void DisplayCategories()
        {
            // if this display mode is already on
            if (_currentDisplayedCatTypeIndex > -1)
            {
                return;
            }

            control_dataGridView.cmb_categoryFilter.Width = 0;
            control_dataGridView.cmb_categoryFilter.Margin = new Thickness(0);
            TurnOffCurrentDisplayMode();

            // add cat type buttons to navigate
            Button btn_catType;
            for (int i = 1; i <= Enum.GetValues(typeof(Category.CategoryType)).Length; i++)
            {
                btn_catType = new Button
                {
                    Uid = (i - 1).ToString(),
                    Content = Enum.GetName(typeof(Category.CategoryType), i),
                    Style = (Style)FindResource("style_btn_item"),
                    Template = (ControlTemplate)FindResource("template_btn_catType")
                };
                btn_catType.MouseEnter += Btn_catType_MouseEnter;
                btn_catType.MouseLeave += Btn_catType_MouseLeave;
                btn_catType.Click += btn_catType_Click;
                control_dataGridView.stckPanel_filterWrapper.Children.Add(btn_catType);
            }

            // add cat type cursor to see the currently chosen cat type
            Grid catTypeCursor = new Grid
            {
                Style = (Style)FindResource("style_grid_catTypeCursor")
            };
            control_dataGridView.grid_displayFilter.Children.Add(catTypeCursor);
            Grid.SetRow(catTypeCursor, 1);

            // the first cat type is displayed on first load
            _currentDisplayedCatTypeIndex = 0;
            ReloadItemList();
        }

        private void Btn_catType_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = Brushes.NavajoWhite;
        }

        private void Btn_catType_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = GUIConstants.NormalButtonBackgroundOnMouseLeave;
        }

        private void btn_catType_Click(object sender, RoutedEventArgs e)
        {
            Button chosenCatTypeButton = (Button)sender;
            int chosenCatTypeId = int.Parse(chosenCatTypeButton.Uid);

            if (_currentDisplayedCatTypeIndex == chosenCatTypeId)
            {
                return;
            }

            _currentDisplayedCatTypeIndex = chosenCatTypeId;
            ((Grid)(control_dataGridView.grid_displayFilter.Children[1])).Margin = new Thickness(_currentDisplayedCatTypeIndex * 150, 0, 0, 0);

            ReloadItemList();
        }

        private void LoadCategoriesByChosenType()
        {
            control_dataGridView.border_header.Child = null;
            control_dataGridView.border_header.Height = 0;

            if (control_dataGridView.stckPanel_itemList.Children.Count > 0)
            {
                control_dataGridView.stckPanel_itemList.Children.Clear();
            }

            List<Category> catsOfTheChosenType = Presenter.Budget.Categories.GetCategoriesByType(_currentDisplayedCatTypeIndex + 1);

            Border catWrapper;
            foreach (Category cat in catsOfTheChosenType)
            {
                catWrapper = new Border
                {
                    Style = (Style)FindResource("style_border_itemWrapper")
                };
                catWrapper.Child = new TextBlock
                {
                    Text = cat.Description,
                    Style = (Style)FindResource("style_txtBlock_cat"),
                    Margin = new Thickness(10, 0, 10, 0),
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                control_dataGridView.stckPanel_itemList.Children.Add(catWrapper);
            }
        }

        // -----------------------------

        // -----------------------------
        // Display Budget Items 
        // -----------------------------

        private Border BuildBudgetButton(BudgetItem budgetItem)
        {
            Border budgetBtnBorder = new Border
            {
                Style = (Style)FindResource("style_border_itemWrapper")
            };

            budgetBtnBorder.SizeChanged += BudgetBtnBorder_SizeChanged;
            budgetBtnBorder.MouseEnter += BudgetBtnBorder_MouseEnter;
            budgetBtnBorder.MouseLeave += BudgetBtnBorder_MouseLeave;

            Button budgetButton = new Button
            {
                Uid = budgetItem.ExpenseId.ToString(),
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)FindResource("style_btn_item"),
                Template = (ControlTemplate)FindResource("template_btn_budgetItem")
            };

            if (_expsDisplayedWithinSelectedDays)
            {
                budgetButton.ToolTip = budgetItem.Date.ToString("dd MMM yyyy", GUIConstants.CurrentCulture);
            }

            budgetButton.MouseDoubleClick += BudgetButton_MouseDoubleClick;
            budgetButton.Click += BudgetButton_Click;

            // add info about this budget item
            Grid budgetInfoWrapper = new Grid();
            budgetInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition 
            {
                Width = new GridLength(2, GridUnitType.Star)
            });
            budgetInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            budgetInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            budgetInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength()
            });

            TextBlock expInfo = new TextBlock
            {
                Text = budgetItem.ExpenseDescription,
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
            };
            budgetInfoWrapper.Children.Add(expInfo);

            expInfo = new TextBlock
            {
                Text = CurrencyFormat(budgetItem.Amount),
                TextAlignment = TextAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Style = (Style)FindResource("style_txtBlock_expDisplay")
            };
            budgetInfoWrapper.Children.Add(expInfo);
            Grid.SetColumn(expInfo, 1);
            expInfo = new TextBlock
            {
                Text = CurrencyFormat(budgetItem.Balance),
                Margin = new Thickness(0, 0, 10, 0),
                TextAlignment = TextAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Style = (Style)FindResource("style_txtBlock_expDisplay")
            };
            budgetInfoWrapper.Children.Add(expInfo);
            Grid.SetColumn(expInfo, 2);
            Image deleteBtnImage = new Image
            {
                Style = (Style)FindResource("style_img_delete"),
                MaxHeight = budgetBtnBorder.Height / 2
            };
            Button deleteBtn = new Button
            {
                Uid = budgetItem.ExpenseId.ToString(),
                Content = deleteBtnImage,
                ToolTip = "Delete",
                Style = (Style)FindResource("style_btn_delete"),
                Template = (ControlTemplate)FindResource("template_btn_delete"),
                Visibility = Visibility.Hidden
            };
            deleteBtn.SizeChanged += DeleteBtn_SizeChanged;
            deleteBtn.Click += DeleteBtn_Click;
            budgetInfoWrapper.Children.Add(deleteBtn);
            Grid.SetColumn(deleteBtn, 3);

            budgetButton.Content = budgetInfoWrapper;

            budgetBtnBorder.Child = budgetButton;

            return budgetBtnBorder;
        }

        private Border BuildCatSummaryButton(BudgetItemsByCategory catSummary)
        {
            Border catSummaryBtnBorder = new Border
            {
                Style = (Style)FindResource("style_border_itemWrapper")
            };

            catSummaryBtnBorder.SizeChanged += BudgetBtnBorder_SizeChanged;
            catSummaryBtnBorder.MouseLeave += BudgetBtnBorder_MouseLeave;
            catSummaryBtnBorder.MouseEnter += BudgetBtnBorder_MouseEnter;

            Button catSummaryButton = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)FindResource("style_btn_item"),
                Template = (ControlTemplate)FindResource("template_btn_budgetItem")
            };

            catSummaryButton.Click += BudgetButton_Click;

            // add info about this cat summary
            Grid catSummaryInfoWrapper = new Grid();
            catSummaryInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition());
            catSummaryInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition());
            catSummaryInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(30)
            });

            TextBlock catSummaryInfo = new TextBlock
            {
                Text = catSummary.CategoryDescription,
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            catSummaryInfoWrapper.Children.Add(catSummaryInfo);
            catSummaryInfo = new TextBlock
            {
                Text = CurrencyFormat(catSummary.Total),
                TextAlignment = TextAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Style = (Style)FindResource("style_txtBlock_expDisplay")
            };
            catSummaryInfoWrapper.Children.Add(catSummaryInfo);
            Grid.SetColumn(catSummaryInfo, 1);

            catSummaryButton.Content = catSummaryInfoWrapper;

            catSummaryBtnBorder.Child = catSummaryButton;

            return catSummaryBtnBorder;
        }

        private Grid BuildExpenseHeader()
        {
            // add the column headers
            Grid headerInfoWrapper = new Grid();
            headerInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(2, GridUnitType.Star)
            });
            headerInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            headerInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            headerInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength()
            });

            TextBlock headerInfo = new TextBlock
            {
                Text = "Description",
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Style = (Style)FindResource("style_txtBlock_header")
            };
            headerInfoWrapper.Children.Add(headerInfo);
            headerInfo = new TextBlock
            {
                Text = "Amount",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 15, 0),
                Style = (Style)FindResource("style_txtBlock_header")
            };
            headerInfoWrapper.Children.Add(headerInfo);
            Grid.SetColumn(headerInfo, 1);
            headerInfo = new TextBlock
            {
                Text = "Balance",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 20, 0),
                Style = (Style)FindResource("style_txtBlock_header")
            };
            headerInfoWrapper.Children.Add(headerInfo);
            Grid.SetColumn(headerInfo, 2);

            return headerInfoWrapper;
        }

        private Grid BuildCatSummaryHeader()
        {
            // add info about this cat summary
            Grid catSummaryInfoWrapper = new Grid();
            catSummaryInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition());
            catSummaryInfoWrapper.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock catSummaryInfo = new TextBlock
            {
                Text = "Description",
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Style = (Style)FindResource("style_txtBlock_header")
            };
            catSummaryInfoWrapper.Children.Add(catSummaryInfo);
            catSummaryInfo = new TextBlock
            {
                Text = "Total",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 40, 0),
                Style = (Style)FindResource("style_txtBlock_header")
            };
            catSummaryInfoWrapper.Children.Add(catSummaryInfo);
            Grid.SetColumn(catSummaryInfo, 1);

            return catSummaryInfoWrapper;
        }

        private void BudgetBtnBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)((Border)sender).Child;

            if (!_expsDisplayedByCategory)
            {
                ((Button)((Grid)button.Content).Children[3]).Visibility = Visibility.Hidden;
            }

            if (_currentSelectedExpBtn != button)
            {
                button.Background = GUIConstants.NormalButtonBackgroundOnMouseLeave;
            }
        }

        private void BudgetBtnBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)((Border)sender).Child;

            if (!_expsDisplayedByCategory)
            {
                ((Button)((Grid)button.Content).Children[3]).Visibility = Visibility.Visible;
            }

            if (_currentSelectedExpBtn != button)
            {
                button.Background = GUIConstants.NormalButtonBackgroundOnMouseEnter;
            }
        }

        private void BudgetBtnBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // if expenses are dsiplayed within a day range or by cats
            if (_currentDisplayedStartDate != null)
            {
                // avoid recursion
                if (_numLoadedItems == control_dataGridView.stckPanel_itemList.Children.Count)
                {
                    return;
                }

                _numLoadedItems++;
            }

            // when all items are loaded with their own size
            if (_numLoadedItems == control_dataGridView.stckPanel_itemList.Children.Count)
            {
                // resize elements on display to window size
                ResizeDisplayedElementsToWindowSize();

                // scroll to the selected expense
                ScrollToSelectedExpense();
            }         
        }

        private void ResizeDisplayedElementsToWindowSize()
        {
            Grid headerInfoWrapper = (Grid)control_dataGridView.border_header.Child;
            double deleteButtonSize = 0;

            // if scrollbar is visible
            if (control_dataGridView.scroll_itemList.ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                // resize elements to give space for the scrollbar

                // if displaying exps within selected days or by cats
                if (_currentDisplayedStartDate != null)
                {
                    foreach (Border budgetBtnBorder in control_dataGridView.stckPanel_itemList.Children)
                    {
                        Grid budgetInfoWrapper = (Grid)((Button)budgetBtnBorder.Child).Content;

                        // resize budget button
                        budgetInfoWrapper.Width = window.ActualWidth - SystemParameters.VerticalScrollBarWidth;
                        // resize headers wrapper
                        headerInfoWrapper.Width = window.ActualWidth - SystemParameters.VerticalScrollBarWidth;

                        // if displaying exps within selected days
                        if (deleteButtonSize == 0 && _expsDisplayedWithinSelectedDays)
                        {
                            // get delete button size
                            deleteButtonSize = budgetInfoWrapper.ColumnDefinitions[3].ActualWidth;
                        }
                    }
                }
                // if displaying exps by month
                else if (_currentDisplayedMonth > -1)
                {
                    foreach (StackPanel dayWrapper in control_dataGridView.stckPanel_itemList.Children)
                    {
                        foreach (Border budgetBtnBorder in ((StackPanel)((Border)dayWrapper.Children[1]).Child).Children)
                        {
                            Grid budgetInfoWrapper = (Grid)((Button)budgetBtnBorder.Child).Content;

                            // resize budget button
                            budgetInfoWrapper.Width = window.ActualWidth - SystemParameters.VerticalScrollBarWidth;
                            // resize headers wrapper
                            headerInfoWrapper.Width = window.ActualWidth - SystemParameters.VerticalScrollBarWidth;

                            if (deleteButtonSize == 0)
                            {
                                // get delete button size
                                deleteButtonSize = budgetInfoWrapper.ColumnDefinitions[3].ActualWidth;
                            }
                        }
                    }
                }
            }
            // if scrollbar is not visible
            else
            {
                if (_currentDisplayedStartDate != null)
                {
                    foreach (Border budgetBtnBorder in control_dataGridView.stckPanel_itemList.Children)
                    {
                        Grid budgetInfoWrapper = (Grid)((Button)budgetBtnBorder.Child).Content;

                        // resize budget button
                        budgetInfoWrapper.Width = window.ActualWidth;
                        // resize headers wrapper
                        headerInfoWrapper.Width = window.ActualWidth;

                        // if displaying exps within selected days
                        if (deleteButtonSize == 0 && _expsDisplayedWithinSelectedDays)
                        {
                            // get delete button size
                            deleteButtonSize = budgetInfoWrapper.ColumnDefinitions[3].ActualWidth;
                        }
                    }
                }
                else if (_currentDisplayedMonth > -1)
                {
                    foreach (StackPanel dayWrapper in control_dataGridView.stckPanel_itemList.Children)
                    {
                        foreach (Border budgetBtnBorder in ((StackPanel)((Border)dayWrapper.Children[1]).Child).Children)
                        {
                            Grid budgetInfoWrapper = (Grid)((Button)budgetBtnBorder.Child).Content;

                            // resize budget button
                            budgetInfoWrapper.Width = window.ActualWidth;
                            // resize headers wrapper
                            headerInfoWrapper.Width = window.ActualWidth;

                            if (deleteButtonSize == 0)
                            {
                                // get delete button size
                                deleteButtonSize = budgetInfoWrapper.ColumnDefinitions[3].ActualWidth;
                            }
                        }
                    }
                }
            }

            // if displaying within selected days or by month (when delete buttons are available)
            if (_expsDisplayedWithinSelectedDays || _currentDisplayedMonth > -1)
            {
                // resize headers to algin with delete buttons
                headerInfoWrapper.ColumnDefinitions[3].Width = new GridLength(deleteButtonSize);
            }
        }

        private void ScrollToSelectedExpense()
        {
            if (_currentSelectedExpBtn == null)
            {
                return;
            }

            double destination = 0;

            if (_expsDisplayedWithinSelectedDays)
            {
                foreach (Border budgetBtnBorder in control_dataGridView.stckPanel_itemList.Children)
                {
                    Button budgetBtn = (Button)budgetBtnBorder.Child;
                    if (budgetBtn == _currentSelectedExpBtn)
                    {
                        break;
                    }

                    destination += budgetBtnBorder.ActualHeight;
                }
            }
            else if (_currentDisplayedMonth > -1)
            {
                bool foundSelectedExpBtn = false;
                foreach (StackPanel dayWrapper in control_dataGridView.stckPanel_itemList.Children)
                {
                    StackPanel budgetsWrapper = (StackPanel)((Border)dayWrapper.Children[1]).Child;

                    foreach (Border budgetBtnBorder in budgetsWrapper.Children)
                    {
                        Button budgetBtn = (Button)budgetBtnBorder.Child;
                        if (budgetBtn == _currentSelectedExpBtn)
                        {
                            foundSelectedExpBtn = true;
                            break;
                        }

                        destination += budgetBtnBorder.ActualHeight;
                    }

                    // add dayHeading's height
                    destination += ((Border)dayWrapper.Children[0]).ActualHeight;

                    if (foundSelectedExpBtn) break;

                    // add dayWrapper's bottom margin
                    destination += GUIConstants.DayWrapperMargin.Bottom;
                }
            }

            control_dataGridView.scroll_itemList.ScrollToVerticalOffset(destination);
        }

        private void DeleteBtn_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Button deleteBtn = (Button)sender;
            deleteBtn.Width = deleteBtn.ActualHeight;
        }

        private void BudgetButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Button selectedButton = (Button)sender;
            _currentSelectedExpBtn = null;
            // focus the selected button
            FocusExpense(selectedButton);

            Presenter.BudgetButton_OnDoubleClicked(int.Parse((selectedButton).Uid));
        }

        // FROM INTERFACE
        public void ShowExpenseFullDetails(int expId, HomeBudget budget)
        {
            // display full details about the clicked expense
            Expense selectedExp = budget.Expenses.GetExpenseById(expId);
            Category category = budget.Categories.GetList().Find(cat => cat.Id == selectedExp.CategoryId);
            ExpenseInfoWindow expenseInfoWindow = new ExpenseInfoWindow(budget, selectedExp, category);
            // The window will return true if the expense was updated
            if (expenseInfoWindow.ShowDialog() == true)
            {
                _lastEnteredOrEditedExpId = expenseInfoWindow.LastEditedExpenseId;
                ReloadCategoryFilterCombobox(budget);
                ReloadItemList();
            }
        }

        private void BudgetButton_Click(object sender, RoutedEventArgs e)
        {
            // focus the selected button
            FocusExpense((Button)sender);
        }

        private void FocusExpense(Button selectedButton)
        {
            if (selectedButton == null)
            {
                return;
            }

            _focusChangedByUserDuringTraversal = true;

            // if the selected button is already focused
            if (_currentSelectedExpBtn == selectedButton)
            {
                // unfocus that button

                // reset background of the previously selected button
                if (selectedButton.IsMouseOver)
                {
                    _currentSelectedExpBtn.Background = GUIConstants.NormalButtonBackgroundOnMouseEnter;
                }
                else
                {
                    _currentSelectedExpBtn.Background = GUIConstants.NormalButtonBackgroundOnMouseLeave;
                }

                _currentSelectedExpBtn.FontWeight = GUIConstants.NormalButtonFontWeight;
                _currentSelectedExpBtn = null;
            }
            else
            {
                if (_currentSelectedExpBtn != null)
                {
                    // reset background of the previously selected button
                    _currentSelectedExpBtn.Background = GUIConstants.NormalButtonBackgroundOnMouseLeave;
                    _currentSelectedExpBtn.FontWeight = GUIConstants.NormalButtonFontWeight;
                }

                _currentSelectedExpBtn = selectedButton;
                selectedButton.Background = Brushes.NavajoWhite;
                selectedButton.FontWeight = FontWeights.SemiBold;
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // Prevents the click event from bubbling up the UI and trigger the BudgetButton_Click
            e.Handled = true;

            Presenter.DeleteButton_OnClicked(int.Parse(((Button)sender).Uid));
        }

        // FROM INTERFACE
        public bool ShowDeleteWarning(string expDescription)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete " + expDescription + "?",
                "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }

        // -----------------------------
        // Search
        // -----------------------------

        public void Executed_OpenSearchTextbox(object sender, ExecutedRoutedEventArgs e)
        {
            if (txtBox_find.IsEnabled)
            {
                _searchModeTurnedBackOn = true;

                // if there are matches
                if (_matchedBudgetBtns != null && _matchedBudgetBtns.Count > 0)
                {
                    // un-highlight every matche
                    UnhighlightMatches();
                }

                txtBox_find.Text = "Ctrl+F to find";
                txtBox_find.IsEnabled = false;
                btn_find.IsEnabled = false;
            }
            else
            {
                txtBox_find.Text = _currentKeyword ?? "";
                txtBox_find.IsEnabled = true;
                btn_find.IsEnabled = true;
                txtBox_find.Focus();
            }
        }

        public void CanExecute_OpenSearchTextbox(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void txtBox_find_TextChanged(object sender, TextChangedEventArgs e)
        {
            // if there are matches
            if (_matchedBudgetBtns != null && _matchedBudgetBtns.Count > 0)
            {
                // un-highlight every matche
                UnhighlightMatches();
            }
        }

        private void UnhighlightMatches()
        {
            foreach (Button btn in _matchedBudgetBtns)
            {
                TextBlock txtBlock_desc = (TextBlock)((Grid)btn.Content).Children[0];
                TextBlock txtBlock_amt = (TextBlock)((Grid)btn.Content).Children[1];
                TextBlockHighlighter.SetHighlightColor(txtBlock_desc, null);
                TextBlockHighlighter.SetHighlightColor(txtBlock_amt, null);
            }
        }

        private void txtBox_find_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }

        private void ResetSearchStates()
        {
            if (!_expsSummaryDisplayed)
            {
                // if this is the very first search
                if (_matchedBudgetBtns == null)
                {
                    _matchedBudgetBtns = new List<Button>();
                }
                else
                {
                    _matchedBudgetBtns.Clear();
                }
            }         
            else
            {
                // if this is the very first search
                if (_matchedSummaryRows == null)
                {
                    _matchedSummaryRows = new List<IDictionary<string, object>>();
                }
                else
                {
                    _matchedSummaryRows.Clear();
                }
            }

            _searchModeTurnedBackOn = false;
            _currentMatchedIndex = -1;
            _traversalStartIndex = -1;
            _reachedTraversalStart = false;
        }

        private bool RequiresSearching(string keyword)
        {
            // if there are items in the list AND
            // search mode is turned back on
            // or if the user changed the focus during traversal
            // or if the recently input keyword is different from the keyword stored in memory
            // (the keyword stored in memory is cleared whenever the item list is reloaded)
            return (_expsSummaryDisplayed || (!_expsSummaryDisplayed && _numLoadedItems > 0)) && 
                (_searchModeTurnedBackOn || _focusChangedByUserDuringTraversal || keyword != _currentKeyword);
        }

        private void SearchThroughExpensesWithinSelectedDaysOrByCategory()
        {
            // reset all current search states
            ResetSearchStates();

            // start the search

            bool passedSelectedBtn = false;
            foreach (Border budgetBtnBorder in control_dataGridView.stckPanel_itemList.Children)
            {
                Button budgetBtn = (Button)budgetBtnBorder.Child;

                if (budgetBtn == _currentSelectedExpBtn)
                {
                    passedSelectedBtn = true;
                }

                TextBlock txtBlock_desc = (TextBlock)((Grid)budgetBtn.Content).Children[0];
                TextBlock txtBlock_amt = (TextBlock)((Grid)budgetBtn.Content).Children[1];
                string desc = txtBlock_desc.Text.ToLower();
                string amt = txtBlock_amt.Text.ToLower();
                bool matched = false;

                if (desc.Contains(_currentKeyword))
                {
                    matched = true;
                    // highlight the keyword
                    TextBlockHighlighter.SetHighlightColor(txtBlock_desc, Brushes.Yellow);
                    TextBlockHighlighter.SetSelection(txtBlock_desc, _currentKeyword);
                }
                if (amt.Contains(_currentKeyword))
                {
                    matched = true;
                    // highlight the keyword
                    TextBlockHighlighter.SetHighlightColor(txtBlock_amt, Brushes.Yellow);
                    TextBlockHighlighter.SetSelection(txtBlock_amt, _currentKeyword);
                }

                if (matched)
                {
                    _matchedBudgetBtns.Add(budgetBtn);

                    // if this is the first matched result below the current selected button
                    if (_traversalStartIndex == -1 && passedSelectedBtn)
                    {
                        // mark the traversal start
                        _traversalStartIndex = _matchedBudgetBtns.Count - 1;
                    }
                }
            }
        }

        private void SearchThroughExpensesByMonth()
        {
            // reset all current search states
            ResetSearchStates();

            // start the search

            bool passedSelectedBtn = false;
            foreach (StackPanel dayWrapper in control_dataGridView.stckPanel_itemList.Children)
            {
                StackPanel budgetsWrapper = (StackPanel)((Border)dayWrapper.Children[1]).Child;

                foreach (Border budgetBtnBorder in budgetsWrapper.Children)
                {
                    Button budgetBtn = (Button)budgetBtnBorder.Child;

                    if (budgetBtn == _currentSelectedExpBtn)
                    {
                        passedSelectedBtn = true;
                    }

                    TextBlock txtBlock_desc = (TextBlock)((Grid)budgetBtn.Content).Children[0];
                    TextBlock txtBlock_amt = (TextBlock)((Grid)budgetBtn.Content).Children[1];
                    string desc = txtBlock_desc.Text.ToLower();
                    string amt = txtBlock_amt.Text.ToLower();
                    bool matched = false;

                    if (desc.Contains(_currentKeyword))
                    {
                        matched = true;
                        // highlight the keyword
                        TextBlockHighlighter.SetHighlightColor(txtBlock_desc, Brushes.Yellow);
                        TextBlockHighlighter.SetSelection(txtBlock_desc, _currentKeyword);
                    }
                    if (amt.Contains(_currentKeyword))
                    {
                        matched = true;
                        // highlight the keyword
                        TextBlockHighlighter.SetHighlightColor(txtBlock_amt, Brushes.Yellow);
                        TextBlockHighlighter.SetSelection(txtBlock_amt, _currentKeyword);
                    }

                    if (matched)
                    {
                        _matchedBudgetBtns.Add(budgetBtn);

                        // if this is the first matched result below the current selected button
                        if (_traversalStartIndex == -1 && passedSelectedBtn)
                        {
                            // mark the traversal start
                            _traversalStartIndex = _matchedBudgetBtns.Count - 1;
                        }
                    }
                }
            }
        }

        private void FocusNextMatchedExpenseButton()
        {
            if (_matchedBudgetBtns == null || _matchedBudgetBtns.Count == 0)
            {
                MessageBox.Show("The given keyword was not found!", "No match", MessageBoxButton.OK);
                return;
            }
            else
            {
                if (_reachedTraversalStart)
                {
                    MessageBox.Show("You have gone through all matches!", "End of search is reached", MessageBoxButton.OK);
                    _reachedTraversalStart = false;
                }

                // if current matched index is not set
                if (_currentMatchedIndex == -1)
                {
                    _currentMatchedIndex = _traversalStartIndex = _traversalStartIndex == -1 ? 0 : _traversalStartIndex;
                }

                // unfocus the current selected exp
                FocusExpense(_currentSelectedExpBtn);

                // focus the next matched result and increment current matched index
                FocusExpense(_matchedBudgetBtns[_currentMatchedIndex++]);

                // because the program is changing the focus by itself
                _focusChangedByUserDuringTraversal = false;

                // if the index is out of bound
                if (_currentMatchedIndex > _matchedBudgetBtns.Count - 1)
                {
                    // reset index to 0 
                    _currentMatchedIndex = 0;
                }

                // if coming back to start
                if (_currentMatchedIndex == _traversalStartIndex)
                {
                    _reachedTraversalStart = true;
                }

                ScrollToSelectedExpense();
            }
        }

        private void SearchThroughSummaryGrid(DataGrid summaryGrid)
        {
            // reset all current search states
            ResetSearchStates();

            // start the search

            bool passedSelectedBtn = false;
            var rows = summaryGrid.Items;
            foreach (IDictionary<string, object> row in rows.Cast<IDictionary<string, object>>().ToList())
            {
                if (row == summaryGrid.SelectedItem)
                {
                    passedSelectedBtn = true;
                }

                if ((row.ContainsKey("Month") && ((string)row["Month"]).Contains(_currentKeyword)) || 
                    (row.ContainsKey("Total") && CurrencyFormat(Convert.ToDouble(row["Total"])).Contains(_currentKeyword)))
                {
                    _matchedSummaryRows.Add(row);

                    // if this is the first matched result below the current selected button
                    if (_traversalStartIndex == -1 && passedSelectedBtn)
                    {
                        // mark the traversal start
                        _traversalStartIndex = _matchedSummaryRows.Count - 1;
                    }
                }
                else
                {
                    // temporarily access model from Presenter
                    foreach (Category cat in Presenter.Budget.Categories.GetList())
                    {
                        if (row.ContainsKey(cat.Description) && CurrencyFormat(Convert.ToDouble(row[cat.Description])).Contains(_currentKeyword))
                        {
                            _matchedSummaryRows.Add(row);

                            // if this is the first matched result below the current selected button
                            if (_traversalStartIndex == -1 && passedSelectedBtn)
                            {
                                // mark the traversal start
                                _traversalStartIndex = _matchedSummaryRows.Count - 1;
                            }

                            break;
                        }
                    }
                }
            }
        }

        private void FocusNextMatchedDataGridRow(DataGrid summaryGrid)
        {
            if (_matchedSummaryRows.Count == 0)
            {
                MessageBox.Show("The given keyword was not found", "No match", MessageBoxButton.OK);
                return;
            }
            else
            {
                if (_reachedTraversalStart)
                {
                    MessageBox.Show("You have gone through all matches!", "Start of search is reached", MessageBoxButton.OK);
                    _reachedTraversalStart = false;
                }

                // if current matched index is not set
                if (_currentMatchedIndex == -1)
                {
                    _currentMatchedIndex = _traversalStartIndex = _traversalStartIndex == -1 ? 0 : _traversalStartIndex;
                }

                // focus the next matched result and increment current matched index
                summaryGrid.SelectedItem = _matchedSummaryRows[_currentMatchedIndex++];
                var row = (DataGridRow)summaryGrid.ItemContainerGenerator.ContainerFromIndex(summaryGrid.SelectedIndex);
                row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                summaryGrid.ScrollIntoView(row);

                // because the program is changing the focus by itself
                _focusChangedByUserDuringTraversal = false;

                // if the idex is out of bound
                if (_currentMatchedIndex > _matchedSummaryRows.Count - 1)
                {
                    // reset index to 0 
                    _currentMatchedIndex = 0;
                }

                // if coming back to start
                if (_currentMatchedIndex == _traversalStartIndex)
                {
                    _reachedTraversalStart = true;
                }
            }
        }

        private void btn_find_Click(object sender, RoutedEventArgs e)
        {
            FindNext();
        }

        private void FindNext()
        {
            string keyword = txtBox_find.Text.ToLower();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return;
            }

            if (!_expsSummaryDisplayed)
            {
                // if displaying exps within SELECTED DAYS or BY CATS
                if (_currentDisplayedStartDate != null)
                {
                    if (RequiresSearching(keyword))
                    {
                        _currentKeyword = keyword;

                        SearchThroughExpensesWithinSelectedDaysOrByCategory();
                    }
                }
                // if displaying exps BY MONTH
                else if (_currentDisplayedMonth > -1)
                {
                    if (RequiresSearching(keyword))
                    {
                        _currentKeyword = keyword;

                        SearchThroughExpensesByMonth();
                    }
                }

                // focus next match
                FocusNextMatchedExpenseButton();
            }
            // if displaying exp SUMMARY
            else
            {
                DataGrid summaryGrid = (DataGrid)control_dataGridView.stckPanel_itemList.Children[0];

                if (RequiresSearching(keyword))
                {
                    _searchModeTurnedBackOn = false;
                    _currentKeyword = keyword;

                    // start the search
                    SearchThroughSummaryGrid(summaryGrid);
                }

                // focus next match
                FocusNextMatchedDataGridRow(summaryGrid);
            }
        }

        // FROM INTERFACE
        public void ChangeDisplayedDateInDatePicker(DateTime? firstDate, DateTime? lastDate)
        {
            if (_currentDisplayedStartDate != firstDate)
            {
                SetUpForDateChange();
            }
            ((DatePicker)FindName("datePicker_StartDate")).SelectedDate = firstDate;
            CleanUpAfterDateChange();

            if (_currentDisplayedEndDate != lastDate)
            {
                SetUpForDateChange();
            }
            ((DatePicker)FindName("datePicker_EndDate")).SelectedDate = lastDate;
            CleanUpAfterDateChange();
        }

        // -----------------------------
        // Display Budget Items Within selected days
        // -----------------------------

        // FROM INTERFACE
        public void SetupStandardDisplay()
        {
            _expsDisplayedWithinSelectedDays = true;

            if (_currentDisplayedStartDate == null)
            {
                AddDatePickers();
            }
        }

        // FROM INTERFACE
        public void InitializeStandardDisplay()
        {
            LoadExpensesWithinSelectedDays();
        }

        private void AddDatePickers()
        {
            // remove unique names for the datePickers if they were used previously
            if (FindName("datePicker_StartDate") != null)
                UnregisterName("datePicker_StartDate");
            if (FindName("datePicker_EndDate") != null)
                UnregisterName("datePicker_EndDate");
            if (FindName("chkBox_ShowAll") != null)
                UnregisterName("chkBox_ShowAll");

            // by default, the date pickers will be set such that every expense is displayed

            TextBlock datePickerLabel = new TextBlock
            {
                Text = "From:",
                ToolTip = "Start date",
                Style = (Style)FindResource("style_txtBlock_heading"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
            control_dataGridView.stckPanel_filterWrapper.Children.Add(datePickerLabel);

            // add start date filter
            DatePicker datePicker = new DatePicker
            {
                SelectedDate = _currentDisplayedStartDate,
                ToolTip = "Start date",
                MinWidth = 100,
                Resources = (ResourceDictionary)FindResource("datePicker_DateFilter"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
            datePicker.SelectedDateChanged += startDate_SelectedDateChanged;
            datePicker.CalendarOpened += DatePicker_CalendarOpened;
            control_dataGridView.stckPanel_filterWrapper.Children.Add(datePicker);
            RegisterName("datePicker_StartDate", datePicker);

            datePickerLabel = new TextBlock
            {
                Text = "To:",
                ToolTip = "End date",
                Style = (Style)FindResource("style_txtBlock_heading"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
            control_dataGridView.stckPanel_filterWrapper.Children.Add(datePickerLabel);

            // add end date filter
            datePicker = new DatePicker
            {
                // pick end date as one week 
                SelectedDate = _currentDisplayedEndDate,
                ToolTip = "End date",
                MinWidth = 100,
                Resources = (ResourceDictionary)FindResource("datePicker_DateFilter"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
            datePicker.SelectedDateChanged += endDate_SelectedDateChanged;
            datePicker.CalendarOpened += DatePicker_CalendarOpened;
            control_dataGridView.stckPanel_filterWrapper.Children.Add(datePicker);
            RegisterName("datePicker_EndDate", datePicker);

            // add checkbox to display all expenses
            CheckBox chkBoxShowAll = new CheckBox
            {
                Content = "Show All Records",
                ToolTip = "Show all records in the database",
                FontSize = 18,
                FontWeight = FontWeights.Light,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
            RegisterName("chkBox_ShowAll", chkBoxShowAll);
            chkBoxShowAll.Checked += chkBoxShowAll_Checked;
            chkBoxShowAll.Unchecked += chkBoxShowAll_Unchecked;
            chkBoxShowAll.IsChecked = true;
            control_dataGridView.stckPanel_filterWrapper.Children.Add(chkBoxShowAll);
        }

        private void DatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            _calendarOpenedByUser = true;
        }

        private void chkBoxShowAll_Checked(object sender, RoutedEventArgs e)
        {
            _showAllExps = true;
            ReloadItemList();
        }

        private void chkBoxShowAll_Unchecked(object sender, RoutedEventArgs e)
        {
            _showAllExps = false;

            if (DateWasChangedByUser()) 
            {
                _calendarOpenedByUser = false;
                return;
            }

            // change the date and reload item list
            DateTime startDate = DateTime.Today;
            DateTime endDate = startDate.AddDays(6);
            if (_currentDisplayedStartDate != startDate)
            {
                SetUpForDateChange();
            }
            ((DatePicker)FindName("datePicker_StartDate")).SelectedDate = DateTime.Today;
            CleanUpAfterDateChange();

            if (_currentDisplayedEndDate != endDate)
            {
                SetUpForDateChange();
            }
            ((DatePicker)FindName("datePicker_EndDate")).SelectedDate = endDate;
            CleanUpAfterDateChange();
        }

        private void SetUpForDateChange()
        {
            _firstSelectedDateChangedCall = true;
            _dateChangedByProgram = true;
        }

        private void CleanUpAfterDateChange()
        {
            _firstSelectedDateChangedCall = true;
            _dateChangedByProgram = false;
            _calendarOpenedByUser = false;
        }

        private bool DateWasChangedByUser()
        {
            return !_dateChangedByProgram && _calendarOpenedByUser;
        }

        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dateChangedByProgram)
            {
                if (_firstSelectedDateChangedCall) _firstSelectedDateChangedCall = false;
                else
                {
                    _firstSelectedDateChangedCall = true;
                    return;
                }
            }

            if (_showAllExps)
            {
                if (DateWasChangedByUser()) ((CheckBox)FindName("chkBox_ShowAll")).IsChecked = false;
            }

            DateTime? selectedDate = ((DatePicker)sender).SelectedDate;

            _selectedDate = selectedDate;
            _currentDisplayedStartDate = selectedDate;

            if (!_showAllExps) ReloadItemList();
        }

        private void endDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dateChangedByProgram)
            {
                if (_firstSelectedDateChangedCall) _firstSelectedDateChangedCall = false;
                else
                {
                    _firstSelectedDateChangedCall = true;
                    return;
                }
            }

            if (_showAllExps)
            {
                if (DateWasChangedByUser()) ((CheckBox)FindName("chkBox_ShowAll")).IsChecked = false;
            }

            DateTime? selectedDate = ((DatePicker)sender).SelectedDate;

            _currentDisplayedEndDate = selectedDate;

            if (!_showAllExps) ReloadItemList();
        }

        private void LoadExpensesWithinSelectedDays()
        {
            control_dataGridView.border_header.Child = null;
            control_dataGridView.border_header.Height = GUIConstants.ItemWrapperBorderHeight;

            if (!ItemListIsEmpty())
            {
                DataClear();
            }   

            if (DataSource.Count == 0)
            {
                if (_showAllExps)
                {
                    control_dataGridView.stckPanel_itemList.Children.Add(new TextBlock
                    {
                        Text = "No records in the database",
                        Style = (Style)FindResource("style_txtBlock_heading"),
                        Margin = new Thickness(10, 0, 0, 0)
                    });
                }
                else
                {
                    control_dataGridView.stckPanel_itemList.Children.Add(new TextBlock
                    {
                        Text = "No records in this day range",
                        Style = (Style)FindResource("style_txtBlock_heading"),
                        Margin = new Thickness(10, 0, 0, 0)
                    });
                }
            }
            else
            {
                control_dataGridView.border_header.Child = BuildExpenseHeader();
                foreach (BudgetItem budgetItem in DataSource.Cast<BudgetItem>().ToList())
                {
                    Border budgetBtnBorder = BuildBudgetButton(budgetItem);
                    control_dataGridView.stckPanel_itemList.Children.Add(budgetBtnBorder);

                    if (budgetItem.ExpenseId == _lastEnteredOrEditedExpId)
                    {
                        FocusExpense((Button)budgetBtnBorder.Child);
                        _lastEnteredOrEditedExpId = -1;
                    }
                }
            }
        }

        // -----------------------------
        // Display Budget Items By Month
        // -----------------------------

        // FROM INTERFACE
        public void SetupByMonthDisplay()
        {
            if (_selectedDate == null)
            {
                _selectedDate = DateTime.Today;
            }

            // add month filter
            DatePicker datePicker = new DatePicker
            {
                SelectedDate = _selectedDate,
                Resources = (ResourceDictionary)FindResource("datePicker_DateFilter"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
            datePicker.SelectedDateChanged += monthFilter_SelectedDateChanged;
            control_dataGridView.stckPanel_filterWrapper.Children.Add(datePicker);

            _currentDisplayedMonth = ((DateTime)_selectedDate).Month;
        }

        // FROM INTERFACE
        public void InitializeByMonthDisplay()
        {
            LoadExpensesByChosenMonth((DateTime)_selectedDate);
        }

        // FROM INTERFACE
        public void SetCurrentDisplayedMonth(int month)
        {
            _currentDisplayedMonth = month;
        }

        private void LoadExpensesByChosenMonth(DateTime chosenDate)
        {
            control_dataGridView.border_header.Child = null;
            control_dataGridView.border_header.Height = GUIConstants.ItemWrapperBorderHeight;

            if (control_dataGridView.stckPanel_filterWrapper.Children.Count == 3)
            {
                control_dataGridView.stckPanel_filterWrapper.Children.RemoveAt(2);
            }

            if (!ItemListIsEmpty())
            {
                DataClear();
            }
            
            if (DataSource.Count == 0)
            {
                control_dataGridView.stckPanel_itemList.Children.Add(new TextBlock
                {
                    Text = "No records in this month",
                    Style = (Style)FindResource("style_txtBlock_heading"),
                    Margin = new Thickness(10, 0, 0, 0)
                });
            }
            else
            {
                List<BudgetItemsByMonth> results = DataSource.Cast<BudgetItemsByMonth>().ToList();

                control_dataGridView.stckPanel_filterWrapper.Children.Add(new TextBlock
                {
                    Text = $"Total Expense in {results[0].Month}: {CurrencyFormat(results[0].Total)}",
                    Style = (Style)FindResource("style_txtBlock_heading"),
                    FontWeight = FontWeights.Regular,
                    Margin = new Thickness(30, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                });

                List<BudgetItem> budgetItemsInChosenMonth = results[0].Details;

                StackPanel dayWrapper;
                Border dayHeadingBorder;
                StackPanel dayHeading;
                Border budgetsWrapperBorder;
                StackPanel budgetsWrapper = null;

                int currentDay = 0;
                control_dataGridView.border_header.Child = BuildExpenseHeader();
                foreach (BudgetItem budgetItem in budgetItemsInChosenMonth)
                {
                    if (currentDay != budgetItem.Date.Day)
                    {
                        currentDay = budgetItem.Date.Day;

                        // add a wrapper for the current day
                        dayWrapper = new StackPanel
                        {
                            Uid = currentDay.ToString(),
                            Margin = GUIConstants.DayWrapperMargin
                        };
                        dayWrapper.SizeChanged += DayWrapper_SizeChanged;
                        control_dataGridView.stckPanel_itemList.Children.Add(dayWrapper);

                        // add heading for the current day
                        dayHeadingBorder = new Border
                        {
                            Style = (Style)FindResource("style_border_dayHeading")
                        };
                        dayHeading = new StackPanel
                        {
                            Orientation = Orientation.Horizontal
                        };
                        dayHeading.Children.Add(new TextBlock
                        {
                            Text = budgetItem.Date.ToString("dddd, dd MMMM yyyy", GUIConstants.CurrentCulture),
                            Style = (Style)FindResource("style_txtBlock_label"),
                            FontStyle = FontStyles.Italic,
                            FontWeight = FontWeights.Regular,
                            Margin = new Thickness(10, 5, 0, 5)
                        });
                        dayHeadingBorder.Child = dayHeading;
                        dayWrapper.Children.Add(dayHeadingBorder);

                        // add a wrapper for all budget items in this day
                        budgetsWrapperBorder = new Border
                        {
                            BorderBrush = Brushes.LightGray,
                            BorderThickness = new Thickness(0, 0, 0, 1)
                        };
                        budgetsWrapper = new StackPanel();
                        budgetsWrapperBorder.Child = budgetsWrapper;
                        dayWrapper.Children.Add(budgetsWrapperBorder);

                        // add button for this budget item
                        Border budgetBtnBorder = BuildBudgetButton(budgetItem);
                        budgetsWrapper.Children.Add(budgetBtnBorder);

                        if (budgetItem.ExpenseId == _lastEnteredOrEditedExpId)
                        {
                            FocusExpense((Button)budgetBtnBorder.Child);
                            _lastEnteredOrEditedExpId = -1;
                        }
                    }
                    else
                    {
                        // add button for this budget item
                        Border budgetBtnBorder = BuildBudgetButton(budgetItem);
                        budgetsWrapper.Children.Add(budgetBtnBorder);

                        if (budgetItem.ExpenseId == _lastEnteredOrEditedExpId)
                        {
                            FocusExpense((Button)budgetBtnBorder.Child);
                            _lastEnteredOrEditedExpId = -1;
                        }
                    }
                }
            }
        }

        private void DayWrapper_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_numLoadedItems < control_dataGridView.stckPanel_itemList.Children.Count)
            {
                _numLoadedItems++;
            }

            ((StackPanel)sender).Height = ((StackPanel)sender).ActualHeight;

            // when all items are loaded
            if (_numLoadedItems == control_dataGridView.stckPanel_itemList.Children.Count)
            {
                ScrollToSelectedDate();
                ScrollToSelectedExpense();
            }
        }

        private void monthFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime selectedDate = (DateTime)((DatePicker)sender).SelectedDate;

            if (selectedDate.Month != _currentDisplayedMonth)
            {
                _selectedDate = selectedDate;
                ReloadItemList();
            }

            if (_numLoadedItems > 0)
            {
                ScrollToSelectedDate();
            }
        }

        private void ScrollToSelectedDate()
        {
            bool selectedDayIsInTheList = false;
            double selectedDayWrapperPosition = 0;
            string selectedDay = ((DateTime)_selectedDate).Day.ToString();

            for (int i = 0; i < control_dataGridView.stckPanel_itemList.Children.Count; i++)
            {
                if (control_dataGridView.stckPanel_itemList.Children[i].Uid == selectedDay)
                {
                    selectedDayIsInTheList = true;
                    break;
                }

                selectedDayWrapperPosition += ((StackPanel)control_dataGridView.stckPanel_itemList.Children[i]).ActualHeight + GUIConstants.DayWrapperMargin.Bottom;
            }

            if (selectedDayIsInTheList)
            {
                control_dataGridView.scroll_itemList.ScrollToVerticalOffset(selectedDayWrapperPosition);
            }
        }

        // -----------------------------
        // Display Budget Items By Category
        // -----------------------------

        // FROM INTERFACE
        public void SetupByCategoryDisplay()
        {
            _expsDisplayedByCategory = true;

            if (_currentDisplayedStartDate == null)
            {
                AddDatePickers();
            }
        }

        // FROM INTERFACE
        public void InitializeByCategoryDisplay()
        {
            LoadExpensesByCategory();
        }

        private void LoadExpensesByCategory()
        {
            control_dataGridView.border_header.Child = null;
            control_dataGridView.border_header.Height = GUIConstants.ItemWrapperBorderHeight;

            if (!ItemListIsEmpty())
            {
                DataClear();
            }

            if (DataSource == null)
            {
                control_dataGridView.stckPanel_itemList.Children.Add(new TextBlock
                {
                    Text = "No records in the database",
                    Style = (Style)FindResource("style_txtBlock_heading"),
                    Margin = new Thickness(10, 0, 0, 0)
                });
            }
            else if (DataSource.Count == 0)
            {
                control_dataGridView.stckPanel_itemList.Children.Add(new TextBlock
                {
                    Text = "No records in this day range",
                    Style = (Style)FindResource("style_txtBlock_heading"),
                    Margin = new Thickness(10, 0, 0, 0)
                });
            }
            else
            {
                List<BudgetItemsByCategory> catSummaries = DataSource.Cast<BudgetItemsByCategory>().ToList();
                control_dataGridView.border_header.Child = BuildCatSummaryHeader();
                foreach (BudgetItemsByCategory catSummary in catSummaries)
                {
                    control_dataGridView.stckPanel_itemList.Children.Add(BuildCatSummaryButton(catSummary));
                }
            }
        }

        // -----------------------------
        // Display Budget Items Summary
        // -----------------------------

        // FROM INTERFACE
        public void SetupByCategoryAndMonthDisplay()
        {
            _expsSummaryDisplayed = true;
        }

        // FROM INTERFACE
        public void InitializeByCategoryAndMonthDisplay(List<string> categoryNames)
        {
            LoadExpenseSummary(categoryNames);
        }

        private void LoadExpenseSummary(List<string> categoryNames)
        {
            control_dataGridView.border_header.Child = null;
            control_dataGridView.border_header.Height = 0;

            if (!ItemListIsEmpty())
            {
                DataClear();
            }

            DataGrid summaryGrid = new DataGrid
            {
                IsReadOnly = true,
                AutoGenerateColumns = false,
                IsTextSearchEnabled = true,
                IsTextSearchCaseSensitive = false
            };
            summaryGrid.SelectionChanged += SummaryGrid_SelectionChanged;
            control_dataGridView.stckPanel_itemList.Children.Add(summaryGrid);
            summaryGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Month",
                Binding = new Binding("[Month]"),
                CellStyle = (Style)FindResource("style_cell_firstColumn")
            });

            Binding colBinding;
            // if showing all categories
            if (_currentTargetCatId == -1)
            {
                foreach (Category cat in Presenter.Budget.Categories.GetList())
                {
                    colBinding = new Binding($"[{cat.Description}]");
                    colBinding.StringFormat = "c2";

                    summaryGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = cat.Description,
                        Binding = colBinding,
                        CellStyle = (Style)FindResource("style_cell_total")
                    });
                }

                colBinding = new Binding("[Total]");
                colBinding.StringFormat = "c2";
                summaryGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Month's Total",
                    Binding = colBinding,
                    CellStyle = (Style)FindResource("style_cell_total")
                });
            }
            else
            {
                Category targetCat = (Category)control_dataGridView.cmb_categoryFilter.SelectedItem;

                colBinding = new Binding($"[{targetCat.Description}]");
                colBinding.StringFormat = "c2";
                summaryGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = targetCat.Description,
                    Binding = colBinding,
                    CellStyle = (Style)FindResource("style_cell_total")
                });
            }

            for (int i = 0; i < DataSource.Count; i++)
            {
                summaryGrid.Items.Add(DataSource[i]);
            }
        }

        private void SummaryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _focusChangedByUserDuringTraversal = true;
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_numLoadedItems == control_dataGridView.stckPanel_itemList.Children.Count)
            {
                ResizeDisplayedElementsToWindowSize();
            }
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

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Presenter.Budget.Disconnect();
        }

        // FROM INTERFACE
        public void ResetFocusAfterUpdate(int itemIndex)
        {
            // Expense is automatically focused on MouseDoubleCLick to modify
            return;
        }

        // FROM INTERFACE
        public bool ItemListIsEmpty()
        {
            return control_dataGridView.stckPanel_itemList.Children.Count == 0;
        }

        // FROM INTERFACE
        public void DataClear()
        {
            control_dataGridView.stckPanel_itemList.Children.Clear();
            _numLoadedItems = 0;
        }

        // FROM INTERFACE
        public bool IsStandardDisplay()
        {
            return _expsDisplayedWithinSelectedDays;
        }

        // FROM INTERFACE
        public bool IsByMonthDisplay()
        {
            return _currentDisplayedMonth > -1;
        }

        // FROM INTERFACE
        public bool IsByCategoryDisplay()
        {
            return _expsDisplayedByCategory;
        }

        // FROM INTERFACE
        public bool IsByCategoryAndMonthDisplay()
        {
            return _expsSummaryDisplayed;
        }

        // FROM INTERFACE
        public bool IsShowingAll()
        {
            return _showAllExps;
        }

        // FROM INTERFACE
        public int GetCurrentTargetCategoryId()
        {
            return _currentTargetCatId;
        }

        // FROM INTERFACE
        public DateTime? GetCurrentDisplayedStartDate()
        {
            return _currentDisplayedStartDate;
        }

        // FROM INTERFACE
        public DateTime? GetCurrentDisplayedEndDate()
        {
            return _currentDisplayedEndDate;
        }

        // FROM INTERFACE
        public DateTime? GetSelectedDate()
        {
            return _selectedDate;
        }

        // -----------------------------
    }
}