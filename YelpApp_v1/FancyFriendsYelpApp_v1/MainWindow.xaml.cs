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
            public double distance { get; set; }
            public int num_checkins { get; set; }
            public int num_tips { get; set; }
            public int stars { get; set; }
            public int zip_code { get; set; }
            public bool is_open { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            addState();
            addColumnsToGrid();
        }

        private string buildConnectionString()
        {
            return "Host = localhost; Username = postgres; Database = fancyfriendsdb; password = PASSWORD";
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
            businessDataGrid.Items.Add(new Business() { business_id = R.GetString(0), name = R.GetString(1), num_checkins = R.GetInt32(2), 
                stars = R.GetInt32(3), num_tips = R.GetInt32(4), state = R.GetString(5), city = R.GetString(6), zip_code = R.GetInt32(7), 
                is_open = R.GetBoolean(8), latitude = R.GetDouble(9), longitude = R.GetDouble(10), address  = R.GetString(11) }); // Collect results from reader
            count += 1;
            Console.WriteLine(count);
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

        private void searchBusinessesButton_Click(object sender, RoutedEventArgs e)
        {
            businessDataGrid.Items.Clear(); // Clears business Grid

            if (categoryFilterList.Items.Count > 0 && zipcodeList.SelectedIndex > -1)
            {
                string sqlStr = $"Select distinct business.business_id, business.name, num_checkins, stars, num_tips, state, city, zip_code, is_open, latitude, longitude, " +
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

                // THIS WILL BREAK IF THE CATEGORY NAME CONTAINS AN APOSTROPHE
                sqlStr += $"GROUP BY Business.business_id HAVING COUNT(Business.business_id) = {categoryFilterList.Items.Count} ORDER BY business.name";

                Console.WriteLine(sqlStr);

                executeQuery(sqlStr, addGridRow);
            }
            else if (zipcodeList.SelectedIndex > -1)
            {
                string sqlStr = $"Select business_id, name, num_checkins, stars, num_tips, state, city, zip_code, is_open, latitude, longitude, address FROM business WHERE business.zip_code = {zipcodeList.SelectedItem} AND business.state = '{stateList.SelectedItem.ToString()}' AND business.city = '{cityList.SelectedItem.ToString()}' ORDER BY business.name";
                executeQuery(sqlStr, addGridRow);
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
                    TipsWindow tips_window = new TipsWindow(B.business_id.ToString());
                    tips_window.Show();
                }
            }
        }
    }
}
