using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models
{
    public class BankResult
    {
        public int BankID { get; set; }
        public string AccoutName { get; set; }
        public string AccoutNum { get; set; }
        public string Total { get; set; }
        public string MaxDate { get; set; }
        public int ReconcileCount { get; set; }
        public string TotalBalance { get; set; }
        public int ShowDashboard { get; set; }
    }

    public class BankResultCollection
    {
        public List<BankResult> BankResults { get; set; }
        public int DraftComingInCount { get; set; }
        public string TotalDraftComingIn { get; set; }

        public int OverdueComingInCount { get; set; }
        public string TotalOverdueComingIn { get; set; }

        public int DraftGoingOutCount { get; set; }
        public string TotalDraftGoingOut { get; set; }

        public int OverdueGoingOutCount { get; set; }
        public string TotalOverdueGoingOut { get; set; }

        /// <summary>
        /// 0 - Draft current user
        /// </summary>
        public string Your_CurrentClaim_Total { get; set; }

        /// <summary>
        /// 0 - Draft current company
        /// </summary>
        public int All_CurrentClaim { get; set; }
        public string All_CurrentClaim_Total { get; set; }

        /// <summary>
        /// 1 - Authorisation
        /// </summary>
        public int AuthorisationClaim { get; set; }
        public string AuthorisationClaim_Total { get; set; }

        /// <summary>
        /// 3 -  Awaiting Payment
        /// </summary>
        public int AwaitingPaymentClaim { get; set; }
        public string AwaitingPaymentClaim_Total { get; set; }
    }

    public class URL_Chart
    {
        public URL_Chart() { }
        public double y { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class LabelValue_StackedColumn
    {
        public URL_Chart[] DataItems_1 { get; set; }
        public URL_Chart[] DataItems_2 { get; set; }
        public string[] Categories { get; set; }
        public string FormatString  { get; set; }
        public string UrlName   { get; set; }
        public string SetName_1 { get; set; }
        public string SetName_2 { get; set; }
        public int SetHeight { get; set; }

        public void SetValueDefault()
        {
            DataItems_1 = null;
            DataItems_2 = null;
            Categories = null;
            FormatString = null;
            UrlName = null;
            SetName_1 = null;
            SetName_2 = null;
            SetHeight = 0;
        }
    }
}