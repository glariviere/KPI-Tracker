<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="UpdatePage.aspx.cs" Inherits="UpdatePage" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/css/styles.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        function ExpandCollapse() {
            var collPanel = $find("CollapsiblePanelExtender1");
            if (collPanel.get_Collapsed())
                collPanel.set_Collapsed(false);
            else
                collPanel.set_Collapsed(true);
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MC" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Panel ID="PAN_Header" runat="server" CssClass="PAN_Header">
                <h5 style="color: #ffffff;">Plant</h5>
                <asp:DropDownList ID="DDL_plant" runat="server" DataSourceID="KPI_Plants" DataTextField="City" DataValueField="Plant_ID" AutoPostBack="True" AppendDataBoundItems="True" OnSelectedIndexChanged="DDL_plant_SelectedIndexChanged" CssClass="DDL">
                    <asp:ListItem Value="">Pick a plant</asp:ListItem>
                </asp:DropDownList>
                <h5 style="color: #ffffff;">Period</h5>
                <asp:DropDownList ID="DDL_week" runat="server" DataSourceID="KPI_Dates" DataTextField="DateW" DataValueField="DateW" AutoPostBack="True" AppendDataBoundItems="True" OnSelectedIndexChanged="DDL_week_SelectedIndexChanged" CssClass="DDL" DataTextFormatString="{0:MMM dd, yyyy}">
                    <asp:ListItem Value="">Pick a week</asp:ListItem>
                </asp:DropDownList>
            </asp:Panel>
            <asp:HiddenField ID="HF_Date" runat="server" />
            <asp:HiddenField ID="HF_Plant" runat="server" />
            <asp:SqlDataSource ID="KPI_Plants" runat="server" ConnectionString="<%$ ConnectionStrings:KPI-ReportConnectionString %>" SelectCommand="SELECT Plant_ID, City FROM KPI_Plants ORDER BY City"></asp:SqlDataSource>
            <asp:SqlDataSource ID="KPI_Dates" runat="server" ConnectionString="<%$ ConnectionStrings:KPI-ReportConnectionString %>" SelectCommand="SELECT DISTINCT [DateW] FROM [KPI_Results_Raw] ORDER BY [DateW]"></asp:SqlDataSource>

            <asp:Panel ID="PAN_Info" runat="server" Width="875px" Visible="False">
                <asp:Panel ID="PAN_Info_Buttons" runat="server" CssClass="PAN_Info_Buttons">
                    <asp:Button ID="BTN_Edit" runat="server" CssClass="float-left" Text="Back To Summary" PostBackUrl="Summary.aspx" />
                </asp:Panel>
                <asp:Panel ID="PAN_Info_Title" runat="server" CssClass="PAN_Info_Title">
                    Updating KPI results for
                <asp:Label ID="lbl_Plant" runat="server" Text="Plant"></asp:Label>
                    &nbsp;for the period of
                <asp:Label ID="lbl_Date" runat="server" Text="Date"></asp:Label>
                </asp:Panel>
            </asp:Panel>
            &nbsp;<asp:GridView ID="GV" runat="server" CellPadding="8" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" Width="800px" DataKeyNames="Id,DateW,Plant_ID,Type" OnRowEditing="GridView1_RowEditing" CssClass="GV_Layout" OnDataBound="GV_DataBound" OnPreRender="GV_PreRender">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundField DataField="Plant_Dashboard" HeaderText="Plant Dashboard" SortExpression="Plant_Dashboard" ReadOnly="True">
                        <HeaderStyle CssClass="GV_Border_Bottom_Overlay" Width="80px" />
                        <ItemStyle Wrap="True" CssClass="GV_Category" Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="KPI" HeaderText="KPI" SortExpression="KPI" ReadOnly="True">
                        <HeaderStyle CssClass="GV_Border_Left_Overlay GV_Border_Bottom_Overlay" Width="230px" />
                        <ItemStyle CssClass="GV_Border_Left_Overlay" Wrap="False" Width="230px" />
                    </asp:BoundField>
                    <%--Keep result column hidden, but use value to compute adjusted results--%>
                    <asp:TemplateField HeaderText="Result" SortExpression="Result" Visible="true">
                        <ItemTemplate>
                            <asp:Label ID="lblRes" runat="server" Text='<%# Bind("Result") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Left_Overlay GV_Border_Bottom_Overlay" Width="90px" />
                        <ItemStyle CssClass="GV_Border_Left_Overlay" HorizontalAlign="Center" Width="90px" Wrap="False" />
                    </asp:TemplateField>
                    <%--Adjusted results, computed from raw results--%>
                    <asp:TemplateField HeaderText="Result">
                        <ItemTemplate>
                            <asp:Label ID="lblResAdj" runat="server" Text="Label"></asp:Label>
                            <asp:TextBox ID="tbxResAdj" runat="server" Text='<%# Bind("Result") %>' CssClass="TBX_Result" Width="80px"
                                Visible="false"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Left_Overlay GV_Border_Bottom_Overlay" Width="90px" />
                        <ItemStyle CssClass="GV_Border_Left_Overlay" HorizontalAlign="Center" Width="90px" Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Comments" SortExpression="Comments">
                        <ItemTemplate>
                            <ajaxToolkit:CollapsiblePanelExtender ID="CollapsiblePanelExtender1" runat="server"
                                TargetControlID="Panel1"
                                CollapsedSize="18"
                                Collapsed="True"
                                ExpandControlID="BTN_Collapse"
                                CollapseControlID="BTN_Collapse"
                                AutoCollapse="False"
                                AutoExpand="False"
                                ScrollContents="False"
                                TextLabelID="BTN_Collapse"
                                CollapsedText="+"
                                ExpandedText="-"
                                ExpandDirection="Vertical" SuppressPostBack="True" />
                            <asp:Panel ID="Panel1" runat="server">
                                <asp:Label ID="lblComments" runat="server" Text='<%# Bind("Comments") %>' CssClass="collapsePanelLabel"></asp:Label>
                                <asp:TextBox ID="tbxComments" runat="server" Text='<%# Bind("Comments") %>' TextMode="MultiLine" Width="280px" Visible="false" />
                                <asp:LinkButton ID="BTN_Collapse" runat="server" Text="" CssClass="commentLink" Height="18px"></asp:LinkButton>
                            </asp:Panel>
                        </ItemTemplate>
                        <ControlStyle Width="349px" />
                        <HeaderStyle CssClass="GV_Border_Left_Overlay GV_Border_Bottom_Overlay" Width="350px" />
                        <ItemStyle HorizontalAlign="Left" Wrap="True" CssClass="GV_Border_Left_Overlay" Width="349px" />
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Panel ID="Panel2" runat="server">Edit all</asp:Panel>
                            <asp:CheckBox ID="chkAll" runat="server" AutoPostBack="true" OnCheckedChanged="OnCheckedChanged" CssClass="GV_Edit_Header" />
                            <asp:Button ID="btnUpdate" runat="server" Text="Save" Visible="false" CssClass="GV_Edit_Header" OnClick="Update" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkEdit" runat="server" AutoPostBack="true" OnCheckedChanged="OnCheckedChanged" />
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Left_Overlay_Edit GV_Border_Bottom_Overlay" Width="50px" />
                        <ItemStyle HorizontalAlign="center" Wrap="True" CssClass="GV_Border_Left_Overlay_Edit pos_relative" Width="49px" />
                        <ControlStyle Width="49px" />
                    </asp:TemplateField>
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
