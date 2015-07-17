using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.CustomFields.Fields
{
    public class GeneralLinkWithSource : Sitecore.Shell.Applications.ContentEditor.Link
    {
        private string _itemid;

        public string ItemID
        {
            get
            {
                return StringUtil.GetString(new string[1]
                                            {
                                              this._itemid
                                            });
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this._itemid = value;
            }
        }


        public override string Source
        {
            get
            {
                return this.GetViewStateString("Source");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                String newValue = value;
                if (value.StartsWith("query:", StringComparison.InvariantCulture))
                {

                    //base.SetViewStateString("Source", "/sitecore/content/Ricoh/Sites/USA/Home");

                    string query = value.Substring(6);
                    bool flag = query.StartsWith("fast:", StringComparison.InvariantCulture);

                    if (!flag)
                    {
                        Item item = Client.ContentDatabase.GetItem(this.ItemID);
                        if (item != null)
                        {
                            Item sourceItem = item.Axes.SelectSingleItem(value.Substring("query:".Length));
                            if (sourceItem != null)
                            {
                                base.SetViewStateString("Source", sourceItem.Paths.FullPath);
                            }
                        }
                    }


                }
                else
                {
                    string str = MainUtil.UnmapPath(newValue);
                    if (str.EndsWith("/", StringComparison.InvariantCulture))
                    {
                        str = str.Substring(0, str.Length - 1);
                    }
                    base.SetViewStateString("Source", str);
                }
            }
        }
    }
}
