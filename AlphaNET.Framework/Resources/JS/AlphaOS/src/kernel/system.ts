/*
 *  system.ts
 *  The greatest system... THE ALPHANET SYSTEM!!
 *  Provides many convient functions for operating within AlphaNET
 *  (c) Trigex 2019, Licensed under the MIT license
 */
/// <reference path="./types/os.d.ts" />

var Processes = new List<Process>();

// Create a new process, return new Process with PID
function fork(): Process {
    Terminal.WriteLine(`Fork called`);
    let process = {Script: null, Pid: Processes.Count + 1};
    Processes.Add(process);
    return process;
}

/*
 *  Exec functions load a new program into the current process
 */

function exec(path: string, args: string[], process: Process): number {
    Terminal.WriteLine(`exec called with path ${path}`);
    let file = Filesystem.GetFilesystemObjectByAbsolutePath(path) as FILE;
    Terminal.WriteLine(`${file.Title}`);
    process.Script = UTF8.GetString(file.Contents);
    return JSInterpreter.ExecuteScript(process.Script, false, args);
}

function wait() {
    
}

function GetFileText(file: FILE): string {
    return UTF8.GetString(file.Contents);
}

function GetFileBytes(file: FILE): Uint8Array {
    return file.Contents;
}

function GetDirectoryChildren(dir: DIRECTORY): List<FilesystemObject> {
    return dir.Children;
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

const console: CONSOLE;

String.prototype.includes = function(substr: string): boolean {
    if(this.toString())
}