using Microsoft.CSharp;
using model;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace host
{
    public partial class fReduce_Edit : Form
    {
        public fReduce_Edit()
        {
            InitializeComponent();
        }

        public static string id = "";

        public fReduce_Edit(string id_)
        {
            InitializeComponent();
            id = id_;
        }


        public Tuple<MethodInfo, string> f_CreateFunction(string src)
        {
            try
            {
                string class_name = "cls_" + Guid.NewGuid().ToString().Replace("-", "");
                string fun_name = "f_" + Guid.NewGuid().ToString().Replace("-", "");


                string code = @"
using System;
using System.Collections.Generic;
            
                namespace UserFunctions
                {                
                    public class " + class_name + @"
                    {                
                        public static Func<KeyValuePair<Tuple<long, int, int>, List<double>>, bool> " + fun_name + @"()
                        {
                             
" + src + @"                   
         
                            return where;
                        }
                    }
                }
            ";

                //CSharpCodeProvider provider = new CSharpCodeProvider();
                //CompilerResults results = provider.CompileAssemblyFromSource(new CompilerParameters(), code);

                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp",
                    new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
                CompilerParameters parameters = new CompilerParameters();

                // Reference  
                //parameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
                //parameters.ReferencedAssemblies.Add("mscorlib.dll");
                //parameters.ReferencedAssemblies.Add("System.dll");
                //parameters.ReferencedAssemblies.Add("System.Core.dll");
                //parameters.ReferencedAssemblies.Add("System.Xml.dll");
                //parameters.ReferencedAssemblies.Add("System.IO.dll");
                //parameters.ReferencedAssemblies.Add("System.Net.dll");
                //parameters.ReferencedAssemblies.Add("System.Web.dll");
                //parameters.ReferencedAssemblies.Add("System.Data.dll");
                //parameters.ReferencedAssemblies.Add("System.Web.Extensions.dll");

                parameters.ReferencedAssemblies.Add("System.dll"); // System, System.Net, etc namespaces
                //parameters.ReferencedAssemblies.Add("System.Data.dll"); // System.Data namespace
                //parameters.ReferencedAssemblies.Add("System.Data.SQLite.dll"); // System.Data.SqlLite namespace
                //parameters.ReferencedAssemblies.Add("System.Xml.dll"); // System.Xml namespace
                //parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll"); // System.Windows.Forms namespace

                // True - memory generation, false - external file generation
                parameters.GenerateInMemory = true;
                // True - exe file generation, false - dll file generation
                parameters.GenerateExecutable = false;

                CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

                if (results.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (CompilerError error in results.Errors)
                    {
                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    }

                    string errors = sb.ToString();

                    return new Tuple<MethodInfo, string>(null, errors);
                    //throw new InvalidOperationException(sb.ToString());
                }

                Type binaryFunction = results.CompiledAssembly.GetType("UserFunctions." + class_name);
                var mf = binaryFunction.GetMethod(fun_name);

                return new Tuple<MethodInfo, string>(mf, null);
            }
            catch (Exception ex)
            {
                return new Tuple<MethodInfo, string>(null, ex.Message);
            }
        }


        private void bAdd_Click(object sender, EventArgs e)
        {
            string src = txt_src.Text.Trim();
            string code = txt_code.Text.Trim();
            string name = txt_name.Text.Trim();

            if (src == "")
            {
                MessageBox.Show("Nhập mã nguồn thủ tục");
                return;
            }

            if (code == "")
            {
                MessageBox.Show("Nhập mã truy vấn thủ tục");
                return;
            }

            if (name == "")
            {
                MessageBox.Show("Nhập tên thủ tục");
                return;
            }
            
            //var fm = f_CreateFunction(src);
            //string msg = fm.Item2;
            //if (string.IsNullOrEmpty(msg))
            //{
                //MethodInfo function = fm.Item1;

                //var betterFunction = (Func<Func<KeyValuePair<Tuple<long, int, int>, List<double>>, bool>>)
                //    Delegate.CreateDelegate(typeof(Func<Func<KeyValuePair<Tuple<long, int, int>, List<double>>, bool>>), function);

                //var where = betterFunction();

                m_reduce  item = new m_reduce();
                item.api = code;
                item.name = name; 
                //item.where = where;

                var rs = db_reduce.edit_Item(item);
                if (rs)
                {
                    main.main_redure_reload();
                    main.show_notification("Thêm mới thủ tục thành công", 3000);
                    this.Close();
                }
                else {
                    main.show_notification("Thêm mới thủ tục không thành công. Kiểm tra lại hệ thống.", 3000);
                    return;
                }
            //}
            //else
            //{
            //    MessageBox.Show(msg);
            //}
        }

        private void fReduce_Add_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.ShowInTaskbar = false;

            var item = db_reduce.get_ItemByID(id);
            if (item.api != null)
            {
                txt_code.Text = item.api;
                txt_name.Text = item.name; 
            }
            else {
                main.show_notification("Không tìm thấy thủ tục", 3000);
                this.Close();
            }
        }
    }
}
