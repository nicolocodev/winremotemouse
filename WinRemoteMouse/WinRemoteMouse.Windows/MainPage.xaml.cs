using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace WinRemoteMouse
{
    internal static class MisExtensiones
    {
        public static IObservable<TResult> WithPrevious<TSource, TResult>(this IObservable<TSource> source, Func<TSource, TSource, TResult> projection)
        {
            return source.Scan(Tuple.Create(default(TSource), default(TSource)),
                               (previous, current) => Tuple.Create(previous.Item2, current))
                         .Select(t => projection(t.Item1, t.Item2));
        }
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        private static readonly Guid RfcommChatServiceUuid = Guid.Parse("87240836-54AF-41F8-A881-09F06364EEFC");


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
                .Select(x => x.EventArgs.GetCurrentPoint(this).Position)
                .Subscribe(async pos =>

                                 {
                                     var x = (int)pos.X;
                                     var y = (int)pos.Y;
                                     var msg = string.Format("Type:{0};X:{1};Y:{2};", "Move", x, y);
                                     _chatWriter.WriteUInt32((uint)msg.Length);
                                     _chatWriter.WriteString(msg);
                                     await _chatWriter.StoreAsync();
                                 });
            //.Select(evnt => evnt.EventArgs)
            //.WithPrevious((previous, current) =>
            //              {
            //                  var currentPoint = current == null
            //                      ? new Point(0, 0)
            //                      : current.GetCurrentPoint(MainGrid).Position;
            //                  var previousPoint = previous == null
            //                      ? new Point(0, 0)
            //                      : previous.GetCurrentPoint(MainGrid).Position;

            //                  Debug.WriteLine("pX{0}-cX{1} || pY{2}-cY{3}", previousPoint.X, currentPoint.X,
            //                      previousPoint.Y, currentPoint.Y);
            //                  return new
            //                         {
            //                             CurrentPoint = currentPoint,
            //                             PreviousPoint = previousPoint,
            //                         };
            //              })
            //.Subscribe(async obj =>

            //                 {
            //                     var x = obj.CurrentPoint.X.CompareTo(obj.PreviousPoint.X);
            //                     var y = obj.CurrentPoint.Y.CompareTo(obj.PreviousPoint.Y);
            //                     var msg = string.Format("Type:{0};X:{1};Y:{2};", "Move", x, y);
            //                     _chatWriter.WriteUInt32((uint) msg.Length);
            //                     _chatWriter.WriteString(msg);
            //                     await _chatWriter.StoreAsync();
            //                 });

            LeftButton.Click += LeftButtonOnClick;
            RightButton.Click += RightButtonOnClick;
        }

        private async void RightButtonOnClick(object sender, RoutedEventArgs e)
        {
            var msg = string.Format("Type:{0};", "RClick");
            _chatWriter.WriteUInt32((uint)msg.Length);
            _chatWriter.WriteString(msg);
            await _chatWriter.StoreAsync();
        }

        private async void LeftButtonOnClick(object sender, RoutedEventArgs e)
        {
            var msg = string.Format("Type:{0};", "LClick");
            _chatWriter.WriteUInt32((uint)msg.Length);
            _chatWriter.WriteString(msg);
            await _chatWriter.StoreAsync();
        }

        private async Task<bool> StartMouse()
        {
            _serviceInfoCollection = await DeviceInformation.FindAllAsync(
                RfcommDeviceService.GetDeviceSelector(RfcommServiceId.FromUuid(RfcommChatServiceUuid)));

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
