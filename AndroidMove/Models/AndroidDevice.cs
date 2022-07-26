using Cysharp.Diagnostics;
using Livet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace AndroidMove.Models {
    public class AndroidDevice : NotificationObject {
        private AndroidDevice() : base() {
        }
        public DeviceConfig Config {
            get {
                return AppConfig.GetInstance().DeviceConfigurations[this];
            }
        }

        public string Name {
            get;
            private set;
        }
        public string Serial {
            get;
            private set;
        }
        private static string sequenceText = "?SEQUENCE?";
        public string GetRenameFileName(string fileName) {
            var extension = System.IO.Path.GetExtension(fileName);
            var localFiles = System.IO.Directory.GetFiles(this.Config.DestDirectory)
                .Where(x => x.EndsWith(System.IO.Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase))
                .Select(x => System.IO.Path.GetFileNameWithoutExtension(x));
            var max = localFiles.Select(x => Regex.Match(x, $"{this.Config.RenameRule.Replace(sequenceText, "([0-9]+)")}$"))
                    .Where(x => x.Success)
                    .Select(x => x.Groups[1].Value.ToInt32(0)).DefaultIfEmpty(0)
                    .Max();
            return this.Config.RenameRule.Replace(sequenceText, (max + 1).ToString()) + extension;
        }
        public void Delete(AndroidFile file) {
            Logger.Info($"Delete file {file.Path}");
            var p = new Process() { StartInfo = CreateProcessStartInfo() };
            try {
                p.StartInfo.Arguments = $"-s {this.Serial} shell rm -f {file.Path}";
                p.Start();
                p.WaitForExit();
            } catch (Exception ex) {
                Logger.Error(ex.Message);
                throw;
            }
        }
        public void Pull(PullParameter parameter) {
            Logger.Info($"{parameter.File.Path} -> {this.Config.DestDirectory}");
            var destPath = System.IO.Path.Combine(this.Config.DestDirectory, GetRenameFileName(parameter.File.FileName));

            var p = new Process() { StartInfo = CreateProcessStartInfo() };
            try {
                p.StartInfo.Arguments = $"-s {this.Serial} pull \"{parameter.File.Path}\" \"{destPath}\"";
                p.Start();
                p.WaitForExit();
            } catch (Exception ex) {
                Logger.Error(ex.Message);
                throw;
            }
            if (parameter.DeleteFile) {
                this.Delete(parameter.File);
            }
            if (parameter.CopyToClipboard) {
                ClipboardUtil.CopyImage(destPath,parameter.ScalePercent);
            }
        }
        public ObservableCollection<AndroidFile> Files {
            get;
        }
        public async void RefreshFiles() {
            this.Files.Clear();
            await foreach(var file in GetAllFiles(this.Config.TargetDirectory)) {
                this.Files.Add(file);
            }
        }

        internal AndroidFile Capture() {
            using (var p = new Process { StartInfo = CreateProcessStartInfo() }) {
                p.StartInfo.Arguments = $"-s {this.Serial} shell screencap -p {this.Config.TargetDirectory}/screenshot.png";
                p.Start();
                p.WaitForExit();
                return AndroidFile.Create(this.Config.TargetDirectory,"screenshot.png");
            }
        }

        private static ProcessStartInfo CreateProcessStartInfo() {
            var ps = new ProcessStartInfo();
            ps.FileName = AppConfig.GetInstance().AdbPath;
            ps.UseShellExecute = false;
            ps.CreateNoWindow = true;
            ps.RedirectStandardError = true;
            ps.RedirectStandardOutput = true;
            ps.StandardErrorEncoding = System.Text.Encoding.UTF8;
            ps.StandardOutputEncoding = System.Text.Encoding.UTF8;
            return ps;
        }
        public async IAsyncEnumerable<AndroidFile> GetAllFiles(string dir) {
            var ps = CreateProcessStartInfo();
            ps.Arguments = $"-s {this.Serial} shell ls -la {dir}";
            await foreach (string line in ProcessX.StartAsync(ps)) {
                var f = AndroidFile.CreateFromLine(dir, line);
                if (f != null) {
                    yield return f;
                }
            }
        }
        public static IEnumerable<AndroidDevice> GetAll() {
            using (var p = new Process { StartInfo = CreateProcessStartInfo() }) {
                p.StartInfo.Arguments = "devices -l";
                p.Start();
                p.WaitForExit();
                var lines = p.StandardOutput.ReadToEnd().Trim().Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.None).Skip(1);
                foreach (var line in lines) {
                    var d = AndroidDevice.CreateFromLine(line);
                    if (d != null) {
                        yield return d;
                    }
                }
            }
        }

        internal static AndroidDevice CreateFromLine(string line) {
            if(line.IndexOf("device product:") < 0) {
                return null;
            }
            var device = new AndroidDevice();
            device.Serial = line.Substring(0,line.IndexOf("device product:")).Trim();
            device.Name = line.Substring(line.IndexOf(" model:") + " model:".Length).Split(' ')[0].Trim();
            return device;
        }
    }
}
