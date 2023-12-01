using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenRecorder.FinalVer.CloudStorage
{
    public static class FileContentTypeExtension
    {
        private static readonly FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();

        public static string GetContentType(this string filename)
        {
            if(!provider.TryGetContentType(filename, out string contentType))
            {
                return "application/octet-stream";
            }
            return contentType;
        }
    }
}
