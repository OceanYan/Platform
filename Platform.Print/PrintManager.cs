using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platform.Print
{
    /// <summary>
    /// 打印管理提供
    /// </summary>
    public class PrintManager
    {
        #region Instance
        private PrintManager()
        {
        }

        private static PrintManager _instance;

        public static PrintManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PrintManager();
                return _instance;
            }
        }
        #endregion

        #region DoPrint
        public event EventHandler<DoPrintworkEventArgs> DoPrintwork;

        public class DoPrintworkEventArgs : EventArgs
        {
            public DoPrintworkEventArgs(PrintContainer[] pl = null)
            {
                PrintList = new List<PrintContainer>();
                if (pl != null)
                    PrintList.AddRange(pl);
            }
            public List<PrintContainer> PrintList { get; private set; }
        }
        public void DoPrint(PrintContainer[] pl)
        {
            DoPrintwork?.Invoke(this, new DoPrintworkEventArgs(pl));
        }
        #endregion
    }
}
