
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Nancy;
using Nancy.ModelBinding;
using System.IO;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;
using Nancy.Owin;
using System.Linq;
using Nancy.Conventions;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Net;

//using sysSearchLib;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using host;
using model;
using System.Linq.Mustache;

namespace host
{
    public static class export_header
    {
        public static string get_header(string key)
        {
            switch (key)
            {
                case "stt": return "STT";
                case "madiemdo": return "Mã điểm đo";
                case "tendiemdo": return "Tên điểm đo";
                case "code": return "Code";
                case "sodiemdocolandulieu": return "Số điểm đo có lần dữ liệu";
                case "hsnhan": return "Hệ số nhân";
                case "name": return "Name";
                case "sodiemdodalapdat": return "Số điểm đo đã lắp đặt";
                case "tyle": return "Đơn vị";
                case "comportserver": return "Comport Server";
                case "timestart": return "Time Start";
                case "timeend": return "Time End";
                case "typepha": return "Loại pha";
                case "pgiao": return "P Giao";
                case "pnhan": return "P Nhận";
                case "qtonggiao": return "Q Tổng giao";
                case "qtongnhan": return "Q Tổng Nhận";
                case "rowscount": return "Rows Count";
                case "rnum": return "R Num";
                case "time": return "Time";
                case "ua": return "U A";
                case "ub": return "U B";
                case "uc": return "U C";
                case "ia": return "I A";
                case "ib": return "I B";
                case "ic": return "I C";
                case "cosa": return "Cos A";
                case "cosb": return "Cos A";
                case "cosc": return "Cos A";
                case "pa": return "P A";
                case "pb": return "P B";
                case "pc": return "P C";
                case "qa": return "Q A";
                case "qb": return "Q B";
                case "qc": return "Q C";
                case "timechange": return "Time Change";
                case "voltageimbalance": return "VoltageImbalance";
                case "proglog": return "Prolog";
                case "phasefail": return "Phase Fail";
                case "passchange": return "Pass Change";
                case "reverserun": return "Reverser Run";
                case "powerfail": return "Power Fail";
                case "tong": return "Tổng";
                case "id": return "ID";
                case "imei": return "IMEI";
                case "socongto": return "Số công tơ";
                case "congtydienluc": return "Công ty điện lực";
                case "dienluc": return "Điện lực";
                case "loaidiemdo": return "Loại điểm đo";
                case "ghichu": return "Ghi chú";
                case "note": return "Note";
                case "ikhaibao": return "I Khai báo";
                case "loidinhdang": return "Lỗi định dạng";
                //case "name": return "Name";
                case "tongdiemdolapdoxa": return "Tổng điểm đo lắp đo xa";
                case "tongdiemdothucte": return "Tổng điểm đo thực tế";
                case "sokhnhohon5p_thang0": return "Số KH nhỏ hơn 5P tháng 0";
                case "sokhlonhon5p_thang0": return "Số KH lớn hơn 5P tháng 0";
                case "sophutlonhon5p_thang0": return "Số phút lớn hơn 5P tháng 0";
                case "sokhnhohon5p_thang1": return "Số KH nhỏ hơn 5P tháng 1";
                case "sokhlonhon5p_thang1": return "Số kh lớn hơn 5P tháng 1";
                case "sophutlonhon5p_thang1": return "Số phút lớn hơn 5P tháng 1";
                case "sokhnhohon5p_thang2": return "Số KH nhỏ hơn 5P tháng 2";
                case "sokhlonhon5p_thang2": return "Số KH lớn hơn %P tháng 2";
                case "sophutlonhon5p_thang2": return "Số phút lớn hơn 5P tháng 2";
                case "sokhnhohon5p_thang3": return "Số KH nhỏ hơn 5P tháng 3";
                case "sokhlonhon5p_thang3": return "Số KH lớn hơn %P tháng 3";
                case "sophutlonhon5p_thang3": return "Số phút lớn hơn 5P tháng 3";
                case "sokhnhohon5p_thang4": return "Số KH nhỏ hơn 5P tháng 4";
                case "sokhlonhon5p_thang4": return "Số KH lớn hơn 5P tháng 4";
                case "sophutlonhon5p_thang4": return "Số phút lớn hơn 5P tháng 4";
                //case "hsnhan": return "Hệ số nhân";
                case "billingdatestart": return "Billing Date Start";
                case "billingdateend": return "Billing Date End";
                case "billingresetnumber": return "Billing Reset Number";
                case "tpgaio1": return "TP Giao 1";
                case "tpgiao2": return "TP Giao 2";
                case "tpgiao3": return "TP Giao 3";
                case "tpnhan1": return "TP Nhận 1";
                case "tpnhan2": return "TP Nhận 2";
                case "tpnhan3": return "TP Nhận 3";
                case "tpgiaotong": return "TP Giao tổng";
                case "tpnhantong": return "TP Nhận tổng";
                case "tcd2": return "TCD 2";
                case "tcd1": return "TCD 1";
                case "mds1": return "MDS 1";
                case "mds2": return "MDS 2";
                case "mds1_time": return "MDS 1 Time";
                case "mds2_time": return "MDS 2 Time";
                case "msd3": return "MDS 3";
                case "msd3_time": return "MDS 3 Time";
                case "stong": return "S Tổng";
                //case "socongto": return "Số công tơ";
                case "chungloai": return "Chủng loại";
                case "type_pha": return "Type pha";
                case "loaiheso": return "Loại hệ số";
                case "heso_ti": return "Hệ số TI";
                case "heso_tu": return "Hệ số TU";
                case "tu": return "TU";
                case "ti": return "TI";
                case "pgiao1": return "P Giao 1";
                case "pgiao2": return "P Giao 2";
                case "pgiao3": return "P Giao 3";
                case "pnhan1": return "P Nhận 1";
                case "pnhan2": return "P Nhận 2";
                case "pnhan3": return "P Nhận 3";
                case "pgiaotong": return "P Giao tổng";
                case "cd1": return "CD1";
                case "cd2": return "CD2";
                case "pnhantong": return "P Nhận tổng";
                //case "tcd1": return "TCD 1";
                //case "tcd2": return "TCD 2";
                case "timemeter": return "Time Meter";
                case "id_auto": return "ID Auto";
                case "sort": return "Sort";
                case "time_warning": return "Time Warning";
                case "warning_code": return "Warning Code";
                case "warning_Name": return "Warning Name";
                case "format": return "Format";
                case "format2": return "Format 2";
                case "wcontent": return "WContent";
                case "value_norm": return "Value Norm";
                case "value_warning": return "Value Warning";
                case "khacphuc": return "Khắc phục";
                case "countday": return "Count Day";
                case "counthour": return "Count Hour";
                case "countminute": return "Count Minute";
                case "typekh": return "Type KH";
                case "statuskp": return "Status Khắc phục";
                case "solanbieu1": return "Số lần biểu 1";
                case "meter_id": return "Meter ID";
                case "sophutbieu1": return "Số phút biểu 1";
                case "sophutbieu2": return "Số phút biểu 2";
                case "sophutbieu3": return "Số phút biểu 3";
                case "minuterange": return "Minute Range";
                case "solanof<5p": return "Số lần of < 5P";
                case "solanof>5p": return "Số lần of > 5P";
                case "sokhof<5p": return "Số KH of < 5P";
                case "sokhof>5p": return "Số KH of > 5P";
                case "sophutof>5p": return "Số phút of > 5P";
                case "range_time": return "Range Time";
                case "timepgiaotong": return "Time P Giao tổng";
                case "timepnhantong": return "Time P Nhận tổng";
                case "countmeter_id": return "Count Meter ID";
                case "event": return "Event";
                case "phase": return "Phase";
                case "ballot_id": return "Ballot ID";
                case "ballot_name": return "Ballot Name";
                case "usercreat": return "Người Tạo";
                case "updatedate": return "Ngày Cập Nhật";
                case "userupdate": return "Người Cập Nhật";
                case "status": return "Trạng thái";
                case "nhanvien": return "Nhân Viên";
                case "action": return "Action";
                case "time_fixed": return "Time Fixed";
                case "value_fixed": return "Value Fixed";
                case "thu2": return "Thứ 2";
                case "thu3": return "Thứ 3";
                case "thu4": return "Thứ 4";
                case "thu5": return "Thứ 5";
                case "thu6": return "Thứ 6";
                case "thu7": return "Thứ 7";
                case "chunhat": return "Chủ nhật"; ;
                case "count_u": return "Count U";
                case "count_i": return "Count I";
                case "count_cos": return "Count Cos";
                case "count_f": return "Count F";
                case "count_pha": return "Count Pha";
                case "count_p": return "Count P";
                case "count_angle": return "Count Angle";
                case "count_time": return "Count Time";
                case "count_sl": return "Count SL";
                case "thutupha": return "Thứ tự pha";
                case "tua": return "TU A";
                case "tia": return "TI A";
                case "tpa": return "TP A";
                case "tqa": return "TQ A";
                case "tanglea": return "T Angle A";
                case "tcosa": return "T Cos A";
                case "tfrega": return "T Freg A";
                case "tub": return "TU B";
                case "tib": return "TI B";
                case "tpb": return "TP B";
                case "tqb": return "TQ B";
                case "tangleb": return "T Angle B";
                case "tcosb": return "T Cos B";
                case "tfregb": return "T Freg B";
                case "tuc": return "TU C";
                case "tic": return "TI C";
                case "tpc": return "TP C";
                case "tqc": return "TQ C";
                case "tanglec": return "T Angle C";
                case "tcosc": return "T Cos C";
                case "tfregc": return "T Freg C";
                case "anglea": return "Angle A";
                case "angleb": return "Angle B";
                case "anglec": return "Angle C";
                case "frega": return "Freg A";
                case "fregb": return "Freg B";
                case "fregc": return "Freg C";
                case "totalcos": return "Total Cos";
                case "totalp": return "Total P";
                case "totalq": return "Total Q";
                case "profileid": return "Pro File ID";
                case "profilename": return "Pro File Name";
                case "insertdate": return "Insert Date";
                case "username": return "Tên người dùng";
                case "countapply": return "Count Apply";
                case "total": return "Total";
                case "permissionapply": return "Permission Apply";
                case "permissionedit": return "Permission Edit";
                case "permissiondelete": return "Permission Delete";
                case "countunaplly": return "Count Unapply";
                case "rangetime": return "Range Time";
                case "rangepgiaotong": return "Range P Giao tổng";
                case "rangepnhantong": return "Range P Nhận tổng";
                case "countmeterid": return "Count Meter ID";
                case "docthanhcong": return "Đọc thành công";
                case "loionlinenotdata": return "Lỗi online không có dữ liệu";
                case "loitocdo": return "Lỗi tốc độ";
                case "loinguon": return "Lỗi Nguồn";
                case "loimatdien": return "Lỗi mất điện";
                //case "tong": return "Tổng";
                case "time_gannhat": return "Time Gần nhất";
                case "minutesrange": return "Minutes Range";
                case "thoigiancongto": return "Thời gian công tơ";
                case "thoigianhethong": return "Thời gian hệ thống";
                case "tu_congto": return "TU Công tơ";
                case "ti_congto": return "TI Công tơ";
                case "hsntrongcongto": return "HSN trong công tơ";
                case "tungoai": return "TU Ngoài";
                case "tingoai": return "TI ngoài";
                //case "ua": return "UA"
                case "cosφ A": return "Cosφ A";
                case "cosφ B": return "Cosφ B";
                case "cosφ C": return "Cosφ C";
                case "Σcos": return "Σ Cos";
                case "fa": return "F A";
                case "fb": return "F B";
                case "fc": return "F C";
                case "Σp": return "Σ P";
                case "Σq": return "Σ Q";
                case "Σqnhan": return "Σ Q Nhận";
                case "Σqgiao": return "Σ Q Giao";

                case "Σpnhan": return "Σ P Nhận";
                case "pmax1": return "P max 1";
                case "pmax2": return "P max 2";

                case "timepmax1": return "Time P max 1";

                case "timepmax2": return "Time P max 2";



            }

            return key;
        }
    }

    public class nancyExport : NancyModule
    {

        // To Add the Styles to Report while Exporting to Excel
        private string AddExcelStyling()
        {
            StringBuilder sb = new StringBuilder();
            //////sb.Append("<html xmlns:o='urn:schemas-microsoft-com:office:office'\n" +
            //////"xmlns:x='urn:schemas-microsoft-com:office:excel'\n" +
            //////"xmlns='http://www.w3.org/TR/REC-html40'>\n" +
            //////"<head>\n");
            sb.Append("<style>\n");
            sb.Append("@page");
            sb.Append("{margin:0.5in 0.5in 0.5in 0.5in;\n");
            sb.Append("mso-header-margin:.5in;\n");
            sb.Append("mso-footer-margin:.1in;\n");
            sb.Append("mso-page-Fit To:yes;\n");
            sb.Append("mso-page-Scaling:Fit2Page;\n");
            sb.Append("{mso-footer-data:'&L&D   &T  &RPage &P of &n ';");
            sb.Append("mso-page-FitToPagesTall:True;\n");
            sb.Append("mso-paper-source:0;\n");
            sb.Append("mso-page-Scaling:Fit2Page;");
            // If we want to export as Portrait
            sb.Append("mso-page-orientation:Portrait;}\n");
            // If we want to export as Landscape
            //sb.Append("mso-page-orientation: Landscape;}\n");  
            sb.Append("</style>\n");
            //////sb.Append("<!--[if gte mso 9]><xml>\n");
            //////sb.Append("<x:ExcelWorkbook>\n");
            //////sb.Append("<x:ExcelWorksheets>\n");
            //////sb.Append("<x:ExcelWorksheet>\n");
            //////sb.Append("<x:Name>Sheet1</x:Name>\n");
            //////sb.Append("<x:WorksheetOptions>\n");
            //////sb.Append("<x:Print>\n");
            //////sb.Append("<x:ValidPrinterInfo/>\n");
            //////sb.Append("<x:PaperSizeIndex>9</x:PaperSizeIndex>\n");
            //////// Paper Size Index 9 – A4, 5 -Legal
            //////sb.Append("<x:HorizontalResolution>20</x:HorizontalResolution\n");
            //////sb.Append("<x:VerticalResolution>20</x:VerticalResolution\n");
            //////sb.Append("<x:Scale>100</x:Scale>");  //Scaling        

            //////sb.Append("<x:FitWidth>1</x:FitWidth>");
            //////sb.Append("<x:FitHeight>700</x:FitHeight>");
            //////sb.Append("</x:Print>\n");
            //////sb.Append("<x:Selected/>\n");
            //////sb.Append("<x:DoNotDisplayGridlines/>\n");
            //////sb.Append("<x:ProtectContents>False</x:ProtectContents>\n");
            //////sb.Append("<x:ProtectObjects>False</x:ProtectObjects>\n");
            //////sb.Append("<x:ProtectScenarios>False</x:ProtectScenarios>\n");
            //////sb.Append("</x:WorksheetOptions>\n");
            //////sb.Append("</x:ExcelWorksheet>\n");
            //////sb.Append("</x:ExcelWorksheets>\n");
            //////sb.Append("<x:WindowHeight>12780</x:WindowHeight>\n");
            //////sb.Append("<x:WindowWidth>19035</x:WindowWidth>\n");
            //////sb.Append("<x:WindowTopX>0</x:WindowTopX>\n");
            //////sb.Append("<x:WindowTopY>15</x:WindowTopY>\n");
            //////sb.Append("<x:ProtectStructure>False</x:ProtectStructure>\n");
            //////sb.Append("<x:ProtectWindows>False</x:ProtectWindows>\n");
            //////sb.Append("</x:ExcelWorkbook>\n");
            //////sb.Append("<x:ExcelName>\n");
            //////sb.Append("<x:Name>Print_Titles</x:Name>\n");
            //////sb.Append("<x:SheetIndex>1</x:SheetIndex>\n");
            //////sb.Append("<x:Formula>=Sheet1!$1:$5</x:Formula>\n");
            //////sb.Append("</x:ExcelName>\n");
            //////sb.Append("</xml><![endif]-->\n");
            //////sb.Append("</head>\n");
            //////sb.Append("<body>\n");
            return sb.ToString();
        }

        //public nancyExport(IConfigProvider configProvider, IJwtWrapper jwtWrapper)
        public nancyExport()
            : base("/export")
        {
            Get["/"] = x =>
            {
                #region //....
                var o = (Response)@"

<!DOCTYPE html>
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <title>@domain/@path</title>
</head>
<body>
    <form name=form id=form method=post action=""#"">
        <input type=text name=msg id=msg />
        <input type=submit />
    </form>

    <script>
        function listener(event) {
            //if (event.origin !== 'http://javascript.info') return;
            var data = event.data;
            document.getElementById('msg').value = data;
            //alert(data);
            document.getElementById('form').submit();
        }

        if (window.addEventListener) {
            addEventListener('message', listener, false)
        } else {
            attachEvent('onmessage', listener)
        }
    </script>
</body>
</html>


";
                o.StatusCode = Nancy.HttpStatusCode.OK;
                o.ContentType = "text/html";
                return o;
                #endregion
            };

            Post["/"] = x =>
            {
                string s_html = "";
                string s_description = "";
                string s_msg = Request.Form["msg"];
                string[] ab = s_msg.Split('|');
                if (ab.Length == 6)
                {
                    s_description = ab[2].f_Base64ToString();
                    string
                        s_body_template_64 = ab[0],
                        s_config_64 = ab[1],

                        s_header_64 = ab[3],
                        s_master_template_64 = ab[4],
                        s_detail_template_64 = ab[5];

                    string config = s_config_64.f_Base64ToString();

                    if (!string.IsNullOrWhiteSpace(config))
                    {
                        string[] a = config.ToLower().Split(new string[] { "api" }, StringSplitOptions.None);
                        if (a.Length > 0)
                        {
                            string
                                s_main_template = s_body_template_64.f_Base64ToString(),
                                s_header = s_header_64.f_Base64ToString(),
                                s_master_template = s_master_template_64.f_Base64ToString(),
                                s_detail_template = s_detail_template_64.f_Base64ToString();

                            string api = a[1].Replace(@"""", "").Replace(@":", "").Split(',')[0];

                            msgPara m = new msgPara()
                            {
                                api = api,
                                config = s_config_64
                            };



                            string s_query = "json|" + JsonConvert.SerializeObject(m);
                            var ds = msg.ProcessMessage(s_query, 1, 1000000,
                                msg_type.table_td, s_detail_template);


                            FormatCompiler compiler = new FormatCompiler();
                            Generator generator = compiler.Compile(s_detail_template);
                            switch (api)
                            {
                                default:
                                    string tbody2 = ds.Item2;

                                    if (s_main_template.Contains(msg_render.col_tbody_key) && tbody2.Trim().Length>10)
                                        s_html = s_main_template.Replace(msg_render.col_tbody_key, tbody2);
                                    else
                                        s_html = s_main_template + "<table>" + tbody2 + "</table>";
                                    if (ds.Item4 != null)
                                    {
                                        var arrTempCols = ExtractCols(s_master_template);

                                        if (arrTempCols.Length > 0 && (s_master_template != ""))
                                        {
                                            var bodyItems = "";
                                            int index = 0;
                                            string bodyTemplate = s_master_template;
                                            string body = s_detail_template;
                                            //Generator generator_am = compiler.Compile(s_master_template);
                                            foreach (var i in ds.Item4)
                                            {
                                                index += 1;
                                                var bodyDetail = s_detail_template.Replace("[{{meter_id}}]", bodyTemplate);
                                                var temp = FillToExTemplateWithObject(bodyDetail, i, arrTempCols, index);
                                                bodyItems += temp;
                                            }
                                            s_html = s_html.Replace(msg_render.col_tbody_key, bodyItems);
                                        }
                                        else
                                            s_html = generator.Render(ds.Item4);
                                    }
                                    break;
                                case "store":
                                    string tbody = ds.Item2;

                                    if (s_main_template.Contains(msg_render.col_tbody_key))
                                        s_html = s_main_template.Replace(msg_render.col_tbody_key, tbody);
                                    else
                                        s_html = s_main_template + "<table>" + tbody + "</table>";

                                    m_meter[] am2 = ds.Item5;
                                    //if (am != null && (am.Length > 0 && s_master_template != ""))
                                    //{
                                    //    string[] masterTemplateValues = s_master_template.Split(new string[] { "<td>" }, StringSplitOptions.None).Where(o => o != "").Select(o => "").ToArray();
                                    //    string s_master_template_def = string.Join("<td></td>", masterTemplateValues);
                                    //    Generator generator_am = compiler.Compile(s_master_template);

                                    //    foreach (var i in am)
                                    //    {
                                    //        string val = generator_am.Render(i);
                                    //        if (string.IsNullOrWhiteSpace(val))
                                    //            s_html = s_html.Replace("[" + i.meter_id.ToString() + "]", s_master_template_def);
                                    //        else
                                    //            s_html = s_html.Replace("[" + i.meter_id.ToString() + "]", val);
                                    //    }
                                    //}


                                    if (am2 != null && (am2.Length > 0 && s_master_template != ""))
                                    {
                                        string[] masterTemplateValues = s_master_template.Split(new string[] { "<td>" }, StringSplitOptions.None).Where(o => o != "").Select(o => "").ToArray();
                                        string s_master_template_def = string.Join("<td></td>", masterTemplateValues);
                                        Generator generator_am = compiler.Compile(s_master_template);
                                        var arrsplitCols = ExtractCols(s_master_template);
                                        bool has_split_cols = arrsplitCols.Length > 0;

                                        foreach (var i in am2)
                                        {
                                            string val = generator_am.Render(i);
                                            if (has_split_cols)
                                            {
                                                var tempBody = FillToTemplateWithObject(s_master_template, i, arrsplitCols);
                                                s_html = s_html.Replace("[" + i.meter_id.ToString() + "]", tempBody);
                                            }
                                            else if (string.IsNullOrWhiteSpace(val))
                                                s_html = s_html.Replace("[" + i.meter_id.ToString() + "]", s_master_template_def);
                                            else
                                                s_html = s_html.Replace("[" + i.meter_id.ToString() + "]", val);
                                        }
                                    }


                                    break;
                            }

                            //FormatCompiler compiler = new FormatCompiler();
                            //Generator generator = compiler.Compile(s_detail_template);
                            //switch (api)
                            //{
                            //    default:
                            //        var arrTempCols = ExtractCols(s_detail_template);
                            //        if (arrTempCols.Length > 0)
                            //        {
                            //            string body = ExtracBody(s_detail_template);
                            //            string result = s_detail_template.Replace(body, "##bodyitems##");
                            //            string bodyItems = "";
                            //            int index = 0;
                            //            foreach (var obj in ds.Item4)
                            //            {
                            //                index += 1;
                            //                bodyItems += FillToExTemplateWithObject(body, obj, arrTempCols, index);
                            //            }
                            //            s_html = result.Replace("##bodyitems##", bodyItems);
                            //        }
                            //        else
                            //            s_html = generator.Render(ds.Item4);
                            //        break;
                            //    case "store":
                            //        string j_data = ds.Item2;
                            //        m_meter[] am = ds.Item5;

                            //       // j_data = @"[{""index_"":1, ""data_type"":1602001,""file_id"":53539,""dcu_id"":105353094,""device_id"":1501000425,""yyyyMMdd"":""04/01/2016"",""HHmmss"":""23:37"",""phase"":""_1pha"",""data"":""FIX_DAY"",""tech"":""PLC"",""factory"":""PSMART"",""meter_id"":1501000425,""time_chot"":160103,""time_read"":1601040010,""p_giao_tong"":30.79,""p_giao_bieu_1"":30.79,""p_giao_bieu_2"":0,""p_giao_bieu_3"":0,""p_giao_bieu_4"":0,""p_nhan_tong"":0,""p_nhan_bieu_1"":0,""p_nhan_bieu_2"":0,""p_nhan_bieu_3"":0,""p_nhan_bieu_4"":0}]";

                            //        var dt_Data = JsonConvert.DeserializeObject<dynamic[]>(j_data);
                            //        s_html = generator.Render(dt_Data);

                            //        if (am != null && (am.Length > 0 && s_master_template != ""))
                            //        {
                            //            string[] masterTemplateValues = s_master_template.Split(new string[] { "<td>" }, StringSplitOptions.None).Where(o => o != "").Select(o => "").ToArray();
                            //            string s_master_template_def = string.Join("<td></td>", masterTemplateValues);
                            //            Generator generator_am = compiler.Compile(s_master_template);
                            //            var arrsplitCols = ExtractCols(s_master_template);
                            //            bool has_split_cols = arrsplitCols.Length > 0;

                            //            foreach (var i in am)
                            //            {
                            //                string val = generator_am.Render(i);
                            //                if (has_split_cols)
                            //                {
                            //                    var tempBody = FillToTemplateWithObject(s_master_template, i, arrsplitCols);
                            //                    s_html = s_html.Replace("[" + i.meter_id.ToString() + "]", tempBody);
                            //                }
                            //                else if (string.IsNullOrWhiteSpace(val))
                            //                    s_html = s_html.Replace("[" + i.meter_id.ToString() + "]", s_master_template_def);
                            //                else
                            //                    s_html = s_html.Replace("[" + i.meter_id.ToString() + "]", val);
                            //            }
                            //        }
                            //        break;
                            //}//end template


                        }
                    }
                }

                var r = new Response();
                r.Contents = w =>
                {
                    StringBuilder s = new StringBuilder();

                    s.Append(
@"<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel' xmlns='http://www.w3.org/TR/REC-html40'>
<head>
    <!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>DanhSach</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]-->
</head>");
                    s.Append("<body>");
                    s.Append(s_description);
                    s.Append(s_html);
                    s.Append("</body>");
                    s.Append("</html>");


                    byte[] bytes = Encoding.UTF8.GetBytes(s.ToString());
                    for (int i = 0; i < 10; ++i)
                    {
                        w.Write(bytes, 0, bytes.Length);
                        w.Flush();
                    }
                };
                string fileName = "danhsach_export.xls";
                r.Headers.Add("Content-Disposition", string.Format("attachment;filename={0}", fileName));
                return r.AsAttachment(fileName, "application/vnd.ms-exce");
            };




            Get["/{img_name}.jpg"] = x =>
            {
                string img_name = x.img_name;

                ObjectCache cache = MemoryCache.Default;
                string data = cache[img_name] as string;

                //byte[] myImageByteArray = null;
                //return Response.FromByteArray(myImageByteArray, "image/jpeg");

                //data:image/gif;base64,
                //byte[] bytes = Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAAAAACH5BAAAAAAALAAAAAABAAEAAAICTAEAOw==");
                byte[] bytes = Convert.FromBase64String(data);
                Stream stream = new MemoryStream(bytes);

                return Response.FromStream(stream, "image/jpeg");
            };



            Get["/demo"] = x =>
            {
                var r = new Response();
                r.Contents = w =>
                {
                    StringBuilder s = new StringBuilder();
                    s.Append(@"<html xmlns:x=""urn: schemas - microsoft - com:office: excel"">");
                    s.Append("<head>");
                    s.Append(@"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf8"">");
                    s.Append("<!--[if gte mso 9]>");
                    s.Append("<xml>");
                    s.Append("<x:ExcelWorkbook>");
                    s.Append("<x:ExcelWorksheets>");
                    s.Append("<x:ExcelWorksheet>");
                    //this line names the worksheet
                    s.Append("<x:Name>gridlineTest</x:Name>");
                    s.Append("<x:WorksheetOptions>");
                    //these 2 lines are what works the magic
                    s.Append("<x:Panes>");
                    s.Append("</x:Panes>");
                    s.Append("</x:WorksheetOptions>");
                    s.Append("</x:ExcelWorksheet>");
                    s.Append("</x:ExcelWorksheets>");
                    s.Append("</x:ExcelWorkbook>");
                    s.Append("</xml>");
                    s.Append("<![endif]-->");
                    s.Append("</head>");
                    s.Append("<body>");
                    s.Append("<table>");
                    s.Append("<tr><td>ID</td><td>Name</td><td>Balance</td></tr>");
                    s.Append("<tr><td>1234</td><td>Al Bundy</td><td>45</td></tr>");
                    s.Append("<tr><td>9876</td><td>Homer Simpson</td><td>-129</td></tr>");
                    s.Append("<tr><td>5555</td><td>Peter Griffin</td><td>0</td></tr>");
                    s.Append("</table>");
                    s.Append("</body>");
                    s.Append("</html>");

                    byte[] bytes = Encoding.UTF8.GetBytes(s.ToString());
                    for (int i = 0; i < 10; ++i)
                    {
                        w.Write(bytes, 0, bytes.Length);
                        w.Flush();
                        //Thread.Sleep(500);
                    }
                };
                string fileName = "test2.xls";
                r.Headers.Add("Content-Disposition", string.Format("attachment;filename={0}", fileName));
                return r.AsAttachment(fileName, "application/vnd.ms-exce");
            };

        }


        public string ExtracBody(string html)
        {
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            Regex regx = new Regex(".*?<tbody>(.*?)</tbody>.*?", options);
            Regex regx1 = new Regex("(.*)<tbody>(.*)</tbody>(.*)", options);
            Regex regx2 = new Regex(".*?<tbody>(.*?)</tbody>.*?", options);

            MatchCollection matches = regx.Matches(html);
            MatchCollection matches1 = regx1.Matches(html);
            MatchCollection matches2 = regx2.Matches(html);

            Match match = regx.Match(html);
            Match match1 = regx.Match(html);
            Match match2 = regx.Match(html);

            if (match.Success)
            {
                string theBody = match.Groups[1].Value;
                return theBody;
            }
            if (match1.Success)
            {
                string theBody = match1.Groups[1].Value;
                return theBody;
            }
            if (match2.Success)
            {
                string theBody = match2.Groups[1].Value;
                return theBody;
            }

            if (matches.Count > 0)
            {
                string theBody = matches[0].Value;
                return theBody;
            }
            if (matches1.Count > 0)
            {
                string theBody = matches1[0].Value;
                return theBody;
            }
            if (matches2.Count > 0)
            {
                string theBody = matches2[0].Value;
                return theBody;
            }
            int index1 = html.IndexOf("<tbody>", System.StringComparison.Ordinal);
            int index2 = html.IndexOf("</tbody>", System.StringComparison.Ordinal);
            if (index1 >= 0 && index2 >= 0) return html.Substring(index1 + 6, index2);
            return html;
        }

        public string FillToExTemplateWithObject(string template, Object objectData, string[] arrSplitCols, Int32 indexRow = 0)
        {
            var propertyInfos = objectData.GetType().GetFields();
            foreach (FieldInfo pInfo in propertyInfos)
            {
                var propertyName = pInfo.Name;
                var propertyValue = pInfo.GetValue(objectData) + "";
                var indexStrings = new string[] { "1", "2", "3" };
                var value = "";

                if (template.IndexOf(string.Format("{{{0}}}", propertyName), System.StringComparison.Ordinal) >= 0)
                {
                    template = propertyName == "meter_id" ? template.Replace(string.Format("{{{0}}}", propertyName), " 00" + propertyValue + " ") : template.Replace(string.Format("{{{0}}}", propertyName), propertyValue);
                }
                if (template.IndexOf("{#index}", System.StringComparison.Ordinal) >= 0)
                {
                    template = template.Replace("{#index}", indexRow.ToString());
                }
                foreach (string colSplit in arrSplitCols)
                {
                    string[] valueStrings = (propertyValue + "").Split(';');
                    if (colSplit.IndexOf(propertyName, System.StringComparison.Ordinal) >= 0 && valueStrings.Length > 1)
                    {
                        var index = Array.IndexOf(indexStrings, colSplit.Replace(propertyName, ""));
                        if (index < 0) continue;
                        try { value = valueStrings[index]; }
                        catch (Exception) { }
                        template = template.Replace(string.Format("##{0}##", colSplit), value);
                    }
                }
            }

            template = arrSplitCols.Aggregate(template, (current, colSplit) => current.Replace(string.Format("##{0}##", colSplit), ""));

            string[] itemsSource = { "{{#each this}}", "{{#newline}}", "{{/each}}", "{}" };
            string[] itemsReplace = { "", "<br/>", "", "" };
            for (int i = 0; i < itemsSource.Length; i++)
            {
                template = template.Replace(itemsSource[i] + "", itemsReplace[i] + "");
            }
            return template;
        }

        public string FillToTemplateWithObject(string template, Object objectData, string[] arrSplitCols)
        {
            var propertyInfos = objectData.GetType().GetFields();
            foreach (FieldInfo pInfo in propertyInfos)
            {
                var propertyName = pInfo.Name;
                var propertyValue = pInfo.GetValue(objectData) + "";
                var indexStrings = new string[] { "1", "2", "3" };
                var value = "";

                if (template.IndexOf(string.Format("{{{0}}}", propertyName), System.StringComparison.Ordinal) >= 0)
                {
                    template = propertyName == "meter_id" ? template.Replace(string.Format("{{{0}}}", propertyName), " 00" + propertyValue + " ") : template.Replace(string.Format("{{{0}}}", propertyName), propertyValue);
                }
                foreach (string colSplit in arrSplitCols)
                {
                    string[] valueStrings = (propertyValue + "").Split(';');
                    if (colSplit.IndexOf(propertyName, System.StringComparison.Ordinal) >= 0 && valueStrings.Length > 1)
                    {
                        var index = Array.IndexOf(indexStrings, colSplit.Replace(propertyName, ""));
                        if (index >= 0)
                        {
                            try { value = valueStrings[index]; }
                            catch (Exception) { }
                            template = template.Replace(string.Format("##{0}##", colSplit), value);
                        }
                    }
                }
            }
            template = arrSplitCols.Aggregate(template, (current, colSplit) => current.Replace(string.Format("##{0}##", colSplit), ""));
            return template;
        }

        public string[] ExtractCols(string headerTemplate)
        {
            var result = Regex.Matches(headerTemplate, @"##(.+?)##").Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
            var result2 = Regex.Matches(headerTemplate, @"##[^#]##").Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
            return result;
        }

    }//end class

}
