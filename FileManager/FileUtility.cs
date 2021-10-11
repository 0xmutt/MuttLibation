﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileManager
{
    public static class FileUtility
    {
//public static string GetValidFilename(string template, Dictionary<string, object> parameters) { }
        public static string GetValidFilename(string dirFullPath, string filename, string extension, params string[] metadataSuffixes)
        {
            if (string.IsNullOrWhiteSpace(dirFullPath))
                throw new ArgumentException($"{nameof(dirFullPath)} may not be null or whitespace", nameof(dirFullPath));

            filename ??= "";

            // file max length = 255. dir max len = 247

            // sanitize. omit invalid characters. exception: colon => underscore
            filename = filename.Replace(':', '_');
            filename = Dinah.Core.PathLib.ToPathSafeString(filename);

            // manage length
            if (filename.Length > 50)
                filename = filename.Substring(0, 50) + "[...]";

            // append metadata
            if (metadataSuffixes != null && metadataSuffixes.Length > 0)
                filename += " [" + string.Join("][", metadataSuffixes) + "]";

            // extension is null when this method is used for directory names
            if (!string.IsNullOrWhiteSpace(extension))
                extension = '.' + extension.Trim('.');

            // ensure uniqueness
            var fullfilename = Path.Combine(dirFullPath, filename + extension);
            var i = 0;
            while (File.Exists(fullfilename))
                fullfilename = Path.Combine(dirFullPath, filename + $" ({++i})" + extension);

            return fullfilename;
        }
    }
}
