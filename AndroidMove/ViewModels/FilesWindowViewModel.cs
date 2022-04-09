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
    public class FilesWindowViewModel : ViewModel {
        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).

        // This method would be called from View, when ContentRendered event was raised.
        public async void Initialize() {
            var files = this.Device.GetAllFiles(this.Device.Config.TargetDirectory);
            await foreach (var file in files) {
                this.Files.Add(file);
            }
        }
        public FilesWindowViewModel(AndroidDevice device) : base() {
            this.Device = device;
            this.Files = new DispatcherCollection<AndroidFile>(DispatcherHelper.UIDispatcher);
            this.Files.CollectionChanged += (sender, e) => {
                if (e.OldItems != null) {
                    RaisePropertyChanged(nameof(Files));
                }
                if (e.NewItems != null) {
                    RaisePropertyChanged(nameof(Files));
                }
            };
        }    
        public AndroidDevice Device {
            get;
        }

        public DispatcherCollection<AndroidFile> Files {
            get;
        }

        private ListenerCommand<AndroidFile> _SendFileCommand;

        public ListenerCommand<AndroidFile> SendFileCommand {
            get {
                if (_SendFileCommand == null) {
                    _SendFileCommand = new ListenerCommand<AndroidFile>(SendFile);
                }
                return _SendFileCommand;
            }
        }

        public void SendFile(AndroidFile parameter) {
            this.Device.Pull(parameter,true);
        }

    }
}
