using BlazorAppSerial.Serial.Enums;

using BlazorAppSerial.Serial.Exceptions;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorAppSerial.Serial
{
    public class SerialPort : ISerialPort
    {
        public bool IsConnected { get; private set; }
        public bool IsPortChosen { get; private set; }

        private readonly IJSRuntime _jsRuntime;

        public SerialPort(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> IsSupported() => await _jsRuntime.InvokeAsync<bool>("blazorSerialIsSupported");

        public async Task<RequestPortResponseEnum> RequestPort()
        {
            var result = Enum.Parse<RequestPortResponseEnum>(await _jsRuntime.InvokeAsync<string>("blazorSerialGetPort"));

            if (result == RequestPortResponseEnum.Ok)
            {
                IsPortChosen = true;
            }

            return result;
        }

        public async Task<ConnectResponseEnum> Open(int baudRate)
        {
            if (!IsPortChosen)
            {
                throw new PortNotChoosenException();
            }

            if (IsConnected)
            {
                throw new AlreadyConnectedException();
            }
            try
            {
                var resp = await _jsRuntime.InvokeAsync<string>("blazorSerialOpen", baudRate);
                var connectionResult = Enum.Parse<ConnectResponseEnum>(resp);

                if (connectionResult == ConnectResponseEnum.Ok)
                {
                    IsConnected = true;
                }

                return connectionResult;
            }
            catch (Exception ex)
            {
                return ConnectResponseEnum.Unknown;
            }
        }

        public async Task Close()
        {
            if (IsConnected)
            {
                await _jsRuntime.InvokeVoidAsync("blazorSerialClose");
                IsConnected = false;
            }
        }

        public async Task Write(string text) => await _jsRuntime.InvokeAsync<string>("blazorSerialWriteText", text);

        public async Task<bool> IsDataForRead() => await _jsRuntime.InvokeAsync<bool>("blazorSerialIsDataReceived");
        public async Task<string> Read() => await _jsRuntime.InvokeAsync<string>("blazorSerialReadText");

    }
}