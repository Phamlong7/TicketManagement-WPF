using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using Fstore.BLL.Services;
using Fstore.DAL.Models;

namespace Fstore
{
    public partial class AddTicketForUserWindow : Window
    {
        private readonly TicketService _ticketService;
        private readonly User _currentUser;

        // Constructor that accepts the ticket service and the current user
        public AddTicketForUserWindow(TicketService ticketService, User currentUser)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _currentUser = currentUser;
            UserNameTextBlock.Text = $"Creating ticket for: {_currentUser.Username}"; // Display the username

            LoadTitlesAsync(); // Load available titles when the window is initialized
        }

        private async Task LoadTitlesAsync()
        {
            try
            {
                // Fetch titles from the service and bind to ComboBox
                var titles = await _ticketService.GetAllTitlesAsync();
                ticketTitleComboBox.ItemsSource = titles;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading titles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler for the Add Ticket button
        private async void AddTicketButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (ticketTitleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a ticket title.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ticketDescriptionTextBox.Text))
            {
                MessageBox.Show("Please enter a ticket description.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (priorityComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a priority.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create a new ticket object
            var newTicket = new Ticket
            {
                Title = ticketTitleComboBox.SelectedItem.ToString(), // Get the selected title
                Description = ticketDescriptionTextBox.Text.Trim(),
                Priority = (priorityComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Low", // Default to "Low" if null
                CreatedAt = DateTime.Now,
                Status = "Open",
                UserId = _currentUser.Id
            };

            try
            {
                // Save the new ticket using the TicketService
                await _ticketService.AddTicketAsync(newTicket);

                // Show a success message
                MessageBox.Show("Ticket added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Close the window and return a dialog result indicating success
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                // Show an error message
                MessageBox.Show($"An error occurred while adding the ticket: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler for the Cancel button
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window without saving
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window without saving
            Close();
        }
    }
}
