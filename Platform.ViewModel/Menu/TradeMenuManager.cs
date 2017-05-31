using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Platform.ViewModel.Menu
{
    public class TradeMenuManager
    {
        #region 单例模式
        private static TradeMenuManager _instance;

        /// <summary>
        /// 获取单例对象
        /// </summary>
        /// <returns></returns>
        public static TradeMenuManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TradeMenuManager(_menuFile);
                return _instance;
            }
        }

        public TradeMenuManager(FileInfo menuFile)
        {
            TradeModelList = new List<TradeModel>();
            LoadDataByXml(menuFile);
        }
        #endregion

        #region 存储方案
        private static FileInfo _menuFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Config/Menu.XML");
        private static FileInfo _menuXsd = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Config/Menu.xsd");

        /// <summary>
        /// 加载的菜单列表，TradeModel对象
        /// </summary>
        public List<TradeModel> TradeModelList { get; private set; }
        #endregion

        #region 私有方法
        /// <summary>
        /// 加载数据源(来自于xml文件)
        /// </summary>
        private void LoadDataByXml(FileInfo menuFile)
        {
            XDocument doc = XDocument.Load(menuFile.FullName);
            if (_menuXsd.Exists)
            {
                var xsd = new XmlSchemaSet();
                xsd.Add(null, _menuXsd.FullName);
                //默认验证会发生异常
                doc.Validate(xsd, null);
            }
            TradeModelList.Clear();
            GetModel(doc.Root, null);
        }

        private string GetAttribute(XElement element, string attribute)
        {
            var attr = element.Attribute(attribute);
            if (attr == null) return string.Empty;
            return attr.Value.Trim();
        }

        /// <summary>
        /// xml解析成TradeModel集合
        /// </summary>
        /// <param name="parentElement"></param>
        /// <returns></returns>
        private void GetModel(XElement parentElement, TradeModel parent)
        {
            var children = parentElement.Elements();
            foreach (var ele in children)
            {
                string guid = string.Empty;
                var id = ele.Attribute("id");
                if (id == null)
                    guid = Guid.NewGuid().ToString("N");
                else
                    guid = id.Value.Trim();
                //加载基础信息
                TradeModel model = new TradeModel()
                {
                    ID = guid,
                    IsVisible = GetAttribute(ele, "isvisible") == "1",
                    Parent = parent,
                    Name = GetAttribute(ele, "name"),
                    Desc = GetAttribute(ele, "desc")
                };
                //考虑isvisible属性，采用true/false模式设定
                bool isVisible;
                if (bool.TryParse(GetAttribute(ele, "isvisible"), out isVisible))
                    model.IsVisible = isVisible;
                switch (ele.Name.LocalName)
                {
                    case "category":
                        //获取子项，并填充父元素
                        GetModel(ele, model);
                        break;
                    case "trade":
                        model.Code = GetAttribute(ele, "code");
                        model.Action = GetAttribute(ele, "action");
                        foreach (var meta in ele.Elements())
                            model.Metadata.Add(
                                GetAttribute(meta, "key"),
                                 GetAttribute(meta, "value")
                                );
                        //检测是否存在重复Code节点
                        if (TradeModelList.Exists(x => x.Code == model.Code))
                        {
                            throw new ArgumentException(string.Format("菜单解析：存在重复Code的交易节点=[{0}]{1}", model.Code, model.Name));
                        }
                        break;
                    default:
                        break;
                }
                TradeModelList.Add(model);
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// 获取基于TradeViewModel的结构
        /// </summary>
        /// <returns></returns>
        public List<TradeViewModel> GetTradeVMs()
        {
            var groups = TradeModelList.GroupBy(x => x.Parent, x => new TradeViewModel(x)).ToList();
            //拿到顶层的节点
            var roots = groups.Find(x => x.Key == null);
            groups.Remove(roots);
            //处理完的节点集合
            var parents = roots.ToList();
            //循环处理完所有的组
            while (groups.Count != 0)
            {
                //处理首位元素
                var item = groups.ElementAt(0);
                groups.Remove(item);
                var parent = parents.FirstOrDefault(x => item.Key == x.Model);
                if (parent == null)
                { //没有找到父元素，将元素放置到队尾
                    groups.Insert(groups.Count, item);
                    continue;
                }
                //找到父元素，将组员存入到Children，并且存入到已处理集合
                foreach (var vm in item)
                    parent.Children.Add(vm);
                parents.AddRange(item);
            }
            return roots.ToList();
        }
        #endregion
    }
}
