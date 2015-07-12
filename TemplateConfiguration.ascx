<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="TemplateConfiguration.ascx.cs" Inherits="DotNetNuke.Modules.Announcements.TemplateConfiguration" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<div class="dnnForm dnnTemplateConfig dnnClear" id="TemplateConfigForm">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="plHeaderTemplate" runat="server" ControlName="txtHeaderTemplate" Suffix=":" />
            <asp:TextBox ID="txtHeaderTemplate" Width="350" Columns="30" TextMode="MultiLine"
                Rows="10" MaxLength="2000" runat="server" />
            <asp:LinkButton ID="cmdLoadDefHeader" runat="server" CausesValidation="False" CssClass="dnnSecondaryAction" resourcekey="LoadDefault">Load Default</asp:LinkButton>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plTemplate" runat="server" ControlName="txtTemplate" Suffix=":"></dnn:Label>
            <asp:TextBox ID="txtTemplate" Width="350" Columns="30" TextMode="MultiLine"
                Rows="10" MaxLength="2000" runat="server" />
            <asp:LinkButton ID="cmdLoadDefItemTemplate" runat="server" CausesValidation="False" CssClass="dnnSecondaryAction" resourcekey="LoadDefault">Load Default</asp:LinkButton>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plAltItemTemplate" runat="server" ControlName="txtAltItemTemplate" Suffix=":"></dnn:Label>
            <asp:TextBox ID="txtAltItemTemplate" Width="350" Columns="30" TextMode="MultiLine"
                Rows="10" MaxLength="2000" runat="server" />
            <asp:LinkButton ID="cmdLoadDefAltItemTemplate" runat="server" CausesValidation="False" CssClass="dnnSecondaryAction" resourcekey="LoadDefault">Load Default</asp:LinkButton>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plSeparator" runat="server" ControlName="txtSeparator" Suffix=":"></dnn:Label>
            <asp:TextBox ID="txtSeparator" Width="350" Columns="30" TextMode="MultiLine"
                Rows="10" MaxLength="2000" runat="server" />
            <asp:LinkButton ID="cmdLoadDefSeparator" runat="server" CausesValidation="False" CssClass="dnnSecondaryAction" resourcekey="LoadDefault">Load Default</asp:LinkButton>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="plFooterTemplate" runat="server" ControlName="txtFooterTemplate" Suffix=":"></dnn:Label>
            <asp:TextBox ID="txtFooterTemplate" Width="350" Columns="30" TextMode="MultiLine"
                Rows="10" MaxLength="2000" runat="server" />
            <asp:LinkButton ID="cmdLoadDefFooterTemplate" runat="server" CausesValidation="False" CssClass="dnnSecondaryAction" resourcekey="LoadDefault">Load Default</asp:LinkButton>

        </div>
    </fieldset>
    <h2 id="dnnPanel-TemplateHelp" class="dnnFormSectionHead">
        <a href="">
            <%=LocalizeString("TemplateHelp.head")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <asp:Literal runat="server" ID="litTemplateHelp"></asp:Literal>

        </div>
    </fieldset>
    <div class="dnnActions dnnClear">
        <ul class="dnnActions dnnClear">
            <li>
                <asp:LinkButton ID="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" ResourceKey="cmdUpdate" /></li>
            <li>
                <asp:HyperLink ID="cancelHyperLink" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" /></li>
        </ul>
    </div>

</div>

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setupForm() {
            $('#TemplateConfigForm').dnnPanels();

            $('#<%= cmdLoadDefHeader.ClientID %>').dnnConfirm({
                text: '<%= Localization.GetSafeJSString("LoadDefault.Confirm", LocalResourceFile) %>',
                yesText: '<%= Localization.GetSafeJSString("Yes.Text", LocalResourceFile) %>',
                noText: '<%= Localization.GetSafeJSString("No.Text", LocalResourceFile) %>',
                title: '<%= Localization.GetSafeJSString("LoadDefault.Text", LocalResourceFile) %>'
            });
            $('#<%= cmdLoadDefItemTemplate.ClientID %>').dnnConfirm({
                text: '<%= Localization.GetSafeJSString("LoadDefault.Confirm", LocalResourceFile) %>',
                yesText: '<%= Localization.GetSafeJSString("Yes.Text", LocalResourceFile) %>',
                noText: '<%= Localization.GetSafeJSString("No.Text", LocalResourceFile) %>',
                title: '<%= Localization.GetSafeJSString("LoadDefault.Text", LocalResourceFile) %>'
            });
            $('#<%= cmdLoadDefAltItemTemplate.ClientID %>').dnnConfirm({
                text: '<%= Localization.GetSafeJSString("LoadDefault.Confirm", LocalResourceFile) %>',
                yesText: '<%= Localization.GetSafeJSString("Yes.Text", LocalResourceFile) %>',
                noText: '<%= Localization.GetSafeJSString("No.Text", LocalResourceFile) %>',
                title: '<%= Localization.GetSafeJSString("LoadDefault.Text", LocalResourceFile) %>'
            });
            $('#<%= cmdLoadDefSeparator.ClientID %>').dnnConfirm({
                text: '<%= Localization.GetSafeJSString("LoadDefault.Confirm", LocalResourceFile) %>',
                yesText: '<%= Localization.GetSafeJSString("Yes.Text", LocalResourceFile) %>',
                noText: '<%= Localization.GetSafeJSString("No.Text", LocalResourceFile) %>',
                title: '<%= Localization.GetSafeJSString("LoadDefault.Text", LocalResourceFile) %>'
            });
            $('#<%= cmdLoadDefFooterTemplate.ClientID %>').dnnConfirm({
                text: '<%= Localization.GetSafeJSString("LoadDefault.Confirm", LocalResourceFile) %>',
                yesText: '<%= Localization.GetSafeJSString("Yes.Text", LocalResourceFile) %>',
                noText: '<%= Localization.GetSafeJSString("No.Text", LocalResourceFile) %>',
                title: '<%= Localization.GetSafeJSString("LoadDefault.Text", LocalResourceFile) %>'
            });
        }

        $(document).ready(function () {
            setupForm();

            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setupForm();
            });
        });
    }(jQuery, window.Sys));
</script>