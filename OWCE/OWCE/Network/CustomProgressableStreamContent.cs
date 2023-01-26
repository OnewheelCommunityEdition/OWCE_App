using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OWCE.Network
{
    public class CustomProgressableStreamContent : HttpContent
    {
        FileStream fileStream;
        int bufferSize = 4096;
        IProgress<double> progress;
        int lastProgress = 0;

        public CustomProgressableStreamContent(FileStream fileStream, IProgress<double> progress)
        {
            //ArgumentNullException.ThrowIfNull(fileStream);
            //ArgumentNullException.ThrowIfNull(progress);

            this.fileStream = fileStream;
            this.progress = progress;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            return SerializeToStreamAsync(stream, context, default(CancellationToken));
        }

        protected async Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
        {
            var buffer = new byte[bufferSize];
            var size = fileStream.Length;
            var uploaded = 0;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var length = await fileStream.ReadAsync(buffer, 0, bufferSize).ConfigureAwait(false);

                if (length <= 0)
                {
                    break;
                }

                uploaded += length;

                await stream.WriteAsync(buffer, 0, length).ConfigureAwait(false);



                // Only report progress when we have actually gone up a percent
                var currentProgress = (double)uploaded / size;
                var currentProgressInt = (int)(currentProgress * 100);
                if (lastProgress != currentProgressInt)
                {
                    progress.Report(currentProgress);
                    lastProgress = currentProgressInt;
                }
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = fileStream.Length;
            return true;
        }
    }

}

