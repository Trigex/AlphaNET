/// <reference path="kernel/types/os.d.ts" />
/// <reference path="kernel/types/thirdparty.d.ts" />
/// <reference path="kernel/system.ts" />
var _dir: DIRECTORY;

function cd(args: string[]) {
    if(args[0]) {
        _dir = Filesystem.GetObjectByAbsolutePath(args[0]) as DIRECTORY;
        if(_dir instanceof DIRECTORY)
            Global.CurrentPath = _dir;

    } else {
        Terminal.WriteLine("No path argument was provided");
        return;
    }
}

cd(args);