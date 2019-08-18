/// <reference path="kernel/types/os.d.ts" />
/// <reference path="kernel/types/thirdparty.d.ts" />
/// <reference path="kernel/system.ts" />
var file: FILE;

function cat(args: string[]) {
    if(args[0]) {
        file = Filesystem.GetObjectByAbsolutePath(args[0]) as FILE;
        if(file.IsPlaintext)
        {
            Terminal.WriteLine(UTF8.GetString(file.Contents));
            return;
        }
    } else {
        Terminal.WriteLine("No file path argument was provided");
        return;
    }
}

cat(args);