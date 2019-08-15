/// <reference path="kernel/types/os.d.ts" />

// Globals
const PROMPT = "> ";
const VERSION = 1.0;

var running = true;
var searchPath = ["/bin"];
Global.CurrentPath = Filesystem.GetDirectoryByTitle("root");

function Main() {
    Terminal.WriteLine(`AlphaShell version ${VERSION}`);
    Start();
}

function Start() {
    while (running) {
        let prompt = `${Filesystem.GetAbsolutePathByFilesystemObject(Global.CurrentPath)} ${PROMPT}`;
        // Set the CurrentPath every loop
        Terminal.Write(prompt);
        let input = Terminal.ReadLine();
        // check if the command is a built in util
        if(!BuiltInUtils(input))
        {
            let status = ParseCommand(input);
            if(status!=null)
            {
                Terminal.WriteLine(`Process ended with return code ${status}`);
            }
        }
    }
}

function BuiltInUtils(input: string): boolean {
    let status = false;

    switch(input) {
        case "clear":
            Terminal.Clear();
            status = true;
            break;
    }

    return status;
}

function ParseCommand(input: string) {
    // split string into array by spaces
    let command = input.split(" ");
    let filename = command[0];
    // check if filename doesn't include ".js"
    if(!(_.includes(filename, '.js')))
        filename += ".js";

    let binaryPath: string = "/bin";
    // search for binary
    // loop search paths
    for(let i=0;i<searchPath.length;i++)
    {
        // get fs object of search path
        let obj = Filesystem.GetFilesystemObjectByAbsolutePath(searchPath[i]);
        // if it's a directory, look through it's children for the binary
        if(obj instanceof DIRECTORY)
        {
            let dir = obj as DIRECTORY;
            for(let x=0;x<dir.Children.Count;x++)
            {
                if(dir.Children[x].Title == filename)
                {
                    let child = dir.Children[x] as FILE;
                    // Found matching child, set binary
                    binaryPath += Filesystem.GetAbsolutePathByFilesystemObject(child);
                }
            }
        }
    }

    // no binary was found
    if(binaryPath === undefined)
    {
        Terminal.WriteLine(`Unable to find binary "${filename}"!`);
        return null;
    }
    
    // create a new process for the program
    let process = fork();
    // execute program, return status output
    return exec(binaryPath, command, process);
}