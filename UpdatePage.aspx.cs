using AjaxControlToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class UpdatePage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    private void BindGrid()
    {
        HF_Date.Value = DDL_week.SelectedValue;
        HF_Plant.Value = DDL_plant.SelectedValue.ToString();

        string conString = ConfigurationManager.ConnectionStrings["KPI-ReportConnectionString"].ConnectionString;
        using (SqlConnection con = new SqlConnection(conString))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = "uspGetResultsByPlantForOneWeek";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "SELECT");
                cmd.Parameters.AddWithValue("@DateW", HF_Date.Value);
                cmd.Parameters.AddWithValue("@Plant_ID", HF_Plant.Value);
                cmd.Connection = con;
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    sda.SelectCommand = cmd;
                    using (DataSet ds = new DataSet())
                    {
                        sda.Fill(ds);
                        GV.DataSource = ds;
                        GV.DataBind();
                    }
                }
            }
        }
    }

    protected void GV_DataBound(object sender, EventArgs e)
    {
        #region Function to compute KPI dependent on other KPI
        //Because some KPIs are computed from other KPIs, a new column is needed to hold this mix of dependant and precendent KPIs
        //Precedent KPIs = initial;
        //Dependent KPIs = function of some precedent KPIs;
        foreach (GridViewRow row in GV.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                //Update button to hide if KPI is computed
                LinkButton btnEdit = (LinkButton)row.FindControl("btnEdit");
                TextBox tbxResAdj = (TextBox)row.FindControl("txbResAdj") ?? new TextBox();

                //Look up values of Precedent KPI
                Label lblResAdj = (Label)row.FindControl("lblResAdj");
                decimal KPIVal;

                decimal KPI20Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("VA Sales (thousands"), "lblRes").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI21Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("NVA Sales (thousands)"), "lblRes").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI36Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD OT hours"), "lblRes").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI37Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD total hours"), "lblRes").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI39Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOI total hours"), "lblRes").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI40Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("Scrap Cost"), "lblRes").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI41Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MOD Earned hours"), "lblRes").Text, out KPIVal)) ? 0 : KPIVal;
                decimal KPI42Val = (decimal.TryParse(LabelFinder(GetRowIndexByKPI("MRO Costs"), "lblRes").Text, out KPIVal)) ? 0 : KPIVal;

                //Compute value of Dependent KPI and hide there edit button
                if (lblResAdj != null)
                {
                    switch (row.Cells[1].Text)
                    {
                        case "Scrap as % of Total Sales":
                            lblResAdj.Text = (KPI40Val == 0 || KPI21Val == 0) ? "0" : (KPI39Val / KPI21Val).ToString();
                            break;
                        case "Manufacturing Costs (% of VASales)":
                            lblResAdj.Text = (KPI20Val == 0) ? "0" : (KPI42Val / KPI20Val).ToString();
                            break;
                        case "Total Sales (thousands)":
                            lblResAdj.Text = (KPI20Val + KPI21Val).ToString();
                            break;
                        case "DLE %":
                            lblResAdj.Text = (KPI37Val == 0) ? "0" : (KPI41Val / KPI37Val).ToString();
                            break;
                        case "MOD HC Equivalent":
                            lblResAdj.Text = (1 + KPI37Val / (8 * 4)).ToString();
                            break;
                        case "MOD OT %":
                            lblResAdj.Text = (KPI36Val == 0 || KPI37Val == 0) ? "0" : (KPI36Val / KPI37Val).ToString();
                            break;
                        default:
                            lblResAdj.Text = ((Label)row.FindControl("lblRes")).Text;
                            break;
                    }
                }
            }
        }
        #endregion

        #region Function to set format based on type
        //Styling based on type
        foreach (GridViewRow row in GV.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                string Id = GV.DataKeys[row.RowIndex].Values[0].ToString();
                string DateW = GV.DataKeys[row.RowIndex].Values[1].ToString();
                string Plant_ID = GV.DataKeys[row.RowIndex].Values[2].ToString();
                string type = GV.DataKeys[row.RowIndex].Values[3].ToString();

                decimal colVal;
                //styling controls
                Label lblResAdj = (Label)row.FindControl("lblResAdj");
                TextBox tbxResAdj = (TextBox)row.FindControl("tbxResAdj");

                switch (type)
                {
                    case "p0":
                        if (lblResAdj != null)
                            if (Decimal.TryParse(lblResAdj.Text, out colVal))
                                lblResAdj.Text = colVal.ToString("p0");
                        //Fix Validation
                        //if (tbxResAdj != null)
                        //    if (Decimal.TryParse(tbxResAdj.Text, out colVal))
                        //        tbxResAdj.Text = colVal.ToString("p0");
                        break;
                    case "p1":
                        if (lblResAdj != null)
                            if (Decimal.TryParse(lblResAdj.Text, out colVal))
                                lblResAdj.Text = colVal.ToString("p1");
                        break;
                    case "p2":
                        if (lblResAdj != null)
                            if (Decimal.TryParse(lblResAdj.Text, out colVal))
                                lblResAdj.Text = colVal.ToString("p2");
                        break;
                    case "c0":
                        if (lblResAdj != null)
                            if (Decimal.TryParse(lblResAdj.Text, out colVal))
                                lblResAdj.Text = colVal.ToString("c0");
                        break;
                    case "n0":
                        if (lblResAdj != null)
                            if (Decimal.TryParse(lblResAdj.Text, out colVal))
                                lblResAdj.Text = colVal.ToString("n0");
                        break;
                    case "n2":
                        if (lblResAdj != null)
                            if (Decimal.TryParse(lblResAdj.Text, out colVal))
                                lblResAdj.Text = colVal.ToString("n2");
                        break;
                    default:
                        break;
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
                Label comments = ((Label)row.FindControl("lblComments"));
                string date = Convert.ToDateTime(HF_Date.Value).ToShortDateString();

                if (comments != null && comments.Text == "")
                {
                    comments.Text = $"This is a default comment for {HF_Plant.Value} for the week  starting on {date}, This is a default comment for {HF_Plant.Value} for the week  starting on {date}, This is a default comment for {HF_Plant.Value} for the week  starting on {date}, This is a default comment for {HF_Plant.Value} for the week  starting on {date},This is a default comment for {HF_Plant.Value} for the week  starting on {date}";
                }
            }
        }
        #endregion
    }
    protected void GV_PreRender(object sender, EventArgs e)
    {
        MergeRows(GV);
    }

    protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GV.EditIndex = e.NewEditIndex;
    }

    protected void DDL_plant_SelectedIndexChanged(object sender, EventArgs e)
    {
        HF_Plant.Value = DDL_plant.SelectedValue.ToString();
        lbl_Plant.Text = HF_Plant.Value.ToString();
        if (DDL_week.SelectedValue != "")
        {
            PAN_Info.Visible = true;
        }
        this.BindGrid();
    }

    protected void DDL_week_SelectedIndexChanged(object sender, EventArgs e)
    {
        HF_Date.Value = DDL_week.SelectedValue;
        lbl_Date.Text = $"{Convert.ToDateTime(HF_Date.Value):MMM dd, yyyy}";

        if (DDL_plant.SelectedValue != "")
        {
            PAN_Info.Visible = true;
        }
        this.BindGrid();
    }

    //Normalizing function for passing-in specific parameters to each CRUD operation 
    //and prevent "Procedure or function has too many arguments specified" error
    //protected void SqlDataSource1_Updating(object sender, SqlDataSourceCommandEventArgs e)
    //{
    //    DbParameterCollection CmdParams = e.Command.Parameters;
    //    ParameterCollection UpdParams = ((SqlDataSourceView)sender).UpdateParameters;

    //    Hashtable ht = new Hashtable();
    //    foreach (Parameter UpdParam in UpdParams)
    //        ht.Add(UpdParam.Name, true);

    //    for (int i = 0; i < CmdParams.Count; i++)
    //    {
    //        if (!ht.Contains(CmdParams[i].ParameterName.Substring(1)))
    //            CmdParams.Remove(CmdParams[i--]);
    //    }

    //}


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
            if (row.Cells[1].Text.Equals(kpi))
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

    protected void OnCheckedChanged(object sender, EventArgs e)
    {
        bool isUpdateVisible = false;
        CheckBox chk = (sender as CheckBox);
        //If check All is checked, check all other checkboxes
        if (chk.ID == "chkAll")
        {
            foreach (GridViewRow row in GV.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkEdit = (CheckBox)row.FindControl("chkEdit");
                    chkEdit.Checked = chk.Checked;
                }
            }
        }

        CheckBox chkAll = (CheckBox)GV.HeaderRow.FindControl("chkAll");
        Button btnUpdate = (Button)GV.HeaderRow.FindControl("btnUpdate");
        chkAll.Checked = true;
        foreach (GridViewRow row in GV.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                //If true (isChecked), unhide editing TBX & hide LBL
                //except computed KPI, cannot be edited so leave LBL
                bool isChecked = ((CheckBox)row.FindControl("chkEdit")).Checked;

                ((Label)row.FindControl("lblComments")).Visible = !isChecked;
                ((TextBox)row.FindControl("tbxComments")).Visible = isChecked;
                Label lblResAdj = (Label)row.FindControl("lblResAdj");
                TextBox tbxResAdj = (TextBox)row.FindControl("tbxResAdj") ?? new TextBox();

                switch (row.Cells[1].Text)
                {
                    case "Scrap as % of Total Sales":
                        lblResAdj.Visible = true;
                        break;
                    case "Manufacturing Costs (% of VASales)":
                        lblResAdj.Visible = true;
                        break;
                    case "Total Sales (thousands)":
                        lblResAdj.Visible = true;
                        break;
                    case "DLE %":
                        lblResAdj.Visible = true;
                        break;
                    case "MOD HC Equivalent":
                        lblResAdj.Visible = true;
                        break;
                    case "MOD OT %":
                        lblResAdj.Visible = true;
                        break;
                    default:
                        lblResAdj.Visible = !isChecked;
                        tbxResAdj.Visible = isChecked;
                        break;
                }

                if (isChecked && !isUpdateVisible)
                {
                    isUpdateVisible = true;
                }
                if (!isChecked)
                {
                    chkAll.Checked = false;
                }
            }
        }
        btnUpdate.Visible = isUpdateVisible;
    }

    protected void Update(object sender, EventArgs e)
    {
        Button btnUpdate = (Button)GV.HeaderRow.FindControl("btnUpdate");

        foreach (GridViewRow row in GV.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                bool isChecked = ((CheckBox)row.FindControl("chkEdit")).Checked;
                if (isChecked)
                {
                    string conString = ConfigurationManager.ConnectionStrings["KPI-ReportConnectionString"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE dbo.KPI_Results_Raw SET KPI_Results_Raw.Result = @Result, KPI_Results_Raw.Comments = @Comments WHERE dbo.KPI_Results_Raw.Id = @Id "))
                        {
                            cmd.Parameters.AddWithValue("@Action", "UPDATE");
                            cmd.Parameters.AddWithValue("@Result", ((TextBox)row.FindControl("tbxResAdj")).Text);
                            cmd.Parameters.AddWithValue("@Comments", ((TextBox)row.FindControl("tbxComments")).Text);
                            cmd.Parameters.AddWithValue("@Id", GV.DataKeys[row.RowIndex].Values[0]);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteScalar();
                            con.Close();
                        }
                    }
                }
            }
        }
        btnUpdate.Visible = false;
        this.BindGrid();
    }
}
