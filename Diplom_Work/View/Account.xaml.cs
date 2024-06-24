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

namespace Diplom_Work.View
{
    /// <summary>
    /// Логика взаимодействия для Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        public int userId;
        public Account(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUserData();
            LoadTickets();
        }

        public void LoadUserData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Server=DESKTOP;Database=Db_Diplom;Integrated Security=True;TrustServerCertificate=True;"))
                {
                    connection.Open();
                    string query = "SELECT FirstName, LastName, PhoneNumber, Email FROM Users WHERE UserId = @UserId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                FirstNameTextBlock.Text = reader["FirstName"].ToString();
                                LastNameTextBlock.Text = reader["LastName"].ToString();
                                PhoneNumberTextBlock.Text = reader["PhoneNumber"].ToString();
                                EmailTextBlock.Text = reader["Email"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadTickets()
        {
            using (SqlConnection connection = new SqlConnection("Server=DESKTOP;Database=Db_Diplom;Integrated Security=True;TrustServerCertificate=True;"))
            {
                connection.Open();
                string query = @"
                    SELECT t.TicketID, s.Name AS ShowName, t.Price, t.PurchaseDate, t.SeatNumber
                    FROM Tickets t
                    JOIN Shows s ON t.ShowID = s.ShowID
                    WHERE t.UserID = @UserId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string ticketInfo = $"Билет {reader["TicketID"]}, Шоу: {reader["ShowName"]}, Цена: {reader["Price"]}, Дата покупки: {reader["PurchaseDate"]}, Место: {reader["SeatNumber"]}";
                            TicketsListBox.Items.Add(ticketInfo);
                        }
                    }
                }
            }
        }

        private void ContinueShoppingButton_Click(object sender, RoutedEventArgs e)
        {
            Showing showing = new Showing(userId);
            showing.Show();
            this.Close();
        }
    }
}
