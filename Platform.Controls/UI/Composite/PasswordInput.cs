using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Platform.Controls.UI.Composite
{
    public class PasswordInput : TextInput
    {
        #region Ctor
        public PasswordInput()
        {
            Title = "密码域";
            //禁用输入法
            InputMethod.SetIsInputMethodEnabled(tb_Host, false);
            //获得焦点时，全选
            GotFocus += (sender, e) => tb_Host.SelectAll();
            PreviewKeyDown += PasswordInput_PreviewKeyDown;
            tb_Host.TextChanged += Tb_Host_TextChanged;
        }

        private string _canche = string.Empty;

        private void Tb_Host_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            tb_Host.TextChanged -= Tb_Host_TextChanged;
            var text = tb_Host.Text;
            foreach (var item in e.Changes)
            {
                if (item.RemovedLength > 0)
                    _canche = _canche.Remove(item.Offset, item.RemovedLength);
                if (item.AddedLength > 0)
                {
                    var add = text.Substring(item.Offset, item.AddedLength);
                    _canche = _canche.Insert(item.Offset, add);
                }
            }
            var careIndex = tb_Host.CaretIndex;
            tb_Host.Text = "".PadRight(text.Length, '*');
            tb_Host.CaretIndex = careIndex;
            tb_Host.TextChanged += Tb_Host_TextChanged;
        }

        private void PasswordInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }
        #endregion

        #region Override
        public override string Value
        {
            get
            {
                return _canche;
            }
            set
            {
                base.Value = value;
            }
        }
        #endregion
    }
}
