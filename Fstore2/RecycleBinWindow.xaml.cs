using System.Windows;
using Fstore.BLL.Services;
using Fstore.DAL.ViewModels;

namespace Fstore2
{
    public partial class RecycleBinWindow : Window
    {
        private readonly TicketService _ticketService;

        public RecycleBinWindow(TicketService ticketService)
        {
            InitializeComponent();
            _ticketService = ticketService;
            LoadDeletedTickets();
        }

        // Tải danh sách các vé đã xóa và hiển thị trong DataGrid
        private async void LoadDeletedTickets()
        {
            List<TicketViewModel> deletedTickets = await _ticketService.GetDeletedTicketsAsync();
            recycleBinDataGrid.ItemsSource = deletedTickets;
        }

        // Khôi phục vé đã xóa
        private async void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (recycleBinDataGrid.SelectedItem is TicketViewModel selectedTicket)
            {
                bool restored = await _ticketService.RestoreTicketAsync(selectedTicket.Id);
                if (restored)
                {
                    MessageBox.Show("Ticket has been restored.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDeletedTickets(); // Tải lại danh sách vé đã xóa sau khi khôi phục
                }
                else
                {
                    MessageBox.Show("Failed to restore ticket. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a ticket to restore.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Đóng cửa sổ
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnDeletePrimary_Click(object sender, RoutedEventArgs e)
        {
            var selectedTicket = recycleBinDataGrid.SelectedItem as TicketViewModel;
            if (selectedTicket != null)
            {
                var result = MessageBox.Show($"Are you sure you want to permanently delete the ticket '{selectedTicket.Title}'?",
                                             "Delete Primary", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _ticketService.DeleteTicketPermanentlyAsync(selectedTicket.Id);
                        MessageBox.Show("Ticket deleted permanently.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDeletedTickets(); // Refresh data after deletion
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting ticket: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a ticket to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
