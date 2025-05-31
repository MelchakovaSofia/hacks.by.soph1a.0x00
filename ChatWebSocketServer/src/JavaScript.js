const ws = new WebSocket('ws://192.168.181.148:5000/ws');

ws.onopen = () => {
    addMessage('Verbindung hergestellt.');
};

ws.onmessage = event => {
    addMessage(event.data);
};

ws.onclose = () => {
    addMessage('Verbindung getrennt.');
};

function addMessage(msg) {
    const chat = document.getElementById('chat');
    chat.innerHTML += `<div>${msg}</div>`;
    chat.scrollTop = chat.scrollHeight;
}

function sendMessage() {
    const input = document.getElementById('message');
    if (input.value.trim() !== '') {
        ws.send(input.value);
        input.value = '';
    }
}