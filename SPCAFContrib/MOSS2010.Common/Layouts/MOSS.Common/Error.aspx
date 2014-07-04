<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="MOSS.Common.Layouts.Error" %>
<%@ Register TagPrefix="SPCommon" TagName="IssueInformerUserControl" Src="~/_controltemplates/MOSS.Common/IssueInformerUserControl.ascx" %> 

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Error page</title>
        
    <link rel="stylesheet" type="text/css" href="/_layouts/moss.common/styles/error_page.css"/>
    <link rel="stylesheet" type="text/css" href="/_layouts/moss.common/styles/smoothness/jquery-ui.custom.min.css"/>

    <script type="text/javascript" src="/_layouts/moss.common/scripts/jquery.min.js"></script>
    <script type="text/javascript" src="/_layouts/moss.common/scripts/jquery-ui.custom.min.js"></script>
</head>
<body>
    <form id="Form1" runat="server">
        <div class="container_error">
            <div id="masthead">
                <div id="logo">
                    <a href="<% $SPUrl:~SiteCollection/%>" title="Go to site home page" runat="server">
                        <img src="<% $SPUrl: ~sitecollection/_layouts/images/MOSS.Common/error.gif%>" runat="server" alt="Error" />
                    </a>
                </div>
            </div>
            <div class="grid">
                <h1>
                    Error</h1>
                <p>
                    We apologize, but an error occurred and your request could not be completed.</p>
                <p>
                    This error has been logged. If you have additional information that you believe
                    may have caused this error, please contact technical support.</p>
                <SPCommon:IssueInformerUserControl runat="server" ID="ctlErrorMessage" Visible="false" ForeColor="Red" />
            </div>            
        </div>
    </form>
</body>
</html>    
