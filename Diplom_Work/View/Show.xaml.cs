using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;
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
    public partial class Show : Window
    {
        private int showId;
        private int availableSeats;
        private int userId;

        public Show(int showId, int userId)
        {
            InitializeComponent();
            this.showId = showId;
            LoadShowDetails();
            this.userId = userId;
        }

        private void LoadShowDetails()
        {
            using (SqlConnection connection = new SqlConnection("Server=DESKTOP;Database=Db_Diplom;Integrated Security=True;TrustServerCertificate=True;"))
            {
                connection.Open();
                string query = "SELECT Name, Description, StartDate, Price, SeatCount FROM Shows WHERE ShowId = @ShowId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ShowId", showId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ShowNameTextBlock.Text = reader["Name"].ToString();
                            DescriptionTextBlock.Text = reader["Description"].ToString();
                            StartDateTextBlock.Text = reader["StartDate"].ToString();
                            PriceTextBlock.Text = reader["Price"].ToString();
                            availableSeats = (int)reader["SeatCount"];
                            SeatCountTextBlock.Text = availableSeats.ToString();
                            PopulateSeatsComboBox();
                        }
                    }
                }
            }
        }

        private void PopulateSeatsComboBox()
        {
            SeatsComboBox.Items.Clear();
            for (int i = 1; i <= availableSeats; i++)
            {
                SeatsComboBox.Items.Add(i);
            }
            SeatsComboBox.SelectedIndex = 0; // Default to 1 seat
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (SeatsComboBox.SelectedItem != null)
            {
                int selectedSeats = (int)SeatsComboBox.SelectedItem;

                using (SqlConnection connection = new SqlConnection("Server=DESKTOP;Database=Db_Diplom;Integrated Security=True;TrustServerCertificate=True;"))
                {
                    connection.Open();

                    // Check if enough seats are available
                    string checkQuery = "SELECT SeatCount FROM Shows WHERE ShowID = @ShowID";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@ShowID", showId);
                        int currentAvailableSeats = (int)checkCommand.ExecuteScalar();
                        if (currentAvailableSeats < selectedSeats)
                        {
                            MessageBox.Show("Недостаточно мест для выбранного количества.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    // Update the seat count
                    string updateQuery = "UPDATE Shows SET SeatCount = SeatCount - @SelectedSeats WHERE ShowId = @ShowId";
                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@SelectedSeats", selectedSeats);
                        updateCommand.Parameters.AddWithValue("@ShowID", showId);
                        updateCommand.ExecuteNonQuery();
                    }

                    // Add records to the Tickets table
                    string insertQuery = "INSERT INTO Tickets (UserID, ShowID, Price, PurchaseDate, SeatNumber) VALUES (@UserID, @ShowID, @Price, @PurchaseDate, @SeatNumber)";
                    for (int i = 1; i <= selectedSeats; i++)
                    {
                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@UserID", userId);
                            insertCommand.Parameters.AddWithValue("@ShowID", showId);
                            insertCommand.Parameters.AddWithValue("@Price", Convert.ToDecimal(PriceTextBlock.Text)); // Convert price to decimal
                            insertCommand.Parameters.AddWithValue("@PurchaseDate", DateTime.Now);
                            insertCommand.Parameters.AddWithValue("@SeatNumber", availableSeats - i + 1); // Assign seat number
                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Билеты успешно куплены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    availableSeats -= selectedSeats;
                    SeatCountTextBlock.Text = availableSeats.ToString();
                    PopulateSeatsComboBox();
                }
            }
        }

        private void BackBatton_Click(object sender, RoutedEventArgs e)
        {
            Showing showing = new Showing(userId);
            showing.Show();
            this.Close();

        }
    }
}