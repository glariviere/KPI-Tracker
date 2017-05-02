using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

public partial class Summary : System.Web.UI.Page
{
    DateTime col4Week;
    DateTime col3Week;
    DateTime col2Week;
    DateTime col1Week; 
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //Date HF from which all dates are derived
            HF_Date.Value = DateFunctions.GetDateForMond(DateTime.Today).ToShortDateString();
            DefineSelectParam();
        }
    }
    public void DefineSelectParam()
    {
        //Create selection params for gv      
        SqlDataSource ds = KPI_Results_Raw;

        //Calling get date for Monday of any date
        //Starting from today as col4Week (4th col)
        col4Week = Convert.ToDateTime(HF_Date.Value);
        col3Week = col4Week.AddDays(-7);
        col2Week = col4Week.AddDays(-14);
        col1Week = col4Week.AddDays(-21);

        ds.SelectParameters["DateW4"].DefaultValue = col4Week.ToShortDateString();
        ds.SelectParameters["DateW3"].DefaultValue = col3Week.ToShortDateString();
        ds.SelectParameters["DateW2"].DefaultValue = col2Week.ToShortDateString();
        ds.SelectParameters["DateW1"].DefaultValue = col1Week.ToShortDateString();
        ds.SelectParameters["Plant_ID"].DefaultValue = HF_Plant.Value;
    } 

    protected void ComputeMTD()
    {    
        foreach (GridViewRow row in GV.Rows)
        {
            decimal total = 0;
            decimal colMTD = 0;
            string mtd = GV.DataKeys[row.RowIndex].Values[1].ToString();

            if (row.RowType == DataControlRowType.DataRow)
            {
                Label lblMTD = ((Label)row.FindControl("lblMTD"));

                List<Label> listWAdj = new List<Label>() {
                    LabelFinder(row.RowIndex, "lblW1Adj"),
                    LabelFinder(row.RowIndex, "lblW2Adj"),
                    LabelFinder(row.RowIndex, "lblW3Adj"),
                    LabelFinder(row.RowIndex, "lblW4Adj")};

                foreach (Label lbl in listWAdj)
                {
                    decimal add2total;
                    if (Decimal.TryParse(lbl.Text, out add2total))
                    {
                        total = total + add2total;
                    }
                }
                //Caluclate MTD value depend on aggregate rule
                switch (mtd)
                {
                    case "SUM":
                        colMTD = total;
                        break;
                    case "AVG":
                        colMTD = total / 4;
                        break;
                    default:
                        break;
                }
                lblMTD.Text = colMTD.ToString();

            }
        }
    }              
    
    protected void GV_DataBound(object sender, EventArgs e)
    {
        #region Compute KPI
        //Because some KPIs are computed from other KPIs, a new column is needed to hold this mix of dependant and precendent KPIs
        //Precedent KPIs = initial;
        //Dependent KPIs = function of some precedent KPIs;
        foreach (GridViewRow row in GV.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                //Look up values of Precedent KPI
                Label lblW1Adj = (Label)row.FindControl("lblW1Adj");
                decimal KPIVal;

                decimal KPI20Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("VA Sales (thousands"), "lblW1").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI21Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("NVA Sales (thousands)"), "lblW1").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI36Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD OT hours"), "lblW1").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI37Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD total hours"), "lblW1").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI39Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOI total hours"), "lblW1").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI40Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("Scrap Cost"), "lblW1").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI41Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD Earned hours"), "lblW1").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI42Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MRO Costs"), "lblW1").Text, out KPIVal)) ? 0 : KPIVal;

                if (lblW1Adj != null)
                {
                    switch (row.Cells[2].Text)
                    {
                        case "Scrap as % of Total Sales":
                            lblW1Adj.Text = (KPI40Val == 0 || KPI21Val == 0) ? "0" : (KPI39Val / KPI21Val).ToString();
                            break;
                        case "Manufacturing Costs (% of VASales)":
                            lblW1Adj.Text = (KPI20Val == 0) ? "0" : (KPI42Val / KPI20Val).ToString();
                            break;
                        case "Total Sales (thousands)":
                            lblW1Adj.Text = (KPI20Val + KPI21Val).ToString();
                            break;
                        case "DLE %":
                            lblW1Adj.Text = (KPI37Val == 0) ? "0" : (KPI41Val / KPI37Val).ToString();
                            break;
                        case "MOD HC Equivalent":
                            lblW1Adj.Text = (1 + KPI37Val / (8 * 4)).ToString();
                            break;
                        case "MOD OT %":
                            lblW1Adj.Text = (KPI36Val == 0 || KPI37Val == 0) ? "0" : (KPI36Val / KPI37Val).ToString();
                            break;
                        default:
                            lblW1Adj.Text = ((Label)row.FindControl("lblW1")).Text;
                            break;
                    }

                }
            }
            //W2
            if (row.RowType == DataControlRowType.DataRow)
            {
                //Look up values of Precedent KPI
                Label lblW2Adj = (Label)row.FindControl("lblW2Adj");
                decimal KPIVal;

                decimal KPI20Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("VA Sales (thousands"), "lblW2").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI21Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("NVA Sales (thousands)"), "lblW2").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI36Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD OT hours"), "lblW2").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI37Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD total hours"), "lblW2").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI39Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOI total hours"), "lblW2").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI40Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("Scrap Cost"), "lblW2").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI41Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD Earned hours"), "lblW2").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI42Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MRO Costs"), "lblW2").Text, out KPIVal)) ? 0 : KPIVal;

                if (lblW2Adj != null)
                {
                    switch (row.Cells[2].Text)
                    {
                        case "Scrap as % of Total Sales":
                            lblW2Adj.Text = (KPI40Val == 0 || KPI21Val == 0) ? "0" : (KPI39Val / KPI21Val).ToString();
                            break;
                        case "Manufacturing Costs (% of VASales)":
                            lblW2Adj.Text = (KPI20Val == 0) ? "0" : (KPI42Val / KPI20Val).ToString();
                            break;
                        case "Total Sales (thousands)":
                            lblW2Adj.Text = (KPI20Val + KPI21Val).ToString();
                            break;
                        case "DLE %":
                            lblW2Adj.Text = (KPI37Val == 0) ? "0" : (KPI41Val / KPI37Val).ToString();
                            break;
                        case "MOD HC Equivalent":
                            lblW2Adj.Text = (1 + KPI37Val / (8 * 4)).ToString();
                            break;
                        case "MOD OT %":
                            lblW2Adj.Text = (KPI36Val == 0 || KPI37Val == 0) ? "0" : (KPI36Val / KPI37Val).ToString();
                            break;
                        default:
                            lblW2Adj.Text = ((Label)row.FindControl("lblW2")).Text;
                            break;
                    }
                }
            }

            //W3
            if (row.RowType == DataControlRowType.DataRow)
            {
                //Look up values of Precedent KPI
                Label lblW3Adj = (Label)row.FindControl("lblW3Adj");
                decimal KPIVal;

                decimal KPI20Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("VA Sales (thousands"), "lblW3").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI21Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("NVA Sales (thousands)"), "lblW3").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI36Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD OT hours"), "lblW3").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI37Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD total hours"), "lblW3").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI39Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOI total hours"), "lblW3").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI40Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("Scrap Cost"), "lblW3").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI41Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD Earned hours"), "lblW3").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI42Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MRO Costs"), "lblW3").Text, out KPIVal)) ? 0 : KPIVal;

                if (lblW3Adj != null)
                {
                    switch (row.Cells[2].Text)
                    {
                        case "Scrap as % of Total Sales":
                            lblW3Adj.Text = (KPI40Val == 0 || KPI21Val == 0) ? "0" : (KPI39Val / KPI21Val).ToString();
                            break;
                        case "Manufacturing Costs (% of VASales)":
                            lblW3Adj.Text = (KPI20Val == 0) ? "0" : (KPI42Val / KPI20Val).ToString();
                            break;
                        case "Total Sales (thousands)":
                            lblW3Adj.Text = (KPI20Val + KPI21Val).ToString();
                            break;
                        case "DLE %":
                            lblW3Adj.Text = (KPI37Val == 0) ? "0" : (KPI41Val / KPI37Val).ToString();
                            break;
                        case "MOD HC Equivalent":
                            lblW3Adj.Text = (1 + KPI37Val / (8 * 4)).ToString();
                            break;
                        case "MOD OT %":
                            lblW3Adj.Text = (KPI36Val == 0 || KPI37Val == 0) ? "0" : (KPI36Val / KPI37Val).ToString();
                            break;
                        default:
                            lblW3Adj.Text = ((Label)row.FindControl("lblW3")).Text;
                            break;
                    }
                }
            }

            //W4
            if (row.RowType == DataControlRowType.DataRow)
            {
                //Look up values of Precedent KPI
                Label lblW4Adj = (Label)row.FindControl("lblW4Adj");
                decimal KPIVal;

                decimal KPI20Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("VA Sales (thousands"), "lblW4").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI21Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("NVA Sales (thousands)"), "lblW4").Text, out KPIVal)) ?0: KPIVal;
                decimal KPI36Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD OT hours"), "lblW4").Text, out KPIVal))?0: KPIVal;
                decimal KPI37Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD total hours"), "lblW4").Text, out KPIVal))?0:KPIVal;
                decimal KPI39Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOI total hours"), "lblW4").Text, out KPIVal))?0:KPIVal;
                decimal KPI40Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("Scrap Cost"), "lblW4").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI41Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD Earned hours"), "lblW4").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI42Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MRO Costs"), "lblW4").Text, out KPIVal)) ? 0 : KPIVal;                                                                 

                if (lblW4Adj != null)
                {
                    switch (row.Cells[2].Text)
                    {
                        case "Scrap as % of Total Sales":
                            lblW4Adj.Text = (KPI40Val == 0 || KPI21Val==0) ? "0" : (KPI39Val / KPI21Val).ToString();
                            break;
                        case "Manufacturing Costs (% of VASales)":
                            lblW4Adj.Text = (KPI20Val==0)?"0": (KPI42Val / KPI20Val).ToString();
                            break;
                        case "Total Sales (thousands)":
                            lblW4Adj.Text = (KPI20Val + KPI21Val).ToString();
                            break;
                        case "DLE %":
                            lblW4Adj.Text = (KPI37Val == 0) ? "0" : (KPI41Val / KPI37Val).ToString();
                            break;
                        case "MOD HC Equivalent":
                            lblW4Adj.Text = (1 + KPI37Val / (8 * 4)).ToString();
                            break;
                        case "MOD OT %":
                            lblW4Adj.Text = (KPI36Val == 0 || KPI37Val == 0) ? "0" : (KPI36Val / KPI37Val).ToString();
                            break;
                        default:
                            lblW4Adj.Text = ((Label)row.FindControl("lblW4")).Text;
                            break;
                    }
                }
            }
        }

        #endregion

        ComputeMTD();

        #region Function to set format based on type
        //Style KPI based on their data type
        foreach (GridViewRow row in GV.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                Label lblW1Adj = ((Label)row.FindControl("lblW1Adj"));
                Label lblW2Adj = ((Label)row.FindControl("lblW2Adj"));
                Label lblW3Adj = ((Label)row.FindControl("lblW3Adj"));
                Label lblW4Adj = ((Label)row.FindControl("lblW4Adj"));
                Label lblMTD = ((Label)row.FindControl("lblMTD"));
                Label lblBudget = ((Label)row.FindControl("lblBudget"));


                List<Label> lblWAdj = new List<Label>();
                lblWAdj.Add(lblW1Adj);
                lblWAdj.Add(lblW2Adj);
                lblWAdj.Add(lblW3Adj);
                lblWAdj.Add(lblW4Adj);
                lblWAdj.Add(lblMTD);
                lblWAdj.Add(lblBudget);

                string type = GV.DataKeys[row.RowIndex].Values[0].ToString();
                decimal colVal;

                foreach (Label lbl in lblWAdj)
                {
                    if (lbl != null)
                    {
                        switch (type)
                        {
                            case "p0":
                                if (lbl != null)
                                    if (Decimal.TryParse(lbl.Text, out colVal))
                                        lbl.Text = colVal.ToString("p0");
                                break;
                            case "p1":
                                if (lbl != null)
                                    if (Decimal.TryParse(lbl.Text, out colVal))
                                        lbl.Text = colVal.ToString("p1");
                                break;
                            case "p2":
                                if (lbl != null)
                                    if (Decimal.TryParse(lbl.Text, out colVal))
                                        lbl.Text = colVal.ToString("p2");
                                break;
                            case "c0":
                                if (lbl != null)
                                    if (Decimal.TryParse(lbl.Text, out colVal))
                                        lbl.Text = colVal.ToString("c0");
                                break;
                            case "n0":
                                if (lbl != null)
                                    if (Decimal.TryParse(lbl.Text, out colVal))
                                        lbl.Text = colVal.ToString("n0");
                                break;
                            case "n2":
                                if (lbl != null)
                                    if (Decimal.TryParse(lbl.Text, out colVal))
                                        lbl.Text = colVal.ToString("n2");
                                break;
                            default:
                                break;
                        }
                    }
                }
                
            }
        }
        #endregion

        #region Random content for comment column
        //Create default comments
        foreach (GridViewRow row in GV.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                int col4WeekNb = DateFunctions.GetWeekNumber(Convert.ToDateTime(HF_Date.Value));
                Label comments = ((Label)row.FindControl("lblComments"));
                if (comments.Text == "")
                {
                    comments.Text = $"This is a default comment for {HF_Plant.Value} for the week W{col4WeekNb}, This is a default comment for {HF_Plant.Value} for the week W{col4WeekNb}, This is a default comment for {HF_Plant.Value} for the week W{col4WeekNb}, This is a default comment for {HF_Plant.Value} for the week W{col4WeekNb},This is a default comment for {HF_Plant.Value} for the week W{col4WeekNb}";
                }
            }
        }
        #endregion

    }


    protected void GV_PreRender(object sender, EventArgs e)
    {
        MergeRows(GV);

        if (GV.Rows.Count > 0)
        {
            //Get week number for each of the 4 col        
            int col4WeekNb = DateFunctions.GetWeekNumber(Convert.ToDateTime(HF_Date.Value));
            int col3WeekNb = col4WeekNb - 1;
            int col2WeekNb = col4WeekNb - 2;
            int col1WeekNb = col4WeekNb - 3;

            //Set gv headers as W + weekNb            
            GV.HeaderRow.Cells[6].Text = $"W{col4WeekNb}";
            GV.HeaderRow.Cells[5].Text = $"W{col3WeekNb}";
            GV.HeaderRow.Cells[4].Text = $"W{col2WeekNb}";
            GV.HeaderRow.Cells[3].Text = $"W{col1WeekNb}";

            GV.HeaderRow.Cells[10].Text = $"W{col4WeekNb}";
            GV.HeaderRow.Cells[9].Text = $"W{col3WeekNb}";
            GV.HeaderRow.Cells[8].Text = $"W{col2WeekNb}";
            GV.HeaderRow.Cells[7].Text = $"W{col1WeekNb}";
        }
    }

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        HF_Plant.Value = DDL_plant.SelectedValue.ToString();
        KPI_Results_Raw.SelectParameters["Plant_ID"].DefaultValue = HF_Plant.Value;
        lbl_Plant.Text = DDL_plant.SelectedItem.Text;

        PAN_Info.Visible = true;
    }

    //Change period (-)
    protected void lkbtnPrevious_Click(object sender, EventArgs e)
    {
        //Remove 7 days to HF_Date on each click until week 4 is reach (showing weeks 1-4)
        if (DateFunctions.GetWeekNumber(Convert.ToDateTime(HF_Date.Value)) > 4)
        {
            HF_Date.Value = Convert.ToDateTime(HF_Date.Value).AddDays(-7).ToShortDateString();
            DefineSelectParam();
        }
        
    }
    //Change period (+)
    protected void lkbtnNext_Click(object sender, EventArgs e)
    {
        //Add 7 days to HF_Date on each click until latest week is reach)
        if (DateFunctions.GetWeekNumber(Convert.ToDateTime(HF_Date.Value)) < DateFunctions.GetWeekNumber(DateTime.Today))
        {
            HF_Date.Value = Convert.ToDateTime(HF_Date.Value).AddDays(+7).ToShortDateString();
            DefineSelectParam();
        }
    }

    protected void BTN_ExportExcel_Click(object sender, EventArgs e)
    {
        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition",
        "attachment;filename=GridViewExport.xls");
        Response.Charset = "";
        Response.ContentType = "application/vnd.ms-excel";
        StringWriter sw = new StringWriter();
        HtmlTextWriter hw = new HtmlTextWriter(sw);

        GV.AllowPaging = false;
        //Hide Comment column because of Ajax conflict
        GV.Columns[13].Visible = false;
        GV.DataBind();

        ////Apply style to Individual Cells       
        for (int i = 1; i < GV.Rows.Count; i++)
        {
            GridViewRow row = GV.Rows[i];

            //Change Color back to white
            row.BackColor = System.Drawing.Color.White;
            row.Font.Size = 10;

            //Apply text style to each Row
            row.Attributes.Add("class", "textmode");

            //Apply style to Individual Cells of Alternating Row
            if (i % 2 != 0)
            {
                row.Cells[0].Style.Add("background-color", "#F7F6F3");
                row.Cells[1].Style.Add("background-color", "#F7F6F3");
                row.Cells[2].Style.Add("background-color", "#F7F6F3");
                row.Cells[3].Style.Add("background-color", "#F7F6F3");
                row.Cells[4].Style.Add("background-color", "#F7F6F3");
                row.Cells[5].Style.Add("background-color", "#F7F6F3");
                row.Cells[6].Style.Add("background-color", "#F7F6F3");
                row.Cells[7].Style.Add("background-color", "#F7F6F3");
                row.Cells[8].Style.Add("background-color", "#F7F6F3");
            }
        }

        GV.RenderControl(hw);

        //style to format numbers to string
        string style = @"<style> .textmode { mso-number-format:\@; } </style>";
        Response.Write(style);
        Response.Output.Write(sw.ToString());
        Response.Flush();
        Response.End();
    }

    protected void BTN_ExportPDF_Click(object sender, EventArgs e)
    {
        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                //Hide Comment column because of Ajax conflict
                GV.Columns[13].Visible = false;
                GV.RenderControl(hw);
                StringReader sr = new StringReader(sw.ToString());
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();
            }
        }
    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        /* Verifies that the control is rendered */
    }

    //Grouping function to show plant Dashboard as one single merged cell
    public static void MergeRows(GridView gridView)
    {
        for (int rowIndex = gridView.Rows.Count - 2; rowIndex >= 0; rowIndex--)
        {
            GridViewRow row = gridView.Rows[rowIndex];
            GridViewRow previousRow = gridView.Rows[rowIndex + 1];

            if (row.Cells[0].Text == previousRow.Cells[0].Text)
            {
                row.Cells[0].RowSpan = previousRow.Cells[0].RowSpan < 2 ? 2 :
                                       previousRow.Cells[0].RowSpan + 1;
                previousRow.Cells[0].Visible = false;
            }

        }
    }

    protected int GetRowIndexByKPI(string kpi)
    {
        int rowIndex = 0;

        foreach (GridViewRow row in GV.Rows)
        {
            if (row.Cells[2].Text.Equals(kpi))
            {
                rowIndex = row.RowIndex;
                break;
            }
        }
        return rowIndex;
    }
    protected Label LabelFinder(int rowIndex, string lblName)
    {
        return (Label)GV.Rows[rowIndex].FindControl(lblName);
    }
    protected string LabelStrFinder(Label label)
    {
        return label.Text;
    } 
}










