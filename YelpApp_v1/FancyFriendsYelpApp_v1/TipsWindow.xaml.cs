using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql;

namespace FancyFriendsYelpApp_v1
{
    /// <summary>
    /// Interaction logic for TipsWindow.xaml
    /// </summary>
    public partial class TipsWindow : Window
    {
        public class Tip
        {
            public string date { get; set; }
            public string user_name { get; set; }
            public int likes { get; set; }
            public string text { get; set; }
        }
        private string bid = "";
        public TipsWindow(string bid)
        {
            InitializeComponent();
            this.bid = bid;
            addColumns2Grid();
            getTips(bid);
        }
        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = fancyfriendsdb; password=Ddiger12";
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
        private void getTips(string bid)
        {
            //string sqlstr = $"SELECT tip_time,first_name,likes,tip_text FROM Tip,Users WHERE Tip.business_id = '{bid.ToString()}' AND Tip.user_id = Users.user_id";
            string sqlstr = $"SELECT TO_CHAR(tip_time,'YYYY-MM-DD HH24:MI:SS') as tip_date,first_name,likes,tip_text FROM Tip,Users WHERE Tip.business_id = '{this.bid}' AND Tip.user_id = Users.user_id";
            executeQuery(sqlstr, populateTipsGrid);
        }

        private void populateTipsGrid(NpgsqlDataReader R)
        {
            tipsGrid.Items.Add(new Tip() { date = R.GetString(0), user_name = R.GetString(1), likes = R.GetInt32(2), text = R.GetString(3) });
        }

        private void addColumns2Grid()
        {
            allTipsLabel.Content = "All Tips For Business ID: " + this.bid.ToString();

            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("date");
            col1.Header = "Date";
            col1.Width = 70;
            tipsGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("user_name");
            col2.Header = "User Name";
            col2.Width = 70;
            tipsGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("likes");
            col3.Header = "Likes";
            col3.Width = 40;
            tipsGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("text");
            col4.Header = "Text";
            col4.Width = DataGridLength.Auto;
            tipsGrid.Columns.Add(col4);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if((tipTextBox.Text.ToString().CompareTo("") != 0) && (tipTextBox.Text.ToString().CompareTo("Add tip here!") != 0))
            {
                //if these is actual text in the tip box insert to database
                //RANDOM USER ID USED
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string sqlStr = $"INSERT INTO Tip (business_id, user_id, tip_time, tip_text, likes) VALUES ('{this.bid.ToString()}', 'Wkb8b9QJ35XTp-KYO0ojBQ', '{date}', '{tipTextBox.Text.ToString()}', '0')";

                executeQuery(sqlStr, null);
                Console.WriteLine(date);
                MessageBox.Show("Tip Added!");
            }
        }
    }
}
