<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<TMT.ERP.Models.SaleInvoices>" %>

<!DOCTYPE html>
<script runat="server">

    protected void lvGoods_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        
    }

    public IQueryable<TMT.ERP.DataAccess.Model.Good> lvGoods_GetData()
    {
        return 
        Data.Assets.Where(x => x.EntityState != System.Data.EntityState.Deleted && (x.AssetType == iLogix.Model.Lookups.AssetType.OtherRealEstate.GetDescription() || x.AssetType == iLogix.Model.Lookups.AssetType.SubjectProperty.GetDescription())).AsQueryable();
    }
    
</script>


<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Update</title>
</head>
<body>
    <div>

        <asp:ListView ID="lvGoods" runat="server" ItemType="TMT.ERP.DataAccess.Model.Good" InsertItemPosition="LastItem" OnItemCommand="lvGoods_ItemCommand" SelectMethod="lvGoods_GetData"> 

        </asp:ListView>
    </div>
</body>
</html>
