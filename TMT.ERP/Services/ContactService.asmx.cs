using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;

namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for ContactService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ContactService : System.Web.Services.WebService
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetContactByType(int contactType)
        {
            object result = "Success";
            try
            {
                var items = new GenericManager<Contact>().Get();
                if (contactType > 0)
                    result = items.Where(x => x.Type == contactType).Select(x => new { ID = x.ID, Name = x.ContactName }).OrderBy(x => x.ID);
                else
                    result = items.Select(x => new { ID = x.ID, Name = x.ContactName }).OrderBy(x => x.ID);
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetContactStartWith(string name_startsWith)
        {
            object result = "Success";
            try
            {
                var items = new GenericManager<Contact>().Get();
                if (name_startsWith != "")
                    result = items.Where(x => (x.Type == 0 || x.Type == 2) && (x.ContactName.StartsWith(name_startsWith.ToLower()) || x.ContactName.StartsWith(name_startsWith.ToUpper()) || x.ContactName.Contains(name_startsWith))).Select(x => new { ID = x.ID, Name = x.ContactName }).OrderBy(x => x.ID);
                else
                    result = items.Select(x => new { ID = x.ID, Name = x.ContactName }).OrderBy(x => x.ID);
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetCustomerStartWith(string nameStartsWith)
        {
            object result = "Success";
            try
            {
                var items = new GenericManager<Contact>().Get();
                result = nameStartsWith != "" ? items.Where(x => (x.Type == 1 || x.Type == 2) && (x.ContactName.Contains(nameStartsWith.ToLower()) || x.ContactName.Contains(nameStartsWith.ToUpper()) || x.ContactName.Contains(nameStartsWith))).Select(x => new { ID = x.ID, Name = x.ContactName }).OrderBy(x => x.ID) : items.Select(x => new { ID = x.ID, Name = x.ContactName }).OrderBy(x => x.ID);
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }

        //ContactType: 0 - Supplier, 1 - Customer, 2-Both
        public int GetContactByName(string contactName, int contactType)
        {
            var manager = new GenericManager<Contact>();
            var contact = manager.Find(x => x.ContactName == contactName);
            if (contact == null)
            {
                contact = new Contact();
                contact.ContactName = contactName;
                contact.Type = contactType;
                manager.Add(contact);
                manager.Save();
            }
            return contact.ID;
        }

    }
}
