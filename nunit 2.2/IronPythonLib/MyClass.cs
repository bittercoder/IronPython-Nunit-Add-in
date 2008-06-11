using System;
using System.Collections.Generic;
using System.IO;

namespace IronPythonLib
{
    internal class SimpleFile
    {
        internal readonly byte[] Contents;
        internal readonly string MimeType;

        internal SimpleFile(byte[] contents, string mimeType)
        {
            Contents = contents;
            MimeType = mimeType;
        }
    }

    /// <summary>
    /// Sample implementation of the IMyFileStore interface, used in this post: http://blog.bittercoder.com/PermaLink,guid,243d59b7-0f11-4ee1-9f64-5aaafd8d0e14.aspx
    /// </summary>
    public class MyFileStore : IMyFileStore
    {
        private const string DefaultMimeType = "application/octet-stream";
        private Dictionary<Guid, SimpleFile> _files = new Dictionary<Guid, SimpleFile>();

        public Guid AddFile(string fileName)
        {
            return AddFile(fileName, DefaultMimeType);
        }

        public Guid AddFile(string fileName, string mimeType)
        {
            return AddFile(File.ReadAllBytes(fileName), mimeType);
        }

        public Guid AddFile(byte[] contents, string mimeType)
        {
            Guid id = Guid.NewGuid();
            SimpleFile file = new SimpleFile(contents, mimeType);
            _files.Add(id, file);
            return id;
        }

        public Guid[] AddFiles(params string[] fileNames)
        {
            Guid[] identifiers = new Guid[fileNames.Length];

            for (int i = 0; i < fileNames.Length; i++)
            {
                identifiers[i] = AddFile(fileNames[i]);
            }

            return identifiers;
        }

        public Guid AddFile(byte[] contents)
        {
            return AddFile(contents, DefaultMimeType);
        }

        public byte[] ReadFile(Guid id, out string mimeType)
        {
            SimpleFile file = _files[id];
            mimeType = file.MimeType;

            return file.Contents;
        }

        public void ReadFile(Guid id, string fileName)
        {
            SimpleFile file = _files[id];
            File.WriteAllBytes(fileName, file.Contents);
        }

        public void ExtractFiles(ref IList<byte[]> myList)
        {
            if (myList == null) myList = new List<byte[]>();
            foreach (SimpleFile file in _files.Values) myList.Add(file.Contents);
        }
    }
}