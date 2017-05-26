# EllApp [![Build Status](https://travis-ci.org/LuigiElleBalotta/EllApp.svg?branch=master)](https://travis-ci.org/LuigiElleBalotta/EllApp) [![Build status](https://ci.appveyor.com/api/projects/status/6290gij98ukvt6ob/branch/master?svg=true)](https://ci.appveyor.com/project/LuigiElleBalotta/ellapp/branch/master)
Instant Message chat for iOS, Android and Windows Phone, written with Apache Cordova with Visual Studio 2015. (In Development)

Follow these steps in order.

## [Server Repo](https://github.com/LuigiElleBalotta/EllappServer)

## How to Use
### Server Side

#### Chat Server (C#)
1. Open the Solution
2. Compile in Release Mode.
3. If you have trouble compiling, add the DLLS into "references" folder. [Let me google it for you](http://lmgtfy.com/?q=How+to+add+reference+in+visual+studio)
4. Add your configuration inside EllApp_server.exe.config

### Client Side
1. Install the application on your phone (to generate the .apk or iOS app search on google, you also need a custom made certificate that you can build with java SDK)
2. Start the application on another phone or execute the project in visual studio or use an emulator.
3. Register an account.
4. Login
5. Start Chat.

#### Note
- You have to change the IP in the index.js to properly connect to YOUR chat server.
- Registration have to be made. For the moment the user can be recognized by his client ID.

## ScreenShots
# ![Chatrooms](http://i.imgur.com/FvKba5G.png)

# ![Chat](http://i.imgur.com/DlWUm2O.png)

# ![SearchUser](http://i.imgur.com/8ZQe24n.png)

What Works
------
- Registration
- Login
- Global Chat
