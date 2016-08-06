using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Net;


public class DDNetOpt : MonoBehaviour {

    public static float updateTick = 0.066f;
    //static int net_char_delimiter = 0;
    static DDNetOpt instance = null;

    public Int32 port = 7755;
    public float tick = 66;

    bool connected = false;

    TcpListener server;
    TcpClient client;
    NetworkStream stream;
    Byte[] wbuffer = new Byte[128];
    Byte[] rbuffer = new Byte[128];

	void Awake ()
    {
        instance = this;
        Init();
	}

    void Init()
    {
        // configure singleton
        updateTick = tick / 1000;

        // find all devices in network
        string sHostName = Dns.GetHostName();
        IPAddress[] IpA = Dns.GetHostAddresses(sHostName);
        for (int i = 0; i < IpA.Length; i++)
        {
            // attempt to setup TCP-CLIENT
            try
            {
                client = new TcpClient(IpA[i].ToString(), port);
                stream = client.GetStream();
                connected = true;
                break;
            }
            catch (ArgumentNullException e) { Debug.Log(e); }
            catch (SocketException e) { Debug.Log(e.Message); }
        }

        // Start Server - if connecting failed
        if (!connected)
        {
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();
                
                // TODO : hold many clients
                /*client = server.AcceptTcpClient();
                stream = client.GetStream();

                Console.Write("Waiting for a connection... ");
                client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                stream = client.GetStream();*/

                /*int i;
                string data;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(rbuffer, 0, rbuffer.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(rbuffer, 0, i);

                    //byte[] msg = System.Text.Encoding.ASCII.GetBytes("yolo response");
                    //stream.Write(msg, 0, msg.Length);
                }*/

            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Debug.Log("shutting down server");
            if (client != null) client.Close();
            server.Stop();
        }
    }


    void OnApplicationQuit()
    {
        Shutdown();
    }


    void Update() 
    {
        if (!connected) return;
        ReadJson();
    }

    // TODO : remove json overhead send only bytes
    public static Byte[] GetSendBuffer()
    {
        if (instance == null) return null;
        return instance.wbuffer;
    }

    // TODO : remove json overhead send only bytes
    public static void SendBuffer()
    {
        if (instance == null || !instance.connected) return; // make sure network was initialized

        // TODO : handle connection closed exception
        instance.wbuffer[instance.wbuffer.Length - 1] = 99; // make sure last symbol is delimiter
        instance.stream.Write(instance.wbuffer, 0, instance.wbuffer.Length);
    }

    public static void SendJson(string json)
    {
        if (instance == null || !instance.connected) return; // make sure network was initialized

        instance.wbuffer = System.Text.Encoding.ASCII.GetBytes(json + "\n");
        instance.stream.Write(instance.wbuffer, 0, instance.wbuffer.Length);
    }

    void ReadJson()
    {
        if (!instance.stream.DataAvailable) return;

        Debug.Log("read json");
        rbuffer = new Byte[10];

        // String to store the response ASCII representation.
        String responseData = String.Empty;

        // Read the first batch of the TcpServer response bytes.
        //Int32 bytes = stream.Read(rbuffer, 0, rbuffer.Length);
        responseData = System.Text.Encoding.ASCII.GetString(rbuffer, 0, rbuffer.Length);
        Debug.Log("in: " + responseData);
    }

    // Close everything.
    static void Shutdown()
    {
        if (instance != null && instance.stream != null) instance.stream.Close();
        if (instance != null && instance.client != null) instance.client.Close();
    }


    // SERILIZATION HELPER
    public static void BufferWriteFloat(byte[] buffer, int pos, float val)
    {
        Buffer.BlockCopy(BitConverter.GetBytes(val), 0, buffer, pos, 4);
    }

    public static string ToJsonVal(string name, int val)
    {
        return "\"" + name + "\":" + val.ToString();
    }
    public static string ToJsonVal(string name, float val)
    {
        return "\"" + name + "\":" + val.ToString();
    }
    public static string ToJsonVal(string name, string val)
    {
        return "\"" + name + "\":\"" + val + "\"";
    }
    public static string ToJsonObj(params string[] vals)
    {
        var obj = "{";
        for (var i = 0; i < vals.Length; i++ )
        {
            obj += vals[i];
            if (i < vals.Length - 1) obj += ", ";
        }

        return obj + "}";
    }
}
