using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace AndroidMove {
    public class DeviceConfig {
        public DeviceConfig(XElement element) {
            this.Serial = element.Attribute("Serial").Value;
            this.TargetDirectory = element.Attribute("TargetDirectory")?.Value ?? "/sdcard/";
            this.DestDirectory = element.Attribute("DestDirectory")?.Value ?? System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            this.RenameRule = element.Attribute("RenameRule")?.Value ?? "";
        }
        public DeviceConfig(string serial) {
            this.Serial = serial;
            this.TargetDirectory = "/sdcard/";
            this.DestDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            this.RenameRule = "";
        }
        public string Serial {
            get;
        }
        public string TargetDirectory {
            get;
            set;
        }
        public string DestDirectory {
            get;
            set;
        }

        public string RenameRule {
            get;
            set;
        }
    }
}
