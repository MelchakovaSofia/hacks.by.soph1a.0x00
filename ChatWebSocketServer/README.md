# ChatWebSocketServer

## Overview
ChatWebSocketServer is a minimal ASP.NET Core WebSocket server designed for real-time chat applications. It allows multiple clients to connect and communicate with each other through WebSocket connections.

## Project Structure
```
ChatWebSocketServer
├── src
│   ├── Program.cs
│   └── Chat
│       └── ChatHandler.cs
├── ChatWebSocketServer.csproj
└── README.md
```

## Getting Started

### Prerequisites
- .NET SDK (version 6.0 or later)

### Setup
1. Clone the repository:
   ```
   git clone <repository-url>
   ```
2. Navigate to the project directory:
   ```
   cd ChatWebSocketServer
   ```
3. Restore the project dependencies:
   ```
   dotnet restore
   ```

### Running the Application
To run the application, use the following command:
```
dotnet run --project src/ChatWebSocketServer.csproj
```

### Usage
- Connect to the WebSocket server using a WebSocket client (e.g., a web browser or a dedicated WebSocket client).
- Send messages to the server, and the server will broadcast them to all connected clients.

## Contributing
Contributions are welcome! Please feel free to submit a pull request or open an issue for any enhancements or bug fixes.

## License
This project is licensed under the MIT License. See the LICENSE file for more details.