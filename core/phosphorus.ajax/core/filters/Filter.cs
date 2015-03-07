/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Text;

/// <summary>
///     Contains all the HTTP response filters in Phosphorus.Ajax.
/// 
///     Contains the HTTP Response Filters necessary to render the correct response back to client, depending upon what type of
///     request we're handling. Normally you do not have to fiddle with anything inside of this namespace, since the framework
///     takes care of the filters for your response automatically.
/// </summary>
namespace phosphorus.ajax.core.filters
{
    /// <summary>
    ///     Base class for all http response filters in phosphorus.ajax.
    /// 
    ///     The Filter class is used to override the response back
    ///     to the client, to make sure we either return an HTML document containing the necessary JavaScript inclusions, or a
    ///     JSON object during Ajax requests. Normally you do not need to fiddle with this class yourself, since the framework
    ///     takes care of initializing instances of this, on a per need basis automatically. If you absolutely must access
    ///     the Filter for your page, you can do so by accessing it through Page.Response.Filter.
    /// </summary>
    public abstract class Filter : Stream
    {
        private readonly Stream _next;
        private readonly MemoryStream _stream;

        /// <summary>
        ///     Initializes a new instance of the Filter class.
        /// </summary>
        /// <param name="manager">The manager for this filter</param>
        protected Filter (Manager manager)
        {
            Manager = manager;
            _stream = new MemoryStream ();
            _next = Manager.Page.Response.Filter;
            Encoding = manager.Page.Response.ContentEncoding;
        }

        /// <summary>
        ///     Returns the manager for this filter.
        /// </summary>
        /// <value>the manager</value>
        protected Manager Manager { get; private set; }

        /// <summary>
        ///     Gets the encoding used when rendering the response.
        /// 
        ///     The Encoding is extracted from your Page
        ///     instance during intialization. If you wish to override the Encoding used while rendering your
        ///     response, then change the Encoding on your Page object, before instantiation of your Manager instance.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding { get; set; }

        /// <summary>
        ///     Returns a value indicating whether this instance can read.
        /// </summary>
        /// <value><c>true</c> if this instance can read; otherwise, <c>false</c>.</value>
        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        /// <summary>
        ///     Returns a value indicating whether this instance can seek.
        /// </summary>
        /// <value><c>true</c> if this instance can seek; otherwise, <c>false</c>.</value>
        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        /// <summary>
        ///     Returns a value indicating whether this instance can write.
        /// </summary>
        /// <value><c>true</c> if this instance can write; otherwise, <c>false</c>.</value>
        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        /// <summary>
        ///     Returns the length of the stream.
        /// </summary>
        /// <value>The length.</value>
        public override long Length
        {
            get { return _stream.Length; }
        }

        /// <summary>
        ///     Gets or sets the position of the stream.
        /// </summary>
        /// <value>The position.</value>
        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        /// <summary>
        ///     Returns a value indicating whether this instance can timeout.
        /// </summary>
        /// <value><c>true</c> if this instance can timeout; otherwise, <c>false</c>.</value>
        public override bool CanTimeout
        {
            get { return _stream.CanTimeout; }
        }

        /// <summary>
        ///     Gets or sets the read timeout.
        /// </summary>
        /// <value>The read timeout.</value>
        public override int ReadTimeout
        {
            get { return _stream.ReadTimeout; }
            set { _stream.ReadTimeout = value; }
        }

        /// <summary>
        ///     Gets or sets the write timeout.
        /// </summary>
        /// <value>The write timeout.</value>
        public override int WriteTimeout
        {
            get { return _stream.WriteTimeout; }
            set { _stream.WriteTimeout = value; }
        }

        /// <summary>
        ///     Renders the response, override this method and return the rendered response, 
        ///     if you create your own Filter classes.
        /// </summary>
        /// <returns>The response rendered back to the client.</returns>
        protected abstract string RenderResponse ();

        /// <summary>
        ///     Closes the stream and renders its content to the next filter in the chain of filters.
        /// </summary>
        public override void Close ()
        {
            _stream.Seek (0, SeekOrigin.Begin);
            var responseContent = RenderResponse ();
            base.Close ();
            var buffer = Encoding.GetBytes (responseContent);
            _next.Write (buffer, 0, buffer.Length);
        }

        /// <summary>
        ///     Flush this instance.
        /// </summary>
        public override void Flush () { _stream.Flush (); }

        /// <summary>
        ///     Seek the specified offset and origin.
        /// </summary>
        /// <param name="offset">Offset.</param>
        /// <param name="origin">Origin from where to count your offset from.</param>
        public override long Seek (long offset, SeekOrigin origin) { return _stream.Seek (offset, origin); }

        /// <summary>
        ///     Sets the length.
        /// </summary>
        /// <param name="value">New length of stream.</param>
        public override void SetLength (long value) { _stream.SetLength (value); }

        /// <summary>
        ///     Reads into the specified buffer, offset and count.
        /// </summary>
        /// <param name="buffer">Buffer to hold the content you wish to read.</param>
        /// <param name="offset">Offset from where you start reading.</param>
        /// <param name="count">Number of bytes to read.</param>
        public override int Read (byte[] buffer, int offset, int count) { return _stream.Read (buffer, offset, count); }

        /// <summary>
        ///     Write the specified buffer's content, offset and count.
        /// </summary>
        /// <param name="buffer">Buffer containing bytes to write.</param>
        /// <param name="offset">Offset from where to start reading from the buffer.</param>
        /// <param name="count">Number of bytes to write.</param>
        public override void Write (byte[] buffer, int offset, int count) { _stream.Write (buffer, offset, count); }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> disposing</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing)
                _stream.Dispose ();
            base.Dispose (disposing);
        }

        /// <summary>
        ///     Reads one byte.
        /// </summary>
        /// <returns>The byte it just read.</returns>
        public override int ReadByte () { return _stream.ReadByte (); }

        /// <summary>
        ///     Writes one byte.
        /// </summary>
        /// <param name="value">The byte you wish to write.</param>
        public override void WriteByte (byte value) { _stream.WriteByte (value); }
    }
}
