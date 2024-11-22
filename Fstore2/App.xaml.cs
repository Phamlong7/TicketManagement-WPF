using Fstore.BLL.Services;
using Fstore.DAL.Repositories;
using Fstore;
using System.Windows;
using Fstore.DAL;

namespace Fstore2
{
    public partial class App : Application
    {
        private readonly UserService _userService;
        private readonly TicketService _ticketService;
        private readonly TicketRepository _ticketRepository;
        private readonly UserRepository _userRepository;
        private readonly AppDbContext _context;

        public App()
        {
            // Initialize your services here
            _context = new AppDbContext(); // Initialize AppDbContext
            _userRepository = new UserRepository(_context); // Pass AppDbContext to UserRepository
            _userService = new UserService(_userRepository); // Pass UserRepository to UserService
            _ticketRepository = new TicketRepository(_context); // Pass AppDbContext to TicketRepository
            _ticketService = new TicketService(_ticketRepository); // Pass TicketRepository to TicketService
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create and show the LoginWindow with dependency injection
            var loginWindow = new LoginWindow(_userService, _ticketService);
            loginWindow.Show();
        }
    }
}
