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
        private string current_userid;
        public TipsWindow(string bid, string userid)
        {
            InitializeComponent();
            this.bid = bid;
            addColumns2Grid();
            addColumns2Grid2();
            getTips(bid);
            current_userid = userid;
            getFriendTips(bid);
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
        private void getTips(string bid)
        {
            //string sqlstr = $"SELECT tip_time,first_name,likes,tip_text FROM Tip,Users WHERE Tip.business_id = '{bid.ToString()}' AND Tip.user_id = Users.user_id";
            string sqlstr = $"SELECT TO_CHAR(tip_time,'YYYY-MM-DD HH24:MI:SS') as tip_date,first_name,likes,tip_text FROM Tip,Users WHERE Tip.business_id = '{this.bid}' AND Tip.user_id = Users.user_id";
            executeQuery(sqlstr, populateTipsGrid);
        }

        private void getFriendTips(string bid)
        {
            Console.WriteLine("bid: " + this.bid);
            string sqlstr = $"SELECT TO_CHAR(tip_time,'YYYY-MM-DD HH24:MI:SS') as tip_date,first_name,tip_text FROM Tip,Users,Friends WHERE Tip.business_id = '{this.bid}' AND Tip.user_id = Users.user_id AND (Friends.user_id = '{this.current_userid}' OR Friends.user_id2 = '{this.current_userid}') AND (Tip.user_id = Friends.user_id OR Tip.user_id = Friends.user_id2)";
            executeQuery(sqlstr, populateFriendTipsGrid);
            /*
            SELECT TO_CHAR(tip_time,'YYYY-MM-DD HH24:MI:SS') as tip_date,first_name,likes,tip_text FROM Tip,Users,Friends WHERE Tip.business_id = 'mUVAMNN7BCQ9HGA9w_7C1g' AND Tip.user_id = Users.user_id AND (Friends.user_id = 'FgQCX3ztjhellw2hyRedxg' OR Friends.user_id2 = 'FgQCX3ztjhellw2hyRedxg') AND (Tip.user_id = Friends.user_id OR Tip.user_id = Friends.user_id2);
            */
        }

        private void populateFriendTipsGrid(NpgsqlDataReader R)
        {
            //// test bid: gnKjwL_1w79qoiV3IC_xQQ       OR   mUVAMNN7BCQ9HGA9w7C1g (ingo's tasty food)
            //// test userid: Wkb8b9QJ35XTp-KYO0ojBQ    OR   FgQCX3ztjhellw2hyRedxg (Lee Ann)
            Console.WriteLine("found 1 friend tip");
            friendReviewsGrid.Items.Add(new Tip() { date = R.GetString(0), user_name = R.GetString(1), text = R.GetString(2) });
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

        /// <summary>
        /// adds columns to friends grid
        /// </summary>
        private void addColumns2Grid2()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("date");
            col1.Header = "Date";
            col1.Width = 70;
            friendReviewsGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("user_name");
            col2.Header = "User Name";
            col2.Width = 70;
            friendReviewsGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("text");
            col3.Header = "Text";
            col3.Width = DataGridLength.Auto;
            friendReviewsGrid.Columns.Add(col3);
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
