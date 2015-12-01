using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebSocketSharp;

namespace NodeJS_Client
{
    public class WebConnect
    {
        public WebSocket Socket;
        public MainWindow appWin;

        public WebConnect()
        { }

        public void ConnectWebSocket(string Address)
        {
            Socket = new WebSocket(Address, "connection");
            Socket.OnOpen += ServerToClientConnected;
            Socket.OnMessage += ServerToClientOnMessage;
            Socket.OnClose += ServerToClientDisconnected;

            Socket.Connect();
        }

        public void ReConnectWebSocket(string Address)
        {
            if( Socket != null )
                Socket.Close();

            Socket = null;
            ConnectWebSocket(Address);
        }

        public async void ServerToClientOnMessage(object sender, MessageEventArgs e)
        {
            Task<JSonPacket> PacketTask = JsonConvert.DeserializeObjectAsync<JSonPacket>(e.Data);
            JSonPacket ReceivePacket = await PacketTask;
            
            if (ReceivePacket.Protocol == "request_login")
                request_login_OK(ReceivePacket);
            else if (ReceivePacket.Protocol == "request_chat")
                request_chat_OK(ReceivePacket);
        }


        public async void ServerToClientConnected(object sender, EventArgs e)
        {
            appWin.SetLogText("Connect to Server");
        }
        
        public async void ServerToClientDisconnected(object sender, CloseEventArgs e)
        {
            appWin.SetLogText("Disconnected from Server");
        }

        public void SendPacket(JSonPacket Packet)
        {
            Socket.Send(JsonConvert.SerializeObject(Packet));
        }

        public void request_login_OK(JSonPacket LoginPacket)
        {
            string UserID = LoginPacket.Packets[0].ToString();
            string UserNumber = LoginPacket.Packets[1].ToString();

            UserInfo ServerUserInfo = new UserInfo();
            ServerUserInfo.UserID = UserID;
            ServerUserInfo.UserServerIndex = UserNumber;

            appWin.LoginSuccess(ServerUserInfo);
            appWin.SetLogText(UserID + "  " + UserNumber + " Connected");
        }

        public void request_chat_OK(JSonPacket ChatPacket)
        {
            string ChatUser = ChatPacket.Packets[0].ToString();
            string ChatData = ChatPacket.Packets[1].ToString();

            appWin.SetLogText(ChatUser + ": " + ChatData);
        }
    }

    public struct JSonPacket
    {
        public string Protocol { get; set; }
        public List<Object> Packets { get; set; }

        public void AddPacket( Object value )
        {
            if (Packets == null)
                Packets = new List<Object>();

            Packets.Add(value);
        }
    };

}
