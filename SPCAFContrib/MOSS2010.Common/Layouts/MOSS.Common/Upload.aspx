<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="MOSS.Common.ApplicationPage.Upload" %>

<SharePoint:CssRegistration runat="server" Name="<% $SPUrl:~SiteCollection/_layouts/MOSS.Common/styles/upload_page.css %>" After="corev4.css" />

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="Expires" content="0" />
    <title></title>
</head>
<body>
    <script type="text/javascript">
        function SubmitForm() {
            document.form1.submit();
        }
    </script>
    <div>
        <form id="form1" name="form1" action="Upload.aspx" method="post" enctype="multipart/form-data" runat="server">
        <input type="file" runat="server" id="myFile" name="myFile" style="display: none;
            visibility: hidden;" onchange="javascript: btnSubmit.click();" />
        <input type="button" runat="server" id="btnSubmit" name="btnSubmit" onclick="javascript:SubmitForm();"
            style="display: none; visibility: hidden;" />
            <asp:Repeater ID="rptFiles" runat="server" OnItemDataBound="rptFiles_ItemDataBound">
                <HeaderTemplate>
                    <table class="regular_table">
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="fileLinkContainer">
                            <img style="top: -2px; left: -1px;" src="<% $SPUrl: ~sitecollection/_layouts/images/MOSS.Common/fileIcon.gif%>" runat="server"
                                alt="Download" />
                            <asp:HyperLink runat="server" ID="lnkFile" Text='<%# Eval("Name") %>' NavigateUrl='<%$SPUrl: ~sitecollection/_layouts/MOSS.Common/Download.aspx?ID={{{0}}}%>'  />
                        </td>
                        <td>
                            <asp:LinkButton ID="btnDeleteFile" runat="server" CommandArgument='<%# Eval("Id")%>'
                                OnClick="btnDeleteFile_Click" CausesValidation="False" OnClientClick="_spResetFormOnSubmitCalledFlag();">                                
                                    <img alt="Удалить" src="<% $SPUrl: ~sitecollection/_layouts/images/MOSS.Common/delete.gif%>" runat="server"/>                                
                            </asp:LinkButton>
                        </td>
                        <td>
                            <asp:Literal runat="server" Text='<%# Eval("Description") %>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
                
        </form>
    </div>
</body>
</html>
