<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Temp.aspx.cs" Inherits="Temp_Temp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">


        <div class="card no-b  no-r">
            <div class="card-body">
                <asp:GridView ShowHeaderWhenEmpty="true" ID="GridDyanmic" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover text-center">
                    <HeaderStyle CssClass="kt-shape-bg-color-3" />
                    <Columns>
                        <asp:TemplateField ControlStyle-CssClass="col-xs-1" HeaderText="Sr.No">
                            <ItemTemplate>
                                <asp:HiddenField ID="id" runat="server" Value="id" />
                                <span>
                                    <%#Container.DataItemIndex + 1%>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>


    </form>
</body>
</html>
