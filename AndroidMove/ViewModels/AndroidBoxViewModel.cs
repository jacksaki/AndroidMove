﻿using Livet;
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
    public class AndroidBoxViewModel : ViewModel {
        // Some useful code snippets for ViewModel are defined as l*(llcom, llcomn, lvcomm, lsprop, etc...).

        // This method would be called from View, when ContentRendered event was raised.
        public AndroidBoxViewModel(AndroidDevice device) : base() {
            this.Device = device;
        }

        public AndroidDevice Device {
            get;
        }

        private ViewModelCommand _ShowFilesCommand;

        public ViewModelCommand ShowFilesCommand {
            get {
                if (_ShowFilesCommand == null) {
                    _ShowFilesCommand = new ViewModelCommand(ShowFiles, CanShowFiles);
                }
                return _ShowFilesCommand;
            }
        }


        private ViewModelCommand _MoveFileCommand;

        public ViewModelCommand MoveFileCommand {
            get {
                if (_MoveFileCommand == null) {
                    _MoveFileCommand = new ViewModelCommand(MoveFile, CanMoveFile);
                }
                return _MoveFileCommand;
            }
        }

        public bool CanMoveFile() {
            return true;
        }

        public void MoveFile() {
            var file = this.Device.Files.OrderByDescending(x => x.Created).FirstOrDefault();
            if (file != null) {
                this.Device.Pull(new PullParameter() {
                    File = file,
                    DeleteFile = true,
                    CopyToClipboard = this.Clipboard,
                    ScalePercent = this.ScalePercent
                });
            }
        }

        private bool _Clipboard = true;

        public bool Clipboard {
            get {
                return _Clipboard;
            }
            set { 
                if (_Clipboard == value) {
                    return;
                }
                _Clipboard = value;
                RaisePropertyChanged();
            }
        }

        private int _ScalePercent = 100;

        public int ScalePercent {
            get {
                return _ScalePercent;
            }
            set { 
                if (_ScalePercent == value) {
                    return;
                }
                _ScalePercent = value;
                RaisePropertyChanged();
            }
        }


        private ViewModelCommand _CaptureAndMoveCommand;

        public ViewModelCommand CaptureAndMoveCommand {
            get {
                if (_CaptureAndMoveCommand == null) {
                    _CaptureAndMoveCommand = new ViewModelCommand(CaptureAndMove);
                }
                return _CaptureAndMoveCommand;
            }
        }

        public void CaptureAndMove() {
            var file =  this.Device.Capture();
            if (file != null) {
                this.Device.Pull(new PullParameter() {
                    File = file,
                    DeleteFile = true,
                    CopyToClipboard = this.Clipboard,
                    ScalePercent = this.ScalePercent
                });
            }
        }
        private ViewModelCommand _ConfigCommand;

        public ViewModelCommand ConfigCommand {
            get {
                if (_ConfigCommand == null) {
                    _ConfigCommand = new ViewModelCommand(Config, CanConfig);
                }
                return _ConfigCommand;
            }
        }

        public bool CanConfig() {
            return true;
        }

        public void Config() {
            using (var vm = new DeviceConfigWindowViewModel(this.Device)) {
                Messenger.Raise(new TransitionMessage(vm, "ShowConfig"));
            }
        }

        public bool CanShowFiles() {
            return !string.IsNullOrEmpty(this.Device.Config.TargetDirectory);
        }

        public void ShowFiles() {
            using(var vm= new FilesWindowViewModel(this.Device)) {
                Messenger.Raise(new TransitionMessage(vm, "ShowFiles"));
            }
        }

    }
}
