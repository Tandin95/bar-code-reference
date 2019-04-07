using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Barcode
{
    /// <summary>
    /// CamDeviceOperationControl.xaml の相互作用ロジック
    /// </summary>
    public partial class CamDeviceOperationControl : UserControl
    {
        public event NewFrameEventHandler NewFrameGot = delegate { };

        private VideoCaptureDevice device;

        public CamDeviceOperationControl()
        {
            InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            device = new VideoCaptureDevice((string)deviceListCombo.SelectedValue);
            device.NewFrame += NewFrameGot;
            device.Start();
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            device.NewFrame -= NewFrameGot;
            device.SignalToStop();
            device = null;
        }
    }
}
