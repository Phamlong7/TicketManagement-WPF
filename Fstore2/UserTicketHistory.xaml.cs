using System.Collections.Generic;
using System.Windows;
using Fstore.BLL.Services;
using Fstore.DAL.Models;
using Fstore.DAL.ViewModels;
using System.Linq;
using System.ComponentModel;

namespace Fstore
{
    public partial class UserTicketHistory : Window
    {
        private readonly TicketService _ticketService;
        private readonly int _currentUserId;
        private readonly UserService _userService;
        private bool isFirstLoad = true; // Flag to track if this is the first load

        // Constructor accepting TicketService, current user ID, and UserService
        public UserTicketHistory(TicketService ticketService, int currentUserId, UserService userService)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _currentUserId = currentUserId;
            _userService = userService;
            LoadUserTicketHistory();
        }

        private async void LoadUserTicketHistory()
        {
            try
            {
                var userTicketHistory = await _ticketService.GetUserTicketsAsync(_currentUserId);

                // Lọc chỉ các vé chưa bị xóa mềm
                var filteredTickets = userTicketHistory.Where(t => !t.IsDeleted).ToList();

                if (filteredTickets.Any())
                {
                    ticketHistoryDataGrid.ItemsSource = filteredTickets;
                }
                else
                {
                    MessageBox.Show("No tickets found for this user.", "Ticket History", MessageBoxButton.OK, MessageBoxImage.Information);
                    ticketHistoryDataGrid.ItemsSource = null; // Xóa dữ liệu nếu không có vé
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading the ticket history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Event handler for the Add Ticket Button click
        private async void AddTicketButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Retrieve the current user object from the service layer
                var currentUser = await _userService.GetUserByIdAsync(_currentUserId);
                AddTicketForUserWindow addTicketWindow = new AddTicketForUserWindow(_ticketService, currentUser);

                if (addTicketWindow.ShowDialog() == true)
                {
                    LoadUserTicketHistory(); // Reload ticket history after adding a new ticket
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding a ticket: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to handle status change for future changes (won't display message again for resolved tickets on login)
        private void TicketStatusChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TicketViewModel.Status))
            {
                var ticket = sender as TicketViewModel;
                if (ticket?.Status == "Resolved" && !isFirstLoad)
                {
                    MessageBox.Show($"Congratulations! Your ticket '{ticket.Title}' has been approved!",
                                    "Ticket Resolved",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
        }

        // Event handler for the Close button
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Event handler for the Reload button click
        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserTicketHistory(); // Reloads the ticket history in the DataGrid
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow(_userService, _ticketService);
            loginWindow.Show();
            this.Close();
        }

        //update
        public void RefreshUserTicketHistory()
        {
            LoadUserTicketHistory(); // Gọi lại phương thức để làm mới danh sách vé
        }


    }
}
