<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangeList.aspx.cs" Inherits="CodeReview.Web.ChangeList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="CodeReview.css" media="screen" />
</head>
<body>
    <form id="form1" runat="server">
    
        <div id="container" >
       <div id="titleBar">Code Review</div>
       <div id="commits" >
           <div id="commitBar" >
               commits:
           </div>
           <div class="commit-detail">
               #fixes112 : Console loop by tcarter
           </div>
           <div class="commit-detail-selected">
               #fixes21212 : zeareazrdj, ezjdfezdezd
           </div>
           <div class="commit-detail">
               #fixes : ezfeazffregererge ddeded
           </div>

       </div>
       <div id="review">

                <table id="fileContent" class="file-content" >
                    <thead>
                        <tr><th colspan="3">File dada.cs</th></tr>
                    </thead>
                    <tbody>
                    <tr class="file-line">
                        <td class="file-line-orig">1</td>
                        <td class="file-line-new">1</td>
                        <td class="file-line-code">&nbsp; for( int i = 0; i &lt; 2000; i++ )</td>
                    </tr>
                    <tr class="file-line added">
                        <td class="file-line-orig">1</td>
                        <td class="file-line-new">1</td>
                        <td class="file-line-code">+ {</td>
                    </tr>
                    <tr class="file-line removed">
                        <td class="file-line-orig">1</td>
                        <td class="file-line-new">1</td>
                        <td class="file-line-code">-&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine("HelloWorld");</td>
                    </tr>
                    <tr class="file-line">
                        <td class="file-line-orig">1</td>
                        <td class="file-line-new">1</td>
                        <td class="file-line-code">&nbsp; }</td>
                    </tr>
                        </tbody>
                    <tfoot></tfoot>
                 </table>
               </div>
      </div>
    </form>
</body>
</html>
