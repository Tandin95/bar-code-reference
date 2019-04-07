using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.DirectShow;

namespace Barcode
{
    /// <summary>
    /// 接続対象カメラデバイス
    /// </summary>
    class DeviceFilters
    {
        public string Name { get; set; }
        public string MonikerString { get; set; }

        /// <summary>
        /// 接続対象カメラデバイス取得
        /// </summary>
        /// <returns>対象カメラデバイス</returns>
        public IEnumerable<DeviceFilters> Get()
        {
            return from FilterInfo info in new FilterInfoCollection(FilterCategory.VideoInputDevice)
                   select new DeviceFilters { Name = info.Name, MonikerString = info.MonikerString };
        }

    }
}
