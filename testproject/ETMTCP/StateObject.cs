using System.Net.Sockets;

namespace Org.Openengsb.Loom.CSharp.Bridge.ETM.TCP
{
    /// <summary>
    /// State Object for asynchronious send/receive
    /// </summary>
    public class StateObject
    {
        #region Const
        private const int bufferSize = 8192;
        #endregion
        #region Variables
        public int BufferSize { get { return bufferSize; } }
        public byte[] Buffer { get; private set; }
        /// <summary>
        /// The socket, on which a message has been received/send
        /// </summary>
        public Socket socket { get; private set; }
        public int SocketID { get; private set; }
        #endregion
        #region constructor
        public StateObject(Socket workSocket, int SocketID)
        {
            this.socket = workSocket;
            Buffer = new byte[BufferSize];
            this.SocketID = SocketID;
        }
        #endregion
    }
}