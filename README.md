# EllApp
Instant Message chat for iOS, Android and Windows Phone, written with Apache Cordova with Visual Studio 2015. (In Development)

Follow these steps in order.

## How to Use
### Server Side
You can choose (for the moment) if use PHP server or C# server
#### Chat Server (PHP)
(Add php command to the CMD, search on google a guide)

1. Open CMD
2. Navigate to the WebSocketChat/bin folder
3. Execute "php chat-server.php"
4. Don't close the Window

#### Chat Server (C#)
1. Open the Solution
2. Compile in Release Mode.
3. If you have trouble compiling, add the DLLS into "references" folder. [Let me google it for you](http://lmgtfy.com/?q=How+to+add+reference+in+visual+studio)

#### Web Server (PHP)
1. Put the content of WebServer into a WebHost root folder (you can register on altervista.org or hostinger.it to register a free domain)
2. Rename config.php.conf into config.php and configure it.

### Client Side
1. Install the application on your phone (to generate the .apk or iOS app search on google, you also need a custom made certificate that you can build with java SDK)
2. Start the application on another phone or execute the project in visual studio or use an emulator.
3. Register an account.
4. Login
5. Start Chat.

#### Note
- You have to change the IP in the index.js to properly connect to YOUR chat server.
- Registration have to be made. For the moment the user can be recognized by his client ID.

What Works
------
- Registration
- Login
- Global Chat
