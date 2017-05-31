using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Platform.Addins.T0001
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            base.DataBus = new DataBus();
            base.Contract = new SubmitEntry(this);
        }

        public new DataBus DataBus
        {
            get { return base.DataBus as DataBus; }
        }

        public new SubmitEntry Contract
        {
            get { return base.Contract as SubmitEntry; }
        }

        public override bool InitPage()
        {
            cbb_1.SetItems(new Dictionary<string, string> { { "A", "A" }, { "B", "B" }, { "C", "C" } });
            cbb_2.SetItems(new Dictionary<string, string> { { "A", "A" }, { "B", "B" }, { "C", "C" } });
            cbb_3.SetItems(new Dictionary<string, string> { { "A", "A" }, { "B", "B" }, { "C", "C" } });

            //MessageBox.Show("" + SubmitButton);
            // MessageBox.Show("" + CancelButton);
            Console.WriteLine("initpage:" + DateTime.Now);

            DataBus.A = "123";
            DataBus.B = "AAA";
            DataBus.C = "中文";
            return true;
        }

        private void cbb_FlowLeave(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("cbb_FlowLeave");
            //cbb_3.ToastMessage("手机打开拉家带口啊就是打开垃圾的卡拉健康的啊速度加快拉家带口垃圾就");

        }
    }
}
