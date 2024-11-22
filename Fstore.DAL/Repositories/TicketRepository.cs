using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fstore.DAL.Models;
using Fstore.DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Fstore.DAL.Repositories
{
    public class TicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        // Add a new ticket
        public async Task<Ticket> AddTicketAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        // Method to get tickets by user ID, projecting to TicketViewModel
        public async Task<IEnumerable<TicketViewModel>> GetTicketsByUserIdAsync(int userId)
        {
            return await _context.Tickets
                .Where(ticket => ticket.UserId == userId)
                .Select(ticket => new TicketViewModel
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status,
                    Priority = ticket.Priority,
                    CreatedAt = ticket.CreatedAt,
                    Username = _context.Users.Where(user => user.Id == ticket.UserId).Select(user => user.Username).FirstOrDefault(),
                    IsDeleted = ticket.IsDeleted
                })
                .ToListAsync();
        }

        // Get all tickets with usernames
        public async Task<List<TicketViewModel>> GetAllTicketsAsync()
        {
            return await _context.Tickets
                .Where(ticket => !ticket.IsDeleted)  // Chỉ lấy các vé chưa bị xóa mềm
                .Join(
                    _context.Users,
                    ticket => ticket.UserId,
                    user => user.Id,
                    (ticket, user) => new TicketViewModel
                    {
                        Id = ticket.Id,
                        Title = ticket.Title,
                        Description = ticket.Description,
                        Status = ticket.Status,
                        Priority = ticket.Priority,
                        CreatedAt = ticket.CreatedAt,
                        DeletedAt = ticket.DeletedAt,  // Gán DeletedAt nếu có
                        Username = user.Username        // Gán Username
                    })
                .AsNoTracking()
                .ToListAsync();
        }


        // Update ticket details
        public async Task<Ticket?> UpdateTicketAsync(Ticket ticket)
        {
            var existingTicket = await GetTicketByIdAsync(ticket.Id);
            if (existingTicket == null) return null;

            existingTicket.Title = ticket.Title;
            existingTicket.Description = ticket.Description;
            existingTicket.Priority = ticket.Priority;
            existingTicket.Status = ticket.Status;

            await _context.SaveChangesAsync();
            return existingTicket;
        }

        //Delete ticket -  Loại phương thức DeleteTicketAsync, nếu giữ lại thì sẽ
        // gây xung đột với DbContext, chỉ sử dụng phương thức xóa mềm SoftDeleteTicketAsync
        //public async Task<bool> DeleteTicketAsync(int id)
        //{
        //    var ticket = await GetTicketByIdAsync(id);
        //    if (ticket == null) return false;

        //    _context.Tickets.Remove(ticket);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        // Method to search tickets
        public async Task<List<Ticket>> SearchTicketsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<Ticket>(); // Return an empty list if the search term is invalid
            }

            return await _context.Tickets
                .Where(t => t.Title.Contains(searchTerm) || t.Id.ToString().Equals(searchTerm))
                .ToListAsync();
        }

        // Get ticket by ID - helper method
        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            return await _context.Tickets.FindAsync(id);
        }
        public async Task<List<string>> GetTitlesAsync()
        {
            // Assuming you have a Ticket model with a Title property
            return await _context.Tickets
                .Select(ticket => ticket.Title)
                .ToListAsync(); // Fetch titles from the Tickets table
        }
        public async Task<List<string>> GetAllTitlesAsync()
        {
            return await _context.Titles
                .Select(t => t.Name) // Adjust property name if necessary
                .ToListAsync();
        }

        public async Task AddTitleAsync(string title)
        {
            var newTitle = new Title { Name = title }; // Assuming Title has a Name property
            await _context.Titles.AddAsync(newTitle);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTitleAsync(string title)
        {
            var titleToRemove = await _context.Titles.FirstOrDefaultAsync(t => t.Name == title);
            if (titleToRemove != null)
            {
                _context.Titles.Remove(titleToRemove);
                await _context.SaveChangesAsync();
            }
        }

        // Xóa mềm vé
        public async Task<bool> SoftDeleteTicketAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null && !ticket.IsDeleted)
            {
                ticket.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Khôi phục vé đã xóa
        public async Task<bool> RestoreTicketAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket != null && ticket.IsDeleted)
            {
                ticket.IsDeleted = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Lấy danh sách các vé đã xóa
        public async Task<List<TicketViewModel>> GetDeletedTicketsAsync()
        {
            return await _context.Tickets
                .Where(t => t.IsDeleted)
                .Select(ticket => new TicketViewModel
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status,
                    Priority = ticket.Priority,
                    CreatedAt = ticket.CreatedAt,
                    Username = _context.Users.Where(user => user.Id == ticket.UserId).Select(user => user.Username).FirstOrDefault()
                })
                .ToListAsync();
        }
        public async Task DeletePermanentlyAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            }
        }


    }
}
