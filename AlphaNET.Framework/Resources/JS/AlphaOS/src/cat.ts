/// <reference path="kernel/types/os.d.ts" />
function Main() {
    var file: FILE;

    if(Global.ProcessArguments[1]) {
        file = Filesystem.GetFilesystemObjectByAbsolutePath(Global.ProcessArguments[1]) as FILE;
    } else {
        Terminal.WriteLine("No file was supplied!");
        return 1;
    }

    Terminal.WriteLine(UTF8.GetString(file.Contents));

    return 0;
}