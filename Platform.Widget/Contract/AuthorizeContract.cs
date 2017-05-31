using Platform.Widget.Core;
using System.Collections.Generic;
using System;

namespace Platform.Widget.Contract
{
    public class AuthorizeContract : ContractBase
    {
        public AuthorizeContract(ContractPage page) : base(page)
        {
        }

        /// <summary>
        /// 触发授权流程
        /// </summary>
        public override bool Raise()
        {
            throw new NotImplementedException();
        }
    }
}
