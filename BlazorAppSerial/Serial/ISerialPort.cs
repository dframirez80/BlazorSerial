using BlazorAppSerial.Serial.Enums;
using System.Threading.Tasks;

namespace BlazorAppSerial.Serial
{
    public interface ISerialPort
    {
        bool IsConnected { get; }
        bool IsPortChosen { get; }

        Task<ConnectResponseEnum> Open(int baudRate);
        Task<bool> IsSupported();
        Task<RequestPortResponseEnum> RequestPort();
        Task Write(string text);
        Task<string> Read();
        Task<bool> IsDataForRead();
        Task Close();
    }
}