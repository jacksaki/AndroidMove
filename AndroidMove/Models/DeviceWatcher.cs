using Livet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace AndroidMove.Models {
    public class DeviceWatcher : IDisposable {
        public delegate void DeviceWatcherEventHandler(object sender,DeviceWatcherEventArgs e);
        public event DeviceWatcherEventHandler Added = delegate { };
        public event DeviceWatcherEventHandler Removed = delegate { };
        private Task _task;

        protected void OnAdded(DeviceWatcherEventArgs e) {
            Logger.Debug($"OnAdded: {e.Device.Name}({e.Device.Serial})");
            this.Added(this, e);
        }
        protected void OnRemoved(DeviceWatcherEventArgs e) {
            Logger.Debug($"OnRemoved: {e.Device.Name}({e.Device.Serial})");
            this.Removed(this, e);
        }
        public DeviceWatcher() {
            _devices = new ObservableSynchronizedCollection<AndroidDevice>();
            _devices.CollectionChanged += (sender, e) => {
                if (e.OldItems != null) {
                    foreach (AndroidDevice d in e.OldItems) {
                        Logger.Debug($"OldItem: {d.Name}({d.Serial})");
//                        OnRemoved(new DeviceWatcherEventArgs(d));
                    }
                }
                if (e.NewItems != null) {
                    foreach (AndroidDevice d in e.NewItems) {
                        Logger.Debug($"NewItem: {d.Name}({d.Serial})");
                        //                        OnAdded(new DeviceWatcherEventArgs(d));
                    }
                }
            };
        }
        private CancellationTokenSource _cancellation;
        private ObservableSynchronizedCollection<AndroidDevice> _devices;
        private void CheckDevice() {
            foreach(var d in _devices) {
                Logger.Debug($"Current: {d.Name}({d.Serial})");
            }
            var devices = AndroidDevice.GetAll();
            foreach (var d in devices) {
                if (!_devices.Any(x => x.Serial.Equals(d.Serial))) {
                    Logger.Debug($"add: {d.Name}({d.Serial})");
                    _devices.Add(d);
                    OnAdded(new DeviceWatcherEventArgs(d));
                }
            }

            var removedDevices = _devices.Where(x => !devices.Where(y => y.Serial.Equals(x.Serial)).Any()).ToList();
            foreach (var d in removedDevices) {
                Logger.Debug($"remove: {d.Name}({d.Serial})");
                _devices.Remove(d);
                OnRemoved(new DeviceWatcherEventArgs(d));
            }
        }
        public void Start() {
            Logger.Info($"Daemon Start.");
            _cancellation = new CancellationTokenSource();
            _task = Task.Run(async() => {
                while (true) {
                    CheckDevice();
                    await Task.Delay(3000);
                }
            },_cancellation.Token);
        }
        public void Stop() {
            Logger.Info($"Daemon Stop.");
            _cancellation?.Cancel();
        }
        private bool disposedValue;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // TODO: 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~DeviceWatcher()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
