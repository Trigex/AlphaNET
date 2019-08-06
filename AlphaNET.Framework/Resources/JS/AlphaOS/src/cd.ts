/// <reference path="kernel/kernel.d.ts" />
function cd() {
    var dir: FSDirectory;

    if(Global.ProcessArguments[1]) {
        dir = Filesystem.GetDirectoryByTitle(Global.ProcessArguments[1]);
        Global.CurrentPath = dir;
    } else {
        Terminal.WriteLine("No directory was supplied!");
        return 1;
    }

    return 0;
}

cd();