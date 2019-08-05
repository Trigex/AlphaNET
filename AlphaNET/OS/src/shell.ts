/// <reference path="kernel/kernel.d.ts" />

// Globals
const PROMPT = "> ";
const VERSION = 1.0;

var running = true;
var path = ["bin"];
Global.CurrentPath = Filesystem.GetDirectoryByTitle("root");

function shell() {
    Terminal.WriteLine(`AlphaShell version ${VERSION}`);
    Start();
}

function Start() {
    while(running) {
        // Set the CurrentPath every loop
        Terminal.Write(PROMPT);
        let input = Terminal.ReadLine();
        // check if the command is a built in util
        if(!BuiltInUtils(input))
            ParseCommand(input);
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
    // search for a binary to run from program argument
    let file = Filesystem.GetFileByTitle(command[0] + ".js");
    if(file != null) {
        // found a binary
        CreateProcess(file, command);
    } else {
        Terminal.WriteLine(command[0] + " could not be found...");
    }
}

shell();