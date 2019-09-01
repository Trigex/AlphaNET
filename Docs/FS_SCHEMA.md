# AlphaFS Binary Filesystem Schema Version 2

This document describes version 2 of the AlphaFs Binary Filesystem Schema.

## Notes

All "Size" properties are assumed to have their value in bytes

Block size is a constant 4096 bytes (4kb)

## Handy Formulas

The Inodes per bytes ratio is 1 INode per 16 kb, which can be expressed `as FilesystemFileSize / 16384`

(I.E, on a 3 gb filesystem file, `3221225472 / 16384` = 196608 inodes)

Total Block count is the entire Filesystem, divided into blocks, which can be expressed as `FilesystemFileSize / Block Size` 

(I.E, on a 3 gb filesystem file with default block size, `3221225472 / 4096` = 786432 blocks)

Inodes, being 132 bytes, means a single 4kb block can hold about 31 inodes (and just a bit more space left over), the total count of blocks taken up by the Inode table can be expressed as `BlockCount / (InodeCount / InodesPerBlock)`

(I.E, `786432 / (196608 / 31)` = 124 blocks used by Inode table)

An Inode's parent inode-table number can be found with `(InodeNumber / InodesPerBlock) + 1`

(I.E, `(42 / 31) + 1` = 2, INode number 42 is stored in inode-table block 2)

An Inode's inode-table index can be found within the table block that holds it with `InodeNumber - (InodesPerBlock * (Inode Table Block Index - 1))`
(I.E, `42 - (31 * (2 - 1)`) = 11, Inode 42 can be found in inode-table block 2 at index 11 )

## Terminology

### Block

A statically sized section that the entire Filesystem is broken down into. (Usually sized as 4096 bytes)
The Filesystem binary is only ever addressed in units of blocks.

### SuperBlock

The first block of the Filesystem, starting at byte 0, it contains important metadata about the Filesystem binary.

### Inode

An Inode is a statically allocated 132 byte object, stored in the Inode table, it describes certain properties about a given File in the Filesystem, such as the data blocks it's contents can be found in,
the total count of blocks allocated to it, and it's unique Inode number.

### Inode Table

A series of sequentially ordered blocks near the start of the Filesystem, dedicated to storing all Inodes in the Filesystem.