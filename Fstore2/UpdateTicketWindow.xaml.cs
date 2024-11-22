using System;
using System.Windows;
using System.Windows.Controls;
using Fstore.BLL.Services;
using Fstore.DAL.Models;

namespace Fstore
{
    public partial class UpdateTicketWindow : Window
    {
        private readonly TicketService _ticketService;
        private readonly Ticket _currentTicket;

        public UpdateTicketWindow(TicketService ticketService, Ticket selectedTicket)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _currentTicket = selectedTicket;
            LoadTicketData();

            // Add event handler for selection change in cmbNewStatus
            cmbNewStatus.SelectionChanged += CmbNewStatus_SelectionChanged;
            btnUpdate.IsEnabled = false; // Initially disable btnUpdate
        }

        private void LoadTicketData()
        {
            txtTicketId.Text = _currentTicket.Id.ToString();
            txtCurrentStatus.Text = _currentTicket.Status;
        }

        // Enable the Update button if a different status is selected
        private void CmbNewStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ensure a new status is selected and is different from the current one
            if (cmbNewStatus.SelectedItem is ComboBoxItem selectedStatusItem)
            {
                string newStatus = selectedStatusItem.Content.ToString();
                btnUpdate.IsEnabled = !string.Equals(newStatus, _currentTicket.Status, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                btnUpdate.IsEnabled = false; // Disable if no selection
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (cmbNewStatus.SelectedItem != null)
            {
                var selectedStatusItem = (ComboBoxItem)cmbNewStatus.SelectedItem;
                string newStatus = selectedStatusItem.Content.ToString();

                // Check if the new status is "Resolved"
                if (string.Equals(newStatus, "Resolved", StringComparison.OrdinalIgnoreCase))
                {
                    // Show confirmation dialog
                    var result = MessageBox.Show("Are you sure you want to approve this ticket?",
                                                 "Confirmation",
                                                 MessageBoxButton.YesNo,
                                                 MessageBoxImage.Warning);
                    if (result != MessageBoxResult.Yes)
                    {
                        return; // Exit if the user clicks "No"
                    }
                }

                _currentTicket.Status = newStatus;

                try
                {
                    var updatedTicket = await _ticketService.UpdateTicketAsync(_currentTicket);
                    if (updatedTicket != null)
                    {
                        MessageBox.Show($"Ticket status has been updated to '{newStatus}'", "Update Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update the ticket. Please try again.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while updating the ticket: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a new status.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void txtCurrentStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
