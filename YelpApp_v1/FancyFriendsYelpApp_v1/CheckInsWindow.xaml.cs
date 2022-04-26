using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using Npgsql;
using Wpf.CartesianChart.Basic_Bars;

namespace FancyFriendsYelpApp_v1
{
    /// <summary>
    /// Interaction logic for CheckInsWindow.xaml
    /// </summary>
    public partial class CheckInsWindow : Window
    {
        public Dictionary<string, int> months = new Dictionary<string, int>();
        BasicColumn column;

        public class CheckInEntry
        {
            public string month { get; set; }
            public string checkin_count { get; set; }
        }
        string bid { get; set; }
        public CheckInsWindow(string bid)
        {
            InitializeComponent();
            addColumns2Grid();
            this.bid = bid;
            getCheckins(bid);

            var monthsTuples = months.Select(x => new Tuple<string, int>(x.Key, x.Value)).ToList();
            monthsTuples.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            List<string> k = new List<string>();
            List<int> v = new List<int>();

            foreach (var item in monthsTuples)
            {
                k.Add(item.Item1);
                v.Add(item.Item2);
            }

            column = new BasicColumn();
            column.SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "2015",
                    Values = new ChartValues<int> (v)
                }
            };

            column.Labels = k.ToArray();
            column.Formatter = value => value.ToString("N");

            DataContext = column;
        }

        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = fancyfriendsdb; password = YOUR_PASS_HERE";
        }
        private void executeQuery(string sqlstr, Action<NpgsqlDataReader> myf)
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = sqlstr;
                    try
                    {
                        var reader = cmd.ExecuteReader(); // Execute Query
                        while (reader.Read())
                        {
                            myf(reader);
                        }
                    }
                    catch (NpgsqlException ex) // Catches any mistakes in the query
                    {
                        Console.WriteLine(ex.Message.ToString()); // Displays error message
                        System.Windows.MessageBox.Show("SQL Error - " + ex.Message.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Populates the tips grid
        /// </summary>
        /// <param name="bid"></param>
        private void getCheckins(string bid)
        {
            //// test bid: gnKjwL_1w79qoiV3IC_xQQ
            //string sqlstr = $"SELECT tip_time,first_name,likes,tip_text FROM Tip,Users WHERE Tip.business_id = '{bid.ToString()}' AND Tip.user_id = Users.user_id";
            string sqlstr = $"SELECT check_in_time FROM check_in WHERE business_id = '{bid.ToString()}'";
            executeQuery(sqlstr, populateCheckinsGrid);
            dictionaryToGrid();
        }

        private void populateCheckinsGrid(NpgsqlDataReader R)
        {
            //System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(R.GetInt32(0))
            //tipsGrid.Items.Add(new Tip() { date = R.GetString(0), user_name = R.GetString(1), likes = R.GetInt32(2), text = R.GetString(3) });
            //Console.WriteLine("month: " + R.GetString(0) + " and corresponding count: " + R.GetInt32(1));
            string date = R.GetDateTime(0).ToString();
            var datetime = DateTime.Parse(date);
            string month = datetime.Month.ToString("d2");
            if (!months.ContainsKey(month))
            {
                months[month] = 0;
            }
            months[month]++;
        }

        private void dictionaryToGrid()
        {
            foreach (KeyValuePair<string, int> month in months.OrderBy(key => key.Key))
            {
                checkinsGrid.Items.Add(new CheckInEntry() { month = month.Key, checkin_count = month.Value.ToString() });
            }
        }

        private void addColumns2Grid()
        {

            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("month");
            col1.Header = "Month";
            col1.Width = 70;
            checkinsGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("checkin_count");
            col2.Header = "Check-ins";
            col2.Width = 70;
            checkinsGrid.Columns.Add(col2);

        }

        private void checkInButton_Click(object sender, RoutedEventArgs e)
        {
            //if these is actual text in the tip box insert to database
            //RANDOM USER ID USED
            //string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string current_time = DateTime.Now.ToString("HH:mm:ss");
            string current_day = DateTime.Now.ToString("dd");
            string current_month = DateTime.Now.ToString("MM");
            string current_year = DateTime.Now.ToString("yyyy");
            string sqlStr = $"INSERT INTO check_in (business_id, check_in_time, day, month, year) VALUES ('{this.bid.ToString()}', '{current_time}', '{current_day}', '{current_month}', '{current_year}')";

            executeQuery(sqlStr, null);
            //Console.WriteLine(date);
            MessageBox.Show("You are checked in!");
        }

        /*private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((tipTextBox.Text.ToString().CompareTo("") != 0) && (tipTextBox.Text.ToString().CompareTo("Add tip here!") != 0))
            {
                //if these is actual text in the tip box insert to database
                //RANDOM USER ID USED
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string sqlStr = $"INSERT INTO Tip (business_id, user_id, tip_time, tip_text, likes) VALUES ('{this.bid.ToString()}', 'Wkb8b9QJ35XTp-KYO0ojBQ', '{date}', '{tipTextBox.Text.ToString()}', '0')";

                executeQuery(sqlStr, null);
                Console.WriteLine(date);
                MessageBox.Show("Tip Added!");
            }
        } */

    }
}
