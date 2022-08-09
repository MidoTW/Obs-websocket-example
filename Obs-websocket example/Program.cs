using System.Net.WebSockets;
using System.Text;

//**** obs-websocket 5.0.1 Protocol Documentation 
//***https://github.com/obsproject/obs-websocket/blob/master/docs/generated/protocol.md#connecting-to-obs-websocket


//*** Init
ClientWebSocket ws = new ClientWebSocket();

string ip = "127.0.0.1";
string port = "4455";
string password = "";

string query = string.Empty;

//*** Connect to the websocket
await ws.ConnectAsync(new Uri($"ws://{ip}:{port}"), CancellationToken.None);
await GetResponse();

//*** Authenfication
//**without Server Password
query = "{ \"op\": 1, \"d\": {  \"rpcVersion\": 1,\"eventSubscriptions\": 33}}";
await SendJSON(ws, query, CancellationToken.None);
await GetResponse();

//*** Send SetStudioModeEnabled == true request to the server  
query = "{ \"op\": 6, \"d\": { \"requestType\": \"SetStudioModeEnabled\", \"requestId\": \"f819dcf0 -89cc-11eb-8f0e-382c4ac93b9c\", \"requestData\": { \"studioModeEnabled\": true } } }";
await SendJSON(ws, query, CancellationToken.None);
await GetResponse();

//*** Close the WebSocketClient
await ws.CloseAsync(WebSocketCloseStatus.NormalClosure,"Goodbye my friend",CancellationToken.None);


//*** Function to send JSON Message to the server
Task SendJSON(ClientWebSocket ws, String data, CancellationToken cancellation)
{
    var encoded = Encoding.UTF8.GetBytes(data);
    var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
    return ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancellation);
}

//*** Function to get JSON response from the server
async Task GetResponse()
{
    byte[] buffer = new byte[256];
    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    if (result.MessageType == WebSocketMessageType.Close)
    {
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }
    else
    {
        Console.WriteLine(Encoding.Default.GetString(buffer));
    }
}
