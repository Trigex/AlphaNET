/*
 *  Kernel.ts
 *  The greatest kernel... The AlphaOS kernel!
 *  Provides many convient functions for operating within AlphaNET
 *  (c) Trigex 2019, Licensed under the MIT license
 */
/// <reference path="kernel.d.ts" />

function CreateProcess(binary: FSFile, args?: Array<string>) {
    if(args)
        Global.ProcessArguments = args;
    else
        Global.ProcessArguments = null;

    JSInterpreter.ExecuteScript(UTF_8.GetString(binary.Contents), false);
    // Erase arguments once execution has ended
    Global.ProcessArguments = null;
}

function GetArgs(): Array<string> {
    return Global.ProcessArguments;
}