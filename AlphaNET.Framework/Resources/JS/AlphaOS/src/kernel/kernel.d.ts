/*  Kernel.d.ts
 *  A hacky solution I'm happy with! Obviously, my arbitrary
 *  C# Jint interpreter values aren't picked up by TypeScript
 *  Intellisense. That's annoying, makes it a lot harder to 
 *  program! So, let's co-opt TypeScript definition files
 *  for that purpose!
 */

/*
 *  C# CLR-PROVIDED API DEFINITIONS
 *
 * 
 */

declare enum StatusCode {
    ObjectNotFound,
    ObjectFound,
    ObjectAlreadyPresent,
    ObjectMoved,
    ObjectDeleted,
    ObjectNotDeleted,
    ObjectRenamed,
    ObjectNotRenamed,
    ObjectAdded,
    FileModified
}

declare interface UTF_8 {
    GetBytes(s: string): Uint8Array;
    GetString(bytes: Uint8Array): string;
}

declare interface Terminal {
    Write(text: String): void;
    WriteLine(text: String): void;
    Read(): number;
    ReadLine(): string;
    Clear(): void;
}

declare interface JSInterpreter {
    ExecuteScript(script: string, isTypescript: boolean): void;
}

declare interface FilesystemObject {
    Title: string;
    Owner: FSDirectory;
    ID: Number;
    Rename(title: string): StatusCode;
}

declare interface List {
    Count: number,
        Add(entry: any): void;
    Remove(entry: any): void;
    ToArray(): Array < any > ;
}

declare interface FSObjectList extends List {}

declare interface FSFile extends FilesystemObject {
    Contents: Uint8Array;
    IsPlaintext: boolean;
    ModifyContents(newContents: Uint8Array, IsPlaintext: boolean): StatusCode;
}

declare interface FSDirectory extends FilesystemObject {
    Children: FSObjectList;
    GetChildrenFilesystemObjects: FSObjectList;
}

declare interface Filesystem {
    GetFileByTitle(title: string): FSFile
    GetDirectoryByTitle(title: string): FSDirectory;
    GetFilesystemObjectByAbsolutePath(path: string): FilesystemObject;
    GetAbsolutePathByFilesystemObject(fsObj: FilesystemObject): string;
}

declare interface TypescriptCompiler {
    CompileTypescript(script: string): string;
    CompileTypescript(scripts: Array<string>): string;
}

declare const Terminal: Terminal;
declare const JSInterpreter: JSInterpreter;
declare const Filesystem: Filesystem;
declare const UTF_8: UTF_8;
declare const TypescriptCompiler: TypescriptCompiler;

declare interface GenericObject {
    [key: string]: any
}

declare const Global: GenericObject;

/* TYPESCRIPT KERNEL API DEFINITIONS
 *
 */

declare function CreateProcess(binary: FSFile, args?: Array<string>): void;
declare function GetArgs(): Array<string>;

/* EXTERNAL LIBRARY API DEFINITIONS
 *
 */

// Type definitions for minimist 1.2.0
/**
 * Return an argument object populated with the array arguments from args
 * 
 * @param args An optional argument array (typically `process.argv.slice(2)`)
 * @param opts An optional options object to customize the parsing
 */
declare function minimist(args?: string[], opts?: minimist.Opts): minimist.ParsedArgs;

/**
 * Return an argument object populated with the array arguments from args. Strongly-typed
 * to be the intersect of type T with minimist.ParsedArgs.
 *
 * @type T The type that will be intersected with minimist.ParsedArgs to represent the argument object
 * @param args An optional argument array (typically `process.argv.slice(2)`)
 * @param opts An optional options object to customize the parsing
 */
declare function minimist<T>(args?: string[], opts?: minimist.Opts): T & minimist.ParsedArgs;

/**
 * Return an argument object populated with the array arguments from args. Strongly-typed
 * to be the the type T which should extend minimist.ParsedArgs
 *
 * @type T The type that extends minimist.ParsedArgs and represents the argument object
 * @param args An optional argument array (typically `process.argv.slice(2)`)
 * @param opts An optional options object to customize the parsing
 */
declare function minimist<T extends minimist.ParsedArgs>(args?: string[], opts?: minimist.Opts): T;

declare namespace minimist {
    export interface Opts {
        /**
         * A string or array of strings argument names to always treat as strings         
         */ 
        string?: string | string[];

        /**
         * A boolean, string or array of strings to always treat as booleans. If true will treat
         * all double hyphenated arguments without equals signs as boolean (e.g. affects `--foo`, not `-f` or `--foo=bar`)
         */
        boolean?: boolean | string | string[];

        /**
         * An object mapping string names to strings or arrays of string argument names to use as aliases
         */
        alias?: { [key: string]: string | string[] };

        /**
         * An object mapping string argument names to default values
         */
        default?: { [key: string]: any };

        /**
         * When true, populate argv._ with everything after the first non-option
         */
        stopEarly?: boolean;

        /**
         * A function which is invoked with a command line parameter not defined in the opts 
         * configuration object. If the function returns false, the unknown option is not added to argv         
         */
        unknown?: (arg: string) => boolean;

        /**
         * When true, populate argv._ with everything before the -- and argv['--'] with everything after the --.
         * Note that with -- set, parsing for arguments still stops after the `--`.
         */
        '--'?: boolean;
    }

    export interface ParsedArgs {
        [arg: string]: any;

        /**
         * If opts['--'] is true, populated with everything after the --
         */
        '--'?: string[];

        /**
         * Contains all the arguments that didn't have an option associated with them
         */
        _: string[];       
    }
}