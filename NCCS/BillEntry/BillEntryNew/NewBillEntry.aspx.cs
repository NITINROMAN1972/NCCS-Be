using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BillEntry_BillEntry : System.Web.UI.Page
{
    string connectionString = ConfigurationManager.ConnectionStrings["Ginie"].ConnectionString;

    protected void Page_Load(object sender, EventArgs e)
    {
        // NCCS - 891

        if (!IsPostBack)
        {
            int refNo = GetBillReferenceNumber();
            ReqNo.Text = refNo.ToString();
            Session["BillReferenceNumber"] = refNo.ToString();

            ReqDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }

    private int GetBillReferenceNumber()
    {
        string nextRefID = "1000001";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT ISNULL(MAX(CAST(RefNo AS INT)), 1000000) + 1 AS NextRefID FROM Requisition1891";
            SqlCommand cmd = new SqlCommand(sql, con);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value) { nextRefID = result.ToString(); }
            return Convert.ToInt32(nextRefID);
        }
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




    //----------============={ Item Add Button Event }=============----------

    protected void btnItemInsert_Click(object sender, EventArgs e)
    {
        // inserting bill
        InsertBillEntry();
    }

    private void InsertBillEntry()
    {
        string BillReferenceNo = Session["BillReferenceNumber"].ToString();

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
        DataColumn RefNo = new DataColumn("BillRefNo", typeof(string));
        dt.Columns.Add(RefNo);

        // cell name
        DataColumn Item = new DataColumn("NmeCell", typeof(string));
        dt.Columns.Add(Item);

        // quantity
        DataColumn UOM = new DataColumn("Quty", typeof(string));
        dt.Columns.Add(UOM);

        // comments
        DataColumn Comment = new DataColumn("Comment", typeof(string));
        dt.Columns.Add(UOM);

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





    //----------============={ Upload Documents }=============----------

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





    //----------============={ Submit Button Event }=============----------

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("../BillEntry.aspx");
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (GridDocument.Rows.Count > 0)
        {
            // inserting bill head
            InsertBillHeader();

            // inserting item details from grid
            InsertBillDetails();

            // inserting documents
            InsertBillDocument();

            btnSubmit.Enabled = false;
            //Response.Redirect("../BillEntry.aspx");

            getSweetAlertSuccessRedirectMandatory("Saved", "Bill saved successfully!", "../BillEntry.aspx");
        }
        else
        {
            
        }
    }

    private void InsertBillHeader()
    {
        string billRefNo = ReqNo.Text.ToString();
        DateTime requisitionDate = DateTime.Parse(ReqDate.Text);

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = $@"INSERT INTO Requisition1891 (RefNo, ReqNo, ReqDte) 
                            VALUES (@RefNo, @ReqNo, @ReqDte)";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@RefNo", billRefNo);
            cmd.Parameters.AddWithValue("@ReqNo", billRefNo);
            cmd.Parameters.AddWithValue("@ReqDte", requisitionDate);
            cmd.ExecuteNonQuery();

            //SqlDataAdapter ad = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //ad.Fill(dt);

            con.Close();
        }
    }

    private void InsertBillDetails()
    {
        DataTable billItemsDT = (DataTable)Session["BillDetails"];

        string billRefNo = ReqNo.Text.ToString();

        string cellRefNo = GetItemRefNo().ToString();

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            foreach (GridViewRow row in itemGrid.Rows)
            {
                int rowIndex = row.RowIndex;

                // cell items
                string cellName = billItemsDT.Rows[rowIndex]["NmeCell"].ToString();
                string qty = billItemsDT.Rows[rowIndex]["Quty"].ToString();
                string comments = billItemsDT.Rows[rowIndex]["Comment"].ToString();

                // inserting bill item details
                string sql = $@"INSERT INTO Requisition2891 (RefNo, BillRefNo, NmeCell, Quty, Comment) 
                                VALUES (@RefNo, @BillRefNo, @NmeCell, @Quty, @Comment)";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@RefNo", billRefNo);
                cmd.Parameters.AddWithValue("@BillRefNo", cellRefNo);
                cmd.Parameters.AddWithValue("@NmeCell", cellName);
                cmd.Parameters.AddWithValue("@Quty", qty);
                cmd.Parameters.AddWithValue("@Comment", comments);
                cmd.ExecuteNonQuery();
            }

            con.Close();
        }
    }

    private int GetItemRefNo()
    {
        string nextRefID = "1000001";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT ISNULL(MAX(CAST(RefNo AS INT)), 1000000) + 1 AS NextRefID FROM Requisition1891";
            SqlCommand cmd = new SqlCommand(sql, con);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value) { nextRefID = result.ToString(); }
            return Convert.ToInt32(nextRefID);
        }
    }




    private void InsertBillDocument()
    {
        string billRefNo = ReqNo.Text.ToString();

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

                // getting new doc ref id
                int docRefNo = GetDocumentRefNo();

                bool isDocExist = checkForDocuUploadedExist(docRefNo.ToString());

                if (isDocExist)
                {
                    //string sql = $@"UPDATE DocUpload874 SET DocName=@DocName, DocPath=@DocPath WHERE RefID=@RefID";

                    //SqlCommand cmd = new SqlCommand(sql, con);
                    //cmd.Parameters.AddWithValue("@DocName", docName);
                    //cmd.Parameters.AddWithValue("@DocPath", docPath);
                    //cmd.Parameters.AddWithValue("@RefID", );
                    //cmd.ExecuteNonQuery();
                }
                else
                {
                    string sql = $@"INSERT INTO BillDocUpload891
                                    (RefNo, BillRefNo, DocName, DocPath) 
                                    values (@RefNo, @BillRefNo, @DocName, @DocPath)";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@RefNo", docRefNo);
                    cmd.Parameters.AddWithValue("@BillRefNo", billRefNo);
                    cmd.Parameters.AddWithValue("@DocName", docName);
                    cmd.Parameters.AddWithValue("@DocPath", docPath);
                    cmd.ExecuteNonQuery();
                }
            }

            con.Close();
        }
    }

    private int GetDocumentRefNo()
    {
        string nextRefID = "1000001";

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();
            string sql = "SELECT ISNULL(MAX(CAST(RefNo AS INT)), 1000000) + 1 AS NextRefID FROM BillDocUpload891";
            SqlCommand cmd = new SqlCommand(sql, con);

            object result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value) { nextRefID = result.ToString(); }
            return Convert.ToInt32(nextRefID);
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


}