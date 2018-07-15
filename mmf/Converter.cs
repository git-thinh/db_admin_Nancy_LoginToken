using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace mmf
{
    public static class Converter
    {
        #region // convert string to byte[] ...

        public static string ConvertByteArrayToString(byte[] values)
        {
            System.Text.UnicodeEncoding encoding = new System.Text.UnicodeEncoding();
            return encoding.GetString(values);
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        #endregion

        #region // convert object[struct, class ...] to byte[] ...


        public static Byte[] fSerializeItem<T>(T msg) where T : struct
        {
            int objsize = Marshal.SizeOf(typeof(T));
            Byte[] ret = new Byte[objsize];
            IntPtr buff = Marshal.AllocHGlobal(objsize);
            Marshal.StructureToPtr(msg, buff, true);
            Marshal.Copy(buff, ret, 0, objsize);
            Marshal.FreeHGlobal(buff);
            return ret;
        }

        public static T fDeserializeItem<T>(Byte[] data) where T : struct
        {
            int objsize = Marshal.SizeOf(typeof(T));
            IntPtr buff = Marshal.AllocHGlobal(objsize);
            Marshal.Copy(data, 0, buff, objsize);
            T retStruct = (T)Marshal.PtrToStructure(buff, typeof(T));
            Marshal.FreeHGlobal(buff);
            return retStruct;
        }



















        public static byte[] fConvertObjectToByteArray<T>(T sourceObj)
        {
            byte[] byteArray = null;

            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, sourceObj);
                byteArray = stream.ToArray();
            }

            return byteArray;
        }

        public static T fConvertByteArrayToObject<T>(byte[] sourceBytes)
        {
            object newObject = null;

            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(sourceBytes))
            {
                newObject = formatter.Deserialize(stream);
            }

            return (T)newObject;
        }

        #endregion


        public static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);
            Console.WriteLine(len.ToString());
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static void ByteArrayToStructure(byte[] bytearray, ref object obj)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            Marshal.Copy(bytearray, 0, i, len);
            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i);
        }





        public static byte[] fToByteArray<T>(T[] source) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = handle.AddrOfPinnedObject();
                byte[] destination = new byte[source.Length * Marshal.SizeOf(typeof(T))];
                Marshal.Copy(pointer, destination, 0, destination.Length);
                return destination;
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }

        public static T[] fFromByteArray<T>(byte[] source) where T : struct
        {
            T[] destination = new T[source.Length / Marshal.SizeOf(typeof(T))];
            GCHandle handle = GCHandle.Alloc(destination, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = handle.AddrOfPinnedObject();
                Marshal.Copy(source, 0, pointer, source.Length);
                return destination;
            }
            finally
            {
                if (handle.IsAllocated)
                    handle.Free();
            }
        }




        public static Byte[] SerializeItem<T>(T msg) where T : struct
        {
            int objsize = Marshal.SizeOf(typeof(T));
            Byte[] ret = new Byte[objsize];
            IntPtr buff = Marshal.AllocHGlobal(objsize);
            Marshal.StructureToPtr(msg, buff, true);
            Marshal.Copy(buff, ret, 0, objsize);
            Marshal.FreeHGlobal(buff);
            return ret;
        }

        public static T DeserializeItem<T>(Byte[] data) where T : struct
        {
            int objsize = Marshal.SizeOf(typeof(T));
            IntPtr buff = Marshal.AllocHGlobal(objsize);
            Marshal.Copy(data, 0, buff, objsize);
            
            T retStruct = (T)Marshal.PtrToStructure(buff, typeof(T));

            Marshal.FreeHGlobal(buff);
            return retStruct;
        }

        //public static unsafe r_item[] GetVertices(byte[] buffer)
        //{
        //    r_item[] vecs = new r_item[buffer.Length / (3 * 4)];
        //    fixed (byte* pBuffer = buffer)
        //    {
        //        for (int i = 0; i < vecs.Length; i++)
        //            vecs[i] = ((r_item*)pBuffer)[i];
        //    }
        //    return vecs;
        //}



        public unsafe static byte[] SerializeByteArray<T>(T[] m_samples)
        {
            var sampleSize = Marshal.SizeOf(typeof(T));
            var size = sampleSize * m_samples.Length;
            var bytes = new byte[size];

            fixed (byte* ptr = bytes)
            {
                var intPtr = (IntPtr)ptr;

                for (var idx = 0; idx < m_samples.Length; idx++)
                {
                    Marshal.StructureToPtr(m_samples[idx], intPtr, false);
                    //intPtr += sampleSize;
                    intPtr = new IntPtr(intPtr.ToInt64() + sampleSize);
                }
            }

            return bytes;
        }

        public unsafe static T[] DeserializeObjectArray<T>(byte[] bytes)
        {
            var sampleSize = Marshal.SizeOf(typeof(T));
            var sampleCount = bytes.Length / sampleSize;

            T[] m_samples = new T[sampleCount];

            fixed (byte* ptr = bytes)
            {
                var intPtr = (IntPtr)ptr;
                var sampleType = typeof(T);

                for (var idx = 0; idx < sampleCount; idx++)
                {
                    //m_samples.Add((T)(Marshal.PtrToStructure(intPtr, sampleType)));
                    m_samples[idx] = (T)(Marshal.PtrToStructure(intPtr, sampleType));
                    //intPtr += sampleSize;
                    intPtr = new IntPtr(intPtr.ToInt64() + sampleSize);
                }
            }

            return m_samples;
        }

        public unsafe static void GetObjectData<T>(List<T> m_samples, SerializationInfo info, StreamingContext context)
        {
            var sampleSize = Marshal.SizeOf(typeof(T));
            var size = sampleSize * m_samples.Count;
            var bytes = new byte[size];

            fixed (byte* ptr = bytes)
            {
                var intPtr = (IntPtr)ptr;

                for (var idx = 0; idx < m_samples.Count; idx++)
                {
                    Marshal.StructureToPtr(m_samples[idx], intPtr, false);
                    //intPtr += sampleSize;
                    intPtr = new IntPtr(intPtr.ToInt64() + sampleSize);
                }
            }

            info.AddValue("Samples", bytes);
        }
        
        public unsafe static void ContainerClass<T>(SerializationInfo info, StreamingContext context)
        {
            var sampleSize = Marshal.SizeOf(typeof(T));
            var bytes = info.GetValue("Samples", typeof(byte[])) as byte[];
            var sampleCount = bytes.Length / sampleSize;

            var m_samples = new List<T>(sampleCount);

            fixed (byte* ptr = bytes)
            {
                var intPtr = (IntPtr)ptr;
                var sampleType = typeof(T);

                for (var idx = 0; idx < sampleCount; idx++)
                {
                    m_samples.Add((T)(Marshal.PtrToStructure(intPtr, sampleType)));
                    //intPtr += sampleSize;
                    intPtr = new IntPtr(intPtr.ToInt64() + sampleSize);
                }
            }
        }

    }
}
