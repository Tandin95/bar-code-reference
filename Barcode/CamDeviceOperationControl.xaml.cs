using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Barcode
{
    /// <summary>
    /// CamDeviceOperationControl.xaml の相互作用ロジック
    /// </summary>
    public partial class CamDeviceOperationControl : UserControl,IDisposable
    {
        /// <summary>
        /// イベント
        /// </summary>
        public event NewFrameEventHandler NewFrameGot = delegate { };

        /// <summary>
        /// 選択デバイス
        /// </summary>
        private VideoCaptureDevice device;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CamDeviceOperationControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 接続ボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            // デバイス名取得
            string deviceName = deviceListCombo.SelectedValue as string;

            // デバイス名取得判定
            if (string.IsNullOrEmpty(deviceName))
            {
                MessageBox.Show("カメラデバイスを選択してから接続して下さい", "注意", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 選択したデバイス生成
            device = new VideoCaptureDevice(deviceName);
            device.NewFrame += NewFrameGot;
            device.Start();
        }

        /// <summary>
        /// 接続解除ボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {

            {
                if (device == null)
                {
                    MessageBox.Show("カメラデバイスとは接続されていません",
                        "注意", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                if (device != null)
                {
                    device.NewFrame -= NewFrameGot;
                    device.SignalToStop();
                    device = null;
                }
               
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        /// <summary>
        /// リソース破棄
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。
                if(device != null)
                {
                    device.NewFrame -= NewFrameGot;
                    device.SignalToStop();
                    device = null;
                }

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        /// <summary>
        /// デストラクタ
        /// </summary>
        ~CamDeviceOperationControl()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(false);
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        /// <summary>
        /// リソース破棄
        /// </summary>
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
