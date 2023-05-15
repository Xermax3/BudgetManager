using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace AppDevGUI_bhm
{
    public class GUIConstants
    {
        private static CultureInfo _currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        private static RegionInfo _localInfo = new RegionInfo(_currentCulture.LCID);

        public static CultureInfo CurrentCulture
        {
            get { return _currentCulture; }
        }

        public static double ItemWrapperBorderHeight
        {
            get { return 50; }
        }

        public static Thickness DayWrapperMargin
        {
            get { return new Thickness(0, 0, 0, 25); }
        }

        public static Thickness DeleteBtnPadding
        {
            get { return new Thickness(5); }
        }

        public static Thickness DeleteBtnMargin
        {
            get { return new Thickness(0, 0, 25, 0); }
        }

        public static FontWeight NormalButtonFontWeight
        {
            get { return FontWeights.Light; }
        }

        public static SolidColorBrush NormalButtonBackgroundOnMouseLeave
        {
            get { return Brushes.White; }
        }

        public static SolidColorBrush NormalButtonBackgroundOnMouseEnter
        {
            get { return Brushes.AliceBlue; }
        }

        public static double ItemButtonFontSize
        {
            get { return 18; }
        }

        public static string CurrencySymbol
        {
            get { return _localInfo.CurrencySymbol; }
        }

        public static string NumberDecimalSeparator
        {
            get { return CurrentCulture.NumberFormat.NumberDecimalSeparator; }
        }

        public static string DefaultExpenseAmount
        {
            get { return string.Format($"{CurrencySymbol}{{0:N2}}", 0); }
        }
    }
}
