using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;

public class DDNet : MonoBehaviour {

    public static float updateTick = 0.066f;
    static int net_char_delimiter = 0;
    static DDNet instance = null;

    public string ip = "";
    public Int32 port = 7755;
    public float tick = 66;

    bool connected = false;

    TcpClient client;
    NetworkStream stream;
    Byte[] wbuffer = new Byte[128];
    Byte[] rbuffer = new Byte[128];

	void Start ()
    {
        Init();
        instance = this;
	}

    void Init()
    {
        // configure singleton
        updateTick = tick / 1000;

        // setup tcp-client / stream / socket
        try {
            client = new TcpClient(ip, port);
            stream = client.GetStream();
            connected = true;
        }
        catch (ArgumentNullException e) { }
        catch (SocketException e)
        {
            Debug.Log(e.Message); // connection refused exception
        }
    }

    void OnApplicationQuit()
    {
        Shutdown();
    }



    void Update()
    {
        if (!connected) Init();
    }

    public static Byte[] GetSendBuffer()
    {
        if (instance == null) return null;
        return instance.wbuffer;
    }

    public static void BufferWriteFloat(byte[] buffer, int pos, float val)
    {
        Buffer.BlockCopy(BitConverter.GetBytes(val), 0, buffer, pos, 4);
    }

    public static void SendBuffer()
    {
        if (instance == null || !instance.connected) return; // make sure network was initializedDebug.Log("net-send:");

        // TODO : handle connection closed exception
        instance.wbuffer[instance.wbuffer.Length - 1] = 99; // make sure last symbol is delimiter
        instance.stream.Write(instance.wbuffer, 0, instance.wbuffer.Length);
    }

    void ReadPacket()
    {
        // weirdly this crashes everything
        //rbuffer = new Byte[256];

        // String to store the response ASCII representation.
        //String responseData = String.Empty;

        // Read the first batch of the TcpServer response bytes.
        //Int32 bytes = stream.Read(rbuffer, 0, 1);
        //responseData = System.Text.Encoding.ASCII.GetString(rbuffer, 0, bytes);
    }

    // Close everything.
    static void Shutdown()
    {
        if (instance.stream != null) instance.stream.Close();
        if (instance.client != null) instance.client.Close();
    }
}
