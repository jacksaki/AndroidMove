using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AndroidMove {
    public class AppConfig {

        private static AppConfig _singleton = null;
        private AppConfig() {

        }
        public static AppConfig GetInstance() {
            if (_singleton == null) {
                _singleton = new AppConfig();
                _singleton.Load();
            }
            return _singleton;
        }
        public string ConfigPath {
            get {
                return System.IO.Path.ChangeExtension(System.Reflection.Assembly.GetExecutingAssembly().Location, ".conf");
            }
        }
        public void SaveConfigurations() {
            var doc = new XDocument(
                new XElement("Configurations",
                    new XElement("Adb", new XAttribute("Path", this.AdbPath)),
                    new XElement("DeviceConfigurations",
                        this.DeviceConfigurations.Select(
                            x => new XElement("DeviceConfiguration",
                                    new XAttribute("Serial", x.Serial),
                                    new XAttribute("TargetDirectory", x.TargetDirectory),
                                    new XAttribute("DestDirectory", x.DestDirectory),
                                    new XAttribute("RenameRule", x.RenameRule)
                                )
                        )
                    )
                )
            );
            doc.Save(this.ConfigPath);
        }
        public void Load() {
            var doc = XDocument.Load(this.ConfigPath);
            var root = doc.Element("Configurations");
            this.AdbPath = root.Element("Adb")?.Attribute("Path")?.Value ?? "";
            this.DeviceConfigurations = new DeviceConfigCollection(root.Element("DeviceConfigurations"));
        }

        public DeviceConfigCollection DeviceConfigurations {
            get;
            private set;
        }
        public string AdbPath {
            get;
            private set;
        }
    }
}
