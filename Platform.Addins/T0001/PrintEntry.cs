using Platform.Print;
using Platform.Widget.Contract;
using Platform.Widget.Core;
using Platform.Widget.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platform.Addins.T0001
{
    public class PrintEntry : PrintContract
    {
        #region Ctor
        public PrintEntry(ContractPage page) : base(page)
        {
        }

        public new MainPage PageData
        {
            get { return base.PageData as MainPage; }
        }

        public DataInput DataInput
        {
            get { return PageData.Contract.DataInput; }
        }

        public DataOutput DataOutput
        {
            get { return PageData.Contract.DataOutput; }
        }

        public DataBus DataBus
        {
            get { return PageData.DataBus; }
        }
        #endregion

        protected void CreatePrintList(out List<PrintContainer> printList)
        {
            printList = new List<PrintContainer>();

            

        }
    }
}
