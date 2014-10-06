/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed readme.me file for details
 */

using System;
using System.IO;
using System.Text;
using System.Web.UI;
using phosphorus.ajax.core;

namespace phosphorus.ajax.core.filters
{
    /// <summary>
    /// base class for all http response filters in phosphorus.ajax
    /// </summary>
    public abstract class Filter : Stream
    {
        private Stream _next;
        private MemoryStream _stream;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.core.filters.Filter"/> class
        /// </summary>
        /// <param name="manager">the manager for this filter</param>
        public Filter (Manager manager)
        {
            Manager = manager;
            _stream = new MemoryStream();
            _next = (Manager.Page as Page).Response.Filter;
        }

        /// <summary>
        /// returns the manager for this filter
        /// </summary>
        /// <value>the manager</value>
        public Manager Manager {
            get;
            private set;
        }

        /// <summary>
        /// renders the response, override this method and return the rendered response
        /// </summary>
        /// <returns>the response</returns>
        protected abstract string RenderResponse ();
        
        public override void Close ()
        {
            _stream.Seek (0, SeekOrigin.Begin);
            string responseContent = RenderResponse ();
            base.Close ();
            byte[] buffer = Manager.Page.Response.ContentEncoding.GetBytes (responseContent);
            _next.Write (buffer, 0, buffer.Length);
        }

        /// <summary>
        /// returns a value indicating whether this instance can read
        /// </summary>
        /// <value><c>true</c> if this instance can read; otherwise, <c>false</c></value>
        public override bool CanRead {
            get { return _stream.CanRead; }
        }

        /// <summary>
        /// returns a value indicating whether this instance can seek
        /// </summary>
        /// <value><c>true</c> if this instance can seek; otherwise, <c>false</c></value>
        public override bool CanSeek {
            get { return _stream.CanSeek; }
        }

        /// <summary>
        /// returns a value indicating whether this instance can write
        /// </summary>
        /// <value><c>true</c> if this instance can write; otherwise, <c>false</c></value>
        public override bool CanWrite {
            get { return _stream.CanWrite; }
        }

        /// <summary>
        /// returns the length
        /// </summary>
        /// <value>the length</value>
        public override long Length {
            get { return _stream.Length; }
        }

        /// <summary>
        /// gets or sets the position of the stream
        /// </summary>
        /// <value>the position</value>
        public override long Position {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        /// <summary>
        /// flush this instance
        /// </summary>
        public override void Flush ()
        {
            _stream.Flush ();
        }

        /// <summary>
        /// seek the specified offset and origin
        /// </summary>
        /// <param name="offset">offset</param>
        /// <param name="origin">origin</param>
        public override long Seek (long offset, SeekOrigin origin)
        {
            return _stream.Seek (offset, origin);
        }

        /// <summary>
        /// sets the length
        /// </summary>
        /// <param name="value">length</param>
        public override void SetLength (long value)
        {
            _stream.SetLength (value);
        }

        /// <summary>
        /// reads into the specified buffer, offset and count
        /// </summary>
        /// <param name="buffer">fuffer</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        public override int Read (byte[] buffer, int offset, int count)
        {
            return _stream.Read (buffer, offset, count);
        }

        /// <summary>
        /// write the specified buffer, offset and count
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        public override void Write (byte[] buffer, int offset, int count)
        {
            _stream.Write (buffer, offset, count);
        }

        /// <summary>
        /// returns a value indicating whether this instance can timeout
        /// </summary>
        /// <value><c>true</c> if this instance can timeout; otherwise, <c>false</c></value>
        public override bool CanTimeout {
            get {
                return _stream.CanTimeout;
            }
        }

        /// <summary>
        /// disposes this instance
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> disposing</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing)
                _stream.Dispose ();
            base.Dispose (disposing);
        }

        /// <summary>
        /// reads one byte
        /// </summary>
        /// <returns>the byte</returns>
        public override int ReadByte ()
        {
            return _stream.ReadByte ();
        }

        /// <summary>
        /// gets or sets the read timeout
        /// </summary>
        /// <value>the read timeout</value>
        public override int ReadTimeout {
            get {
                return _stream.ReadTimeout;
            }
            set {
                _stream.ReadTimeout = value;
            }
        }

        /// <summary>
        /// writes one byte
        /// </summary>
        /// <param name="value">value</param>
        public override void WriteByte (byte value)
        {
            _stream.WriteByte (value);
        }

        /// <summary>
        /// gets or sets the write timeout
        /// </summary>
        /// <value>the write timeout</value>
        public override int WriteTimeout {
            get {
                return _stream.WriteTimeout;
            }
            set {
                _stream.WriteTimeout = value;
            }
        }
    }
}

