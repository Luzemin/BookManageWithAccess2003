using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Book.Models;
using System.Data;
using System.Web.Script.Serialization;

namespace Book
{
    public partial class AJAX : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string op = Request.QueryString["op"];
            string id = Request.QueryString["id"];

            //保存/编辑
            if (op == "edit")
            {
                try
                {
                    string sql = string.Empty;
                    BookModel book = new BookModel();
                    book.Name = Request.Form["name"];
                    book.Author = Request.Form["author"];
                    book.Code = Request.Form["code"];
                    book.Press = Request.Form["press"];
                    book.Price = Convert.ToDecimal(Request.Form["price"]);
                    book.Count = Convert.ToInt32(Request.Form["count"]);

                    if (id != null)//编辑
                    {
                        book.ID = Convert.ToInt32(id);
                        sql = string.Format(@"update [book] set [code]='{0}',
                                                                                          [name]='{1}',
                                                                                          [author]='{2}',
                                                                                          [press]='{3}',
                                                                                          [price]={4},
                                                                                          [count]={5} where id={6}",
                                                                                          book.Code,
                                                                                          book.Name,
                                                                                          book.Author,
                                                                                          book.Press,
                                                                                          book.Price,
                                                                                          book.Count,
                                                                                          book.ID);
                    }
                    else
                    {
                        book.CreateTime = DateTime.Now;
                        sql = string.Format(@"insert into [book] ([code],[name],[author],[press],[price],[count],[createtime]) 
                                                                        values ('{0}','{1}','{2}','{3}',{4},{5},#{6}#)",
                                                      book.Code, book.Name, book.Author, book.Press, book.Price, book.Count, book.CreateTime);
                    }

                    string str = string.Format(WebConfigurationManager.ConnectionStrings["Access"].ConnectionString, Server.MapPath("/App_Data/book.mdb"));
                    OleDbConnection conn = new OleDbConnection(str);
                    conn.Open();
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    Response.Write(cmd.ExecuteNonQuery());
                }
                catch
                {
                    Response.Write(0);
                }
            }
            //查看 
            else if (op == "list")
            {
                try
                {
                    string str = string.Format(WebConfigurationManager.ConnectionStrings["Access"].ConnectionString, Server.MapPath("/App_Data/book.mdb"));
                    OleDbConnection conn = new OleDbConnection(str);
                    conn.Open();
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select * from [book] order by createtime desc";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    DataSet set = new DataSet();
                    adapter.Fill(set);

                    if (set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0)
                    {
                        Response.Write(Serialize(set.Tables[0]));
                    }
                    else
                    {
                        Response.Write("");
                    }
                }
                catch
                {
                    Response.Write("");
                }
            }
            else if (op == "one")
            {
                try
                {
                    string str = string.Format(WebConfigurationManager.ConnectionStrings["Access"].ConnectionString, Server.MapPath("/App_Data/book.mdb"));
                    OleDbConnection conn = new OleDbConnection(str);
                    conn.Open();
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "select * from [book] where id=" + id;
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    DataSet set = new DataSet();
                    adapter.Fill(set);

                    if (set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0)
                    {
                        Response.Write(Serialize(set.Tables[0]));
                    }
                    else
                    {
                        Response.Write("");
                    }
                }
                catch
                {
                    Response.Write("");
                }
            }
            //删除
            else if (op == "del")
            {
                try
                {
                    string str = string.Format(WebConfigurationManager.ConnectionStrings["Access"].ConnectionString, Server.MapPath("/App_Data/book.mdb"));
                    OleDbConnection conn = new OleDbConnection(str);
                    conn.Open();
                    OleDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "delete from [book] where id in (" + Request.Form["ids"] + ")";
                    Response.Write(cmd.ExecuteNonQuery());
                }
                catch
                {
                    Response.Write(0);
                }
            }
        }
        public string Serialize(DataTable dt)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                list.Add(result);
            }
            return serializer.Serialize(list); ;
        }
    }
}