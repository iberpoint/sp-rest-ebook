using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTClientApp
{
    public class Helper
    {
        public string Get_DateTime(string fieldname, SPListItem item, SPWeb web, string sp2016url, string restfieldId)
        {
            var property = "";
            if (item[fieldname] != null)
            {
                DateTime sayacbas = (DateTime)item[fieldname];
                property = "'" + restfieldId + "':'" + sayacbas.ToString("o") + "'";
            }
            else
            {
                property = "'" + restfieldId + "':null";
            }
            return property;
        }

        public string Get_Boolean(string fieldname, SPListItem item, SPWeb web, string sp2016url, string restfieldId)
        {
            var property = "";
            if (item[fieldname] != null)
            {
                property = "'" + restfieldId + "':" + item[fieldname].ToString().ToLower() + "";
            }
            else
            {
                property = "'" + restfieldId + "':null";
            }
            return property;
        }

        public string Get_LookUp(string fieldname, SPListItem item, SPWeb web, string sp2016url, string restfieldId)
        {
            var property = "";
            if (item[fieldname] != null)
            {
                var request = "'" + restfieldId + "':'" + item[fieldname].ToString().Split(new string[] { ";#" }, StringSplitOptions.RemoveEmptyEntries)[0].ToString() + "'";
                property = request;
            }
            else
            {
                property = "'" + restfieldId + "':null";
            }

            return property;
        }

        public string Get_Number(string fieldname, SPListItem item, SPWeb web, string sp2016url, string restfieldId)
        {
            var property = "";
            if (item[fieldname] != null)
            {
                property = "'" + restfieldId + "':'" + item[fieldname] + "'";
            }
            else
            {
                property = "'" + restfieldId + "':null";
            }
            return property;
        }

        public string Get_String(string fieldname, SPListItem item, SPWeb web, string sp2016url, string restfieldId)
        {
            var property = "";
            if (item[fieldname] != null)
            {
                property = "'" + restfieldId + "':'" + item[fieldname].ToString().Replace("'", "").Replace("\\", " \\\\ ") + "'";
            }
            else
            {
                property = "'" + restfieldId + "':null";
            }
            return property;
        }

        public string Get_Users(string fieldname, SPListItem item, SPWeb web, string sp2016url, string restfieldId)
        {
            var kulstr = "";
            if (item[fieldname] != null)
            {
                JArray bilgilendirilecekler_arr = new JArray();
                SPFieldUserValueCollection Bilgilendirilecekler = (SPFieldUserValueCollection)item[fieldname];
                if (Bilgilendirilecekler != null)
                {
                    bilgilendirilecekler_arr = new JArray();
                    foreach (SPFieldUserValue user in Bilgilendirilecekler)
                    {
                        try
                        {
                            SPUser spUser = user.User;
                            if (spUser != null)
                            {
                                int newuserid = RestHelper.GetIdOfUser(sp2016url + "_api/web/siteusers(@v)?@v=%27i%3A0%23.w%7C" + spUser.LoginName + "%27",
                                                                       sp2016url + "_api/contextinfo", spUser.LoginName);                               
                                    bilgilendirilecekler_arr.Add(newuserid);                               
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    List<int> kul = bilgilendirilecekler_arr.ToObject<List<int>>();
                    kulstr = "'" + restfieldId + "':{ '__metadata': { 'type': 'Collection(Edm.Int32)'},'results': [ " + string.Join(",", kul.ConvertAll(m => string.Format("{0}", m)).ToArray()) + " ] }";
                }
            }
            else
            {
                kulstr = "'" + restfieldId + "':{ '__metadata': { 'type': 'Collection(Edm.Int32)'},'results':[] }";

            }
            return kulstr;
        }

        public string Get_Group(string fieldname, SPListItem item, SPWeb web, string sp2016url, string restfieldId)
        {
            var property = "";
            if (item[fieldname] != null)
            {
                SPFieldUser iksorumluField = (SPFieldUser)item.Fields.GetField(fieldname);
                SPFieldUserValue iksorumluValue = (SPFieldUserValue)iksorumluField.GetFieldValue(item[fieldname].ToString());
                if (iksorumluValue.User == null)
                {
                    try
                    {
                        SPGroup iksorumlugrp = web.SiteGroups[iksorumluValue.LookupValue];
                        int newuserid = RestHelper.GetIdOfUser(sp2016url + "_api/web/sitegroups/GetByName('" + iksorumlugrp.Name + "')?$select=id",
                                                              sp2016url + "_api/contextinfo", iksorumlugrp.Name);
                        if (newuserid != 0)
                        {
                            property = "'" + restfieldId + "':'" + newuserid + "'";
                        }
                        else
                        {
                            RestHelper.AddGroup("{ '__metadata':{ 'type': 'SP.Group' }, 'Title':'" + iksorumlugrp.Name + "' }",
                                                                                       sp2016url + "_api/contextinfo",
                                                                                       sp2016url + "_api/web/sitegroups");
                            newuserid = RestHelper.GetIdOfUser(sp2016url + "_api/web/sitegroups/GetByName('" + iksorumlugrp.Name + "')?$select=id",
                                                              sp2016url + "_api/contextinfo", iksorumlugrp.Name);
                            property = "'" + restfieldId + "':'" + newuserid + "'";
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        property = "'" + restfieldId + "':null";
                    }
                }
                else
                {
                    try
                    {
                        SPUser user = web.EnsureUser(iksorumluValue.User.LoginName);
                        int user_id = RestHelper.GetIdOfUser(sp2016url + "_api/web/siteusers(@v)?@v=%27i%3A0%23.w%7C" + user.LoginName + "%27",
                                                                          sp2016url + "_api/contextinfo", user.LoginName);
                        if (user_id != 0)
                        {
                            property = "'" + restfieldId + "':'" + user_id + "'";
                        }
                        else
                        {                           
                            property = "'" + restfieldId + "':null";                           
                        }
                    }
                    catch (Exception)
                    {
                        property = "'" + restfieldId + "':null";
                    }
                }
            }
            else
            {
                property = "'" + restfieldId + "':null";
            }
            return property;
        }
    }
}
