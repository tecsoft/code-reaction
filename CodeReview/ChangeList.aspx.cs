using System;
using System.Web.UI.WebControls;
using CodeReview.Domain;
using CodeReviewer.Domain;
using System.Text;
using System.Web;
using System.Drawing;

namespace CodeReview.Web
{
    public partial class ChangeList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var sut = new CommitReader(@"D:\travail\trunk");

            //SvnFileDiff diff = sut.GetRevisionDiffs(34699)[0];
            //var unifiedDiff = sut.GetUnifiedDiff(diff);

            //var container = (FindControl("container") as PlaceHolder);

            ////StringBuilder builder = new StringBuilder();

            ////builder.Append("<pre>");

            //foreach (var item in unifiedDiff)
            //{
            //    if (item.Changed == SvnFileDiff.Change.None)
            //    {
            //        container.Controls.Add(new Label() { Text = HttpUtility.HtmlEncode(item.Text) + "<br/>" });
            //    }
            //    else if (item.Changed == SvnFileDiff.Change.Removed)
            //    {
            //        //builder.Append("<div style='color:red'>" + HttpUtility.HtmlEncode(item.Text) + "</div>");
            //        container.Controls.Add(new Label() { Text = HttpUtility.HtmlEncode(item.Text) + "<br/>", ForeColor = Color.Red });
            //    }
            //    else
            //    {
            //        //builder.Append("<div style='color:grey'>" + HttpUtility.HtmlEncode(item.Text) + "</div>");
            //        container.Controls.Add(new Label() { Text = HttpUtility.HtmlEncode(item.Text) + "<br/>", ForeColor = Color.Blue });
            //    }
            //}

            ////builder.Append("</pre>");

            ////(FindControl("container") as Literal).Text = builder.ToString();
            
        }

        protected void GetDiffs_Click(object sender, EventArgs e)
        {

        }
    }
}