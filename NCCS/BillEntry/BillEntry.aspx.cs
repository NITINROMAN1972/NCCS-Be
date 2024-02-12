using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.ServiceModel.Security.Tokens;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BillEntry_BillEntry : System.Web.UI.Page
{
    string connectionString = ConfigurationManager.ConnectionStrings["Ginie"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Search_DD_RequistionNo();
        }
    }

    private void alert(string mssg)
    {
        // alert pop - up with only message
        string message = mssg;
        string script = $"alert('{message}');";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "messageScript", script, true);
    }

    //=========================={ Sweet Alert JS }==========================

    // sweet alert - success only
    private void getSweetAlertSuccessOnly()
    {
        string title = "Saved!";
        string message = "Record saved successfully!";
        string icon = "success";
        string confirmButtonText = "OK";

        string sweetAlertScript =
            $@"<script>
                Swal.fire({{ 
                    title: '{title}', 
                    text: '{message}', 
                    icon: '{icon}', 
                    confirmButtonText: '{confirmButtonText}' 
                }});
            </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    // sweet alert - success redirect
    private void getSweetAlertSuccessRedirect(string redirectUrl)
    {
        string title = "Saved!";
        string message = "Record saved successfully!";
        string icon = "success";
        string confirmButtonText = "OK";
        string allowOutsideClick = "false";

        string sweetAlertScript =
            $@"<script>
                Swal.fire({{ 
                    title: '{title}', 
                    text: '{message}', 
                    icon: '{icon}', 
                    confirmButtonText: '{confirmButtonText}',
                    allowOutsideClick: {allowOutsideClick}
                }}).then((result) => {{
                    if (result.isConfirmed) {{
                        window.location.href = '{redirectUrl}';
                    }}
                }});
            </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    // sweet alert - success redirect block
    private void getSweetAlertSuccessRedirectMandatory(string titles, string mssg, string redirectUrl)
    {
        string title = titles;
        string message = mssg;
        string icon = "success";
        string confirmButtonText = "OK";
        string allowOutsideClick = "false"; // Prevent closing on outside click

        string sweetAlertScript =
        $@"<script>
            Swal.fire({{ 
                title: '{title}', 
                text: '{message}', 
                icon: '{icon}', 
                confirmButtonText: '{confirmButtonText}', 
                allowOutsideClick: {allowOutsideClick}
            }}).then((result) => {{
                if (result.isConfirmed) {{
                    window.location.href = '{redirectUrl}';
                }}
            }});
        </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    // sweet alert - question only
    private void getSweetAlertQuestionOnly(string titl, string mssg)
    {
        string title = titl;
        string message = mssg;
        string icon = "question";
        string confirmButtonText = "OK";

        string sweetAlertScript =
            $@"<script>
                Swal.fire({{ 
                    title: '{title}', 
                    text: '{message}', 
                    icon: '{icon}', 
                    confirmButtonText: '{confirmButtonText}' 
                }});
            </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }

    // sweet alert - error only block
    private void getSweetAlertErrorMandatory(string titles, string mssg)
    {
        string title = titles;
        string message = mssg;
        string icon = "error";
        string confirmButtonText = "OK";
        string allowOutsideClick = "false"; // Prevent closing on outside click

        string sweetAlertScript =
        $@"<script>
            Swal.fire({{ 
                title: '{title}', 
                text: '{message}', 
                icon: '{icon}', 
                confirmButtonText: '{confirmButtonText}', 
                allowOutsideClick: {allowOutsideClick}
            }});
        </script>";
        ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", sweetAlertScript, false);
    }



    //=========================={ Binding Search Dropdowns }==========================
    private void Search_DD_RequistionNo()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from Requisition1891 order by ReqNo desc";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            ddScRequisitionNo.DataSource = dt;
            ddScRequisitionNo.DataTextField = "ReqNo";
            ddScRequisitionNo.DataValueField = "ReqNo";
            ddScRequisitionNo.DataBind();
            ddScRequisitionNo.Items.Insert(0, new ListItem("------Select Requisition No------", "0"));
        }
    }



    //=========================={ Fetch Datatable }==========================

    protected void gridSearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //binding GridView to PageIndex object
        gridSearch.PageIndex = e.NewPageIndex;

        DataTable pagination = (DataTable)Session["PaginationDataSource"];

        gridSearch.DataSource = pagination;
        gridSearch.DataBind();
    }

    private DataTable GetRequisitionDT(string ReqNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from Requisition1891 where ReqNo = @ReqNo";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@ReqNo", ReqNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            return dt;
        }
    }




    //=========================={ Search Dropdown }==========================

    protected void btnNewBill_Click(object sender, EventArgs e)
    {
        Response.Redirect("NewBillEntry/NewBillEntry.aspx");
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        BindGridView();
    }

    private void BindGridView()
    {
        searchGridDiv.Visible = true;

        // dropdown values
        string reqNo = ddScRequisitionNo.SelectedValue;

        // date - from & to date
        // DateTime fromDate = DateTime.Parse(ScFromDate.Text);
        // DateTime toDate = DateTime.Parse(ScToDate.Text);

        DateTime fromDate;
        DateTime toDate;

        if (!DateTime.TryParse(ScFromDate.Text, out fromDate)) { fromDate = SqlDateTime.MinValue.Value; }
        if (!DateTime.TryParse(ScToDate.Text, out toDate)) { toDate = SqlDateTime.MaxValue.Value; }


        // DTs
        DataTable reqDT = GetRequisitionDT(reqNo);

        
        // dt values
        string requisitionNo = (reqDT.Rows.Count > 0) ? reqDT.Rows[0]["ReqNo"].ToString() : string.Empty;

        DataTable searchResultDT = SearchRecords(requisitionNo, fromDate, toDate);

        // binding the search grid
        gridSearch.DataSource = searchResultDT;
        gridSearch.DataBind();

        Session["PaginationDataSource"] = searchResultDT;
    }

    public DataTable SearchRecords(string requisitionNo, DateTime fromDate, DateTime toDate)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string sql = "SELECT * FROM Requisition1891 WHERE 1=1";

            if (!string.IsNullOrEmpty(requisitionNo))
            {
                sql += " AND ReqNo = @ReqNo";
            }

            if (fromDate != null)
            {
                sql += " AND ReqDte >= @FromDate";
            }

            if (toDate != null)
            {
                sql += " AND ReqDte <= @ToDate";
            }

            sql += " ORDER BY RefNo DESC";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                if (!string.IsNullOrEmpty(requisitionNo))
                {
                    command.Parameters.AddWithValue("@ReqNo", requisitionNo);
                }

                if (fromDate != null)
                {
                    command.Parameters.AddWithValue("@FromDate", fromDate);
                }

                if (toDate != null)
                {
                    command.Parameters.AddWithValue("@ToDate", toDate);
                }

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }



    //=========================={ Update - Fill Bill & Item Details }==========================
    protected void gridSearch_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "lnkView")
        {
            int rowId = Convert.ToInt32(e.CommandArgument);
            Session["BillReferenceNo"] = rowId;

            searchGridDiv.Visible = false;
            divTopSearch.Visible = false;
            UpdateDiv.Visible = true;

            FillBillDetails(rowId);

            FillItemDetails(rowId);

            // binding doc gridview
            FillBillDocUpload(rowId);
        }
    }

    private void FillBillDetails(int billRefNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select* from Requisition1891 where RefNo = @RefNo";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefNo", billRefNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();


            txtReqNo.Text = dt.Rows[0]["ReqNo"].ToString();

            DateTime reqDate = DateTime.Parse(dt.Rows[0]["ReqDte"].ToString());
            dtReqDate.Text = reqDate.ToString("yyyy-MM-dd");
        }
    }

    private void FillItemDetails(int billRefNo)
    {
        itemDiv.Visible = true;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from Requisition2891 where RefNo = @RefNo";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefNo", billRefNo.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            itemGrid.DataSource = dt;
            itemGrid.DataBind();

            ViewState["BillDetailsVS"] = dt;
            Session["BillDetails"] = dt;
        }
    }

    private void FillBillDocUpload(int billRefNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "select * from BillDocUpload891 where BillRefNo = @BillRefNo";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@BillRefNo", billRefNo.ToString());
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            docGrid.Visible = true;

            GridDocument.DataSource = dt;
            GridDocument.DataBind();

            ViewState["DocDetailsDataTable"] = dt;
            Session["DocUploadDT"] = dt;
        }
    }



    //=========================={ Item Save Button Click Event }==========================
    protected void btnItemInsert_Click(object sender, EventArgs e)
    {
        // inserting bill details
        insertBillDetails();
    }

    private void insertBillDetails()
    {
        string BillReferenceNo = Session["BillReferenceNo"].ToString();

        string nameCellLine = CellLineName.Text.ToString();
        string quantity = Quantity.Text.ToString();
        string commentsIfAny = CommentsIfAny.Text.ToString() ?? "";

        DataTable dt = ViewState["BillDetailsVS"] as DataTable ?? createBillDatatable();

        AddRowToBillDetailsDataTable(dt, BillReferenceNo, nameCellLine, quantity, commentsIfAny);

        ViewState["BillDetailsVS"] = dt;
        Session["BillDetails"] = dt;

        if (dt.Rows.Count > 0)
        {
            itemDiv.Visible = true;

            itemGrid.DataSource = dt;
            itemGrid.DataBind();
        }
    }

    private DataTable createBillDatatable()
    {
        DataTable dt = new DataTable();

        // reference no
        DataColumn BillRefNo = new DataColumn("BillRefNo", typeof(string));
        dt.Columns.Add(BillRefNo);

        // cell name
        DataColumn NmeCell = new DataColumn("NmeCell", typeof(string));
        dt.Columns.Add(NmeCell);

        // quantity
        DataColumn Quty = new DataColumn("Quty", typeof(string));
        dt.Columns.Add(Quty);

        // comments
        DataColumn Comment = new DataColumn("Comment", typeof(string));
        dt.Columns.Add(Comment);

        return dt;
    }

    private void AddRowToBillDetailsDataTable(DataTable dt, string BillReferenceNo, string nameCellLine, string quantity, string commentsIfAny)
    {
        // Create a new row
        DataRow row = dt.NewRow();

        // Set values for the new row
        row["BillRefNo"] = BillReferenceNo;
        row["NmeCell"] = nameCellLine;
        row["Quty"] = quantity;
        row["Comment"] = commentsIfAny;

        // Add the new row to the DataTable
        dt.Rows.Add(row);
    }




    //----------============={ Upload New Documents }=============----------
    protected void btnDocUpload_Click(object sender, EventArgs e)
    {
        // setting the file size in web.config file (web.config should not be read only)
        //settingHttpRuntimeForFileSize();

        if (fileDoc.HasFile)
        {
            string FileExtension = System.IO.Path.GetExtension(fileDoc.FileName);

            if (FileExtension == ".xlsx" || FileExtension == ".xls")
            {

            }

            // file name
            string onlyFileNameWithExtn = fileDoc.FileName.ToString();

            // getting unique file name
            string strFileName = GenerateUniqueId(onlyFileNameWithExtn);

            // saving and getting file path
            string filePath = getServerFilePath(strFileName);

            // Retrieve DataTable from ViewState or create a new one
            DataTable dt = ViewState["DocDetailsDataTable"] as DataTable ?? CreateDocDetailsDataTable();

            // filling document details datatable
            AddRowToDocDetailsDataTable(dt, onlyFileNameWithExtn, filePath);

            // Save DataTable to ViewState
            ViewState["DocDetailsDataTable"] = dt;
            Session["DocUploadDT"] = dt;

            if (dt.Rows.Count > 0)
            {
                docGrid.Visible = true;

                // binding document details gridview
                GridDocument.DataSource = dt;
                GridDocument.DataBind();
            }
        }
    }

    private string GenerateUniqueId(string strFileName)
    {
        long timestamp = DateTime.Now.Ticks;
        //string guid = Guid.NewGuid().ToString("N"); //N to remove hypen "-" from GUIDs
        string guid = Guid.NewGuid().ToString();
        string uniqueID = timestamp + "_" + guid + "_" + strFileName;
        return uniqueID;
    }

    private string getServerFilePath(string strFileName)
    {
        string orgFilePath = Server.MapPath("~/Portal/Public/" + strFileName);

        // saving file
        fileDoc.SaveAs(orgFilePath);

        //string filePath = Server.MapPath("~/Portal/Public/" + strFileName);
        //file:///C:/HostingSpaces/PAWAN/cdsmis.in/wwwroot/Pms2/Portal/Public/638399011215544557_926f9320-275e-49ad-8f59-32ecb304a9f1_EMB%20Recording.pdf

        // replacing server-specific path with the desired URL
        string baseUrl = "http://101.53.144.92/nccs/Ginie/External?url=..";
        string relativePath = orgFilePath.Replace(Server.MapPath("~/Portal/Public/"), "Portal/Public/");

        // Full URL for the hyperlink
        string fullUrl = $"{baseUrl}/{relativePath}";

        return fullUrl;
    }

    private DataTable CreateDocDetailsDataTable()
    {
        DataTable dt = new DataTable();

        // file name
        DataColumn DocName = new DataColumn("DocName", typeof(string));
        dt.Columns.Add(DocName);

        // Doc uploaded path
        DataColumn DocPath = new DataColumn("DocPath", typeof(string));
        dt.Columns.Add(DocPath);

        return dt;
    }

    private void AddRowToDocDetailsDataTable(DataTable dt, string onlyFileNameWithExtn, string filePath)
    {
        // Create a new row
        DataRow row = dt.NewRow();

        // Set values for the new row
        row["DocName"] = onlyFileNameWithExtn;
        row["DocPath"] = filePath;

        // Add the new row to the DataTable
        dt.Rows.Add(row);
    }



    //=========================={ Submit Button Click Event }==========================
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("BillEntry.aspx");
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (GridDocument.Rows.Count > 0)
        {
            string billReferenceNo = Session["BillReferenceNo"].ToString();

            // updating bill head
            //UpdateBillHeader();

            // updating item details
            UpdateBillItemDetails(billReferenceNo);

            // updating bill doc uploads

            //UpdateBillDocDetails();

            //Response.Redirect("BillUpdate.aspx");
            //btnSubmit.Enabled = false;


            //Response.Redirect("BillEntry.aspx");

            //getSweetAlertSuccessRedirectMandatory("Saved!", "Saved successfully", "BillEntry.aspx");
        }
        else
        {
            getSweetAlertErrorMandatory("Error!", "Update failed, please try again");
        }
    }




    private void UpdateBillHeader()
    {
        string billRefno = Session["BillReferenceNo"].ToString(); 

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = $@"UPDATE Requisition1891 SET BillAmt = @BillAmt WHERE RefNo = @RefNo";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefNo", billRefno);
            cmd.ExecuteNonQuery();

            //SqlDataAdapter ad = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //ad.Fill(dt);

            con.Close();
        }
    }




    private void UpdateBillItemDetails(string billReferenceNo)
    {
        string billRefno = billReferenceNo;

        DataTable billItemsDT = (DataTable)Session["BillDetails"]; // requisition 2 details

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            int k = 1;

            foreach (GridViewRow row in itemGrid.Rows)
            {
                int rowIndex = row.RowIndex;

                // cell items
                string cellName = billItemsDT.Rows[rowIndex]["NmeCell"].ToString();
                string qty = billItemsDT.Rows[rowIndex]["Quty"].ToString();
                string comments = billItemsDT.Rows[rowIndex]["Comment"].ToString();

                // item ref no to update
                string itemRefNo = billItemsDT.Rows[rowIndex]["RefNo"].ToString();

                if(k == 2)
                {
                    alert($"itemRefNo : {itemRefNo}");
                }

                // checking for existing items
                //bool isItemExists = IsItemExists(itemRefID);

                //if (isItemExists)
                //if (itemRefNo != "") // update
                if (!string.IsNullOrEmpty(itemRefNo)) // update


                {
                    string sql = $@"UPDATE Requisition2891 SET 
                                    NmeCell=@NmeCell, Quty=@Quty, Comment=@Comment 
                                    WHERE RefNo=@RefNo";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@NmeCell", cellName);
                    cmd.Parameters.AddWithValue("@Quty", qty);
                    cmd.Parameters.AddWithValue("@Comment", comments);
                    cmd.Parameters.AddWithValue("@RefNo", itemRefNo);
                    cmd.ExecuteNonQuery();
                }
                else // insert
                {
                    // getting new ref id for item
                    string itemRefNoNew = GetItemRefNo().ToString();

                    string sql = $@"INSERT INTO Requisition2891 
                                    (RefNo, BillRefNo, NmeCell, Quty, Comment) 
                                    VALUES 
                                    (@RefNo, @BillRefNo, @NmeCell, @Quty, @Comment)";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@RefNo", itemRefNoNew);
                    cmd.Parameters.AddWithValue("@BillRefNo", billRefno);
                    cmd.Parameters.AddWithValue("@NmeCell", cellName);
                    cmd.Parameters.AddWithValue("@Quty", qty);
                    cmd.Parameters.AddWithValue("@Comment", comments);
                    cmd.ExecuteNonQuery();
                }
            }

            k++;

            con.Close();
        }

    }

    private int GetItemRefNo()
    {
        string nextRefID = "1000001";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT ISNULL(MAX(CAST(RefNo AS INT)), 10000) + 1 AS NextRefID FROM Requisition2891";
            SqlCommand cmd = new SqlCommand(sql, con);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value) { nextRefID = result.ToString(); }
            return Convert.ToInt32(nextRefID);
        }
    }




    private void UpdateBillDocDetails()
    {
        string billRefno = Session["BillReferenceNo"].ToString();

        DataTable documentsDT = (DataTable)Session["DocUploadDT"];

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            foreach (GridViewRow row in GridDocument.Rows)
            {
                int rowIndex = row.RowIndex;

                string docName = documentsDT.Rows[rowIndex]["DocName"].ToString();

                HyperLink hypDocPath = (HyperLink)row.FindControl("DocPath");
                string docPath = hypDocPath.NavigateUrl;

                string docRefid = documentsDT.Rows[rowIndex]["RefID"].ToString();

                bool isDocExist = checkForDocuUploadedExist(docRefid);

                if (isDocExist)
                {

                }
                else
                {
                    int docRefNo = getDocRefINo(); // new doc RefID

                    string sql = $@"INSERT INTO BillDocUpload891 
                                    (RefNo, BillRefNo, DocName, DocPath) 
                                    values 
                                    (@RefNo, @BillRefNo, @DocName, @DocPath)";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@RefNo", docRefNo);
                    cmd.Parameters.AddWithValue("@BillRefNo", billRefno);
                    cmd.Parameters.AddWithValue("@DocName", docName);
                    cmd.Parameters.AddWithValue("@DocPath", docPath);
                    cmd.ExecuteNonQuery();
                }
            }

            con.Close();
        }
    }

    private bool checkForDocuUploadedExist(string docRefNo)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT * FROM BillDocUpload891 WHERE RefNo=@RefNo";

            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefNo", docRefNo);
            cmd.ExecuteNonQuery();

            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            con.Close();

            if (dt.Rows.Count > 0) return true;
            else return false;
        }
    }

    private int getDocRefINo()
    {
        string nextRefID = "1000001";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT ISNULL(MAX(CAST(RefNo AS INT)), 10000) + 1 AS NextRefID FROM BillDocUpload891";
            SqlCommand cmd = new SqlCommand(sql, con);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value) { nextRefID = result.ToString(); }
            return Convert.ToInt32(nextRefID);
        }
    }

}