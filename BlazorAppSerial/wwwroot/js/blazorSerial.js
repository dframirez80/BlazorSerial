var blazorSerialPort;
var blazorSerialTextEncoder = new TextEncoder();
var dataReceived = '';
var isDataReceived = false;

function blazorSerialIsSupported() {
    return navigator.serial ? true : false;
}

async function blazorSerialGetPort() {
    try {
        blazorSerialPort = await navigator.serial.requestPort();
        return "Ok";
    }
    catch (ex) {
        if (ex instanceof SecurityError) {
            return "SecurityError";
        }
        else if (ex instanceof AbortError) {
            return "AbortError";
        }
        else {
            return "Unknown";
        }
    }
}

async function blazorSerialOpen(baudRate) {
    try {
        await blazorSerialPort.open({ baudRate: baudRate });

        const appendStream = new WritableStream({
            write(chunk) {
                dataReceived += chunk;
                isDataReceived = true;
            }
        });
        blazorSerialPort.readable
            .pipeThrough(new TextDecoderStream())
            .pipeTo(appendStream);
        return "Ok";
    }
    catch (ex) {
        if (ex instanceof InvalidStateError) {
            return "InvalidStateError";
        }
        else if (ex instanceof NetworkError) {
            return "NetworkError";
        }
        else {
            return "Unknown";
        }
    }
}

function blazorSerialWriteText(text) {
    let writer = blazorSerialPort.writable.getWriter();
    writer.write(blazorSerialTextEncoder.encode(text));
    writer.releaseLock();
}

function blazorSerialClose() {
    blazorSerialPort.close();
}

function blazorSerialReadText() {
    let dataRx = dataReceived;
    isDataReceived = false;
    dataReceived = '';
    return dataRx;
}

function blazorSerialIsDataReceived() {
    return isDataReceived;
}

function blazorSerialClearReceived() {
    
}