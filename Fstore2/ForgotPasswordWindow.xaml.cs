using System.Windows;
using Fstore.BLL.Services;

namespace Fstore
{
    public partial class ForgotPasswordWindow : Window
    {
        private readonly UserService _userService;
        private readonly TicketService _ticketService;

        public ForgotPasswordWindow(UserService userService, TicketService ticketService)
        {
            InitializeComponent();
            _userService = userService;
            _ticketService = ticketService;
        }

        private async void VerifyEmail_Click(object sender, RoutedEventArgs e)
        {
            string email = emailTextBox.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter an email address.");
                return;
            }

            // Call the async method to check if the email exists
            bool isRegistered = await _userService.CheckIfEmailExistsAsync(email);

            if (isRegistered)
            {
                var resetPasswordWindow = new ResetPasswordWindow(email, _userService);
                resetPasswordWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Email not found. Please enter a registered email address.");
            }
        }

        private void BackToLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var loginWindow = new LoginWindow(_userService, _ticketService);
            loginWindow.Show();
            this.Close();
        }
    }
}
