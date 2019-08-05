/// <reference path="kernel/kernel.d.ts" />

function cc() {
    let file: FSFile;

    if(Global.ProcessArguments[1]) {
        file = Filesystem.GetFilesystemObjectByAbsolutePath(Global.ProcessArguments[1]) as FSFile;
        let output = TypescriptCompiler.CompileTypescript(UTF_8.GetString(file.Contents));
    } else {
        Terminal.WriteLine("Please supply a file");
        return;
    }
}

cc();