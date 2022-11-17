using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;

namespace TMT.ERP.BusinessLogic.Utils
{
    public class AccountInfo
    {
        public int? AccountID { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public int AccountType { get; set; }
        public double? YTD { get; set; }
        private string sYTD;
        public string s_YTD
        {
            get
            {
                if (YTD.HasValue)
                {
                    var formatString = "#,0.00";
                    var sValue = YTD.Value.ToString(formatString);
                    sYTD = (sValue.IndexOf("-") > -1) ? sValue.Replace("-", "(") + ")" : sValue;
                }
                return sYTD;
            }
            set
            {
                sYTD = value;
            }
        }
      

        public int TypeID { get; set; }
        public string Type { get; set; }
        public string TaxRate { get; set; }
        public int? TaxRateID { get; set; }
        public bool? isParent { get; set; }
    }
    public class AccountManager
    {

        public static List<AccountInfo> GetAllAccount() 
        {
            var accountInfors = new List<AccountInfo>();
            var bankAccounts = new GenericManager<BankAccount>().Get().ToList();
            var accounts = new GenericManager<Account>().Get().ToList();
            foreach (var item in bankAccounts)
            {
                accountInfors.Add(new AccountInfo { 
                    AccountID=item.ID,
                    AccountCode = "",
                    AccountName=item.BankName,
                    AccountType = 1,
                    TypeID = 0,
                    Type = "Bank",
                    TaxRate = "Tax Exempt (0%)",
                    TaxRateID = null,
                    YTD = item.AccountTrans.Sum(x => x.Received) - item.AccountTrans.Sum(x => x.Spent),
                    s_YTD = string.Empty,
                    isParent=false
                });
            }

            foreach (var item in accounts)
            {
                accountInfors.Add(new AccountInfo
                {
                    AccountID = item.ID,
                    AccountCode = item.Code,
                    AccountName = item.Name,
                    AccountType =0,
                    TypeID = item.AccountType.ID,
                    Type = item.AccountType.Name,                    
                    TaxRate = item.TaxRate.Name,
                    TaxRateID = item.TaxRate.ID,
                    YTD = item.AccountTrans.Sum(x => x.Received) - item.AccountTrans.Sum(x => x.Spent),
                    isParent = false
                });
            }

            return accountInfors;            
        }

        public static List<AccountInfo> GetAccountByGroups(int[] groupID)
        {
            var accountInfors = new List<AccountInfo>();
            var bankAccounts = new GenericManager<BankAccount>().Get();

            foreach (var item in bankAccounts)
            {
                accountInfors.Add(new AccountInfo
                {
                    AccountID = item.ID,
                    AccountCode = "",
                    AccountName = item.BankName,
                    AccountType = 0,
                    isParent = false
                });
            }

            var accounts = new GenericManager<Account>().Get().Where(x => x.AccountInGroups.FirstOrDefault(y => groupID.Contains(y.AccountGroupID)) != null);
            foreach (var item in accounts)
            {
                accountInfors.Add(new AccountInfo
                {
                    AccountID = item.ID,
                    AccountCode = item.Code,
                    AccountName = item.Name,
                    AccountType = 1,
                    isParent = false
                });
            }

            return accountInfors;            

        }

    }
}
