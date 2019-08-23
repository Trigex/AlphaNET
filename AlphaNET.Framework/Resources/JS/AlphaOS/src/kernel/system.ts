/*
 *  system.ts
 *  The greatest system... THE ALPHANET SYSTEM!!
 *  Provides many convient functions for operating within AlphaNET
 *  (c) Trigex 2019, Licensed under the MIT license
 */
/// <reference path="./types/os.d.ts" />

function CreateProcess(script: string, args: string[]): Process {
    return {Script: script, Args: args};
}

const JS = {
    ExecuteFromPath: (path: string, args: string[], blocking: boolean): void => {
        // Get binary from path
        let binary = Filesystem.GetObjectByAbsolutePath(path);
        if((binary !== undefined || binary !== null) && binary instanceof FILE)
        {
            let file = binary as FILE;
            JSInterpreter.Execute(UTF8.GetString(file.Contents), args, blocking);
        }
    },

    ExecuteFromFile: (binary: FILE, args: string[], blocking: boolean): void => {
        if(binary instanceof FILE)
        {
            JSInterpreter.Execute(UTF8.GetString(binary.Contents), args, blocking);
        }
    },

    ExecuteFromScript: (script: string, args: string[], blocking: boolean): void => {
        JSInterpreter.Execute(script, args, blocking);
    }
}

const IO = {
    GetFileText: (file: FILE): string => {
        return UTF8.GetString(file.Contents);
    }
}

// Net
const Net = {
}

// Wrapper around a CLR FILE, makes working with them a bit nicer
class _File {
    Title: string
    Contents: Uint8Array;
    IsPlaintext: boolean;
    Text: string;
    _internalFile: FILE;

    constructor(title: string, text?: string, binary?: Uint8Array) {
        this.Title = title;

        if(text)
        {
            this.Text = text;
            this.Contents = UTF8.GetBytes(text);
            this.IsPlaintext = true;
        } else if(binary)
        {
            this.Text = null;
            this.Contents = binary;
            this.IsPlaintext = false;
        }

        this._internalFile = new FILE(title, Filesystem.GenerateFilesystemObjectID(), this.IsPlaintext, this.Contents);
    }
}

// Console implementation, for compatability with non-AlphaNET
// JS applications
class CONSOLE implements Console {
    memory: any;    
    assert(condition?: boolean, message?: string, ...data: any[]): void {
    }
    clear(): void {
        Terminal.Clear();
    }
    count(label?: string): void {
    }
    debug(message?: any, ...optionalParams: any[]): void {
        Terminal.WriteLine(message);
    }
    dir(value?: any, ...optionalParams: any[]): void {
    }
    dirxml(value: any): void {
    }
    error(message?: any, ...optionalParams: any[]): void {
        Terminal.WriteLine(message);
    }
    exception(message?: string, ...optionalParams: any[]): void {
    }
    group(groupTitle?: string, ...optionalParams: any[]): void {
    }
    groupCollapsed(groupTitle?: string, ...optionalParams: any[]): void {
    }
    groupEnd(): void {
    }
    info(message?: any, ...optionalParams: any[]): void {
        Terminal.WriteLine(message);
    }
    log(message?: any, ...optionalParams: any[]): void {
        Terminal.WriteLine(message);
    }
    markTimeline(label?: string): void {
    }
    profile(reportName?: string): void {
    }
    profileEnd(reportName?: string): void {
    }
    table(...tabularData: any[]): void {
    }
    time(label?: string): void {
    }
    timeEnd(label?: string): void {
    }
    timeStamp(label?: string): void {
    }
    timeline(label?: string): void {
    }
    timelineEnd(label?: string): void {
    }
    trace(message?: any, ...optionalParams: any[]): void {
    }
    warn(message?: any, ...optionalParams: any[]): void {
        Terminal.WriteLine(message);
    }
}

// Globals
// @ts-ignore
const console: Console = new CONSOLE();
const Sockets = new List<Socket>();