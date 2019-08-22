# Binary Filesystem Schema

This document describes version 1 of the AlphaNET Binary Filesystem Schema. Each described section appears in the binary, in the order they are sequenced here (Except for Filesystem Object schemas, which are positioned and described under their respective list in this document).

## Notes

Values marked as `(Constant)`, are arbitrary numbers, usually used as flags for the Filesystem loader to identify sections of the binary.

List sections contain an array of Filesystem Objects (File or Directory), held in-between the list's `Start` and `End` flags.  

# Binary Filesystem Headers

_Headers, describing properties of this filesystem binary_

| Flag/Property          | Type          | Value        |
| ---------------------- |:--------------|:-------------|
| FS File Header Start   | Byte          | 6 (Constant) |
| FS Version             | Byte          | 1-255        |
| FS File Header End     | Byte          | 9 (Constant) |

# Directory List Meta (Not yet implemented)

_Generic metadata for the list that succeeds it_

| Flag/Property          | Type          | Value                  |
| ---------------------- |:--------------|:-----------------------|
| List Meta Flag         | Byte          | 100 (Constant)         |
| List Length            | Uint          | 1-4294967295           |
| List Byte Size         | Ulong         | 1-18446744073709551615 |

# Directory List

_A list of directories present in the filesystem._

| Flag/Property                       | Type          | Value            |
| ----------------------------------- |:--------------|:-----------------|
| Directory List Start Flag           | Byte          | 1 (Constant)     | 
| Directory Objects Array (See below) | Directory[]   | Cool folders :`) |
| Directory List End Flag	      | Byte          | 2 (Constant)     |

## Directory Object

_A Filesystem Directory object, which, when loaded into AlphaNET, contains other Directory objects, and Files_

| Flag/Property                       | Type                 | Value                                                                     |
| ----------------------------------- |:---------------------|:--------------------------------------------------------------------------|
| Directory Object Start Flag         | Byte                 | 5 (Constant)                                                              |
| ID                                  | Uint                 | 1-4294967295                                                              |
| OwnerID                             | Uint                 | 1-4294967295                                                              |
| Title Length                        | Ushort               | 1-65535                                                                   |
| Title                               | UTF-8 Encoded String | A valid UTF-8 Encoded string, with a max character length of 65535        |
| Directory Object End Flag           | Byte                 | 5 (Constant)                                                              |

# File List Meta (Not yet implemented)

_Generic metadata for the list that succeeds it_

| Flag/Property          | Type          | Value                  |
| ---------------------- |:--------------|:-----------------------|
| List Meta Flag         | Byte          | 100 (Constant)         |
| List Length            | Uint          | 1-4294967295           |
| List Byte Size         | Ulong         | 1-18446744073709551615 |

# File List

_A list of files present in the filesystem._

| Flag/Property                       | Type          | Value            |
| ----------------------------------- |:--------------|:-----------------|
| File List Start Flag                | Byte          | 3 (Constant)     | 
| File Objects Array (See below)      | File[]        | Cool files :`)   |
| File List End Flag		      | Byte          | 4 (Constant)     |

## File Object

_A Filesystem File object, which contains arbitrary or UTF-8 Encoded string bytes, parented to a Directory object via OwnerID_

| Flag/Property                       | Type                 | Value                                                                                                             |
| ----------------------------------- |:---------------------|:------------------------------------------------------------------------------------------------------------------|
| File Object Start Flag              | Byte                 | 7 (Constant)                                                                                                      |
| ID                                  | Uint                 | 1-4294967295                                                                                                      |
| OwnerID                             | Uint                 | 1-4294967295                                                                                                      |
| Title Length                        | Ushort               | 1-65535                                                                                                           |
| Title                               | UTF-8 Encoded String | A valid UTF-8 Encoded string, with a max character length of 65535                                                |
| Plaintext                           | Byte		     | 1, or 0, 1 = Is Plaintext, 0 = Not Plaintext                                                                      |
| Contents Length		      | Uint                 | 1-4294967295                                                                                                      |
| Contents			      | Byte[]               | Raw array of bytes, with a maximum length of 4294967295 bytes, or a UTF-8 Encoded string, if `Plaintext` was true |
| File Object End Flag                | Byte                 | 8 (Constant)                                                                                                      |
