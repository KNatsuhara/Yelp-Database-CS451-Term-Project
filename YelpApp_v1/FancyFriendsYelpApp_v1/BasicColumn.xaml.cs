using System;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;


namespace Wpf.CartesianChart.Basic_Bars
{
    /// <summary>
    /// Interaction logic for BasicColumn.xaml
    /// </summary>
    public partial class BasicColumn : UserControl
    {
        public BasicColumn()
        {
            InitializeComponent();
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
    }
}
