using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using WebSocketSharp;
using System.Timers;
using System.Collections.Concurrent;





namespace NodeJS_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WebConnect WebConnector;
        string ConnectAddress;

        public UserInfo User;
        public bool IsLogin = false;

        public void LoginSuccess( UserInfo ServerUserInfo )
        {
            User = ServerUserInfo;
            IsLogin = true;
        }

        public MainWindow()
        {
            InitializeComponent();

            ConnectAddress = "ws://localhost:8080";
            WebConnector = new WebConnect();
            WebConnector.appWin = this;
        }

        public void SendString_Click(object sender, RoutedEventArgs e)
        {
            if (IsLogin == false)
                return;

            JSonPacket ChatPacket = new JSonPacket();
            ChatPacket.Protocol = "request_chat";

            String ChattingText = ChatInputBox.Text;
            ChatPacket.AddPacket(User.UserID);
            ChatPacket.AddPacket(ChattingText);
            
            WebConnector.SendPacket(ChatPacket);
            ChatInputBox.Clear();
        }

        public void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string InputUserID = Input_ID.Text;
            string InputPassword = Input_Password.Password;

            JSonPacket LoginPacket = new JSonPacket();
            LoginPacket.Protocol = "request_login";
            
            LoginPacket.AddPacket(InputUserID);
            LoginPacket.AddPacket(InputPassword);

            WebConnector.SendPacket(LoginPacket);
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            WebConnector.ReConnectWebSocket(ConnectAddress);
        }

        public delegate void LogDelegate(string arg);
        public void SetLogText(string log)
        {
            ServerLog.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new LogDelegate(UpdateLogText), log);
        }

        public void UpdateLogText(string log)
        {
            ServerLog.Items.Add(log);
        }

    }

    public struct UserInfo
    {
        public string UserID;
        public string UserServerIndex;
    };
}
