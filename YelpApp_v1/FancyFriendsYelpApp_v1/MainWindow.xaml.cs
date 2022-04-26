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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace FancyFriendsYelpApp_v1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int count = 0;
        public class Business
        {
            public string business_id { get; set; }
            public string name { get; set; }
            public string state { get; set; }
            public string city { get; set; }
            public string address { get; set; }
            public int num_checkins { get; set; }
            public int num_tips { get; set; }
            public int stars { get; set; }
            public int zip_code { get; set; }
            public bool is_open { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public double distance { get; set; }
        }

        public class Tip
        {
            public string user_name { get; set; }
            public string business { get; set; }
            public string city { get; set; }
            public string text { get; set; }
            public string date { get; set; }
        }

        public class Friend
        {
            public string first_name { get; set; }
            public int total_tip_likes { get; set; }
            public double average_stars { get; set; }
            public string date_joined { get; set; }
        }

        public string current_userid { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            addState();
            addColumnsToGrid();
            addColumnsToFriendsGrid();
            addColumnsToTipsGrid();
        }

        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = fancyfriendsdb; password = YOUR_PASS_HERE";
        }

        private void addState()
        {
            using (var connection = new NpgsqlConnection(buildConnectionString()))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT distinct state FROM business ORDER by state";
                    try
                    {
                        var reader = cmd.ExecuteReader(); // Execute Query
                        while (reader.Read())
                        {
                            stateList.Items.Add(reader.GetString(0)); // Collect results from reader
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

        private void addColumnsToGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("name");
            col1.Header = "BusinessName";
            col1.Width = 225;
            businessDataGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("address");
            col2.Header = "Address";
            col2.Width = 175;
            businessDataGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("city");
            col3.Header = "City";
            col3.Width = 100;
            businessDataGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("state");
            col4.Header = "State";
            col4.Width = 50;
            businessDataGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Binding = new Binding("distance");
            col5.Header = "Distance (miles)";
            col5.Width = 100;
            businessDataGrid.Columns.Add(col5);

            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Binding = new Binding("stars");
            col6.Header = "Stars";
            col6.Width = 50;
            businessDataGrid.Columns.Add(col6);

            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Binding = new Binding("num_tips");
            col7.Header = "# of Tips";
            col7.Width = 100;
            businessDataGrid.Columns.Add(col7);

            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Binding = new Binding("num_checkins");
            col8.Header = "Total Checkins";
            col8.Width = 100;
            businessDataGrid.Columns.Add(col8);

            DataGridTextColumn col9 = new DataGridTextColumn();
            col9.Binding = new Binding("business_id");
            col9.Header = "Business ID";
            col9.Width = 150;
            businessDataGrid.Columns.Add(col9);
        }

        private void addColumnsToFriendsGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("first_name");
            col1.Header = "Name";
            col1.Width = 75;
            friendsGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("total_tip_likes");
            col2.Header = "TotalLikes";
            col2.Width = 75;
            friendsGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("average_stars");
            col3.Header = "AvgStars";
            col3.Width = 65;
            friendsGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("date_joined");
            col4.Header = "Yelping Since";
            col4.Width = 80;
            friendsGrid.Columns.Add(col4);
        }

        private void addColumnsToTipsGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Binding = new Binding("user_name");
            col1.Header = "User Name";
            col1.Width = 100;
            tipsListGrid.Columns.Add(col1);

            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("business");
            col2.Header = "Business";
            col2.Width = 200;
            tipsListGrid.Columns.Add(col2);

            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("city");
            col3.Header = "City";
            col3.Width = 75;
            tipsListGrid.Columns.Add(col3);

            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("text");
            col4.Header = "Text";
            col4.Width = 300;
            tipsListGrid.Columns.Add(col4);

            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Binding = new Binding("date");
            col5.Header = "Date";
            col5.Width = 65;
            tipsListGrid.Columns.Add(col5);
        }

        // executes SELECT queries
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

        // executes UPDATES, INSERTS, DELETES queries
        private void executeUpdate(string sqlstr)
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
                        var reader = cmd.ExecuteNonQuery(); // Execute Query
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

        private void addCity(NpgsqlDataReader R)
        {
            cityList.Items.Add(R.GetString(0)); // Collect results from reader
        }

        private void addZipcode(NpgsqlDataReader R)
        {
            zipcodeList.Items.Add(R.GetInt32(0)); // Collect results from reader
        }

        private void addCategory(NpgsqlDataReader R)
        {
            businessCategoryList.Items.Add(R.GetString(0)); // Collect results from reader
        }

        private void addUserID(NpgsqlDataReader R)
        {
            userIDList.Items.Add(R.GetString(0)); // Collect results from reader
        }

        private void addCategoryFilterToList(string categoryName)
        {
            categoryFilterList.Items.Add(categoryName);
        }

        private void removeCategoryFilterFromList(string categoryName)
        {
            categoryFilterList.Items.Remove(categoryName);
        }

        // Inserts Business Data into businessDataGrid
        private void addGridRow(NpgsqlDataReader R)
        {
            businessDataGrid.Items.Add(new Business()
            {
                business_id = R.GetString(0),
                name = R.GetString(1),
                num_checkins = R.GetInt32(2),
                stars = R.GetInt32(3),
                num_tips = R.GetInt32(4),
                state = R.GetString(5),
                city = R.GetString(6),
                zip_code = R.GetInt32(7),
                is_open = R.GetBoolean(8),
                latitude = R.GetDouble(9),
                longitude = R.GetDouble(10),
                address = R.GetString(11),
                distance = haversine(R.GetDouble(9), R.GetDouble(10), Convert.ToDouble(latitude.Text), Convert.ToDouble(longitude.Text))
            }); // Collect results from reader
            count += 1;
            Console.WriteLine(count);
        }

        private double haversine(double lat1, double lon1,
            double lat2, double lon2)
        {
            // distance between latitudes and longitudes
            double dLat = (Math.PI / 180) * (lat2 - lat1);
            double dLon = (Math.PI / 180) * (lon2 - lon1);

            // convert to radians
            lat1 = (Math.PI / 180) * (lat1);
            lat2 = (Math.PI / 180) * (lat2);

            // apply formula
            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                        Math.Pow(Math.Sin(dLon / 2), 2) *
                        Math.Cos(lat1) * Math.Cos(lat2);
            double rad = 6371;
            double c = 2 * Math.Asin(Math.Sqrt(a));
            double kilometers = rad * c;

            // return miles cause we're american
            return kilometers / 1.609;
        }

        // Inserts Friends Data into friendsDataGrid
        private void addFriendsGridRow(NpgsqlDataReader R)
        {
            friendsGrid.Items.Add(new Friend()
            {
                first_name = R.GetString(0),
                total_tip_likes = R.GetInt32(1),
                average_stars = R.GetDouble(2),
                date_joined = R.GetDate(3).ToString()
            }); // Collect results from reader
            count += 1;
            Console.WriteLine(count);
        }

        // Inserts Friends Tip Data into friendsTipDataGrid
        private void addFriendsTipGridRow(NpgsqlDataReader R)
        {
            tipsListGrid.Items.Add(new Tip()
            {
                user_name = R.GetString(0),
                business = R.GetString(1),
                city = R.GetString(2),
                text = R.GetString(3),
                date = R.GetDateTime(4).ToString()
            }); // Collect results from reader
            count += 1;
            Console.WriteLine(count);
        }

        private void addUserInformation(NpgsqlDataReader R)
        {
            nameTextbox.Text = (R.GetString(0) + " " + R.GetString(1)).Trim();
            starsTextbox.Text = R.GetDouble(2).ToString();
            fansTextbox.Text = R.GetInt32(3).ToString();
            yelpingTextbox.Text = R.GetDate(4).ToString();
            funnyVotes.Text = R.GetInt32(5).ToString();
            coolVotes.Text = R.GetInt32(6).ToString();
            usefulVotes.Text = R.GetInt32(7).ToString();
            tipCount.Text = R.GetInt32(8).ToString();
            tipLikes.Text = R.GetInt32(9).ToString();
            latitude.Text = R.GetDouble(10).ToString();
            longitude.Text = R.GetDouble(11).ToString();
        }

        private void stateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityList.Items.Clear(); // Clears zipcodeList when selection changes to another city [prevents appending]
            categoryFilterList.Items.Clear(); // Clears filter list when state slection changes

            if (stateList.SelectedIndex > -1)
            {
                string sqlStr = $"SELECT distinct city FROM business WHERE state = '{stateList.SelectedItem.ToString()}' ORDER by city";
                executeQuery(sqlStr, addCity);
            }
        }

        private void cityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            zipcodeList.Items.Clear(); // Clears zipcodeList when selection changes to another city [prevents appending]
            categoryFilterList.Items.Clear(); // Clears filter list when state slection changes

            if (cityList.SelectedIndex > -1)
            {
                string sqlStr = $"SELECT distinct zip_code FROM business WHERE state = '{stateList.SelectedItem.ToString()}' AND city = '{cityList.SelectedItem.ToString()}' ORDER by zip_code";
                executeQuery(sqlStr, addZipcode);
            }
        }

        private void zipcodeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessCategoryList.Items.Clear(); // Clears businessCategoryList when selection changes to another zipcode [prevents appending]
            categoryFilterList.Items.Clear(); // Clears filter list when state slection changes

            if (zipcodeList.SelectedIndex > -1)
            {
                string sqlStr = $"SELECT distinct category.name FROM category, business WHERE business.business_id = category.business_id AND state = '{stateList.SelectedItem.ToString()}' AND city = '{cityList.SelectedItem.ToString()}' AND business.zip_code = '{zipcodeList.SelectedItem}' ORDER BY category.name";
                executeQuery(sqlStr, addCategory);
            }
        }

        private void userId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            friendsGrid.Items.Clear(); // Clears friend list when switching to another user [prevents appending]
            tipsListGrid.Items.Clear(); // Clears recent tips from friends when new user "logs in"
            ClearAllUserData();

            if (userIDList.SelectedIndex > -1)
            {
                current_userid = userIDList.SelectedItem.ToString();//Dileep added
                string sqlStr;
                // User information section
                sqlStr = $"SELECT first_name, last_name, average_stars, number_of_fans, date_joined, funny, cool, useful, total_tip_count, total_tip_likes, latitude, longitude " +
                    $"FROM Users " +
                    $"WHERE user_id = '{userIDList.SelectedItem}';";
                executeQuery(sqlStr, addUserInformation);

                // Friends grid
                sqlStr = $"SELECT DISTINCT Users.first_name, Users.total_tip_likes, Users.average_stars, Users.date_joined " +
                    $"FROM Users, Friends " +
                    $"WHERE Friends.user_id = '{userIDList.SelectedItem.ToString()}' " +
                    $"AND Users.user_id = Friends.user_id2";
                executeQuery(sqlStr, addFriendsGridRow);

                // Latest tips grid
                sqlStr = $"SELECT Users.first_name, Business.name, Business.city, TempTip.tip_text, TempTip.tip_time " +
                    $"FROM Users, Friends, Business," +
                    $"(SELECT DISTINCT ON(user_id) user_id, business_id, tip_time, tip_text FROM Tip ORDER BY user_id, tip_time DESC) AS TempTip " +
                    $"WHERE Friends.user_id = '{userIDList.SelectedItem.ToString()}' " +
                    $"AND Users.user_id = Friends.user_id2 " +
                    $"AND TempTip.user_id = Friends.user_id2 " +
                    $"AND TempTip.business_id = Business.business_id";
                executeQuery(sqlStr, addFriendsTipGridRow);
            }
        }

        private void searchBusinessesButton_Click(object sender, RoutedEventArgs e)
        {
            count = 0;
            businessDataGrid.Items.Clear(); // Clears business Grid
            string sqlStr = "";

            if (categoryFilterList.Items.Count > 0 && zipcodeList.SelectedIndex > -1)
            {
                sqlStr = $"Select distinct business.business_id, business.name, num_checkins, stars, num_tips, state, city, zip_code, is_open, latitude, longitude, " +
                    $"address FROM business, category " +
                    $"WHERE business.zip_code = {zipcodeList.SelectedItem} " +
                    $"AND business.state = '{stateList.SelectedItem.ToString()}' " +
                    $"AND business.city = '{cityList.SelectedItem.ToString()}' " +
                    $"AND business.business_id = category.business_id AND category.name IN (";

                foreach (string filter in categoryFilterList.Items)
                {
                    string newFilter = filter.Replace("'", "''"); // Fixes the filter name if it contains an apostrophe

                    if (categoryFilterList.Items.Count == 1) // The only filter
                    {
                        sqlStr += $"'{newFilter}') ";
                    }
                    else if (categoryFilterList.Items.Count-1 == categoryFilterList.Items.IndexOf(filter)) // The last filter
                    {
                        sqlStr += $"'{newFilter}')";
                    }
                    else // First and middle filters
                    {
                        sqlStr += $"'{newFilter}', ";
                    }
                }
            }
            else if (zipcodeList.SelectedIndex > -1)
            {
                sqlStr = $"Select business_id, name, num_checkins, stars, num_tips, state, city, zip_code, is_open, latitude, longitude, address FROM business WHERE business.zip_code = {zipcodeList.SelectedItem} AND business.state = '{stateList.SelectedItem.ToString()}' AND business.city = '{cityList.SelectedItem.ToString()}'";
            }

            if (zipcodeList.SelectedIndex > -1 && ((bool)checkBoxPrice1.IsChecked || (bool)checkBoxPrice2.IsChecked || 
                (bool)checkBoxPrice3.IsChecked || (bool)checkBoxPrice4.IsChecked))
            {
                sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id AND attribute.name = 'RestaurantsPriceRange2'";
                // INCLUDES AND STATEMENT FOR THE FIRST PRICE CHECKED
                if ((bool)checkBoxPrice1.IsChecked)
                {
                    System.Console.WriteLine("checkboxPrice1 is checked!\n");
                    sqlStr += "AND attribute.value='1'";
                }
                else if ((bool)checkBoxPrice2.IsChecked)
                {
                    System.Console.WriteLine("checkboxPrice2 is checked!\n");
                    sqlStr += "AND attribute.value='2'";
                }
                else if ((bool)checkBoxPrice3.IsChecked)
                {
                    System.Console.WriteLine("checkboxPrice3 is checked!\n");
                    sqlStr += "AND attribute.value='3'";
                }
                else if ((bool)checkBoxPrice4.IsChecked)
                {
                    System.Console.WriteLine("checkboxPrice4 is checked!\n");
                    sqlStr += "AND attribute.value='4'";
                }

                // INCLUDES OR STATEMENT FOR ANY ADDITIONAL PRICE CHECKBOXES CHECKED
                if ((bool)checkBoxPrice1.IsChecked)
                {
                    System.Console.WriteLine("checkboxPrice1 is checked!\n");
                    sqlStr += "OR attribute.value='1'";
                }
                if ((bool)checkBoxPrice2.IsChecked)
                {
                    System.Console.WriteLine("checkboxPrice2 is checked!\n");
                    sqlStr += "OR attribute.value='2'";
                }
                if ((bool)checkBoxPrice3.IsChecked)
                {
                    System.Console.WriteLine("checkboxPrice3 is checked!\n");
                    sqlStr += "OR attribute.value='3'";
                }
                if ((bool)checkBoxPrice4.IsChecked)
                {
                    System.Console.WriteLine("checkboxPrice4 is checked!\n");
                    sqlStr += "OR attribute.value='4'";
                }

                sqlStr += ")";
            }

            // ATTRIBUTE CHECKBOX SELECTION
            if (zipcodeList.SelectedIndex > -1)
            {
                // --------------------- Filter By Attributes -------------------------------- //

                // Accepts Credit Cards
                if ((bool)acceptsCreditCard.IsChecked)
                {
                    // sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    // sqlStr += "AND attribute.name='BusinessAcceptsCreditCards'";
                    // sqlStr += "AND attribute.value='True')";

                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id AND attribute.name = 'BusinessAcceptsCreditCards' AND attribute.value = 'True')";
                }

                // Takes Reservations
                if ((bool)takesReservations.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='RestaurantsReservations'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Wheelchair Accessible
                if ((bool)wheelchairAccessible.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='WheelchairAccessible'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Outdoor Seating
                if ((bool)outdoorSeating.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='OutdoorSeating'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Good for Kids
                if ((bool)goodForKids.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='GoodForKids'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Good for Groups
                if ((bool)goodForGroups.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='RestaurantsGoodForGroups'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Delivery
                if ((bool)delivery.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='RestaurantsDelivery'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Take Out
                if ((bool)takeOut.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='RestaurantsTakeOut'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Free Wi-Fi
                if ((bool)freeWiFi.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='WiFi'";
                    sqlStr += " AND attribute.value='free')";
                }

                // Bike Parking
                if ((bool)bikeParking.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='BikeParking'";
                    sqlStr += " AND attribute.value='True')";
                }

                // --------------------- Filter By Meal -------------------------------- //

                // Breakfast
                if ((bool)breakfastCheckBox.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='breakfast'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Lunch
                if ((bool)LunchCheckBox.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='lunch'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Brunch
                if ((bool)brunchCheckBox.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='brunch'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Dinner
                if ((bool)dinnerCheckBox.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='dinner'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Dessert
                if ((bool)dessertCheckBox.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='dessert'";
                    sqlStr += " AND attribute.value='True')";
                }

                // Late Night
                if ((bool)lateNightCheckBox.IsChecked)
                {
                    sqlStr += "AND business.business_id IN (SELECT attribute.business_id FROM business, attribute WHERE business.business_id = attribute.business_id";
                    sqlStr += " AND attribute.name='latenight'";
                    sqlStr += " AND attribute.value='True')";
                }
            }

            // Add GROUP BY statement for business categories
            if (categoryFilterList.Items.Count > 0 && zipcodeList.SelectedIndex > -1)
            {
                sqlStr += $"GROUP BY Business.business_id HAVING COUNT(Business.business_id) = {categoryFilterList.Items.Count}";
            }

            if (zipcodeList.SelectedIndex > -1)
            {
                if (sortResultsComboBox.SelectedItem == sortByName)
                {
                    sqlStr += "ORDER BY business.name";
                }
                else if (sortResultsComboBox.SelectedItem == sortByRating)
                {
                    sqlStr += "ORDER BY business.stars DESC";
                }
                else if (sortResultsComboBox.SelectedItem == sortByTips)
                {
                    sqlStr += "ORDER BY business.num_tips DESC";
                }
                else if (sortResultsComboBox.SelectedItem == sortByCheckins)
                {
                    sqlStr += "ORDER BY business.num_checkins DESC";
                }
                else if (sortResultsComboBox.SelectedItem == sortByDistance)
                {
                    sqlStr += "ORDER BY business.longitude"; // NEED TO FIX DISTANCE
                }

                Console.WriteLine(sqlStr);
                executeQuery(sqlStr, addGridRow);
                numBusinessLabel.Content = count; // Output the number of businesses found from the query
            }
        }

        private void addCategoryFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!checkIfCategoryExists(businessCategoryList.SelectedItem.ToString()))
                {
                    addCategoryFilterToList(businessCategoryList.SelectedItem.ToString());
                }
                else
                {
                    Console.WriteLine("Category has already been added!\n");
                }
            }
            catch
            {
                Console.WriteLine("Nothing has been selected!\n");
            }
        }

        private void removeCategoryFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                removeCategoryFilterFromList(categoryFilterList.SelectedItem.ToString());
            }
            catch
            {
                Console.WriteLine("A category has not been selected!\n");
            }
        }

        private bool checkIfCategoryExists(string categoryName)
        {
            foreach (string filter in categoryFilterList.Items)
            {
                if (filter == categoryName)
                {
                    return true;
                }
            }
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (businessDataGrid.SelectedIndex > -1)
            {
                Business B = businessDataGrid.Items[businessDataGrid.SelectedIndex] as Business;
                if ((B.business_id != null) && (B.business_id.ToString().CompareTo("") != 0))
                {
                    TipsWindow tips_window = new TipsWindow(B.business_id.ToString(), current_userid);
                    tips_window.Show();
                }
            }
        }
        private void updateUserLocation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double user_lat = 0;
                user_lat = Convert.ToDouble(latitude.Text);
                double user_long = 0;
                user_long = Convert.ToDouble(longitude.Text);

                string query =
                        $"UPDATE Users " +
                        $"SET latitude = {user_lat}, longitude = {user_long} " +
                        $"WHERE user_id = '{userIDList.SelectedItem.ToString()}'";
                executeUpdate(query);
            }
            catch
            {
                Console.WriteLine("A user has not been selected!\n");
            }

        }

        private void userName_TextChanged(object sender, TextChangedEventArgs e)
        {
            userIDList?.Items?.Clear();
            friendsGrid?.Items?.Clear();
            tipsListGrid?.Items?.Clear();
            ClearAllUserData();

            string currText = userName.Text;
            if (!string.IsNullOrEmpty(currText))
            {
                string[] split = currText.Split(' ');
                string first = split[0];
                string last = null;
                if (split.Length > 1)
                {
                    last = split[1];
                }

                string query =
                    $"SELECT user_id " +
                    $"FROM users " +
                    $"WHERE first_name LIKE '{first.ToString()}%'";
                if (!string.IsNullOrEmpty(last))
                {
                    query += $"AND last_name LIKE '{last}%'";
                }
                query += "ORDER BY first_name";
                query += ";";

                executeQuery(query, addUserID);
            }
        }

        /// <summary>
        /// Clears all fields in the User Information section.
        /// </summary>
        private void ClearAllUserData()
        {
            nameTextbox?.Clear();
            starsTextbox?.Clear();
            fansTextbox?.Clear();
            yelpingTextbox?.Clear();
            funnyVotes?.Clear();
            coolVotes?.Clear();
            usefulVotes?.Clear();
            tipCount?.Clear();
            tipLikes?.Clear();
            latitude?.Clear();
            longitude?.Clear();
        }

        /// <summary>
        /// Display selected business information whenever the selection is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void businessDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Business B = businessDataGrid.Items[businessDataGrid.SelectedIndex] as Business;
            if ((B.business_id != null) && (B.business_id.ToString().CompareTo("") != 0))
            {
                // change name, address and hours labels
                selectedNameLabel.Content = B.name;
                selectedAddressLabel.Content = B.address + ", " + B.city + ", " + B.state;

                // calculate current day
                System.DayOfWeek current_day = DateTime.Today.DayOfWeek;
                //selectedHoursLabel.Content = current_day.ToString();
                selectedHoursLabel.Content = "";

                string sqlstr = $"SELECT open_time,closing_time FROM hours WHERE business_id = '{B.business_id}' AND day = '{current_day.ToString()}'";
                executeQuery(sqlstr, addSelectedHoursLabel);

            }
        }

        private void addSelectedHoursLabel(NpgsqlDataReader R)
        {
            //tipsGrid.Items.Add(new Tip() { date = R.GetString(0), user_name = R.GetString(1), likes = R.GetInt32(2), text = R.GetString(3) });
            // test bid: gnKjwL_1w79qoiV3IC_xQQ
            selectedHoursLabel.Content = "Today's Hours: \nOpen: " + R.GetTimeSpan(0).ToString() + " \nClosed: " + R.GetTimeSpan(1).ToString();
        }

        private void checkInsButton_Click(object sender, RoutedEventArgs e)
        {
            if (businessDataGrid.SelectedIndex > -1)
            {
                Business B = businessDataGrid.Items[businessDataGrid.SelectedIndex] as Business;
                if ((B.business_id != null) && (B.business_id.ToString().CompareTo("") != 0))
                {
                    CheckInsWindow checkins_window = new CheckInsWindow(B.business_id.ToString());
                    checkins_window.Show();
                }
            }
        }
    }
}
