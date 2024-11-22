using System.Windows;
using System.Windows.Input;
using Fstore.BLL.Services;

namespace Fstore
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private readonly UserService _userService;
        private readonly TicketService _ticketService;

        // Constructor with UserService and TicketService injection
        public RegisterWindow(UserService userService, TicketService ticketService)
        {
            InitializeComponent();
            _userService = userService;
            _ticketService = ticketService;
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // Input validation
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill out all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Register the user
                var result = await _userService.AddUserAsync(username, email, password);

                if (result != null)
                {
                    MessageBox.Show("Registration successful! Please log in.", "Registration Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    var loginWindow = new LoginWindow(_userService, _ticketService);
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Registration failed. This email may already be in use.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during registration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BackToLogin_Click(object sender, MouseButtonEventArgs e)
        {
            var loginWindow = new LoginWindow(_userService, _ticketService);
            loginWindow.Show();
            this.Close();
        }
    }
}
