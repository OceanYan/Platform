using Platform.Widget.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platform.Print;

namespace Platform.Widget.Contract
{
    public class PrintContract : ContractBase
    {
        public PrintContract(ContractPage page) : base(page)
        {
        }

        protected virtual List<PrintContainer> CreatePrintList()
        {
            return null;
        }

        /// <summary>
        /// 触发打印流程
        /// </summary>
        public override bool Raise()
        {
            //产生打印清单
            List<PrintContainer> printList = CreatePrintList();
            if (printList == null || printList.Count == 0)
                return true;
            //拆解批量打印场景
            var pl = new List<PrintContainer>();
            //数据加载
            foreach (PrintContainer item in printList)
            {
                //采用接口方式的打印项需要进行数据加载
                var print = item as IPrint;
                if (print == null || print.Data == null)
                {
                    pl.Add(item);
                }
                else
                {
                    print.Data.LoadContact(this);
                    print.LoadPrintData();
                    if (print.Data.IsBatchPrint)
                    {
                        pl.AddRange(print.Data.BatchList);
                    }
                    else
                    {
                        pl.Add(item);
                    }
                }
            }
            //解析打印规则
            PageData.APIs.PrintCertificate(pl.ToArray());
            return true;
        }

        public interface IPrint
        {
            /// <summary>
            /// 打印数据入口，定义打印所需的数据集合
            /// </summary>
            PrintData Data { get; }

            /// <summary>
            /// 加载打印数据，该方法由系统自动调用
            /// </summary>
            void LoadPrintData();
        }

        /// <summary>
        /// 打印数据入口
        /// </summary>
        public abstract class PrintData
        {
            public void LoadContact(PrintContract contract)
            {
                Contract = contract;
            }

            public PrintContract Contract { get; private set; }

            /// <summary>
            /// 批量打印标志
            /// </summary>
            public bool IsBatchPrint { get; set; }

            /// <summary>
            /// 批量打印清单
            /// </summary>
            public List<PrintContainer> BatchList { get; set; }
        }
    }
}
