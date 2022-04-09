using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidMove.Models {
    public class DeviceWatcherEventArgs:EventArgs {
        public DeviceWatcherEventArgs(AndroidDevice device) :base() {
            this.Device = device;
        }
        public AndroidDevice Device {
            get;
        }
    }
}
