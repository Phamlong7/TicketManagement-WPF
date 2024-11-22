using System;
using System.Linq;
using System.Windows;
using Fstore.BLL.Services; // Ensure this namespace includes your service layer
using Fstore.DAL.Models;
using Fstore.DAL.ViewModels;
using Fstore2;   // Ensure this namespace includes your data models

namespace Fstore
{
    public partial class TicketManagementWindow : Window
    {
        private readonly TicketService _ticketService; // Ticket service for handling ticket operations
        private readonly int _currentUserId; // Current user ID for operations
        private readonly UserService _userService;
        //udpate
        public static UserTicketHistory? UserTicketHistory { get; set; }
        public TicketManagementWindow(TicketService ticketService, int currentUserId, UserService userService)
        {
            InitializeComponent();
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _currentUserId = currentUserId;
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            // Disable Edit and Delete buttons initially
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            LoadTickets();
        }

        // Method to load all tickets
        private async void LoadTickets()
        {
            try
            {
                // Clear existing items
                ticketDataGrid.ItemsSource = null;
                var tickets = await _ticketService.GetAllTicketsAsync();

                // Check if tickets is not null and not empty
                if (tickets != null && tickets.Any())
                {
                    ticketDataGrid.ItemsSource = tickets.ToList(); // Ensure it is a list for proper binding
                }
                else
                {
                    MessageBox.Show("No tickets available.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    ticketDataGrid.ItemsSource = null; // Clear the DataGrid if no tickets are available
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tickets: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler for the Add button
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addTicketWindow = new TicketDetailWindow(_ticketService, _currentUserId);
            if (addTicketWindow.ShowDialog() == true)
            {
                LoadTickets(); // Reload tickets after adding a new ticket
            }
        }

        // Event handler for the Edit button
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedTicket = ticketDataGrid.SelectedItem as TicketViewModel;

            if (selectedTicket != null)
            {
                var ticket = new Ticket
                {
                    Id = selectedTicket.Id,
                    Title = selectedTicket.Title,
                    Description = selectedTicket.Description,
                    Status = selectedTicket.Status,
                    Priority = selectedTicket.Priority,
                    CreatedAt = selectedTicket.CreatedAt,
                };

                var updateTicketWindow = new UpdateTicketWindow(_ticketService, ticket);
                if (updateTicketWindow.ShowDialog() == true)
                {
                    LoadTickets(); // Reloads the data to reflect the update
                }
            }
            else
            {
                MessageBox.Show("Please select a ticket to edit.", "Edit Ticket", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            LoadTickets();
        }

        // Event handler for the Delete button
        //private async void btnDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    var selectedTicket = ticketDataGrid.SelectedItem as TicketViewModel;

        //    if (selectedTicket != null)
        //    {
        //        var result = MessageBox.Show($"Are you sure you want to delete the ticket with ID {selectedTicket.Id}?",
        //                                     "Delete Ticket", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        //        if (result == MessageBoxResult.Yes)
        //        {
        //            try
        //            {
        //                await _ticketService.DeleteTicketasync(selectedTicket.Id);
        //                MessageBox.Show($"Ticket with ID {selectedTicket.Id} has been deleted.", "Delete Ticket", MessageBoxButton.OK, MessageBoxImage.Information);
        //                LoadTickets();  // Làm mới danh sách để ẩn vé đã xóa
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"Error deleting ticket: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please select a ticket to delete.", "Delete Ticket", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedTicket = ticketDataGrid.SelectedItem as TicketViewModel;

            if (selectedTicket != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the ticket with ID {selectedTicket.Id}?",
                                             "Delete Ticket", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _ticketService.DeleteTicketasync(selectedTicket.Id);
                        MessageBox.Show($"Ticket with ID {selectedTicket.Id} has been deleted.", "Delete Ticket", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadTickets();  // Làm mới danh sách vé trong TicketManagementWindow

                        // Gửi thông báo cập nhật tới UserTicketHistory
                        UserTicketHistory?.RefreshUserTicketHistory();  // Gọi phương thức cập nhật nếu cửa sổ UserTicketHistory đang mở
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting ticket: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a ticket to delete.", "Delete Ticket", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // Close the window when the close button is clicked
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Event handler for the Search button click
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            try
            {
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var tickets = await _ticketService.SearchTicketsAsync(searchTerm);
                    ticketDataGrid.ItemsSource = tickets.ToList(); // Ensure it is a list for proper binding
                }
                else
                {
                    LoadTickets(); // Reload all tickets if the search term is empty
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching tickets: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Placeholder text visibility logic for search bar
        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            placeholderText.Visibility = Visibility.Collapsed;
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                placeholderText.Visibility = Visibility.Visible;
            }
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        // Update button state based on ticket selection and status
        private void ticketDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool hasSelectedItem = ticketDataGrid.SelectedItem != null;
            btnDelete.IsEnabled = hasSelectedItem;

            // Check if the selected item has a "Resolved" status
            if (hasSelectedItem && ticketDataGrid.SelectedItem is TicketViewModel selectedTicket)
            {
                btnEdit.IsEnabled = !string.Equals(selectedTicket.Status, "Resolved", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                btnEdit.IsEnabled = false;
            }
        }

        private void btnChange_Title(object sender, RoutedEventArgs e)
        {
            var changeTitleWindow = new ChangeTitleWindow(_ticketService, _currentUserId, _userService);
            if (changeTitleWindow.ShowDialog() == true)
            {
                LoadTickets(); // Reload tickets after adding a new ticket
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

            // Đóng cửa sổ UserTicketHistory nếu đang mở
            if (UserTicketHistory != null)
            {
                UserTicketHistory.Close();
                UserTicketHistory = null; // Reset tham chiếu sau khi đóng
            }

            // Mở lại cửa sổ đăng nhập
            var loginWindow = new LoginWindow(_userService, _ticketService);
            loginWindow.Show();

            // Đóng cửa sổ TicketManagementWindow hiện tại
            this.Close();
        }

        private void btnRecycleBin_Click(object sender, RoutedEventArgs e)
        {
            // Mở cửa sổ RecycleBinWindow và truyền TicketService vào
            var recycleBinWindow = new RecycleBinWindow(_ticketService);
            recycleBinWindow.ShowDialog(); // Sử dụng ShowDialog() để mở cửa sổ dưới dạng modal
        }




    }
}
