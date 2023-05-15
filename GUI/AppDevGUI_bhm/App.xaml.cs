using System;
using System.Windows;
using System.Windows.Threading;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AppDevGUI_bhm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool _dateFilter_TextBox_TextChangedByProgram = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            // this is for unhandled error catching
            Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // this code is from https://stackoverflow.com/questions/2764615/wpf-stringformat-0c-showing-as-dollars/2764668#2764668
            // it helps setting the formatting in DataGrid according to CurrentCulture
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                    GUIConstants.CurrentCulture.IetfLanguageTag)));
            base.OnStartup(e);
        }

        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);

            e.Handled = true;
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);

            e.Handled = true;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private void dateFilter_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dateFilter_TextBox_TextChangedByProgram)
            {
                _dateFilter_TextBox_TextChangedByProgram = false;
                return;
            }

            _dateFilter_TextBox_TextChangedByProgram = true;
            // reformat displayed date in datepicker textbox
            TextBox txtBox_dateFilter = (TextBox)sender;
            try
            {
                if (!string.IsNullOrEmpty(txtBox_dateFilter.Text))
                    txtBox_dateFilter.Text = DateTime.Parse(txtBox_dateFilter.Text, CultureInfo.InvariantCulture).ToString("dd MMM yyyy", GUIConstants.CurrentCulture);
            }
            catch (Exception)
            {
                if (!string.IsNullOrEmpty(txtBox_dateFilter.Text))
                    txtBox_dateFilter.Text = DateTime.Parse(txtBox_dateFilter.Text, GUIConstants.CurrentCulture).ToString("dd MMM yyyy", GUIConstants.CurrentCulture);
            }
        }
    }
}
