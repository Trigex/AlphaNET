/// <reference path="kernel/types/os.d.ts" />
/// <reference path="kernel/system.ts" />

Terminal.WriteLine("AlphaOS Init");
let shell = Filesystem.GetObjectByAbsolutePath("/bin/shell.js") as FILE;
JS.ExecuteFromFile(shell, [], true);