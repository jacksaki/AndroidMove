using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidMove.Models {
    public class PullParameter {
        public AndroidFile File {
            get;
            set;
        }
        public bool DeleteFile {
            get;
            set;
        }
        public bool CopyToClipboard {
            get;
            set;
        }
        public int ScalePercent {
            get;
            set;
        }
    }
}
