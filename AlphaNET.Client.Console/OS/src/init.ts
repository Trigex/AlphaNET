/// <reference path="kernel/kernel.d.ts" />
// Retrieve shell
const DEV_MODE = true; 

if(!DEV_MODE)
    Terminal.Clear();
    
Terminal.WriteLine("AlphaOS Init");
let _shell = Filesystem.GetFileByTitle("shell.js");
CreateProcess(_shell, []);