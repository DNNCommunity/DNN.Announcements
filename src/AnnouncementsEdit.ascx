<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AnnouncementsEdit.ascx.cs" Inherits="DotNetNuke.Modules.Announcements.AnnouncementsEdit" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Tracking" Src="~/controls/URLTrackingControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="FilePickerUploader" Src="~/controls/filepickeruploader.ascx" %>
<%@ Import Namespace="DotNetNuke.Services.Localization" %>
<div class="dnnForm dnnAnnouncementForm dnnClear" id="AnnouncementsForm">
    <ul class="dnnAdminTabNav dnnClear" id="">
        <li id="contentTab" runat="server"><a href="#dnnContent"><%=LocalizeString("content.Tab")%></a></li>
        <li id="imageLinksTab" runat="server"><a href="#dnnImageLinks"><%=LocalizeString("imageLinks.Tab")%></a></li>
        <li id="publishingTab" runat="server"><a href="#dnnPublishing"><%=LocalizeString("publishing.Tab")%></a></li>
        <li id="auditingTab" runat="server"><a href="#dnnAuditing"><%=LocalizeString("auditing.Tab")%></a></li>
    </ul>
    <div id="dnnContent" class="dnnContent dnnClear">
        <div class="dnnClear">
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label ID="plTitle" runat="server" ControlName="txtTitle" Suffix=":" />
                    <asp:TextBox ID="txtTitle" runat="server" />
                    <asp:RequiredFieldValidator ID="valTitle" resourcekey="Title.ErrorMessage" runat="server"
                        CssClass="dnnFormMessage dnnFormError" ControlToValidate="txtTitle" ErrorMessage="You Must Enter A Title For The Announcement"
                        Display="Dynamic" />
                </div>
            </fieldset>
            <h2 id="dnnPanel-Description" class="dnnFormSectionHead">
                <a href="">
                    <%=LocalizeString("plDescription")%></a></h2>
            <fieldset>
                <div class="dnnFormItem">

                    <dnn:TextEditor ID="teDescription" runat="server" height="400" width="100%" />
                    <asp:RequiredFieldValidator ID="valDescription" resourcekey="Description.ErrorMessage"
                        runat="server" CssClass="dnnFormMessage dnnFormError" ControlToValidate="teDescription" ErrorMessage="You Must Enter A Description Of The Announcement"
                        Display="Dynamic" />
                </div>
            </fieldset>
            </div>
    </div>
    <div id="dnnImageLinks" class="dnnImageLinks dnnClear">
        <fieldset>
            <div class="dnnFormItem">
                <dnn:Label ID="plImage" runat="server" ControlName="urlImage" Suffix=":" />
                <dnn:FilePickerUploader ID="urlImage" runat="server" Required="True"  />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plURL" runat="server" ControlName="ctlURL" Suffix=":" />
                <dnn:URL ID="ctlURL" runat="server" Width="300" ShowNone="true" />
            </div>
        </fieldset>
    </div>
    <div id="dnnPublishing" class="dnnPublishing dnnClear">
        <fieldset>
            <div class="dnnFormItem">
                <dnn:Label ID="plViewOrder" runat="server" ControlName="txtViewOrder" Suffix=":">
                </dnn:Label>
                <asp:TextBox ID="txtViewOrder" runat="server" MaxLength="3" Columns="20" Width="72px"
                    CssClass="NormalTextBox"></asp:TextBox>
                <asp:CompareValidator ID="valViewOrder" resourcekey="ViewOrder.ErrorMessage" runat="server"
                    CssClass="NormalRed" ControlToValidate="txtViewOrder" ErrorMessage="<br>View order must be an integer value."
                    Display="Dynamic" Type="Integer" Operator="DataTypeCheck"></asp:CompareValidator>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plPublishDate" Suffix=":" ControlName="publishDate" runat="server" />
 			    <div class="dateDiv"><asp:TextBox ID="publishDate" type="datetime-local" runat="server" /></div>
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="plExpireDate" Suffix=":" ControlName="expireDate" runat="server" />
 			    <div class="dateDiv"><asp:TextBox ID="expireDate" type="datetime-local" runat="server" /></div>
            </div>
        </fieldset>
    </div>
    <div id="dnnAuditing" class="dnnAuditing dnnClear">
        <div class="dnnAnn_Audit">
            <div class="dnnAnn_bold"><%=LocalizeString("plAudit")%></div>
            <dnn:Audit ID="ctlAudit" runat="server" />
        </div>
        <div class="dnnAnn_Audit">
            <div class="dnnAnn_bold"><%=LocalizeString("plTracking")%></div>
            <dnn:Tracking ID="ctlTracking" runat="server" />
        </div>
    </div>
    <div class="dnnActions dnnClear">
        <ul class="dnnActions dnnClear">
            <li>
                <asp:LinkButton ID="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" ResourceKey="cmdUpdate" /></li>
            <li>
                <asp:LinkButton ID="cmdDelete" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdDelete" CausesValidation="False" /></li>
            <li>
                <asp:HyperLink ID="cancelHyperLink" runat="server" CssClass="dnnSecondaryAction" resourcekey="cmdCancel" /></li>
        </ul>
    </div>
</div>

<script  type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function setUpAnnouncementsForm() {
            $('#AnnouncementsForm').dnnTabs().dnnPanels();

            $('#<%= cmdDelete.ClientID %>').dnnConfirm({
                text: '<%= Localization.GetSafeJSString("DeleteItem.Text", Localization.SharedResourceFile) %>',
                yesText: '<%= Localization.GetSafeJSString("Yes.Text", Localization.SharedResourceFile) %>',
                noText: '<%= Localization.GetSafeJSString("No.Text", Localization.SharedResourceFile) %>',
                title: '<%= Localization.GetSafeJSString("Confirm.Text", Localization.SharedResourceFile) %>'
            });

        }

        $(document).ready(function () {
            setUpAnnouncementsForm();

            //set active tab
            var activeTab = '<%= ActiveDnnTab %>';
            if (activeTab) {
                $('#' + activeTab + ' a').click();
            }

            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setUpAnnouncementsForm();
            });
        });
    }(jQuery, window.Sys));
</script>
