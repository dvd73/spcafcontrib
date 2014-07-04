<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IssueInformerUserControl.ascx.cs" Inherits="MOSS.Common.Controls.IssueInformerUserControl" %>

    <script type="text/javascript">
        function ShowIssueDetailsDialog() {
            $("#<% = pnlDetailsDialog.ClientID %>").dialog(
            {
                modal: true,
                width: 880,
                buttons:
                {
                    Ok: function () {
                        $(this).dialog("close");
                    }
                }
            });
            return false;
        }
    </script>

<a href="javascript:void();" id="btnMore" runat="server"  onclick="javascript:ShowIssueDetailsDialog();">
<table class="regular_table">
<tr>
    <td>
        <asp:Label runat="server" ID="lblMessage" Visible="false" />
    </td>
    <td style="width:10px;">&nbsp;</td>
    <td>
        <asp:Image runat="server" ID="imgMore" AlternateText="подробно" ToolTip="подробно" ImageUrl="<%$SPUrl : ~sitecollection/_layouts/images/ICPROP.GIF %>"/>
    </td>
</tr>
</table>
</a>
<asp:Panel runat="server" ID="pnlDetailsDialog" title="Подробности инцидента" style="display:none">
    <asp:Literal runat="server" ID="ltrDetails" /> 
</asp:Panel>
