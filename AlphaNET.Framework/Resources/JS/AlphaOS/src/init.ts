/// <reference path="kernel/types/os.d.ts" />
/// <reference path="kernel/system.ts" />

function Main() {
    Terminal.WriteLine("AlphaOS Init");
    let shellProcess = fork();
    let shell = Filesystem.GetFilesystemObjectByAbsolutePath("/bin/shell.js") as FILE;
    JSInterpreter.ExecuteScript(UTF8.GetString(shell.Contents), false, null);
    return 0;
}