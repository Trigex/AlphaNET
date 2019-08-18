/// <reference path="kernel/types/os.d.ts" />
/// <reference path="kernel/system.ts" />
/// <reference path="kernel/types/lodash/index.d.ts" />

// Globals
const PROMPT = "> ";
const VERSION = 1.0;

var running = true;
var searchPath = ["/bin"];
Global.CurrentPath = Filesystem.GetDirectoryByTitle("root");

function Start() {
    Terminal.WriteLine(`AlphaShell version ${VERSION}`);

    while (running) {
        let prompt = `${Filesystem.GetAbsolutePathByObject(Global.CurrentPath)} ${PROMPT}`;
        Terminal.Write(prompt);
        let input = Terminal.ReadLine();
        // check if the command is a built in util
        if(!BuiltInUtils(input))
        {
            let status = ParseCommand(input);
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
        case "exit":
            running = false;
            status = true;
            break;
    }

    return status;
}

function ParseCommand(input: string) {
    // split string into array by spaces
    let command = input.split(" ");
    let filename = command[0];
    let blocking: boolean;
    if(command[command.length-1] == '&')
        blocking = false;
    else
        blocking = true;

    // check if filename doesn't include ".js"
    if(!(_.includes(filename, '.js')))
        filename += ".js";

    let binary: FILE = SearchForBinary(filename);
    if(binary === null)
    {
        Terminal.WriteLine(`Unable to find binary "${filename}"!`);
        return;
    } else {
        JS.ExecuteFromFile(binary, command.slice(1), blocking);
    }
}

function SearchForBinary(filename: string): FILE {
    let binary: FILE;
    // search for binary
    // loop search paths
    for(let i=0;i<searchPath.length;i++)
    {
        // get fs object of search path
        let obj = Filesystem.GetObjectByAbsolutePath(searchPath[i]);
        // if it's a directory, look through it's children for the binary
        if(obj instanceof DIRECTORY)
        {
            let dir = obj as DIRECTORY;
            for(let x=0;x<dir.Children.Count;x++)
            {
                if(dir.Children[x].Title == filename && dir.Children[x] instanceof FILE)
                {
                    let child = dir.Children[x] as FILE;
                    binary = child;
                }
            }
        }
    }

    // no binary was found
    if(binary === undefined)
        return null;
    else
        return binary;
}

Start();