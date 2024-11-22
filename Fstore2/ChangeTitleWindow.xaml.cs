//using Fstore;
//using Fstore.BLL.Services;
//using System.Collections.ObjectModel;
//using System.Windows;

//namespace Fstore2
//{
//    public partial class ChangeTitleWindow : Window
//    {
//        public ObservableCollection<string> Titles { get; set; }
//        private readonly TicketService _ticketService;
//        private readonly int _currentUserId;
//        private readonly UserService _userService;

//        public ChangeTitleWindow(TicketService ticketService, int currentUserId, UserService userService)
//        {
//            InitializeComponent();
//            Titles = new ObservableCollection<string>();
//            lstTitles.ItemsSource = Titles;
//            _ticketService = ticketService;
//            _currentUserId = currentUserId;
//            _userService = userService;

//            LoadTitlesAsync(); // Load existing titles when the window is initialized
//        }

//        private async void LoadTitlesAsync()
//        {
//            try
//            {
//                // Fetch titles using _ticketService
//                var titles = await _ticketService.GetAllTitlesAsync();

//                // Check if titles were returned
//                if (titles != null)
//                {
//                    Titles.Clear();
//                    foreach (var title in titles)
//                    {
//                        Titles.Add(title);
//                    }
//                }
//                else
//                {
//                    MessageBox.Show("No titles found in the database.", "Data Load Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading titles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private async void btnAddTitle_Click(object sender, RoutedEventArgs e)
//        {
//            // Add a new title if input is not empty
//            if (!string.IsNullOrWhiteSpace(txtTitle.Text))
//            {
//                var newTitle = txtTitle.Text.Trim();

//                // Add to the database and list if successful
//                await _ticketService.AddTitleAsync(newTitle);
//                Titles.Add(newTitle);
//                txtTitle.Clear(); // Clear the textbox after adding
//            }
//            else
//            {
//                MessageBox.Show("Please enter a valid title.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
//            }
//        }

//        private async void btnDeleteTitle_Click(object sender, RoutedEventArgs e)
//        {
//            // Remove the selected title from the list
//            if (lstTitles.SelectedItem != null)
//            {
//                var selectedTitle = lstTitles.SelectedItem.ToString();

//                // Remove from the database and list if successful
//                await _ticketService.DeleteTicketasync(selectedTitle);
//                Titles.Remove(selectedTitle);
//            }
//            else
//            {
//                MessageBox.Show("Please select a title to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
//            }
//        }

//            private void btnBack_Click(object sender, RoutedEventArgs e)
//            {
//                // Navigate back to the TicketManagementWindow
//                var ticketManagementWindow = new TicketManagementWindow(_ticketService, _currentUserId, _userService);
//                ticketManagementWindow.Show();
//                this.Close();
//            }
//        }
//    }
//}

using Fstore;
using Fstore.BLL.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace Fstore2
{
    public partial class ChangeTitleWindow : Window
    {
        public ObservableCollection<string> Titles { get; set; }
        private readonly TicketService _ticketService;
        private readonly int _currentUserId;
        private readonly UserService _userService;

        public ChangeTitleWindow(TicketService ticketService, int currentUserId, UserService userService)
        {
            InitializeComponent();
            Titles = new ObservableCollection<string>();
            lstTitles.ItemsSource = Titles;
            _ticketService = ticketService;
            _currentUserId = currentUserId;
            _userService = userService;

            LoadTitlesAsync(); // Load existing titles when the window is initialized
        }

        private async void LoadTitlesAsync()
        {
            try
            {
                var titles = await _ticketService.GetAllTitlesAsync();

                if (titles != null && titles.Count > 0) // Kiểm tra danh sách titles
                {
                    Titles.Clear();
                    foreach (var title in titles)
                    {
                        Titles.Add(title);
                    }
                }
                else
                {
                    MessageBox.Show("No titles found in the database.", "Data Load Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading titles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnAddTitle_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                var newTitle = txtTitle.Text.Trim();

                try
                {
                    await _ticketService.AddTitleAsync(newTitle);
                    Titles.Add(newTitle);
                    txtTitle.Clear(); // Xóa ô nhập liệu sau khi thêm thành công
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding title: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid title.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void btnDeleteTitle_Click(object sender, RoutedEventArgs e)
        {
            if (lstTitles.SelectedItem != null)
            {
                var selectedTitle = lstTitles.SelectedItem.ToString();

                try
                {
                    await _ticketService.RemoveTitleAsync(selectedTitle); // Sử dụng RemoveTitleAsync thay vì DeleteTicketasync
                    Titles.Remove(selectedTitle); // Xóa khỏi danh sách sau khi xóa thành công
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting title: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a title to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Điều hướng về TicketManagementWindow
            var ticketManagementWindow = new TicketManagementWindow(_ticketService, _currentUserId, _userService);
            ticketManagementWindow.Show();
            this.Close();
        }
    }
}

