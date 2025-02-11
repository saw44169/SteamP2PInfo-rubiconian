using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace SteamP2PInfo.UserControls
{
    /// <summary>
    /// Interaction logic for HistoryTableControl.xaml
    /// </summary>
    public partial class HistoryTableControl : UserControl
    {
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(HistoryTableControl));

        public HistoryTableControl()
        {
            InitializeComponent();
        }

    }
}
