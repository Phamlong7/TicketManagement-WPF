using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using Fstore.BLL.Services;
using Fstore.DAL.Models;

namespace Fstore
{
    public partial class TicketDetailWindow : Window
    {
        private readonly TicketService _ticketService;
        private readonly int _currentUserId;

        public TicketDetailWindow(TicketService ticketService, int currentUserId)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _currentUserId = currentUserId;
            LoadTitlesAsync(); // Load titles if needed
        }

        private async Task LoadTitlesAsync()
        {
            try
            {
                var titles = await _ticketService.GetAllTitlesAsync(); // Fetch titles
                txtTitle.ItemsSource = titles; // Set the ComboBox's item source
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading titles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs())
                return;

            var newTicket = new Ticket
            {
                Title = txtTitle.Text, // Get the title from ComboBox
                Description = txtDescription.Text,
                Priority = cmbPriority.SelectedItem is ComboBoxItem item ? item.Content.ToString() : "Medium", // Default if null
                Status = "Open",
                UserId = _currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _ticketService.AddTicketAsync(newTicket);
                MessageBox.Show("Ticket saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please select a ticket title.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please enter a description.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (cmbPriority.SelectedItem == null)
            {
                MessageBox.Show("Please select a priority.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void CloseWindow_Click(object sender, RoutedEventArgs e) => Close();

        private void txtTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
