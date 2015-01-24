using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using WinRemoteMouse.Common;

namespace WinRemoteMouse
{
    public sealed partial class MainPage
    {
        private StreamSocket _socket;
        private DataWriter _chatWriter;
        private RfcommDeviceService _service;
        private DeviceInformationCollection _serviceInfoCollection;

        public MainPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!await StartMouse())
            {
                // TODO: Show error 
            }

            var moveObserv = Observable.FromEventPattern<PointerRoutedEventArgs>(MainGrid, "PointerMoved");
            moveObserv
            .Select(evnt => evnt.EventArgs.GetCurrentPoint(this).Position)
            .WithPrevious((previous, current) => new
            {
                CurrentPoint = current,
                PreviousPoint = previous,
            })
            .Subscribe(async obj =>
                             {
                                 var x = obj.CurrentPoint.X.CompareTo(obj.PreviousPoint.X);
                                 var y = obj.CurrentPoint.Y.CompareTo(obj.PreviousPoint.Y);
                                 var msg = string.Format("Type:{0};X:{1};Y:{2};", CommandType.Move, x, y);
                                 _chatWriter.WriteUInt32((uint)msg.Length);
                                 _chatWriter.WriteString(msg);
                                 await _chatWriter.StoreAsync();
                             });

            LeftButton.Click += LeftButtonOnClick;
            RightButton.Click += RightButtonOnClick;
        }

        private async void RightButtonOnClick(object sender, RoutedEventArgs e)
        {
            var msg = string.Format("Type:{0};", CommandType.RClick);
            _chatWriter.WriteUInt32((uint)msg.Length);
            _chatWriter.WriteString(msg);
            await _chatWriter.StoreAsync();
        }

        private async void LeftButtonOnClick(object sender, RoutedEventArgs e)
        {
            var msg = string.Format("Type:{0};", CommandType.Lclick);
            _chatWriter.WriteUInt32((uint)msg.Length);
            _chatWriter.WriteString(msg);
            await _chatWriter.StoreAsync();
        }

        private async Task<bool> StartMouse()
        {
            _serviceInfoCollection = await DeviceInformation.FindAllAsync(
                RfcommDeviceService.GetDeviceSelector(RfcommServiceId.FromUuid(Constants.RfcommChatServiceUuid)));

            if (_serviceInfoCollection.Count <= 0) return false;
            var chatServiceInfo = _serviceInfoCollection[0];
            _service = await RfcommDeviceService.FromIdAsync(chatServiceInfo.Id);

            if (_service == null)
            {
                return false;
            }
            lock (this)
            {
                _socket = new StreamSocket();
            }

            await _socket.ConnectAsync(_service.ConnectionHostName, _service.ConnectionServiceName);

            _chatWriter = new DataWriter(_socket.OutputStream);
            return true;
        }
    }
}
