const http = require('http')
const WebSocket = require('ws')

// Configura el servidor HTTP
const server = http.createServer((req, res) => {
    res.writeHead(200, {'Content-Type':'text/plain' })
    res.end('WebSocket Server is running\n')
})

// Configura el servidor WebSocket sobre el servidor HTTP
const wss = new WebSocket.Server({ server })

// Almacena los clientes conectados con sus IDs y nombres de usuario
let clients = []

// Evento cuando el cliente se conecta
wss.on('connection', (ws) => { 
    console.log('New Connection')
    // TODO añadir el WebScoket a un array de clients

    ws.send("Hello from server")

    // Evento cuando un cliente envía un mensaje
    ws.on('message', (message)=>{
        console.log(`Mensaje recibido: ${message}`)
    })
})

// Inicia el servidor en el puerto 3000
const PORT = 3000
server.listen(PORT, () => {
    console.log(`WebSocket server is listening on port ${PORT}`)
})

/*server.on('connection', (socket) => {
    socket.on('close', () => {
        console.log('Client disconnected')
    })
    socket.on('message', (message) => {
        console.log(`Received message from socket${message}`)
    })
})*/
