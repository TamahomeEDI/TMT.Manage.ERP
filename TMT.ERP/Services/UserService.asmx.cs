using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.Models;

namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for UserService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class UserService : System.Web.Services.WebService
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetRoleByUserID(int userID)
        {
            List<string> roleLs = new List<string>();
            try
            {
                ErpEntities db = new ErpEntities();
                var user = db.Users.Find(userID);
                var userInRoles = user.UsersInRoles.ToList();
               
                foreach (var roles in userInRoles)
                {
                    var roleName = roles.Role.RoleName;
                    roleLs.Add(roleName);
                }

            }
            catch (Exception ex) {  }
            return serializer.Serialize(roleLs);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetControlInRole(string functionName)
        {
           // Dictionary<string, List<RoleObj>> dicControlRole = new Dictionary<string, List<RoleObj>>();
            List<int> roleLs = new List<int>();
            List<RoleControlResult> lsCtrRole = new List<RoleControlResult>();
            try
            {
                ErpEntities db = new ErpEntities();
                var function = db.Functions.Where(fu => fu.Name == functionName).FirstOrDefault();
                int functionID = function.ID;
                var controlLs = db.Controls.Where(co => co.FunctionID == functionID).ToList();
                foreach(var item in controlLs) {
                    RoleControlResult control = new RoleControlResult();
                    List<RoleObj> lsRole = new List<RoleObj>();
                    var controlinRoles = db.ControlInRoles.Where(ro => ro.ControlID == item.ID).ToList();
                    GetListRoleFromControlID(ref lsRole, controlinRoles);
                    control.controlID = item.ControlID;
                    control.data = lsRole;
                    lsCtrRole.Add(control);
                   // var roles = db.ControlInRoles.Where(con => con.ControlID == item.ID).Select(x => new { Text = x.Code + "-" + x.Name, Value = x.ID }).Distinct().ToList();
                   // dicControlRole.Add(item.ID, roleLs);
                }      
            }
            catch (Exception ex) { }
            return serializer.Serialize(lsCtrRole);
        }

        private void GetListRoleFromControlID(ref List<RoleObj> lsRole, List<ControlInRole> controlinRoles)
        {
            foreach (var item in controlinRoles)
            {
                RoleObj obj = new RoleObj();
                obj.rolename = item.Role.RoleName;
                obj.viewtype = item.ViewType ??0;
                lsRole.Add(obj);
            }
           
        }
    }
}
