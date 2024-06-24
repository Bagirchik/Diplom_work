using Microsoft.Data.SqlClient;
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
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace Diplom_Work.View
{
    /// <summary>
    /// Логика взаимодействия для Showing.xaml
    /// </summary>
    public partial class Showing : Window
    {
        private readonly DbDiplomContext dbContext;
        private int userId;
        public Showing(int userId)
        {
            InitializeComponent();
            dbContext = new DbDiplomContext();
            this.userId = userId;
            LoadShows();

        }

        private void LoadShows()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Server=DESKTOP;Database=Db_Diplom;Integrated Security=True;TrustServerCertificate=True;"))
                {
                    connection.Open();
                    string query = "SELECT ShowId, Name FROM Shows";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ListBoxItem item = new ListBoxItem();
                            item.Content = reader["Name"].ToString();
                            item.Tag = reader["ShowId"]; // Убедитесь, что это правильное имя столбца
                            ShowsListBox.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShowsListBox.SelectedItem is ListBoxItem selectedItem)
            {
                int showId = (int)selectedItem.Tag;
                Show showWindow = new Show(showId , userId);
                showWindow.Show();
                this.Close();
            }
        }
        private void ShowsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ваш код обработки изменения выбора в ListView
        }
        public void Back_bt_Click(object sender, RoutedEventArgs e)
        {
            Account accountWindow = new Account(userId);           
            accountWindow.Show();
            this.Close();
        }
    }
        
   
}
