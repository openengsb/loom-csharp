using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Collections.Concurrent;
using Apache.NMS.ActiveMQ.Commands;
using log4net;
using Org.Openengsb.Loom.CSharp.Bridge.Interfaces;
using Org.Openengsb.Loom.CSharp.Bridge.Protocol.ActiveMQ;

namespace Org.Openengsb.Loom.CSharp.Bridge.ETM.TCP
{
    /// <summary>
    /// Implementation of the ETM which listens on the TCP protocol
    /// </summary>
    public class ETMTCPImplementation : IETM
    {
        #region Variables
        private ILog logging = LogManager.GetLogger(typeof(ETMTCPImplementation));
        private ETMList interationConfiguration;
        private IDictionary<int, Socket> openClients = new ConcurrentDictionary<int, Socket>();
        private Socket serverSocket;
        /// <summary>
        /// Every message gets a position. This position is bind to the socket
        /// </summary>
        private IDictionary<int, int> receivedMessageposition = new ConcurrentDictionary<int, int>();
        private static Semaphore semaphore = new Semaphore(1, 1);
        public IDictionary<int, Dictionary<int, IProtocol>> ReceivedMessages { get; set; }
        #endregion
        #region Constructors
        public ETMTCPImplementation(List<InteractionMessage> workflow)
        {
            ReceivedMessages = new Dictionary<int, Dictionary<int, IProtocol>>();
            this.interationConfiguration = new ETMList(workflow);
        }
        #endregion        
        #region Private Methods
        /// <summary>
        /// Listen to all sockets, which could connect to this configurations
        /// </summary>
        /// <param name="endpoint">Endpoint on which the server listens</param>
        private void ListenToTCP(IPEndPoint endpoint)
        {
            if (serverSocket != null)
            {
                Dispose();
            }
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, false);
            serverSocket.Bind(endpoint);
            serverSocket.Listen(100);
            serverSocket.ReceiveBufferSize = 16384;
            serverSocket.SendBufferSize = 8192;
            serverSocket.BeginAccept(new AsyncCallback(ListenForClientsAsynchrone), serverSocket);
        }
        /// <summary>
        /// Listen on the network connection and waits that a client connects.
        /// When a client connects, the send and receive operation will be configured
        /// </summary>
        /// <param name="ar"></param>
        private void ListenForClientsAsynchrone(IAsyncResult ar)
        {
            logging.Info("Wating that a client connects");
            Socket server = (Socket)ar.AsyncState;
            Socket client = null;
            int clientID;
            try
            {
                client = server.EndAccept(ar);
                server.BeginAccept(new AsyncCallback(ListenForClientsAsynchrone), server);
            }
            catch (Exception ex)
            {
                logging.Info("It was unpossible to open a connection");
                logging.Debug(ex);
                return;
            }
            clientID = openClients.Count;
            //TODO
            logging.Info("New Client connected: " + clientID);
            receivedMessageposition.Add(clientID, 0);
            openClients.Add(clientID, client);
            StateObject state = new StateObject(client, clientID);

            client.BeginReceive(state.Buffer, 0, state.BufferSize, 0,
              new AsyncCallback(ReceiveCallback), state);
        }
        /// <summary>
        /// Asynchronous Send
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                if (ClientIsStillConnected(state))
                {
                    return;
                } state.socket.EndSend(ar);
            }
            catch
            {
            }
        } 
        /// <summary>
        /// Send the Responses
        /// </summary>
        /// <param name="messagetoSend"></param>
        /// <param name="socketId"></param>
        /// <param name="prot"></param>
        private void SentResponses(InteractionMessage messagetoSend, int socketId, IProtocol prot)
        {
            foreach (InteractionMessage responsemessage in messagetoSend.Responses)
            {
                if (responsemessage.DestinationPort == null || responsemessage.DestinationPort.Value <= 0)
                {
                    responsemessage.DestinationPort = ((IPEndPoint)openClients[socketId].RemoteEndPoint).Port;
                }
                responsemessage.Protocol.RetrieveInfoFromReceivedMessage(prot);
                logging.Info(GetLoggingMsg(responsemessage, socketId));
                SendToTCP(responsemessage, socketId);
            }
        }
        /// <summary>
        /// Asynchronous receive
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.socket;

                int bytesRead = client.EndReceive(ar);
                StateObject receivestate = new StateObject(state.socket, state.SocketID);
                if (ClientIsStillConnected(state))
                {
                    return;
                }
                client.BeginReceive(receivestate.Buffer, 0, receivestate.BufferSize, 0, new AsyncCallback(ReceiveCallback), receivestate);
                if (bytesRead > 0)
                {
                    HandleActions(state.Buffer, state.SocketID);
                }
                else if (ClientIsStillConnected(state))
                {
                    return;
                }
            }
            catch (Exception e)
            {
                logging.Info("Exception. Close the socket. " + e.Message);
            }
        }

        /// <summary>
        /// Checks if the connection to the client is still valid
        /// </summary>
        /// <param name="clientObject">Client socket informations</param>
        /// <returns></returns>
        private Boolean ClientIsStillConnected(StateObject clientObject)
        {
            Socket client;
            if (!openClients.ContainsKey(clientObject.SocketID))
            {
                return true;
            }
            client = openClients[clientObject.SocketID];
            if (!client.Connected)
            {
                client.Close();
                openClients.Remove(clientObject.SocketID);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if an array is empty or has just 0's;
        /// </summary>
        /// <param name="array">array to check</param>
        /// <returns></returns>
        private Boolean IsEmpty(Byte[] array)
        {
            return !array.Any(b => b != 0);
        }
        /// <summary>
        /// Copies all the array from a starting postion to the end
        /// </summary>
        /// <param name="source">array</param>
        /// <param name="start">start position</param>
        /// <returns>new array</returns>
        private byte[] CopyArray(Byte[] source, int start)
        {
            byte[] result = new byte[source.Length - start];
            for (int i = 0; i < source.Length - start; i++)
            {
                result[i] = source[start + i];
            }
            return result;
        }
        /// <summary>
        /// Handels the incoming messages from a socket.
        /// </summary>
        /// <param name="input">The received message from a socket</param>
        /// <param name="socketId">The socket number</param>
        private void HandleActions(byte[] input, int socketId)
        {
            if (IsEmpty(input))
            {
                return;
            }
            byte[] message = input;
            IProtocol prot = null;
            InteractionMessage transactions = interationConfiguration.FindInteraction(message, socketId, out prot);
            if (prot == null)
            {
                return;
            }
            message = CopyArray(input, prot.GetMessage().Length);

            if (prot is ActiveMQProtocol)
            {
                logging.Info(((ActiveMQProtocol)prot).Message.GetType().Name + " on socket: " + socketId + " with commandID: " + ((ActiveMQProtocol)prot).Message.CommandId);
            }
            if (!ReceivedMessages.ContainsKey(socketId))
            {
                ReceivedMessages.Add(socketId, new Dictionary<int, IProtocol>());
            }
            semaphore.WaitOne();
            ReceivedMessages[socketId].Add(receivedMessageposition[socketId]++, prot);
            semaphore.Release();

            if (transactions.Responses == null || transactions.Responses.Count <= 0)
            {
                return;
            }
            SentResponses(transactions, socketId, prot);
            if (!IsEmpty(message))
            {
                HandleActions(message, socketId);
            }
            return;
        }
        /// <summary>
        /// Returns a logging message. The logging will be adjust in a later issues
        /// </summary>
        /// <param name="responsemessage"></param>
        /// <param name="SocketID"></param>
        /// <returns></returns>
        private String GetLoggingMsg(InteractionMessage responsemessage, int SocketID)
        {
            String result = "";
            if (responsemessage.Protocol is ActiveMQProtocol)
            {
                result += "send response: " + ((ActiveMQProtocol)responsemessage.Protocol).Message.GetType().Name + " on socket: " + SocketID + "\n";
                if (((ActiveMQProtocol)responsemessage.Protocol).Message is Response)
                {
                    result += " with CorrelatedID: " + (((ActiveMQProtocol)responsemessage.Protocol).Message as Response).CorrelationId + "\n";
                }
            }
            return result;
        }
        /// <summary>
        /// Send over TCP a message
        /// </summary>
        /// <param name="trans">Information, which should be send</param>
        /// <param name="socketID">Socket number, on which should be send</param>
        private void SendToTCP(InteractionMessage trans, int socketID)
        {
            if (trans == null || trans.Protocol == null)
            {
                return;
            }
            Socket socket;
            int destinationSocketID = trans.Protocol.SocketNumber;
            if (destinationSocketID < 0)
            {
                destinationSocketID = socketID;
            }
            openClients.TryGetValue(socketID, out socket);
            //ToDo
            if (socket == null || !socket.Connected)//|| !socket.RemoteEndPoint.Equals(new IPEndPoint(trans.DestinationIPAddress, trans.DestinationPort.Value)))
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(trans.SourceIPAddress, trans.SourcePort.Value));
                socket.Connect(new IPEndPoint(trans.DestinationIPAddress, trans.DestinationPort.Value));
            }
            Byte[] result = trans.Protocol.GetMessage();
            socket.BeginSend(result, 0, result.Length, SocketFlags.None, new AsyncCallback(SendCallback), new StateObject(socket, destinationSocketID));
            return;
        }
        #endregion
        #region Public methods
        public void Start(IPEndPoint endpoint)
        {
            ListenToTCP(endpoint);
        }
        public void Start(IPAddress adresse, int port)
        {
            ListenToTCP(new IPEndPoint(adresse, port));
        }

        /// <summary>
        /// Sends a message to a client
        /// </summary>
        /// <param name="messagetoSend">Configuration</param>
        public void TriggerMessage(InteractionMessage messagetoSend)
        {
            byte[] input = messagetoSend.Protocol.GetMessage();
            if (IsEmpty(input))
            {
                return;
            }
            int socketId = messagetoSend.Protocol.SocketNumber;
            IProtocol prot = messagetoSend.Protocol;
            SentResponses(messagetoSend, socketId, prot);
        }
        /// <summary>
        /// Add an Interaction to the list
        /// </summary>
        /// <param name="interactionMessage">Interaction Message</param>
        public void AddInteraction(InteractionMessage interactionMessage)
        {
            interationConfiguration.Add(interactionMessage);
        }
        public void Dispose()
        {
            foreach (Socket socket in openClients.Values)
                socket.Close();
            serverSocket.Close();
        }
        #endregion
    }
}