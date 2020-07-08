using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SPSite site = new SPSite("http://testsp2007"))
            {
                using (SPWeb web = site.OpenWeb("/ik"))
                {
                    SPList list = web.GetList(web.Url + "/Lists/Departmanlar");
                    SPQuery query = new SPQuery();
                    query.RowLimit = 5000;
                    string sp2016url = "http://testsp2016/ik";
                    string saatbody = string.Empty, saatoutlookbody = string.Empty;
                    string listurl = "_api/web/Lists?$expand=ListItemAllFields&$filter=Title eq 'Departmanlar'";
                    string encodedlisturl = SPEncode.UrlEncode(listurl);                   
                    string listguid = RestHelper.GetIdOfList(sp2016url + SPEncode.UrlDecodeAsUrl(encodedlisturl),
                                                                           sp2016url + "_api/contextinfo");                   
                   
                    SPListItemCollection items = list.GetItems(query);
                    foreach (SPListItem item in items)
                    {
                        try
                        {
                            if (item != null)
                            {
                                var sirket = Helper.Get_LookUp("Sirket", item, web, sp2016url, "SirketId");
                                var title = Helper.Get_String("Title", item, web, sp2016url, "Title");
                                var mudur = Helper.Get_Group("Mudur", item, web, sp2016url, "MudurId");
                                RestHelper.RestYap("{'__metadata': {'type': 'SP.Data.DepartmanlarListItem'}," + sirket + "," + title + "," + mudur + " }",
                                                    sp2016url + "_api/contextinfo",
                                                    listguid + "/items",
                                                    listguid,
                                                    false, false, false, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                }
            }                                    
        }
    }
}
