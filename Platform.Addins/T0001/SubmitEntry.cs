using Platform.Widget.Contract;
using Platform.Widget.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;

namespace Platform.Addins.T0001
{
    public class SubmitEntry : SubmitContract<DataInput, DataOutput>
    {
        #region Ctor
        public SubmitEntry(ContractPage page) : base(page)
        {
            PrintContract = new PrintEntry(page);
            DataInput = new DataInput();
            DataOutput = new DataOutput();
        }

        public DataBus DataBus
        {
            get { return PageData.DataBus; }
        }

        public new MainPage PageData
        {
            get { return base.PageData as MainPage; }
        }
        #endregion

        protected override bool PreviewSubmitting()
        {
            DataBus.C = "";
            return base.PreviewSubmitting();
        }
    }
}
