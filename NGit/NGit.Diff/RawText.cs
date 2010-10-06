using NGit.Diff;
using NGit.Util;
using Sharpen;

namespace NGit.Diff
{
	/// <summary>A Sequence supporting UNIX formatted text in byte[] format.</summary>
	/// <remarks>
	/// A Sequence supporting UNIX formatted text in byte[] format.
	/// <p>
	/// Elements of the sequence are the lines of the file, as delimited by the UNIX
	/// newline character ('\n'). The file content is treated as 8 bit binary text,
	/// with no assumptions or requirements on character encoding.
	/// <p>
	/// Note that the first line of the file is element 0, as defined by the Sequence
	/// interface API. Traditionally in a text editor a patch file the first line is
	/// line number 1. Callers may need to subtract 1 prior to invoking methods if
	/// they are converting from "line number" to "element index".
	/// </remarks>
	public class RawText : Sequence
	{
		/// <summary>
		/// Number of bytes to check for heuristics in
		/// <see cref="IsBinary(byte[])">IsBinary(byte[])</see>
		/// 
		/// </summary>
		private const int FIRST_FEW_BYTES = 8000;

		/// <summary>The file content for this sequence.</summary>
		/// <remarks>The file content for this sequence.</remarks>
		protected internal readonly byte[] content;

		/// <summary>
		/// Map of line number to starting position within
		/// <see cref="content">content</see>
		/// .
		/// </summary>
		protected internal readonly IntList lines;

		/// <summary>Create a new sequence from an existing content byte array.</summary>
		/// <remarks>
		/// Create a new sequence from an existing content byte array.
		/// <p>
		/// The entire array (indexes 0 through length-1) is used as the content.
		/// </remarks>
		/// <param name="input">
		/// the content array. The array is never modified, so passing
		/// through cached arrays is safe.
		/// </param>
		public RawText(byte[] input) : this(RawTextComparator.DEFAULT, input)
		{
		}

		/// <summary>Create a new sequence from an existing content byte array.</summary>
		/// <remarks>
		/// Create a new sequence from an existing content byte array.
		/// The entire array (indexes 0 through length-1) is used as the content.
		/// </remarks>
		/// <param name="cmp">comparator that will later be used to compare texts.</param>
		/// <param name="input">
		/// the content array. The array is never modified, so passing
		/// through cached arrays is safe.
		/// </param>
		public RawText(RawTextComparator cmp, byte[] input)
		{
			content = input;
			lines = RawParseUtils.LineMap(content, 0, content.Length);
		}

		/// <summary>Create a new sequence from a file.</summary>
		/// <remarks>
		/// Create a new sequence from a file.
		/// <p>
		/// The entire file contents are used.
		/// </remarks>
		/// <param name="file">the text file.</param>
		/// <exception cref="System.IO.IOException">if Exceptions occur while reading the file
		/// 	</exception>
		public RawText(FilePath file) : this(IOUtil.ReadFully(file))
		{
		}

		/// <returns>total number of items in the sequence.</returns>
		public override int Size()
		{
			// The line map is always 2 entries larger than the number of lines in
			// the file. Index 0 is padded out/unused. The last index is the total
			// length of the buffer, and acts as a sentinel.
			//
			return lines.Size() - 2;
		}

		/// <summary>Write a specific line to the output stream, without its trailing LF.</summary>
		/// <remarks>
		/// Write a specific line to the output stream, without its trailing LF.
		/// <p>
		/// The specified line is copied as-is, with no character encoding
		/// translation performed.
		/// <p>
		/// If the specified line ends with an LF ('\n'), the LF is <b>not</b>
		/// copied. It is up to the caller to write the LF, if desired, between
		/// output lines.
		/// </remarks>
		/// <param name="out">stream to copy the line data onto.</param>
		/// <param name="i">
		/// index of the line to extract. Note this is 0-based, so line
		/// number 1 is actually index 0.
		/// </param>
		/// <exception cref="System.IO.IOException">the stream write operation failed.</exception>
		public virtual void WriteLine(OutputStream @out, int i)
		{
			int start = lines.Get(i + 1);
			int end = lines.Get(i + 2);
			if (content[end - 1] == '\n')
			{
				end--;
			}
			@out.Write(content, start, end - start);
		}

		/// <summary>Determine if the file ends with a LF ('\n').</summary>
		/// <remarks>Determine if the file ends with a LF ('\n').</remarks>
		/// <returns>true if the last line has an LF; false otherwise.</returns>
		public virtual bool IsMissingNewlineAtEnd()
		{
			int end = lines.Get(lines.Size() - 1);
			if (end == 0)
			{
				return true;
			}
			return content[end - 1] != '\n';
		}

		/// <summary>
		/// Determine heuristically whether a byte array represents binary (as
		/// opposed to text) content.
		/// </summary>
		/// <remarks>
		/// Determine heuristically whether a byte array represents binary (as
		/// opposed to text) content.
		/// </remarks>
		/// <param name="raw">the raw file content.</param>
		/// <returns>true if raw is likely to be a binary file, false otherwise</returns>
		public static bool IsBinary(byte[] raw)
		{
			return IsBinary(raw, raw.Length);
		}

		/// <summary>
		/// Determine heuristically whether the bytes contained in a stream
		/// represents binary (as opposed to text) content.
		/// </summary>
		/// <remarks>
		/// Determine heuristically whether the bytes contained in a stream
		/// represents binary (as opposed to text) content.
		/// Note: Do not further use this stream after having called this method! The
		/// stream may not be fully read and will be left at an unknown position
		/// after consuming an unknown number of bytes. The caller is responsible for
		/// closing the stream.
		/// </remarks>
		/// <param name="raw">input stream containing the raw file content.</param>
		/// <returns>true if raw is likely to be a binary file, false otherwise</returns>
		/// <exception cref="System.IO.IOException">if input stream could not be read</exception>
		public static bool IsBinary(InputStream raw)
		{
			byte[] buffer = new byte[FIRST_FEW_BYTES];
			int cnt = 0;
			while (cnt < buffer.Length)
			{
				int n = raw.Read(buffer, cnt, buffer.Length - cnt);
				if (n == -1)
				{
					break;
				}
				cnt += n;
			}
			return IsBinary(buffer, cnt);
		}

		/// <summary>
		/// Determine heuristically whether a byte array represents binary (as
		/// opposed to text) content.
		/// </summary>
		/// <remarks>
		/// Determine heuristically whether a byte array represents binary (as
		/// opposed to text) content.
		/// </remarks>
		/// <param name="raw">the raw file content.</param>
		/// <param name="length">
		/// number of bytes in
		/// <code>raw</code>
		/// to evaluate. This should be
		/// <code>raw.length</code>
		/// unless
		/// <code>raw</code>
		/// was over-allocated by
		/// the caller.
		/// </param>
		/// <returns>true if raw is likely to be a binary file, false otherwise</returns>
		public static bool IsBinary(byte[] raw, int length)
		{
			// Same heuristic as C Git
			if (length > FIRST_FEW_BYTES)
			{
				length = FIRST_FEW_BYTES;
			}
			for (int ptr = 0; ptr < length; ptr++)
			{
				if (raw[ptr] == '\0')
				{
					return true;
				}
			}
			return false;
		}
	}
}