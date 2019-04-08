using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Threading;

namespace Barcode
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        private delegate void ConverBmp(Bitmap bitmap);
        private ConverBmp Conver;

        public MainWindow()
        {
            InitializeComponent();
            Conver += ConverBitmap;
            camDeviceCtrl.NewFrameGot += CamDeviceCtrlNewFrameGot;
            ClearResult();
        }

        private void CamDeviceCtrlNewFrameGot(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            picture.Dispatcher.Invoke(Conver, eventArgs.Frame);
        }

        private void ConverBitmap(Bitmap bitmap)
        {
            picture.Source = BitmapToBitmapFrame.Convert(bitmap);
        }

        private async void BarcodRead_Click(object sender, RoutedEventArgs e)
        {
            DoEvent();
            ClearResult();

            var image = picture.Source as BitmapFrame;

            if(image == null)
            {
                return;
            }

            Bitmap bitmap = image.ToBitmap(System.Drawing.Imaging.PixelFormat.Format64bppArgb);

            // バーコード読み取り
            // WPFではZXing.Presentation名前空間のBarcodeReaderを使う
            ZXing.Presentation.BarcodeReader reader = new ZXing.Presentation.BarcodeReader()
            {
                AutoRotate = true,
                Options = { TryHarder = true }
            };

            ZXing.Result result = await Task.Run(() => reader.Decode(image));
            if (result != null)
            {
                ShowResult(result);
            }
        }

        private void ClearResult()
        {
            //picture.Source = null;
            BarcodeFormatText.Text = "(N/A)";
            TextText.Text = "(N/A)";
            OverlayCanvas.Visibility = Visibility.Collapsed;
        }

        private void ShowResult(ZXing.Result result)
        {
            // テキスト表示
            BarcodeFormatText.Text = result.BarcodeFormat.ToString();
            TextText.Text = result.Text;

            // 認識エリア表示
            ZXing.ResultPoint[] points = result.ResultPoints;
            PolygonMark.BeginInit();
            {
                PolygonMark.Points.Clear();
                foreach(var p in points)
                {
                    PolygonMark.Points.Add(new System.Windows.Point(p.X, p.Y));
                }
            }
            PolygonMark.EndInit();

            // 回転
            int orientaion = (int)result.ResultMetadata[ZXing.ResultMetadataType.ORIENTATION];
            switch (orientaion)
            {
                case 180:
                    orientaion = 0;
                    break;
                case 270:
                    orientaion = 90;
                    break;
            }

            BitmapSource bitmapSource = picture.Source as BitmapSource;
            PolygonMark.RenderTransform = new RotateTransform(orientaion, bitmapSource.PixelWidth / 2.0, bitmapSource.PixelHeight / 2.0);

            OverlayCanvas.Visibility = Visibility.Visible;
            AdjustOverlay();
        }

        private void AdjustOverlay()
        {
            if (!OverlayCanvas.IsVisible)
            {
                return;
            }

            if(picture.Source is BitmapSource bs)
            {
                System.Windows.Point imagePoint = picture.TransformToAncestor(ImageGrid).Transform(new System.Windows.Point(0, 0));

                double scale = picture.RenderSize.Width / bs.PixelWidth;
                OverlayCanvas.RenderTransform = new ScaleTransform(scale, scale);
            }
        }

        private void DoEvent()
        {
            DispatcherFrame dispatcherFrame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(obj =>
           {
               ((DispatcherFrame)obj).Continue = false;
               return null;
           });

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, dispatcherFrame);
            Dispatcher.PushFrame(dispatcherFrame);
        }
    }
}
