// SharedMemory (File: SharedMemory\Array.cs)
// Copyright (c) 2014 Justin Stenning
// http://spazzarama.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// The SharedMemory library is inspired by the following Code Project article:
//   "Fast IPC Communication Using Shared Memory and InterlockedCompareExchange"
//   http://www.codeproject.com/Articles/14740/Fast-IPC-Communication-Using-Shared-Memory-and-Int

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace mmf
{
    /// <summary>
    /// A generic fixed-length shared memory array of structures with support for simple inter-process read/write synchronisation.
    /// </summary>
    /// <typeparam name="T">The struct type that will be stored in the elements of this fixed array buffer.</typeparam>
    [PermissionSet(SecurityAction.LinkDemand)]
    [PermissionSet(SecurityAction.InheritanceDemand)]
    public class Array<T> : BufferWithLocks, IEnumerable<T>
            where T : struct
    {
        /// <summary>
        /// Gets a 32-bit integer that represents the total number of elements in the <see cref="Array{T}"/>
        /// </summary>
        public int Length { get; private set; }
        public int Capicity { get; private set; }

        private int _elementSize;
        
        #region /// set - get - push - remove item of array ...

        /// <summary>
        /// Gets or sets the element at the specified index
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 -or- index is equal to or greater than <see cref="Capicity"/>.</exception>
        public T this[int index]
        {
            get
            {
                T item;
                Read(out item, index);
                return item;
            }
            //set
            //{
            //    Write(ref value, index);
            //}
        }

        public int Push(T item)
        {
            Length = LengthCurrent;
            Write(ref item, Length); 

            mmf_Length_update(1);

            return LengthCurrent;
        }
        
        public int Push(T[] a_item)
        {
            Length = LengthCurrent;
            for (int k = 0; k < a_item.Length; k++)
                Write(ref a_item[k], Length + k);
            mmf_Length_update(a_item.Length);
            return Length + a_item.Length;
        }

        public int PushDistinct(T[] a_item)
        {
            Length = LengthCurrent;
            for (int k = 0; k < a_item.Length; k++)
                Write(ref a_item[k], Length + k);
            mmf_Length_update(a_item.Length);
            return Length + a_item.Length;
        }

        #endregion
        
        #region /// Constructors ...

        /// <summary>
        /// Creates the shared memory array with the name specified by <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the shared memory array to be created.</param>
        /// <param name="length">The number of elements to make room for within the shared memory array.</param>
        public Array(string name, int length)
            : base(name, Marshal.SizeOf(typeof(T)) * length, true)
        {
            Capicity = length;
            Length = 0;

            _elementSize = Marshal.SizeOf(typeof(T));

            Open();

            mmf_Length_init();
        }

        public Array(string name, int length, bool ownsSharedMemory = true, bool xxxx = true)
            : base(name, Marshal.SizeOf(typeof(T)) * length, ownsSharedMemory)
        {
            Capicity = length;
            Length = 0;

            _elementSize = Marshal.SizeOf(typeof(T));

            Open();

            mmf_Length_init();
        }

        /// <summary>
        /// Opens an existing shared memory array with the name as specified by <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the shared memory array to open.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the shared memory location specified by <paramref name="name"/> does not have a <see cref="Buffer.BufferSize"/> that is evenly divisable by the size of <typeparamref name="T"/>.</exception>
        public Array(string name)
            : base(name, 0, false)
        {
            Length = 0;

            _elementSize = Marshal.SizeOf(typeof(T));

            Open();

            mmf_Length_init();
        }

        #endregion

        private void event_data_change() 
        {
            mmf_UpdateFlag_when_change_data();

            OnEventUpdated(new eventUpdatedArgs() { });
        }

        #region /// Update flag, update time lastest ...
        
        private static BufferReadWrite mmf_UpdateFlag;
        private void mmf_Update_Flag_init()
        {
            if (IsOwnerOfSharedMemory)
            {
                mmf_UpdateFlag = new BufferReadWrite(Name + "_flag_update", 8, true);
            }
            else
            {
                mmf_UpdateFlag = new BufferReadWrite(Name + "_flag_update", 8, false);
            }
        }

        private void mmf_UpdateFlag_when_change_data()
        {
            mmf_UpdateFlag.Write((IntPtr pw) =>
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                long idate = 0;
                long.TryParse(date, out idate);

                byte[] bArray = BitConverter.GetBytes(idate);
                Marshal.Copy(bArray, 0, pw, 1);
            });
        }

        /// <summary>
        /// get date time lastest database updated
        /// </summary>
        /// <returns></returns>
        public long Update_DatetimeLastest()
        {
            long time = 0;

            using (var read = new BufferReadWrite(Name + "_flag_update"))
            {
                read.Read((IntPtr pr) =>
                {
                    byte[] bArray = new byte[8];
                    Marshal.Copy(pr, bArray, 0, 8);
                    time = BitConverter.ToInt64(bArray, 0);
                });
            }

            return time;
        }
        
        #endregion

        #region /// LengthCurrent: event Length when update ...
        
        private BufferReadWrite mmf_Length;
        private void mmf_Length_init()
        {
            if (IsOwnerOfSharedMemory)
            {
                mmf_Length = new BufferReadWrite(Name + "_length", 16, true);
            }
            else
            {
                mmf_Length = new BufferReadWrite(Name + "_length", 16, false);
            }
        }

        public int LengthCurrent
        {
            get
            {
                int len = 0;
                mmf_Length.Read((IntPtr pr) =>
                {
                    byte[] bArray = new byte[16];
                    Marshal.Copy(pr, bArray, 0, 16);
                    len = BitConverter.ToInt32(bArray, 0);
                });
                return len;
            }
        }

        public event EventHandler<eventUpdatedArgs> eventUpdated;

        public void OnEventUpdated(eventUpdatedArgs e)
        {
            EventHandler<eventUpdatedArgs> handler = eventUpdated;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        private void mmf_Length_update(int len_add)
        {
            int len = 0;
            using (var read = new BufferReadWrite(Name + "_length"))
            {
                read.Read((IntPtr pr) =>
                {
                    byte[] bArray = new byte[16];
                    Marshal.Copy(pr, bArray, 0, 16);
                    len = BitConverter.ToInt32(bArray, 0);

                    mmf_Length.Write((IntPtr pw) =>
                    {
                        len = len + len_add;
                        bArray = BitConverter.GetBytes(len);
                        Marshal.Copy(bArray, 0, pw, 1);
                    });
                });
            }

            event_data_change();
        }

        #endregion

        /// <summary>
        /// Perform any initialisation required when opening the shared memory array
        /// </summary>
        /// <returns>true if successful</returns>
        protected override bool DoOpen()
        {
            if (!IsOwnerOfSharedMemory)
            {
                if (BufferSize % _elementSize != 0)
                    throw new ArgumentOutOfRangeException("name", "BufferSize is not evenly divisable by the size of " + typeof(T).Name);

                Capicity = (int)(BufferSize / _elementSize);
            }
            return true;
        }
        
        #region /// Writing ...

        /// <summary>
        /// Copy <paramref name="data"/> to the shared memory array element at index <paramref name="index"/>.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        /// <param name="index">The zero-based index of the element to set.</param>
        public void Write(ref T data, int index)
        {
           // if (index > Length - 1 || index < 0)
           //     throw new ArgumentOutOfRangeException("index");

            base.Write(ref data, index * _elementSize);
        }

        /// <summary>
        /// Copy the elements of the array <paramref name="buffer"/> into the shared memory array starting at index <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="buffer">The source array to copy elements from.</param>
        /// <param name="startIndex">The zero-based index of the shared memory array element to begin writing to.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 -or- length of <paramref name="buffer"/> + <paramref name="startIndex"/> is greater than <see cref="Capicity"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> must not be null</exception>
        public void Write(T[] buffer, int startIndex = 0)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (buffer.Length + startIndex > Capicity || startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");

            base.Write(buffer, startIndex * _elementSize);
        }

        #endregion

        #region /// Reading ...

        // public long Query

        /// <summary>
        /// Reads a single element from the shared memory array into <paramref name="data"/> located at <paramref name="index"/>.
        /// </summary>
        /// <param name="data">The element at the specified index.</param>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 -or- index is equal to or greater than <see cref="Capicity"/>.</exception>
        public void Read(out T data, int index)
        {
           // if (index > Length - 1 || index < 0)
           //     throw new ArgumentOutOfRangeException("index");

            base.Read(out data, index * _elementSize);
        }

        /// <summary>
        /// Reads buffer.Length elements from the shared memory array into <paramref name="buffer"/> starting at the shared memory array element located at <paramref name="startIndex"/>.
        /// </summary>
        /// <param name="buffer">The destination array to copy the elements into.</param>
        /// <param name="startIndex">The zero-based index of the shared memory array element to begin reading from.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is less than 0 -or- length of <paramref name="buffer"/> + <paramref name="startIndex"/> is greater than <see cref="Capicity"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> must not be null</exception>
        public void CopyTo(T[] buffer, int startIndex = 0)
        {
            // if (buffer == null)
            //    throw new ArgumentNullException("buffer");
            // if (buffer.Length + startIndex > Length || startIndex < 0)
            //    throw new ArgumentOutOfRangeException("startIndex");

            if (buffer != null)
                base.Read(buffer, startIndex * _elementSize);
        }

        #endregion

        #region /// IEnumerable<T>; Query ...

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerator{T}"/> instance that can be used to iterate through the collection</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerable<T> Query(Func<T, bool> where)
        {
            return this.Where(where);
        }

        public T[] QueryArray(Func<T, bool> where)
        {
            return this.Where(where).ToArray();
        }

        #endregion
    }


    public class eventUpdatedArgs : EventArgs
    {
        public int pkey { get; set; }
        public int id { get; set; }
        public int total { get; set; }
        public string json { get; set; }
    }
}
