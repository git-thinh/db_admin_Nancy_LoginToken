
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace host
{
    public static class hostFile
    {
        public static void load()
        {
            store_plc.load();
            //store_meter.load();
            db_dcu.load();
            db_meter.load();

            store.init();

            db_reduce.load();
            db_column.load();


            db_user.load();

            //db_kh_dienluc.load();
            //db_kh_nhom.load();

            db_node.load();
            db_kh_danhmuc.load();

            db_loai_canhbao.load();
            db_canhbao_cauhinh.load();

            //var ls = db_meter.get_All().GroupBy(x => x.meter_id)
            //    .Select(x => new { id = x.Key, count = x.Count() })
            //    .Where(x => x.count > 1)
            //    .ToArray();

            //string li_code = string.Join(Environment.NewLine, ls.Select(x => x.id).ToArray());


        }


        public static T[] read_file_MMF<T>(string path_file)
        {
            T[] a = new T[] { };
            if (File.Exists(path_file))
            {

                byte[] bufferItem;

                MemoryMappedFileSecurity mSec = new MemoryMappedFileSecurity();
                mSec.AddAccessRule(new AccessRule<MemoryMappedFileRights>(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    MemoryMappedFileRights.FullControl, AccessControlType.Allow));

                using (FileStream stream = new FileStream(path_file, FileMode.OpenOrCreate))
                {
                    using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(stream, null, stream.Length,
                        MemoryMappedFileAccess.ReadWrite, mSec, HandleInheritability.None, true))
                    {
                        using (MemoryMappedViewAccessor mmfReader = mmf.CreateViewAccessor())
                        {
                            bufferItem = new byte[mmfReader.Capacity];
                            mmfReader.ReadArray<byte>(0, bufferItem, 0, bufferItem.Length);
                        }
                    }
                }

                using (MemoryStream memoryStream = new MemoryStream(bufferItem))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    a = bf.Deserialize(memoryStream) as T[];
                }
            }
            return a;
        }

        public static void write_file_MMF<T>(List<T> data, string path_folder, string file_name)
        {
            write_file_MMF<T>(data.ToArray(), path_folder, file_name);
        }

        public static void write_file_MMF<T>(T[] data, string path_folder, string file_name)
        {
            if (!path_folder.Contains(':'))
                path_folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\" + path_folder;
            if (!Directory.Exists(path_folder)) Directory.CreateDirectory(path_folder);

            MemoryMappedFileSecurity mSec = new MemoryMappedFileSecurity();
            mSec.AddAccessRule(new AccessRule<MemoryMappedFileRights>(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MemoryMappedFileRights.FullControl, AccessControlType.Allow));


            string file = path_folder + @"\" + file_name + ".mmf";
            file = file.Replace("\\\\", "\\");

            Task.Factory.StartNew((object obj) =>
            {
                Tuple<string, T[]> rs = (obj as Tuple<string, T[]>);
                string v_file = rs.Item1;
                T[] v_data = rs.Item2;

                int buffer_size = 0;
                try
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(stream, v_data);
                        byte[] bw = stream.ToArray();

                        buffer_size = bw.Length;

                        long file_size = 0;
                        if (File.Exists(v_file))
                        {
                            FileInfo fi = new FileInfo(v_file);
                            file_size = fi.Length;
                        }

                        long capacity = buffer_size;
                        if (capacity < file_size) capacity = file_size;


                        using (MemoryMappedFile w = MemoryMappedFile.CreateFromFile(v_file, FileMode.OpenOrCreate, null, capacity))
                        {
                            using (MemoryMappedViewAccessor mmfWriter = w.CreateViewAccessor(0, capacity))
                            {
                                mmfWriter.WriteArray<byte>(0, bw, 0, buffer_size);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }, new Tuple<string, T[]>(file, data));
        }//end function



        public static T read_MMF<T>(string path_file) where T : struct
        {
            T item = new T();

            if (File.Exists(path_file))
            {

                byte[] bufferItem;

                MemoryMappedFileSecurity mSec = new MemoryMappedFileSecurity();
                mSec.AddAccessRule(new AccessRule<MemoryMappedFileRights>(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    MemoryMappedFileRights.FullControl, AccessControlType.Allow));

                using (FileStream stream = new FileStream(path_file, FileMode.OpenOrCreate))
                {
                    using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(stream, null, stream.Length,
                        MemoryMappedFileAccess.ReadWrite, mSec, HandleInheritability.None, true))
                    {
                        using (MemoryMappedViewAccessor mmfReader = mmf.CreateViewAccessor())
                        {
                            bufferItem = new byte[mmfReader.Capacity];
                            mmfReader.ReadArray<byte>(0, bufferItem, 0, bufferItem.Length);
                        }
                    }
                }

                int objsize = Marshal.SizeOf(typeof(T));
                IntPtr buff = Marshal.AllocHGlobal(objsize);
                Marshal.Copy(bufferItem, 0, buff, objsize);
                item = (T)Marshal.PtrToStructure(buff, typeof(T));
                Marshal.FreeHGlobal(buff);


                //using (MemoryStream memoryStream = new MemoryStream(bufferItem))
                //{
                //    BinaryFormatter bf = new BinaryFormatter();
                //    item = (T)bf.Deserialize(memoryStream);
                //}

            }

            return item;
        }


        public static void write_MMF<T>(T item, string path_folder, string file_name) where T : struct
        {
            if (!path_folder.Contains(':'))
                path_folder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\" + path_folder;
            if (!Directory.Exists(path_folder)) Directory.CreateDirectory(path_folder);

            MemoryMappedFileSecurity mSec = new MemoryMappedFileSecurity();
            mSec.AddAccessRule(new AccessRule<MemoryMappedFileRights>(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MemoryMappedFileRights.FullControl, AccessControlType.Allow));


            string filePath = path_folder + @"\" + file_name + ".mmf";
            filePath = filePath.Replace("\\\\", "\\");

            int buffer_size = 0;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    int objsize = Marshal.SizeOf(typeof(T));
                    byte[] bw = new byte[objsize];
                    IntPtr buff = Marshal.AllocHGlobal(objsize);
                    Marshal.StructureToPtr(item, buff, true);
                    Marshal.Copy(buff, bw, 0, objsize);
                    Marshal.FreeHGlobal(buff);

                    buffer_size = bw.Length;

                    long file_size = 0;
                    if (File.Exists(filePath))
                    {
                        FileInfo fi = new FileInfo(filePath);
                        file_size = fi.Length;
                    }

                    long capacity = buffer_size;
                    if (capacity < file_size) capacity = file_size;


                    using (MemoryMappedFile w = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, null, capacity))
                    {
                        using (MemoryMappedViewAccessor mmfWriter = w.CreateViewAccessor(0, capacity))
                        {
                            mmfWriter.WriteArray<byte>(0, bw, 0, buffer_size);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

        }//end function





        public static void writeFilfe(string filePath, string fileName, string data, Encoding encode)
        {
            string file = (filePath + @"\" + fileName).Replace(@"\\", @"\");

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            if (!File.Exists(file)) File.WriteAllText(file, data);
            else
                using (StreamWriter writer = new StreamWriter(file, false, encode))
                {
                    writer.WriteLine(data);
                }
        }





        public static string readFile(string file, Encoding encode)
        {
            string s = "";

            if (File.Exists(file))
            {
                using (StreamReader streamReader = new StreamReader(file, encode))
                {
                    s = streamReader.ReadToEnd();
                }
            }

            return s.Trim();
        }






    }//end class
}
