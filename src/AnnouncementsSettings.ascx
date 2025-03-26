<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AnnouncementsSettings.ascx.cs" Inherits="DotNetNuke.Modules.Announcements.AnnouncementsSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="URL" Src="~/controls/URLControl.ascx" %>

<div class="dnnForm dnnTemplateConfig dnnClear" id="TemplateConfigForm">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plHistory" runat="server" ControlName="txtHistory" Suffix=":" />
            <asp:TextBox ID="txtHistory" runat="server" Width="100px" Text="" />
            <asp:CompareValidator ID="valHistory" resourcekey="History.ErrorMessage" runat="server"
                CssClass="dnnFormMessage dnnFormError" ControlToValidate="txtHistory" ErrorMessage="<br />You Must Enter A Valid Number Of Days"
                Display="Dynamic" Type="Integer" Operator="DataTypeCheck" />
        </div>
    </fieldset>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plDefaultView" runat="server" ControlName="ddlViewType" Suffix=":" />
            <asp:DropDownList ID="ddlViewType" runat="server" Width="100px" />
        </div>
    </fieldset>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plDescriptionLength" Suffix=":" ControlName="txtDescriptionLength" runat="server" />
            <asp:TextBox ID="txtDescriptionLength" runat="server" Width="100px"></asp:TextBox>
            <asp:CompareValidator ID="valDescriptionLength" runat="server" Operator="DataTypeCheck"
                Type="Integer" Display="Dynamic" ErrorMessage="<br />You must enter a valid integer"
                ControlToValidate="txtDescriptionLength" CssClass="dnnFormMessage dnnFormError" resourcekey="Integer.ErrorMessage" />
        </div>
    </fieldset>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plEditorHeight" Suffix=":" ControlName="txtEditorHeight" runat="server" />
            <asp:TextBox ID="txtEditorHeight" runat="server" Width="100px" />
            <asp:CompareValidator ID="valEditorHeight" runat="server" Operator="DataTypeCheck"
                Type="Integer" Display="Dynamic" ErrorMessage="<br />You must enter a valid integer"
                ControlToValidate="txtEditorHeight" CssClass="dnnFormMessage dnnFormError"
                resourcekey="Integer.ErrorMessage" />
        </div>
    </fieldset>
</div>
