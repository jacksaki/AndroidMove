using AndroidMove.Models;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AndroidMove.ViewModels {
    public class MainWindowViewModel : ViewModel {
        public MainWindowViewModel() : base() {
            this.DialogCoordinator = MahApps.Metro.Controls.Dialogs.DialogCoordinator.Instance;
            this.DeviceViewModels = new ObservableCollection<AndroidBoxViewModel>();
            BindingOperations.EnableCollectionSynchronization(this.DeviceViewModels, new object());
            this.Watcher = new DeviceWatcher();
            this.Watcher.Added += Watcher_Added;
            this.Watcher.Removed += Watcher_Removed;
            this.Watcher.Start();
        }
        public DeviceWatcher Watcher {
            get;
        }

        private ViewModelCommand _ShowCountCommand;

        public ViewModelCommand ShowCountCommand {
            get {
                if (_ShowCountCommand == null) {
                    _ShowCountCommand = new ViewModelCommand(ShowCount);
                }
                return _ShowCountCommand;
            }
        }

        public void ShowCount() {
            System.Diagnostics.Debug.WriteLine("a: " + this.DeviceViewModels.Count.ToString());
        }

        public ObservableCollection<AndroidBoxViewModel> DeviceViewModels {
            get;
        }


        public MahApps.Metro.Controls.Dialogs.IDialogCoordinator DialogCoordinator {
            get;
            set;
        }

        private void Watcher_Removed(object sender, DeviceWatcherEventArgs e) {
            var removeTargets = this.DeviceViewModels.Where(x => x.Device.Serial.Equals(e.Device.Serial)).ToList();
            foreach(var target in removeTargets) {
                this.DeviceViewModels.Remove(target);
                Logger.Debug($"removed: {target.Device.Name}");
            }
        }

        private void Watcher_Added(object sender, DeviceWatcherEventArgs e) {
            if (!this.DeviceViewModels.Where(x => x.Device.Serial.Equals(e.Device.Serial)).Any()) {
                this.DeviceViewModels.Add(new AndroidBoxViewModel(e.Device));
                Logger.Debug($"added: {e.Device.Name}");
            }
            Logger.Debug("DeviceCount: " + this.DeviceViewModels.Count.ToString());
        }

        private void Menu_ErrorOccurred(object sender, ErrorOccurredEventArgs e) {
            DialogCoordinator.ShowMessageAsync(this, "エラー", e.Message, MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative);
        }
    }
}