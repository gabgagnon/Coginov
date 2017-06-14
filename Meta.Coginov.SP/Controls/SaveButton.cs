using Microsoft.SharePoint;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.Coginov.SP.Controls
{
    public class SaveButton:Microsoft.SharePoint.WebControls.SaveButton
    {
        protected override bool SaveItem()
        {
            Generate();
            return base.SaveItem();
        }

        private void Generate()
        {
            var audit = Audit.AuditSite(SPContext.Current.Site);
            var ser = new JsonSerializer();
            using (var stream = new MemoryStream())
            {
                using (var sw = new StreamWriter(stream))
                using (var jw = new JsonTextWriter(sw))
                {
                    ser.Serialize(jw, audit);
                    jw.Flush();
                }

                SPContext.Current.Web.AllowUnsafeUpdates = true;
                String auditDate = DateTime.Now.ToString("yyyyMMddhhmmss");
                SPListItem item = SPContext.Current.ListItem;
                item.Attachments.Add("Audit_" + auditDate + ".txt", stream.ToArray());
                item["Title"] = "Audit " + auditDate;
                item.Update();
                SPContext.Current.Web.AllowUnsafeUpdates = false;
            }
        }
    }
}
