<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Summary.aspx.cs" Inherits="Summary" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/css/styles.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .auto-style1 {
            float: left;
            position: relative;
            left: 0px;
            top: 0px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MC" runat="Server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="True">
        </asp:ScriptManager>
            </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
            <asp:PostBackTrigger ControlID="BTN_ExportExcel" />
            <asp:PostBackTrigger ControlID="BTN_ExportPDF" />
        </Triggers>

        <ContentTemplate>
            <asp:Panel ID="PAN_Header" runat="server" CssClass="PAN_Header">
                <h5 style="color: #ffffff;">Plant</h5>
                <asp:DropDownList ID="DDL_plant" runat="server" AppendDataBoundItems="True" AutoPostBack="True" DataSourceID="KPI_Plants" DataTextField="City" DataValueField="Plant_ID" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" CssClass="DDL">
                    <asp:ListItem Value="">Pick a plant</asp:ListItem>
                </asp:DropDownList>
            </asp:Panel>
            <asp:SqlDataSource ID="KPI_Plants" runat="server" ConnectionString="<%$ ConnectionStrings:KPI-ReportConnectionString %>" SelectCommand="SELECT Plant_ID, City FROM KPI_Plants ORDER BY City"></asp:SqlDataSource>
            <asp:HiddenField ID="HF_Date" runat="server" />
            <asp:HiddenField ID="HF_Plant" runat="server" />
            <asp:Panel ID="PAN_Info" runat="server" Visible="False">
                <asp:Panel ID="PAN_Info_Buttons" runat="server" CssClass="PAN_Info_Buttons">
                    <asp:Button ID="BTN_Edit" runat="server" CssClass="auto-style1" Text="Edit Report" PostBackUrl="UpdatePage.aspx" />
                    <asp:Button ID="BTN_ExportExcel" runat="server" Text="Export to Excel" CssClass="float-right" OnClick="BTN_ExportExcel_Click" />
                    <asp:Button ID="BTN_ExportPDF" runat="server" CssClass="float-right" OnClick="BTN_ExportPDF_Click" Text="Export to PDF" />
                </asp:Panel>
                <asp:Panel ID="PAN_Info_Title" runat="server" CssClass="PAN_Info_Title">
                    &nbsp;KPI Summary For The Plant of 
                <asp:Label ID="lbl_Plant" runat="server" Text="Plant"></asp:Label>
                </asp:Panel>
                <asp:Panel ID="PAN_Info_Links" runat="server" CssClass="PAN_Info_Links">
                    <asp:LinkButton ID="lkbtnPrevious" runat="server" CssClass="PAN_Info_Links_Left" OnClick="lkbtnPrevious_Click">&lt;&lt;</asp:LinkButton>
                    <asp:Label ID="Label2" runat="server" Text="Change period" CssClass="PAN_Info_Links_Title"></asp:Label>
                    <asp:LinkButton ID="lkbtnNext" runat="server" CssClass="PAN_Info_Links_Right" OnClick="lkbtnNext_Click">&gt;&gt;</asp:LinkButton>
                </asp:Panel>
            </asp:Panel>
            <asp:GridView ID="GV" runat="server" CellPadding="8" DataSourceID="KPI_Results_Raw" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" Width="1140px" DataKeyNames="Type,MTD,Plant_ID,KPI" Height="200px" OnPreRender="GV_PreRender" CssClass="GV_Layout" OnDataBound="GV_DataBound">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:BoundField DataField="Plant_Dashboard" HeaderText="Plant Dashboard" SortExpression="Plant_Dashboard">
                        <HeaderStyle VerticalAlign="Bottom" Width="80px" CssClass="GV_Border_Bottom_Overlay" />
                        <ItemStyle Wrap="True" CssClass="GV_Category" Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Responsible" HeaderText="Responsible" SortExpression="Responsible">
                        <HeaderStyle Width="100px" CssClass="GV_Border_Left_Overlay GV_Border_Bottom_Overlay" />
                        <ItemStyle CssClass="GV_Border_Left_Overlay" Wrap="False" Width="100px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="KPI" HeaderText="KPI" SortExpression="KPI">
                        <HeaderStyle Width="230px" CssClass="GV_Border_Bottom_Overlay" />
                        <ItemStyle Wrap="False" Width="230px" />
                    </asp:BoundField>
                    <%--Keep result column hidden, but use value to compute adjusted results--%>
                    <asp:TemplateField HeaderText="W1" SortExpression="W1" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblW1" runat="server" Text='<%# Bind("W1") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Left_Overlay GV_Border_Bottom_Overlay" Width="73px" />
                        <ItemStyle CssClass="GV_Border_Left_Overlay" HorizontalAlign="Center" Width="73px" Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="W2" SortExpression="W2" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblW2" runat="server" Text='<%# Bind("W2") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Bottom_Overlay" Width="73px" />
                        <ItemStyle HorizontalAlign="Center" Width="73px" Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="W3" SortExpression="W3" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblW3" runat="server" Text='<%# Bind("W3") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Bottom_Overlay" HorizontalAlign="Center" Width="73px" />
                        <ItemStyle HorizontalAlign="Center" Width="73px" Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="W4" SortExpression="W4" Visible="false">
                        <ItemTemplate>
                            <asp:Label ID="lblW4" runat="server" Text='<%# Bind("W4") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Bottom_Overlay" Width="73px" />
                        <ItemStyle HorizontalAlign="Center" Width="73px" Wrap="False" />
                    </asp:TemplateField>
                    <%--Adjusted results, computed from raw results--%>
                    <asp:TemplateField HeaderText="W1Adj">
                        <ItemTemplate>
                            <asp:Label ID="lblW1Adj" runat="server" Text="Label"></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Left_Overlay GV_Border_Bottom_Overlay" Width="73px" />
                        <ItemStyle CssClass="GV_Border_Left_Overlay" HorizontalAlign="Center" Width="73px" Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="W2Adj">
                        <ItemTemplate>
                            <asp:Label ID="lblW2Adj" runat="server" Text="Label"></asp:Label>
                        </ItemTemplate>
                      <HeaderStyle CssClass="GV_Border_Bottom_Overlay" HorizontalAlign="Center" Width="73px" />
                        <ItemStyle HorizontalAlign="Center" Width="73px" Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="W3Adj">
                        <ItemTemplate>
                            <asp:Label ID="lblW3Adj" runat="server" Text="Label"></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Bottom_Overlay" HorizontalAlign="Center" Width="73px" />
                        <ItemStyle HorizontalAlign="Center" Width="73px" Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="W4Adj">
                        <ItemTemplate>
                            <asp:Label ID="lblW4Adj" runat="server" Text="Label"></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Bottom_Overlay" Width="73px" />
                        <ItemStyle HorizontalAlign="Center" Width="73px" Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Month To Date">
                        <ItemTemplate>
                            <asp:Label ID="lblMTD" runat="server"></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Left_Overlay GV_Border_Bottom_Overlay" Width="73px" Wrap="True" />
                        <ItemStyle CssClass="GV_Border_Left_Overlay" HorizontalAlign="Center" Width="73px" Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Budget" SortExpression="Budget Target">                        
                        <ItemTemplate>
                            <asp:Label ID="lblBudget" runat="server" Text='<%# Bind("[Budget Target]") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle CssClass="GV_Border_Bottom_Overlay" Width="73px" />
                        <ItemStyle HorizontalAlign="Center" Width="73px" Wrap="False" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Comments" SortExpression="Comments">                        
                        <ItemTemplate>
                            <ajaxToolkit:collapsiblepanelextender ID="CollapsiblePanelExtender1" runat="server"
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
                                <asp:LinkButton ID="BTN_Collapse" runat="server" Text="" CssClass="commentLink" Height="18px"></asp:LinkButton>
                            </asp:Panel>
                        </ItemTemplate>
                        <ControlStyle Width="299px" />
                        <HeaderStyle CssClass="GV_Border_Left_Overlay GV_Border_Bottom_Overlay" Width="300px" />
                        <ItemStyle HorizontalAlign="Left" Wrap="True" CssClass="GV_Border_Left_Overlay" Width="400px" />
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

            <asp:SqlDataSource ID="KPI_Results_Raw" runat="server" ConnectionString="<%$ ConnectionStrings:KPI-ReportConnectionString %>" SelectCommand="uspGetResultsByPlantByPeriod" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:Parameter DefaultValue="" Name="Plant_ID" Type="String" />
                    <asp:Parameter DefaultValue="" Name="DateW1" Type="DateTime" />
                    <asp:Parameter DefaultValue="" Name="DateW2" Type="DateTime" />
                    <asp:Parameter DefaultValue="" Name="DateW3" Type="DateTime" />
                    <asp:Parameter DefaultValue="" Name="DateW4" Type="DateTime" />
                </SelectParameters>
            </asp:SqlDataSource>
        </ContentTemplate>
    </asp:UpdatePanel>


</asp:Content>








