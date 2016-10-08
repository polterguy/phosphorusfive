/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.IO;
using System.Text;

/// <summary>
///     Contains all the HTTP response filters in p5.ajax
/// </summary>
namespace p5.ajax.core.filters
{
    /// <summary>
    ///     Base class for all http response filters in p5.ajax
    /// </summary>
    public abstract class Filter : Stream
    {
        private readonly Stream _next;
        private readonly MemoryStream _stream;

        /// <summary>
        ///     Initializes a new instance of the Filter class
        /// </summary>
        /// <param name="manager">The manager for this filter</param>
        protected Filter (Manager manager)
        {
            Manager = manager;
            _stream = new MemoryStream ();
            _next = Manager.Page.Response.Filter;
            ContentEncoding = Manager.Page.Response.ContentEncoding;
        }

        /// <summary>
        ///     Returns the manager for this filter
        /// </summary>
        /// <value>the manager</value>
        protected Manager Manager
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the encoding used when rendering the response
        /// </summary>
        /// <value>The encoding</value>
        protected Encoding ContentEncoding
        {
            get;
            private set;
        }

        /// <summary>
        ///     Returns a value indicating whether this instance can read
        /// </summary>
        /// <value><c>true</c> if this instance can read; otherwise, <c>false</c></value>
        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        /// <summary>
        ///     Returns a value indicating whether this instance can seek
        /// </summary>
        /// <value><c>true</c> if this instance can seek; otherwise, <c>false</c></value>
        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        /// <summary>
        ///     Returns a value indicating whether this instance can write
        /// </summary>
        /// <value><c>true</c> if this instance can write; otherwise, <c>false</c></value>
        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        /// <summary>
        ///     Returns the length of the stream
        /// </summary>
        /// <value>The length</value>
        public override long Length
        {
            get { return _stream.Length; }
        }

        /// <summary>
        ///     Gets or sets the position of the stream
        /// </summary>
        /// <value>The position</value>
        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        /// <summary>
        ///     Returns a value indicating whether this instance can timeout
        /// </summary>
        /// <value><c>true</c> if this instance can timeout; otherwise, <c>false</c></value>
        public override bool CanTimeout
        {
            get { return _stream.CanTimeout; }
        }

        /// <summary>
        ///     Gets or sets the read timeout
        /// </summary>
        /// <value>The read timeout</value>
        public override int ReadTimeout
        {
            get { return _stream.ReadTimeout; }
            set { _stream.ReadTimeout = value; }
        }

        /// <summary>
        ///     Gets or sets the write timeout
        /// </summary>
        /// <value>The write timeout</value>
        public override int WriteTimeout
        {
            get { return _stream.WriteTimeout; }
            set { _stream.WriteTimeout = value; }
        }

        /// <summary>
        ///     Renders the response
        /// </summary>
        /// <returns>The response rendered back to the client</returns>
        protected abstract string RenderResponse ();

        /// <summary>
        ///     Closes the stream and renders its content to the next filter in the chain of filters
        /// </summary>
        public override void Close ()
        {
            _stream.Seek (0, SeekOrigin.Begin);
            var responseContent = RenderResponse ();
            base.Close ();
            var buffer = ContentEncoding.GetBytes (responseContent);
            _next.Write (buffer, 0, buffer.Length);
        }

        /// <summary>
        ///     Flush this instance
        /// </summary>
        public override void Flush ()
        {
            _stream.Flush ();
        }

        /// <summary>
        ///     Seek the specified offset and origin
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="origin">Origin from where to count your offset from</param>
        public override long Seek (long offset, SeekOrigin origin)
        {
            return _stream.Seek (offset, origin);
        }

        /// <summary>
        ///     Sets the length
        /// </summary>
        /// <param name="value">New length of stream</param>
        public override void SetLength (long value)
        {
            _stream.SetLength (value);
        }

        /// <summary>
        ///     Reads into the specified buffer, offset and count
        /// </summary>
        /// <param name="buffer">Buffer to hold the content you wish to read</param>
        /// <param name="offset">Offset from where you start reading</param>
        /// <param name="count">Number of bytes to read</param>
        public override int Read (byte[] buffer, int offset, int count)
        {
            return _stream.Read (buffer, offset, count);
        }

        /// <summary>
        ///     Write the specified buffer's content, offset and count
        /// </summary>
        /// <param name="buffer">Buffer containing bytes to write</param>
        /// <param name="offset">Offset from where to start reading from the buffer</param>
        /// <param name="count">Number of bytes to write</param>
        public override void Write (byte[] buffer, int offset, int count)
        {
            _stream.Write (buffer, offset, count);
        }

        /// <summary>
        ///     Disposes this instance
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> disposing</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing)
                _stream.Dispose ();
            base.Dispose (disposing);
        }

        /// <summary>
        ///     Reads one byte
        /// </summary>
        /// <returns>The byte it just read</returns>
        public override int ReadByte ()
        {
            return _stream.ReadByte ();
        }

        /// <summary>
        ///     Writes one byte
        /// </summary>
        /// <param name="value">The byte you wish to write</param>
        public override void WriteByte (byte value)
        {
            _stream.WriteByte (value);
        }
    }
}
