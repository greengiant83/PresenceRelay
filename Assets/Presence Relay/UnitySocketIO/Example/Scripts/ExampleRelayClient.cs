using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleRelayClient : MonoBehaviour
{
    public SocketIOController io;

    void Start()
    {
        //Server cost: https://glitch.com/edit/#!/relay-server
        //Web client: https://relay-server.glitch.me/
        //
        // Server Commands
        // -Client to Server
        // 💬join (roomKey)
        // 💬leave (roomKey)
        // 💬whoami ()
        // 💬whoslistening () 
        //
        // -Server to Client
        // 💬youare (socketKey)
        // 💬joined
        // 💬peerjoin (socketKey)
        // 💬peerdrop (socketKey)
        // 💬listeners ([{ room: "theRoomKey", sockets: ["123", ...] }, ...])


        //On Connected Event
        io.On("connect", e =>
        {
            Debug.Log("Socket connected");
            io.Emit("💬join", "\"exampleRoom\"");
        });

        //Event notification when this client joins a room on the relay
        io.On("💬joined", e =>
        {
            Debug.Log("Joined room: " + e.data);

            //Broadcast a message for relay
            io.Emit("appSpecificMessage", "\"Hello from Unity\""); //Note the enclosing double quotes. IO is actually expecting JSON values, so strings need to be quoted
        });

        //Event notification when we receive an app specific message
        io.On("appSpecificMessage", e =>
        {
            Debug.Log("Received appSpecificMessage: " + e.data);
        });

        //Actually initiate the connection to the server
        io.Connect();
    }
}
