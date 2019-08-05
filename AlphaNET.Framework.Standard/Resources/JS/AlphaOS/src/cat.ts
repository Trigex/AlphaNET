/// <reference path="kernel/kernel.d.ts" />
function cat() {
    var file: FSFile;

    if(Global.ProcessArguments[1]) {
        file = Filesystem.GetFilesystemObjectByAbsolutePath(Global.ProcessArguments[1]) as FSFile;
    } else {
        Terminal.WriteLine("No file was supplied!");
        return 1;
    }

    Terminal.WriteLine(UTF_8.GetString(file.Contents));

    return 0;
}

cat();