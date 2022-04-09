using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AndroidMove.Models;
namespace AndroidMove {
    public class DeviceConfigCollection:IEnumerable<DeviceConfig> {
        private List<DeviceConfig> _list = new List<DeviceConfig>();
        public DeviceConfig this[AndroidDevice device] {
            get {
                if (_list.Where(x => x.Serial.Equals(device.Serial)).Any()) {
                    return _list.Where(x => x.Serial.Equals(device.Serial)).Single();
                } else {
                    var config = new DeviceConfig(device.Serial);
                    _list.Add(config);
                    return config;
                }
            }
        }

        public DeviceConfigCollection(XElement element) {
            _list.Clear();
            foreach(var e in element.Elements("DeviceConfiguration")) {
                _list.Add(new DeviceConfig(e));
            }
        }
        public IEnumerator<DeviceConfig> GetEnumerator() {
            return ((IEnumerable<DeviceConfig>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)_list).GetEnumerator();
        }
    }
}
