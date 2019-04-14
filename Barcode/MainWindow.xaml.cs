using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Threading;
using System;

namespace Barcode
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Bitmap変換デリゲート
        /// </summary>
        /// <param name="bitmap">対象Bitmap</param>
        private delegate void ConverBmp(Bitmap bitmap);

        /// <summary>
        /// Bitmap変換
        /// </summary>
        private ConverBmp Conver;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Conver += ConverBitmap;
            camDeviceCtrl.NewFrameGot += CamDeviceCtrlNewFrameGot;
            ClearResult();
        }

        /// <summary>
        /// Converデリゲートを実行する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void CamDeviceCtrlNewFrameGot(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            picture.Dispatcher.Invoke(Conver, eventArgs.Frame);
        }

        /// <summary>
        /// BitmapをBitmapFrameに変換して描画イメージのソースに設定する
        /// </summary>
        /// <param name="bitmap"></param>
        private void ConverBitmap(Bitmap bitmap)
        {
            picture.Source = BitmapToBitmapFrame.Convert(bitmap);
        }

        /// <summary>
        /// バーコード読み取り処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BarcodRead_Click(object sender, RoutedEventArgs e)
        {
            var image = picture.Source as BitmapFrame;

            if(image == null)
            {
                return;
            }

            // Bitmap変換
            Bitmap bitmap = image.ToBitmap(System.Drawing.Imaging.PixelFormat.Format64bppArgb);

            // コントロール更新
            DoEvent();

            // 初期化
            ClearResult();

            // バーコード読み取り
            // WPFではZXing.Presentation名前空間のBarcodeReaderを使う
            ZXing.Presentation.BarcodeReader reader = new ZXing.Presentation.BarcodeReader()
            {
                AutoRotate = true,
                Options = { TryHarder = true }
            };

            // 非同期処理
            ZXing.Result result = await Task.Run(() => reader.Decode(image));
            if (result != null)
            {
                ShowResult(result);
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void ClearResult()
        {
            BarcodeFormatText.Text = "(N/A)";
            TextText.Text = "(N/A)";
            OverlayCanvas.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// バーコード読み取り結果表示
        /// </summary>
        /// <param name="result"></param>
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

            // マーク表示
            OverlayCanvas.Visibility = Visibility.Visible;
            AdjustOverlay();
        }

        /// <summary>
        /// マーク表示の調整
        /// </summary>
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

        /// <summary>
        /// コントロールを明示的に更新
        /// 参照：https://www.ipentec.com/document/csharp-wpf-implement-application-doevents
        /// </summary>
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

        /// <summary>
        /// クローズ処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 解放処理
            camDeviceCtrl.Dispose();
        }
    }
}
