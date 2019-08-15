/*  os.d.ts
 *  This file gives definitions for the AlphaNET API exposed
 *  in the CLR by C#, as well as definitions for exposed
 *  C# types, and external libraries AlphaNET uses
 */

/*
 * C# TYPE DEFINITIONS
 * ( and other hacks ;) )
 */

declare interface GenericObject {
    [key: string]: any
}
declare const Global: GenericObject;

declare interface List<T> {
    Count: number;
    Add(entry: any): void;
    Remove(entry: any): void;
    ToArray(): Array <any>;
}
declare interface ListConstructor {
    new<T>(): List<T>; // Empty list
    new<T>(array: Array<T>): List<T>; // List from array
}

declare interface UTF8 {
    GetBytes(s: string): Uint8Array;
    GetString(bytes: Uint8Array): string;
}

declare interface String {
    includes(substr: string): boolean;
}

/*
 *  ALPHANET API DEFINITIONS
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

declare interface Terminal {
    Write(text: String): void;
    WriteLine(text: String): void;
    Read(): number;
    ReadLine(): string;
    Clear(): void;
}

declare interface JSInterpreter {
    ExecuteScript(script: string, isTypescript: boolean, args: string[]): number;
}

declare interface FilesystemObject {
    Title: string;
    Owner: DIRECTORY;
    ID: Number;
    Rename(title: string): StatusCode;
}

declare interface FILE extends FilesystemObject {
    Contents: Uint8Array;
    IsPlaintext: boolean;
    ModifyContents(newContents: Uint8Array, IsPlaintext: boolean): StatusCode;
}
declare interface FileConstructor {
    new(title: string, id: number, isPlaintext: boolean, cotents: Uint8Array)
}

declare interface DIRECTORY extends FilesystemObject {
    Children: List<FilesystemObject>;
    GetChildrenFilesystemObjects(): List<FilesystemObject>;
}
declare interface DirectoryConstructor {
    new(title: string, id: number)
}

declare interface Filesystem {
    GetFileByTitle(title: string): FILE
    GetDirectoryByTitle(title: string): DIRECTORY;
    GetFilesystemObjectByAbsolutePath(path: string): FilesystemObject;
    GetAbsolutePathByFilesystemObject(fsObj: FilesystemObject): string;
    GenerateFilesystemObjectID(): number
}

declare interface TypescriptCompiler {
    CompileTypescript(script: string): string;
    CompileTypescript(scripts: Array<string>): string;
}

declare interface Address {
    Port: number;
    IpAddress: string;
    ToString(): string;
}
declare interface AddressConstructor {
    new(ipAddress: string, port: number)
}

declare interface Socket {
    Address: Address;
    EndpointAddress: Address;
    Connected: boolean;
    Listening: boolean;
}
declare interface SocketConstructor {
    new(address: Address)
}

declare interface SocketManager {
    ConnectSocketToEndpoint(socket: Socket);
    ListenOnSocket(socket: Socket);
}

// Flesh this out a bit more later
declare interface Process {
    Script: string;
    Pid: number;
}

declare const Terminal: Terminal;
declare const JSInterpreter: JSInterpreter;
declare const Filesystem: Filesystem;
declare const UTF8: UTF8;
declare const TypescriptCompiler: TypescriptCompiler;
declare const SocketManager: SocketManager;
declare const List: ListConstructor;

declare const Address: AddressConstructor;
declare const Socket: SocketConstructor;
declare const FILE: FileConstructor
declare const DIRECTORY: DirectoryConstructor;

/* EXTERNAL LIBRARY TYPE DEFINITIONS
 *
 *
 */
/// <reference path="./thirdparty.d.ts" />