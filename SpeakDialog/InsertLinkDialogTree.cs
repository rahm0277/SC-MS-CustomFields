using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Shell;
using Sitecore.Speak.Applications;
using Sitecore.StringExtensions;
using Sitecore.Web;
using Sitecore.Web.PageCodes;
using System.Net;
using System.Xml.Linq;

namespace MS.CustomFields.Fields.SpeakDialog
{
    public class InsertLinkDialogTree : PageCodeBase
    {
        public Sitecore.Mvc.Presentation.Rendering TreeView { get; set; }

        public Sitecore.Mvc.Presentation.Rendering ListViewToggleButton { get; set; }

        public Sitecore.Mvc.Presentation.Rendering TreeViewToggleButton { get; set; }

        public Sitecore.Mvc.Presentation.Rendering InsertEmailButton { get; set; }

        public Sitecore.Mvc.Presentation.Rendering InsertAnchorButton { get; set; }

        public Sitecore.Mvc.Presentation.Rendering TextDescription { get; set; }

        public Sitecore.Mvc.Presentation.Rendering Target { get; set; }

        public Sitecore.Mvc.Presentation.Rendering AltText { get; set; }

        public Sitecore.Mvc.Presentation.Rendering QueryString { get; set; }

        public Sitecore.Mvc.Presentation.Rendering StyleClass { get; set; }

        public override void Initialize()
        {
            string setting = Settings.GetSetting("BucketConfiguration.ItemBucketsEnabled");
            this.ListViewToggleButton.Parameters["IsVisible"] = setting;
            this.TreeViewToggleButton.Parameters["IsVisible"] = setting;
            this.TreeView.Parameters["ShowHiddenItems"] = UserOptions.View.ShowHiddenItems.ToString();
            this.ReadQueryParamsAndUpdatePlaceholders();
        }

        private static string GetXmlAttributeValue(XElement element, string attrName)
        {
            return element.Attribute((XName)attrName) != null ? element.Attribute((XName)attrName).Value : string.Empty;
        }

        private void ReadQueryParamsAndUpdatePlaceholders()
        {
            string queryString1 = WebUtil.GetQueryString("ro");
            string queryString2 = WebUtil.GetQueryString("hdl");
            if (!string.IsNullOrEmpty(queryString1) && queryString1 != "{0}")
                this.TreeView.Parameters["RootItem"] = queryString1;
            this.InsertAnchorButton.Parameters["Click"] = string.Format(this.InsertAnchorButton.Parameters["Click"], (object)WebUtility.UrlEncode(queryString1), (object)WebUtility.UrlEncode(queryString2));
            this.InsertEmailButton.Parameters["Click"] = string.Format(this.InsertEmailButton.Parameters["Click"], (object)WebUtility.UrlEncode(queryString1), (object)WebUtility.UrlEncode(queryString2));
            this.ListViewToggleButton.Parameters["Click"] = string.Format(this.ListViewToggleButton.Parameters["Click"], (object)WebUtility.UrlEncode(queryString1), (object)WebUtility.UrlEncode(queryString2));
            bool flag = queryString2 != string.Empty;
            string text = string.Empty;
            if (flag)
                text = UrlHandle.Get()["va"];
            if (!(text != string.Empty))
                return;
            XElement element = XElement.Parse(text);
            if (InsertLinkDialogTree.GetXmlAttributeValue(element, "linktype") == "internal")
            {
                if (!string.IsNullOrEmpty(InsertLinkDialogTree.GetXmlAttributeValue(element, "id")))
                {
                    Item contextItem = ((Database)ClientHost.Databases.ContentDatabase).GetItem(queryString1 ?? string.Empty) ?? ((Database)ClientHost.Databases.ContentDatabase).GetRootItem();
                    Item linkedItem = (Item)SelectMediaDialog.GetMediaItemFromQueryString(InsertLinkDialogTree.GetXmlAttributeValue(element, "id"));
                    if (contextItem != null && linkedItem != null && linkedItem.Paths.LongID.StartsWith(contextItem.Paths.LongID))
                        this.TreeView.Parameters["PreLoadPath"] = contextItem.ID.ToString() + linkedItem.Paths.LongID.Substring(contextItem.Paths.LongID.Length);
                }
                this.TextDescription.Parameters["Text"] = InsertLinkDialogTree.GetXmlAttributeValue(element, "text");
                this.AltText.Parameters["Text"] = InsertLinkDialogTree.GetXmlAttributeValue(element, "title");
                this.StyleClass.Parameters["Text"] = InsertLinkDialogTree.GetXmlAttributeValue(element, "class");
                this.QueryString.Parameters["Text"] = InsertLinkDialogTree.GetXmlAttributeValue(element, "querystring");
            }
        }
    }
}
