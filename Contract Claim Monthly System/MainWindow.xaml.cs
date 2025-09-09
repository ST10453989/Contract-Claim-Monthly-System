using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClaimSystem
{
    public partial class MainWindow : Window
    {
        // Claim class
        public class Claim
        {
            public string LecturerName;
            public double HoursWorked;
            public double HourlyRate;
            public string Notes;
            public string DocumentFile;
            public string Status = "Pending";
            public string DisplayText => $"{LecturerName} | {HoursWorked}h @ {HourlyRate}/h | {Status}";
        }

        private string CurrentUsername;
        private string CurrentUserRole;
        private string UploadedFilePath = "";

        private List<Claim> AllClaims = new List<Claim>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Login
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Enter username and password.");
                return;
            }

            if (password.Length < 3)
            {
                MessageBox.Show("Password must be at least 3 characters.");
                return;
            }

            CurrentUsername = username;
            CurrentUserRole = role;

            SignInPanel.Visibility = Visibility.Collapsed;

            if (CurrentUserRole == "Lecturer")
            {
                LecturerPanel.Visibility = Visibility.Visible;
                RefreshLecturerClaims();
            }
            else
            {
                ApprovalTitle.Text = CurrentUserRole + " Dashboard";
                ApprovalPanel.Visibility = Visibility.Visible;
                RefreshPendingClaims();
            }
        }

        // Upload document
        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Documents (*.pdf;*.docx;*.xlsx)|*.pdf;*.docx;*.xlsx";
            if (dlg.ShowDialog() == true)
            {
                UploadedFilePath = dlg.FileName;
                UploadedFileName.Text = System.IO.Path.GetFileName(UploadedFilePath);
            }
        }

        // Submit claim
        private void SubmitClaim_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(HoursWorkedBox.Text, out double hours) ||
                !double.TryParse(HourlyRateBox.Text, out double rate))
            {
                MessageBox.Show("Enter valid numbers for hours and rate.");
                return;
            }

            Claim newClaim = new Claim
            {
                LecturerName = CurrentUsername,
                HoursWorked = hours,
                HourlyRate = rate,
                Notes = NotesBox.Text,
                DocumentFile = UploadedFilePath
            };
            AllClaims.Add(newClaim);

            HoursWorkedBox.Text = "";
            HourlyRateBox.Text = "";
            NotesBox.Text = "";
            UploadedFileName.Text = "";
            UploadedFilePath = "";

            RefreshLecturerClaims();
        }

        // Refresh lecturer claims
        private void RefreshLecturerClaims()
        {
            ClaimsListBox.Items.Clear();
            foreach (var claim in AllClaims.Where(c => c.LecturerName == CurrentUsername))
            {
                ClaimsListBox.Items.Add(claim.DisplayText);
            }
        }

        // Refresh pending claims
        private void RefreshPendingClaims()
        {
            PendingClaimsListBox.Items.Clear();
            foreach (var claim in AllClaims.Where(c => c.Status == "Pending"))
            {
                PendingClaimsListBox.Items.Add(claim);
            }
        }

        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            var selected = PendingClaimsListBox.SelectedItem as Claim;
            if (selected != null)
            {
                selected.Status = "Approved";
                RefreshPendingClaims();
            }
        }

        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            var selected = PendingClaimsListBox.SelectedItem as Claim;
            if (selected != null)
            {
                selected.Status = "Rejected";
                RefreshPendingClaims();
            }
        }
    }
}
