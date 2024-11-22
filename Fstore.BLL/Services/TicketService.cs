using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fstore.DAL;
using Fstore.DAL.Models;
using Fstore.DAL.Repositories;
using Fstore.DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Fstore.BLL.Services
{
    public class TicketService
    {
        private readonly TicketRepository _ticketRepository;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public TicketService(TicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        // Add a new ticket
        public async Task<Ticket> AddTicketAsync(Ticket ticket)
        {
            ValidateTicket(ticket);
            return await _ticketRepository.AddTicketAsync(ticket);
        }

        // Get ticket by ID
        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            return await _ticketRepository.GetTicketByIdAsync(id);
        }

        // Get all tickets
        public async Task<IEnumerable<TicketViewModel>> GetAllTicketsAsync()
        {
            
            //return await _ticketRepository.GetAllTicketsAsync();


            await _semaphore.WaitAsync();  // Đảm bảo chỉ một luồng thực thi
            try
            {
                return await _ticketRepository.GetAllTicketsAsync();
            }
            finally
            {
                _semaphore.Release();
            }

        }

        // Update ticket details
        public async Task<bool> UpdateTicketAsync(Ticket ticket)
        {
            if (ticket == null || ticket.Id <= 0)
            {
                throw new ArgumentException("Invalid ticket.");
            }

            return await _ticketRepository.UpdateTicketAsync(ticket) != null;
        }


        // Method to validate the ticket fields
        private void ValidateTicket(Ticket ticket)
        {
            if (string.IsNullOrWhiteSpace(ticket.Title))
            {
                throw new ArgumentException("Title cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(ticket.Description))
            {
                throw new ArgumentException("Description cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(ticket.Priority))
            {
                throw new ArgumentException("Priority cannot be empty");
            }

            // Additional validation logic can be added here
        }

        // Method to search tickets
        public async Task<List<Ticket>> SearchTicketsAsync(string searchTerm)
        {
            return await _ticketRepository.SearchTicketsAsync(searchTerm);
        }

        // Method to get tickets by user ID
        public async Task<IEnumerable<TicketViewModel>> GetUserTicketsAsync(int userId)
        {
            return await _ticketRepository.GetTicketsByUserIdAsync(userId);
        }
        // Fetch all titles
        public async Task<List<string>> GetAllTitlesAsync()
        {
            return await _ticketRepository.GetAllTitlesAsync();
        }

        // Add a new title
        public async Task<bool> AddTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return false; // Optionally add more validations here

            await _ticketRepository.AddTitleAsync(title);
            return true;
        }

        //Remove an existing title by name
        public async Task<bool> RemoveTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return false;

            await _ticketRepository.RemoveTitleAsync(title);
            return true;
        }

        //thêm
        public async Task<bool> DeleteTicketasync(int id)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await _ticketRepository.SoftDeleteTicketAsync(id);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        // Lấy danh sách vé đã xóa cho thùng rác
        public async Task<List<TicketViewModel>> GetDeletedTicketsAsync()
        {
            return await _ticketRepository.GetDeletedTicketsAsync();
        }

        // Khôi phục vé đã xóa
        public async Task<bool> RestoreTicketAsync(int ticketId)
        {
            return await _ticketRepository.RestoreTicketAsync(ticketId);
        }

        public async Task DeleteTicketPermanentlyAsync(int ticketId)
        {
            await _ticketRepository.DeletePermanentlyAsync(ticketId);
        }


    }
}
