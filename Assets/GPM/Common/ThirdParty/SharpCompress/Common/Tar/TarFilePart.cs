#if CSHARP_7_3_OR_NEWER

using System.IO;
using Gpm.Common.ThirdParty.SharpCompress.Common.Tar.Headers;
using Gpm.Common.ThirdParty.SharpCompress.IO;

namespace Gpm.Common.ThirdParty.SharpCompress.Common.Tar
{
    internal class TarFilePart : FilePart
    {
        private readonly Stream _seekableStream;

        internal TarFilePart(TarHeader header, Stream seekableStream)
            : base(header.ArchiveEncoding)
        {
            _seekableStream = seekableStream;
            Header = header;
        }

        internal TarHeader Header { get; }

        internal override string FilePartName => Header.Name;

        internal override Stream GetCompressedStream()
        {
            if (_seekableStream != null)
            {
                _seekableStream.Position = Header.DataStartPosition.Value;
                return new ReadOnlySubStream(_seekableStream, Header.Size);
            }
            return Header.PackedStream;
        }

        internal override Stream GetRawStream()
        {
            return null;
        }
    }
}

#endif