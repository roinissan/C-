using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace ThreadSynchronization
{
    class Node 
    {
        private MailBox m_mbMailBox; //incoming mailbox of the node
        private Dictionary<int, MailBoxWriter> m_dNeighbors; //maps node ids to outgoing mailboxes (the routing table)
        private bool m_bDone; //notifies the thread to terminate
        private Dictionary<int, int> m_distances;
        private Dictionary<int, int> m_routers;
        private Dictionary<int, char[]> m_messages;
        private Mutex baton;
        public int ID { get; private set; } //the identifier of the node

        public Node( int iID )
        {
            ID = iID;
            m_mbMailBox = new SynchronizedMailBox();
            m_dNeighbors = new Dictionary<int, MailBoxWriter>();
            m_bDone = false;
            m_distances = new Dictionary<int, int>();
            m_routers = new Dictionary<int, int>();
            m_messages = new Dictionary<int, char[]>();
            baton = new Mutex();
            
        }

        //Returns access to the node's mailbox
        public MailBoxWriter GetMailBox()
        {
            return new MailBoxWriter(m_mbMailBox);
        }

        //sends routing messages to all the immediate neighbors
        private void SendRoutingMessages()
        {
            foreach (int n in m_routers.Keys)
            {
                RoutingMessage new_routing_message = new RoutingMessage(ID, n, m_distances);
                baton.WaitOne();
                m_dNeighbors[n].Send(new_routing_message);
                baton.ReleaseMutex();
            }
        }

        //handles an incoming routing neighbors according to the Bellman-Ford algorithm
        private void HandleRoutingMessage(RoutingMessage rmsg)
        {
            int iDistance;
            bool is_changed = false;
            if (!m_distances.ContainsKey(ID))
            {
                m_distances.Add(ID, 0);
                m_routers.Add(ID, ID);

            }
            foreach (int dist_id in m_dNeighbors.Keys)
            {
                if (!m_distances.ContainsKey(dist_id))
                {
                    m_distances.Add(dist_id, 1);
                    m_routers.Add(dist_id, dist_id);
                }
            }
            SendRoutingMessages();


            List<int> nodes = rmsg.GetAllNodes();

            foreach (int iNode in nodes)
            {
                iDistance = rmsg.GetDistance(iNode);
                if (!m_distances.ContainsKey(iNode) || m_distances[iNode] > iDistance + 1)
                {
                    m_distances[iNode] = iDistance + 1;
                    m_routers[iNode] = rmsg.Sender;
                    is_changed = true;
                }
            }

            if (is_changed)
            {
                baton.WaitOne();
                SendRoutingMessages();
                baton.ReleaseMutex();
            }
        }




        //handles an incoming packet message 
        //the message can be directed to the current node or to another node, in which case it should be forwarded
        private void HandlePacketMessage(PacketMessage pmsg)
        {
            
            if (pmsg.Target != ID)
            {
                m_dNeighbors[GetRouter(pmsg.Target)].Send(pmsg);
            }
            else
            {
                if (!m_messages.ContainsKey(pmsg.MessageID))
                {
                    m_messages.Add(pmsg.MessageID, new char[pmsg.Size]);
                    for (int i = 0; i < pmsg.Size; i++)
                        m_messages[pmsg.MessageID][i] = '\0';

                }
                bool is_full = true;
                string message = "";
                m_messages[pmsg.MessageID][pmsg.Location] = pmsg.Packet;
                foreach (char c in m_messages[pmsg.MessageID])
                    if (c.Equals('\0'))
                        is_full = false;
                if (is_full)
                {
                    foreach (char c in m_messages[pmsg.MessageID])
                        message = message + c;
                    Debug.WriteLine(message);
                }
            }
        }

        //returns the neighboring router for the target node
        private int GetRouter(int iTarget)
        {
            if(m_routers.ContainsKey(iTarget))
                return m_routers[iTarget];
            return -1;
        }

        //returns the distance of the routing node
        private int GetDistance(int iTarget)
        {
            if (m_routers.ContainsKey(iTarget))
                return m_distances[iTarget];
            return -1;
        }

        //returns the list of all reachable nodes (all the nodes that appear in the routing table)
        private List<int> ReachableNodes()
        {
            List<int> peers = new List<int>();
            foreach (int peer_id in m_routers.Keys)
                peers.Add(peer_id);

            return peers;
        }

        //returns the list of recieved messages
        //if a character in a message was not received (the message was not fully received), the array should contain
        //the sepcail character '\0'
        private List<char[]> ReceivedMessages()
        {
            List<char[]> all_messages = new List<char[]>();
            foreach (char[] c in m_messages.Values)
                all_messages.Add(c);
            return all_messages;
        }


        //Node (thread) main method - repeatedly checks for incoming mail and handles it.
        //when the thread is terminated using the KillMessage, outputs the routing table and the list of accepted messages
        public void Run()
        {
            SendRoutingMessages();
            while (!m_bDone)
            {
                Message msg = m_mbMailBox.Read();
                if (msg is RoutingMessage)
                {
                    HandleRoutingMessage((RoutingMessage)msg);                   
                }
                if (msg is PacketMessage)
                {
                    HandlePacketMessage((PacketMessage)msg);
                }
                if (msg is KillMessage)
                    m_bDone = true;
            }
            PrintRoutingTable();
            PrintAllMessages();
        }

        //Creates a thread that executes the Run method, starts it, and returns the created Thread object
        public Thread Start()
        {
            Thread new_thread = new Thread(Run);
            new_thread.Start();
            return new_thread;
        }

        //prints the routing table 
        public void PrintRoutingTable()
        {
            string s = "Routing table for " + ID + "\n";
            foreach (int iNode in ReachableNodes())
            {
                s += iNode + ", distance = " + GetDistance(iNode) + ", router = " + GetRouter(iNode) + "\n";
            }
            Debug.WriteLine(s);
        }


        //prints the list of accepted messages
        //if a char is missing, writes '?' instead
        public void PrintAllMessages()
        {
            Debug.WriteLine("Message list of " + ID);
            foreach (char[] aMessage in ReceivedMessages())
            {
                string s = "";
                for (int i = 0; i < aMessage.Length; i++)
                {
                    if (aMessage[i] == '\0')
                        s += "?";
                    else
                        s += aMessage[i];
                }
                Debug.WriteLine(s);
            }
        }


        //Sets a link (immediate access) between two nodes
        public static void SetLink(Node n1, Node n2)
        {
            n1.m_dNeighbors[n2.ID] = n2.GetMailBox();
            n2.m_dNeighbors[n1.ID] = n1.GetMailBox();
        }


        //Allows the administrator to send a string message from one machine to another
        //the message must be broken into packets
        //if the node does not recognize the target (the target is not in the routing table)
        //the method returns false
        public bool SendMessage(string sMessage, int iMessageID, int iTarget)
        {

            if (!m_routers.ContainsKey(iTarget))
                return false;

            MailBoxWriter target_router = m_dNeighbors[m_routers[iTarget]];
            for (int i = 0; i < sMessage.Length; i++)
            {
                PacketMessage new_packet = new PacketMessage(ID, iTarget, iMessageID, sMessage[i], i, sMessage.Length);
                target_router.Send(new_packet);
            }

            return true;
        }
    }
}
