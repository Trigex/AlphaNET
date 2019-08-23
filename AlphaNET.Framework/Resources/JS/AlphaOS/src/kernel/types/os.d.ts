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
    RemoveAt(index: number): void;
    ToArray(): Array<T>;
    CopyTo(array: Array<T>)
}
declare interface ListConstructor {
    new<T>(): List<T>; // Empty list
    new<T>(array: Array<T>): List<T>; // List from array
}

declare interface UTF8 {
    GetBytes(s: string): Uint8Array;
    GetString(bytes: Uint8Array): string;
}

/*
 *  ALPHANET API DEFINITIONS
 *
 */

declare enum IOStatusCode {
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

declare enum NetStatusCode {
    AddressInUse,
    InvalidAddress,
    SocketBinded,
    SocketConnected,
    SocketDisconnected
}

declare interface Terminal {
    Write(text: String): void;
    WriteLine(text: String): void;
    Read(): number;
    ReadLine(): string;
    Clear(): void;
}

declare interface JSInterpreter {
    Execute(script: string, args: Array<string>, blocking: boolean): void;
    Execute(process: Process, blocking: boolean): void;
}

declare interface FilesystemObject {
    Title: string;
    Owner: DIRECTORY;
    ID: Number;
    Rename(title: string): IOStatusCode;
}

declare interface FILE extends FilesystemObject {
    Contents: Uint8Array;
    IsPlaintext: boolean;
    ModifyContents(newContents: Uint8Array, IsPlaintext: boolean): IOStatusCode;
}
declare interface FileConstructor {
    new(title: string, id: number, isPlaintext: boolean, contents: Uint8Array)
}

declare interface DIRECTORY extends FilesystemObject {
    Children: List<FilesystemObject>;
    GetChildren(): List<FilesystemObject>;
    GetChildrenByTitle(title: string): List<FilesystemObject>;
    GetChildByTitle(title: string): List<FilesystemObject>;
    GetChildByID(id: number): FilesystemObject;
    RemoveChildByID(id: number): IOStatusCode;
}
declare interface DirectoryConstructor {
    new(title: string, id: number)
}

declare interface Filesystem {
    GetObjectByTitle(title: string): FilesystemObject;
    GetObjectsByTitle(title: string): List<FilesystemObject>;
    GetObjectById(id: number): FilesystemObject;
    GetDirectoryByTitle(title: string): DIRECTORY;
    GetObjectByAbsolutePath(path: string): FilesystemObject;
    GetAbsolutePathByObject(fsObj: FilesystemObject): string;
    GenerateFilesystemObjectID(): number;
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
    new()
}

declare interface SocketManager {
    BindSocket(socket: Socket, address: Address): NetStatusCode;
    ListenOnSocket(socket: Socket): NetStatusCode;
    AcceptOnSocket(socket: Socket): NetStatusCode;
    ConnectSocket(socket: Socket, destinationAddress: Address, localPort?: number): NetStatusCode;
    GetIpAddress(): string;
}

// Flesh this out a bit more later
declare interface Process {
    Script: string;
    Args: Array<string>;
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
declare const args: string[];