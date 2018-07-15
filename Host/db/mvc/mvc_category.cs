using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using System.IO;
using System.Data;
using System.Xml;
using Newtonsoft.Json;
using System.Collections;
using System.Xml.Linq;
using System.Reflection;

using model;

namespace host
{
    public class nancy_mvc_category : NancyModule
    {
        private static List<mvc_category> ls_cat = new List<mvc_category>() { };

        #region // db file ...

        public static void load_user()
        {
            ls_cat = db_mvc_category.load();
        }

        public static class db_mvc_category
        {
            public static string path = hostServer.pathRoot + @"db_category\", file_name = "category";

            public static void update(List<mvc_category> list)
            {

                Task.Factory.StartNew(() =>
                {
                    hostFile.write_file_MMF<mvc_category>(list, path, file_name);
                });
            }

            public static List<mvc_category> load()
            {
                string path_file = path + file_name + ".mmf";
                return hostFile.read_file_MMF<mvc_category>(path_file).ToList();
            }
        }
        
        #endregion

        public nancy_mvc_category()
            : base("mvc/category")
        {
            Get["/all"] = para =>
            {
                string json = JsonConvert.SerializeObject(ls_cat);
                var o = (Response)json;
                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = "application/json";

                return o;
            };

            Get["/query/{cat_id}"] = para =>
            {
                string cat_id = this.Request.Form.cat_id;

                string json = "Không tìm thấy";

                int pos = ls_cat.FindIndex(o => o.cat_id == cat_id);
                if (pos != -1) {
                    json = JsonConvert.SerializeObject(ls_cat[pos]);
                }
                                
                var res = (Response)json;
                res.StatusCode = HttpStatusCode.OK;
                res.ContentType = "application/json";
                return res;
            };


            Get["/query/{cat_id}/sub"] = para =>
            {
                string cat_id = this.Request.Form.cat_id;

                string json = "Không tìm thấy";

                var ls = ls_cat.Where(o => o.parent_id == cat_id).ToList();
                if (ls.Count > 0)
                {
                    json = JsonConvert.SerializeObject(ls);
                }

                var res = (Response)json;
                res.StatusCode = HttpStatusCode.OK;
                res.ContentType = "application/json";
                return res;
            };

            Post["/add"] = para =>
            {
                string parent_id = this.Request.Form.parent_id;
                string cat_id = Guid.NewGuid().ToString();

                int level = this.Request.Form.level;

                string tag = this.Request.Form.tag;
                string name = this.Request.Form.name;
                name = name.Trim();

                string note = this.Request.Form.note; 
                
                string json = "Dữ liệu đã tồn tại";                
                int index = ls_cat.FindIndex(o => o.name.ToLower() == name.ToLower() && o.level == level);
                if (index == -1)
                {
                    mvc_category item = new mvc_category()
                    {
                        cat_id = Guid.NewGuid().ToString(),
                        parent_id = parent_id,
                        level = level,
                        name = name,
                        note = note,
                        status = false,
                        tag = tag,
                        total_sub = 0
                    };
                    ls_cat.Add(item);

                    int pos_parent = ls_cat.FindIndex(x => x.cat_id == parent_id);
                    mvc_category parent = ls_cat[pos_parent];
                    parent.total_sub = parent.total_sub + 1;
                    ls_cat[pos_parent] = parent;

                    db_mvc_category.update(ls_cat);

                    json = JsonConvert.SerializeObject(ls_cat);
                }                

                var res = (Response)json;
                res.StatusCode = HttpStatusCode.OK;
                res.ContentType = "application/json";
                return res;
            };

            Post["/edit"] = para =>
            {
                string parent_id = this.Request.Form.parent_id;
                string cat_id = this.Request.Form.cat_id;
                 
                int level = this.Request.Form.level;

                string tag = this.Request.Form.tag;
                string name = this.Request.Form.name;
                name = name.Trim();

                string note = this.Request.Form.note; 

                string json = "Dữ liệu không tồn tại";
                int index = ls_cat.FindIndex(o => o.cat_id == cat_id);
                if (index != -1)
                {
                    var item = ls_cat[index];

                    item.name = name;
                    item.note = note;
                    item.tag = tag;

                    ls_cat[index] = item;

                    db_mvc_category.update(ls_cat);

                    json = JsonConvert.SerializeObject(ls_cat);
                }

                var res = (Response)json;
                res.StatusCode = HttpStatusCode.OK;
                res.ContentType = "application/json";
                return res;
            };

            Post["/active"] = para =>
            {
                string cat_id = this.Request.Form.cat_id;
                string status_ = this.Request.Form.status;

                string json = "Dữ liệu không tồn tại";

                int pos = ls_cat.FindIndex(x => x.cat_id == cat_id);
                if(pos != -1)
                {
                    var item = ls_cat[pos];
                    item.status = true;
                    json = "actived";

                    if (status_ == "0")
                    {
                        item.status = false;
                        json = "disabled";
                    }

                    ls_cat[pos] = item;

                    db_mvc_category.update(ls_cat);
                }

                var o = (Response)json;
                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = "application/json";

                return o;
            };
              
        }
    }
}
