using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;

namespace AlphaNET.IO
{
    [MoonSharpUserData]
    class File
    {
        /// <summary>
        /// Title of this file
        /// </summary>
        public string title { get; } // title of this file
        /// <summary>
        /// String data the file holds
        /// </summary>
        public string data { get; set; } // data the file holds
        /// <summary>
        /// Directory this file is located in
        /// </summary>
        public Directory parentDirectory { get; set; } // directory that holds this file

        /// <summary>
        /// Constructs file
        /// </summary>
        /// <param name="title">Title of the file</param>
        /// <param name="data">The string the file should contain</param>
        /// <param name="parentDirectory">Optional directory to contain this file</param>
        public File(string title, string data, Directory parentDirectory = null)
        {
            this.title = title;
            this.data = data;
            if (parentDirectory != null)
                this.parentDirectory = parentDirectory;
        }


    }
}
