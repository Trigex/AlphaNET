/// <reference path="kernel/kernel.d.ts" />
function ls() {
    var dir;

    if(Global.ProcessArguments[1]) {
        dir = Filesystem.GetFilesystemObjectByAbsolutePath(Global.ProcessArguments[1]);
    } else {
        dir = Global.CurrentPath as FSDirectory;
    }

    let children = dir.Children.ToArray();

    for(let i=0;i<children.length;i++) {
        let child = children[i] as FilesystemObject;
        Terminal.WriteLine(child.Title);
    }

    return 0;
}

ls();