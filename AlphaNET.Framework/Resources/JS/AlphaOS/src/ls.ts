/// <reference path="kernel/types/os.d.ts" />
function Main(args: string[]) {
    Terminal.WriteLine("ls called");
    var dir;

    if(args[1]) {
        dir = Filesystem.GetFilesystemObjectByAbsolutePath(args[1]);
    } else {
        dir = Global.CurrentPath as DIRECTORY;
    }

    let children = dir.Children.ToArray();

    for(let i=0;i<children.length;i++) {
        let child = children[i] as FilesystemObject;
        Terminal.WriteLine(child.Title);
    }

    return 0;
}