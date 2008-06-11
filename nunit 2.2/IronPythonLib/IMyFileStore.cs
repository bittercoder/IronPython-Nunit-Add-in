using System;
using System.Collections.Generic;

namespace IronPythonLib
{
    /// <summary>
    /// This interface was explored in the post here: http://blog.bittercoder.com/PermaLink,guid,243d59b7-0f11-4ee1-9f64-5aaafd8d0e14.aspx
    /// </summary>
    public interface IMyFileStore
    {
        Guid AddFile(string fileName);
        Guid AddFile(string fileName, string mimeType);
        Guid AddFile(byte[] contents, string mimeType);
        Guid[] AddFiles(params string[] fileNames);
        Guid AddFile(byte[] contents);
        byte[] ReadFile(Guid id, out string mimeType);
        void ReadFile(Guid id, string fileName);
        void ExtractFiles(ref IList<byte[]> myList);
    }
}