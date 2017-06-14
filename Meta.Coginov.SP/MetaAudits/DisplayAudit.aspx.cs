using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.Text;
using System.Web.UI.HtmlControls;

namespace Meta.Coginov.SP.Pages
{
    public partial class DisplayAudit : WebPartPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentItem = SPContext.Current.ListItem;

            if (currentItem.Attachments.Count == 0)
                return;

            var attachFolder = currentItem.ParentList.RootFolder.SubFolders["Attachments"].SubFolders[currentItem.ID.ToString()];

            auditid.InnerText = currentItem.Title;
            auditDataContainer.Controls.Add(
                new HtmlGenericControl("script") {
                    InnerHtml = "var auditData = " + Encoding.UTF8.GetString(attachFolder.Files[currentItem.Attachments[0]].OpenBinary()) });
        }
    }
}
