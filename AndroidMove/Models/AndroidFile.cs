using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace AndroidMove.Models {
    [Flags]
    public enum Permission {
        Read = 4,
        Write = 2,
        Execute = 1,
        None = 0,
    }
    public class AndroidFile {
        private AndroidFile() {
        
        }
        private AndroidFile(string d) :this(){
            this.Directory = d;
        }
        public string Directory {
            get;
        }
        private static Regex _lsReg = new Regex(@"(?<permission>(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x))\s+(?<owner>\S+)\s+(?<group>\S+)\s+(?<size>[0-9]+)\s+(?<date>[0-9\-]+)\s+(?<time>[0-9:]+)\s+(?<name>.+)");
        public static AndroidFile CreateFromLine(string directory, string line) {
            if (string.IsNullOrEmpty(line)) {
                return null;
            }
            if (line[0] == 'd') {
                return null;
            }
            var m = _lsReg.Match(line);
            if (!m.Success) {
                return null;
            }
            return new AndroidFile(directory) {
                PermissionText=m.Groups["permission"].Value,
                Owner = m.Groups["owner"].Value,
                Group = m.Groups["group"].Value,
                FileSizeByte = m.Groups["size"].Value.ToLong(0L),
                Created = (m.Groups["date"].Value + " " + m.Groups["time"].Value).ToDateTime("yyyy-MM-dd HH:mm"),
                FileName = m.Groups["name"].Value
            };
        }
        public static AndroidFile Create(string dir,string fileName) {
            return new AndroidFile(dir) {
                FileName = fileName
            };
        }
        public string PermissionText {
            get;
            private set;
        }
        public string Path {
            get {
                return this.Directory + (this.Directory.EndsWith("/") ? "" : "/") + this.FileName;
            }        
        }
        private static Permission GetPermission(string s) {
            var result = Permission.None;            
            if (s[0] == 'r') {
                result |= Permission.Read;
            }
            if (s[1] == 'w') {
                result |= Permission.Write;
            }
            if (s[2] == 'x') {
                result |= Permission.Execute;
            }
            return result;
        }
        public string Owner {
            get;
            private set;
        }
        public Permission OwnerPermission {
            get {
                return GetPermission(this.PermissionText.Substring(1, 3));
            }
        }
        public string Group {
            get;
            private set;
        }
        public Permission GroupPermission {
            get {
                return GetPermission(this.PermissionText.Substring(4, 3));
            }
        }
        public Permission OthersPermission {
            get {
                return GetPermission(this.PermissionText.Substring(7, 3));
            }
        }
        public string FileName {
            get;
            private set;
        }
        public long FileSizeByte {
            get;
            private set;
        }
        public DateTime? Created {
            get;
            private set;
        }
    }
}
