using System.Net.WebSockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IT3B_Chat.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientWebSocket _clientSocket = new ClientWebSocket();

        public MainWindow() => InitializeComponent();

        public object ChatListBox { get; private set; }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _clientSocket.ConnectAsync(new Uri(ServerTextBox.Text), CancellationToken.None);
                MessageBox.Show("Connected to server.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}");
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = UsernameTextBox.Text;
                string message = MessageTextBox.Text;
                string combinedMessage = $"{username}: {message}";

                await _clientSocket.SendAsync(Encoding.UTF8.GetBytes(combinedMessage), WebSocketMessageType.Text, true, CancellationToken.None);
                object p = ChatListBox.Items.Add($"You: {message}");
                MessageTextBox.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sending message failed: {ex.Message}");
            }
        }
    }
}