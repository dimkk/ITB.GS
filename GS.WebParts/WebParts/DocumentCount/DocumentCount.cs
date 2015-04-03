using Microsoft.SharePoint.Security;
using System.Security.Permissions;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace GS.WebParts
{
    /// <summary>
    /// TODO: Add comment for webpart DocumentCount
    /// </summary>
    [SharePointPermission(SecurityAction.LinkDemand, ObjectModel = true)]
    [SharePointPermission(SecurityAction.InheritanceDemand, ObjectModel = true)]
    public class DocumentCount : WebPart
    {
        private const string ASCXPATH = @"/_CONTROLTEMPLATES/15/GS/DocumentCountUserControl.ascx";

        private UserControl userControl;

        public DocumentCount()
        {
        }

        protected override void CreateChildControls()
        {
            userControl = (UserControl)this.Page.LoadControl(ASCXPATH);
            Controls.Add(userControl);
            base.CreateChildControls();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.RenderContents(writer);
        }
    }
}

