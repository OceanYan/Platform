using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platform.Controls.UI.Composite
{
    public class RateInput : NumericInput
    {
        #region Ctor
        public RateInput()
        {
            Title = "利率域";
            Accuracy = 6;
            MinValue = 0;
            MaxValue = 999.9999999M;
            Suffix = "‰";
        }
        #endregion
    }
}
