using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint.WebPartPages;

namespace Meta.Coginov.SP.Pages
{
    public partial class Duplicates : WebPartPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var audit = Audit.AuditSite(SPContext.Current.Site);
            foreach(var dup in audit.Duplicates)
                this.DuplicateFiles.Controls.Add(new HtmlGenericControl("li") { InnerText = dup.Path + " - " + dup.Hash });
        }
    }
}
