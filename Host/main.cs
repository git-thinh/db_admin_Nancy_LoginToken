using AutoMapper;
using host;
using Microsoft.Owin.Hosting;
using mmf;
using model.memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using host.websocket;
using System.Text.RegularExpressions;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace host
{
    public class main
    {
        private static List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>() { };
        private static fMain fmain = new fMain();

        #region /// notification ...

        public static Form globalForm;
        public static void show_notification(string msg, int duration_ = 0)
        {
            fNoti form = new fNoti(msg, duration_);
            globalForm.Invoke((MethodInvoker)delegate()
            {
                form.Show();
            });
        }

        #endregion

        #region /// mmf ...

        private static string mmf_name = "mmf_array_name";
        private static Array<p3_tsvh> mmf;
        private static int mmf_capicity = 1000000;
        private static int mmf_length = 0;

        private static BufferReadWrite mmf_update = new BufferReadWrite("mmf_update", 1, true);

        #endregion

        #region

        public static void hide_form_Main()
        {
            fmain.Hide();
        }

        public static void show_form_Main()
        {
            fmain.Show();
        }

        public static void main_redure_reload()
        {
            fmain.reduce_reload();
        }
        
        public static void reset()
        {
            MessageBox.Show("Reset application ...");
        }

        #endregion

        [STAThread]
        static void Main(string[] args)
        { 
            #region /// notification ...

            globalForm = new Form();
            globalForm.Show();
            globalForm.Hide();

            #endregion

            if (!Directory.Exists(hostServer.pathModule))
            {
                MessageBox.Show("Can't find: " + hostServer.pathModule);
                return;
            }

            hostFile.load();
                                   
            //store_meter.init();
                         
            Task.Factory.StartNew(() =>
            {
                hostAPI.init();
            });

            Task.Factory.StartNew(() =>
            {
                hostServer.init();


                //Type ty = Type.GetType("host.db_column, host");
                //MethodInfo function = ty.GetMethod("where", BindingFlags.Static | BindingFlags.Public);

                //var fun = (Func<string, string, int, int, Tuple<bool, int, int, string>>)Delegate.CreateDelegate(typeof(Func<string, string, int, int, Tuple<bool, int, int, string>>), function);

                //var fun_rs = fun("index == 1 || index == 2", "", 1, 10);

                //int kx = 0;
            });

            #region /// mmf ...

            mmf = new Array<p3_tsvh>(mmf_name, mmf_capicity, true);
            mmf_length = mmf.Length;

            //////mmf.eventUpdated += delegate(object sender, EventArgs e)
            //////{
            //////    show_notification("Amin >> Update finish ...");
            //////};

            //////Task.Factory.StartNew(() =>
            //////{
            //////    while (true)
            //////    {
            //////        Debug.WriteLine(">> timer check 10 ms update data: " + mmf_length.ToString());

            //////        using (var read = new BufferReadWrite("mmf_update"))
            //////        {
            //////            read.Read((IntPtr pr) =>
            //////            {
            //////                byte[] bArray = new byte[1];
            //////                Marshal.Copy(pr, bArray, 0, 1);
            //////                if (bArray[0] == 1)
            //////                {
            //////                    var item = mmf[0];

            //////                    int len_v = Marshal.SizeOf(item);

            //////                    string id = String.Format("{0:#,###.##}", item.id);

            //////                    p3_tsvh[] rd_a = new p3_tsvh[] { item };

            //////                    //Mapper.CreateMap<r_item_ram, r_item_file>();
            //////                    //var ls = Mapper.Map<r_item_file[]>(rd_a);

            //////                    byte[] b00 = Converter.fSerializeItem<p3_tsvh>(item);
            //////                    p3_tsvh o00 = Converter.fDeserializeItem<p3_tsvh>(b00);

            //////                    //Mapper.CreateMap<r_item_file, r_item_ram>();
            //////                    //var ls1 = Mapper.Map<r_item_ram[]>(ls);

            //////                    Debug.WriteLine("Update data: " + id + "\n " + DateTime.Now.ToString());

            //////                    show_notification("has new data: " + id);
            //////                    mmf_update.Write((IntPtr pw) =>
            //////                    {
            //////                        bArray[0] = 0;
            //////                        Marshal.Copy(bArray, 0, pw, 1);
            //////                    });

            //////                    int len = mmf.LengthCurrent;
            //////                    Debug.WriteLine(">> timer check 10 ms update data >> OKKK: " + len.ToString());
            //////                }
            //////            });
            //////        }

            //////        Thread.Sleep(1000);
            //////    }
            //////});


            //using (var rw = new Array<r_item_ram>("ram_array", 10))
            //{
            //    rw.Push(new r_item_ram("name 1"));

            //    //rw[0] = new r_item { c0 = 3, key = "My Test Name" }; 

            //    using (var rd = new Array<r_item_ram>("ram_array"))
            //    {
            //        r_item_ram a0 = rd[0];
            //        r_item_ram a1 = rd[1];
            //        r_item_ram a4 = rd[4];


            //        //Console.WriteLine("\n\nFinish ....");
            //        //Console.ReadKey();
            //        while (true)
            //        {
            //            Thread.Sleep(10);
            //        }
            //    }
            //}



            #endregion

            #region /// web_socket ...

            Task.Factory.StartNew(() =>
            {
                string url = "ws://" + hostServer.IP_NAT + ":" + hostUser.page_Signal.Split(':')[1];
                FleckLog.Level = LogLevel.Debug;
                var server = new WebSocketServer(url);
                //var server = new WebSocketServer("ws://192.168.1.38:9704");
                //var server = new WebSocketServer("ws://0.0.0.0:8181");
                try
                {
                    server.Start(socket =>
                    {
                        socket.OnOpen = () =>
                        {
                            //Console.WriteLine("Open!");
                            allSockets.Add(socket);
                        };

                        socket.OnClose = () =>
                        {
                            //Console.WriteLine("Close!");
                            allSockets.Remove(socket);
                        };

                        socket.OnMessage = message =>
                        {
                            msg.ProcessMessage(socket, message);
                            //Console.WriteLine(message);
                            //allSockets.ToList().ForEach(s => s.Send("Echo: tiếng việt // " + message));                        
                        };
                    });
                }
                catch (Exception ex)
                {
                    string mes = ex.Message;
                    string yyMMddHHmm = DateTime.Now.ToString("yyMMddHHmm");
                    writeErrorLogs(yyMMddHHmm, ex.Message + "\n" + ex.Source);
                }
                

                main.show_notification(url);
            });

            

            #endregion
            
            fmain = new fMain();
            fmain.Show();
            //fmain.Hide();
            
            Application.ApplicationExit += (sender, e) =>
            {
                mmf.Close();
            };
             
            Application.Run(new fSystemTray()); 
             
             

            //Application.Run(new fSysConfig()); 
        }
        private static void writeErrorLogs(string file_name, string message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            System.IO.File.WriteAllText(path + "\\" + file_name + ".txt", message);
        }

        #region /// mmf ...


        #endregion

    }//end class
}
