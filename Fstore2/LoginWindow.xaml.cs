using System.Windows;
using System.Windows.Input;
using Fstore.BLL.Services;
using Fstore.DAL.Models;

namespace Fstore
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;
        private readonly TicketService _ticketService;

        public LoginWindow(UserService userService, TicketService ticketService)
        {
            InitializeComponent();
            _userService = userService;
            _ticketService = ticketService;
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both email and password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                btnLogin.IsEnabled = false;

                // Authenticate user
                User? user = await _userService.AuthenticateUserAsync(email, password);

                if (user != null)
                {
                    Window nextWindow = user.Role == "Admin"
                        ? new TicketManagementWindow(_ticketService, user.Id, _userService)
                        : new UserTicketHistory(_ticketService, user.Id, _userService);

                    nextWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid email or password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during login: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnLogin.IsEnabled = true;
            }
        }

        private void Register_Click(object sender, MouseButtonEventArgs e)
        {
            var registerWindow = new RegisterWindow(_userService, _ticketService);
            registerWindow.Show();
            this.Close();
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            var forgotPasswordWindow = new ForgotPasswordWindow(_userService, _ticketService);
            forgotPasswordWindow.Show();
            this.Close();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
