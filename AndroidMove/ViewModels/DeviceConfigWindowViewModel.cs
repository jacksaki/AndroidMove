using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AndroidMove.Models;
namespace AndroidMove.ViewModels {
    public class DeviceConfigWindowViewModel : ViewModel {
        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).

        // This method would be called from View, when ContentRendered event was raised.
        public void Initialize() {
        }
        public DeviceConfigWindowViewModel(AndroidDevice device) : base() {
            this.Device = device;
            this.TargetDirectory = this.Device.Config.TargetDirectory;
            this.DestDirectory = this.Device.Config.DestDirectory;
            this.RenameRule = this.Device.Config.RenameRule;
        }
        public AndroidDevice Device {
            get;
        }

        private string _RenameRule;

        public string RenameRule {
            get {
                return _RenameRule;
            }
            set { 
                if (_RenameRule == value) {
                    return;
                }
                _RenameRule = value;
                RaisePropertyChanged();
            }
        }


        private string _DestDirectory;

        public string DestDirectory {
            get {
                return _DestDirectory;
            }
            set { 
                if (_DestDirectory == value) {
                    return;
                }
                _DestDirectory = value;
                OKCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged();
            }
        }

        private string _TargetDirectory;

        public string TargetDirectory {
            get {
                return _TargetDirectory;
            }
            set { 
                if (_TargetDirectory == value) {
                    return;
                }
                _TargetDirectory = value;
                RaisePropertyChanged();
            }
        }

        private ViewModelCommand _OKCommand;

        public ViewModelCommand OKCommand {
            get {
                if (_OKCommand == null) {
                    _OKCommand = new ViewModelCommand(OK, CanOK);
                }
                return _OKCommand;
            }
        }

        public bool CanOK() {
            return System.IO.Directory.Exists(this.DestDirectory);
        }

        public void OK() {
            AppConfig.GetInstance().DeviceConfigurations[this.Device].DestDirectory = this.DestDirectory;
            AppConfig.GetInstance().DeviceConfigurations[this.Device].TargetDirectory = this.TargetDirectory;
            AppConfig.GetInstance().DeviceConfigurations[this.Device].RenameRule = this.RenameRule;
            AppConfig.GetInstance().SaveConfigurations();
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }

        private ViewModelCommand _CancelCommand;

        public ViewModelCommand CancelCommand {
            get {
                if (_CancelCommand == null) {
                    _CancelCommand = new ViewModelCommand(Cancel);
                }
                return _CancelCommand;
            }
        }

        public void Cancel() {
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }

    }
}
