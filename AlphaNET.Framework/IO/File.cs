﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.IO
{
    public class File : IFilesystemObject<File>
    {
        public string Title { get; set; }
        public uint InodeNumber { get; set; }

        public File(string title, uint inodeNumber)
        {
            Title = title;
            InodeNumber = inodeNumber;
        }

        public File() { }

        public File Deserialize(byte[] buffer)
        {
            return FilesystemUtils.Deserialize<File>(buffer, (reader) =>
            {
                var title = reader.ReadBytes(Size.FileTitle - 1);
                var inodeNumber = reader.ReadUInt32();

                return new File(Encoding.UTF8.GetString(title), inodeNumber);
            })
        }

        public byte[] Serialize()
        {
            return FilesystemUtils.Serialize((writer) =>
            {
                writer.Write(Encoding.UTF8.GetBytes(FixTitle(Title)));
                writer.Write(InodeNumber);
            });
        }

        private string FixTitle(string title)
        {
            // strip title if too long
            if (title.Length > Size.FileTitle)
                title.Remove(Size.FileTitle - 1, title.Length - Size.FileTitle);
            else // pad string with ?
            {
                for(int i=title.Length;i<Size.FileTitle;i++)
                {
                    title += '?';
                }
            }

            return title;
        }
    }
}
