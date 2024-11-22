using System.Windows;
using Fstore.BLL.Services;

namespace Fstore
{
    public partial class ResetPasswordWindow : Window
    {
        private readonly UserService _userService;
        private string _email;
        private readonly TicketService _ticketService;

        public ResetPasswordWindow(string email, UserService userService)
        {
            InitializeComponent();
            _email = email;
            _userService = userService;
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = txtPassword.Password;
            string rePassword = txtRePassword.Password;

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(rePassword))
            {
                MessageBox.Show("Please enter both password fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword != rePassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool isPasswordReset = await _userService.ResetUserPasswordAsync(_email, newPassword);

                if (isPasswordReset)
                {
                    MessageBox.Show("Password reset successful! You can now log in with your new password.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    var loginWindow = new LoginWindow(_userService, _ticketService);
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Password reset failed. Try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while resetting the password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
