package io

import (
	"fmt"
	"io/ioutil"
	"log"
)

type Filesystem struct {
	Directories []*Directory
	Files       []*File
}

func (fs *Filesystem) AddFile(file File) {
	fs.Files = append(fs.Files, &file)
}

func (fs *Filesystem) DebugPrint() {
	fmt.Println("=============\nDirectories\n=============")
	for d := range fs.Directories {
		dir := fs.Directories[d]
		fmt.Println("Directory: /" + dir.Name + ", Parent: /" + dir.ParentDirectory.Name)
		fmt.Println("Children: ")
		// children
		for c := range dir.ChildrenDirectories {
			fmt.Println("    Child Directory: /" + dir.ChildrenDirectories[c].Name)
		}

		for c := range dir.ChildrenFiles {
			fmt.Println("    Child File: " + dir.ChildrenFiles[c].Name)
		}

		fmt.Println()
	}

	fmt.Println("=============\nFiles\n=============")
	for f := range fs.Files {
		file := fs.Files[f]
		fmt.Println("File: " + file.Name + ", Parent: /" + file.ParentDirectory.Name + ", Contents:")
		fmt.Println(file.Contents)

		fmt.Println()
	}
}

func (fs *Filesystem) AddDirectory(dir Directory) {
	fs.Directories = append(fs.Directories, &dir)
}

func CreateFilesystem(fsJsonPath string) Filesystem {
	// convert json to FilesystemJSON struct
	jsonBuf, ioErr := ioutil.ReadFile(fsJsonPath)

	if ioErr != nil {
		log.Fatalln(ioErr)
	}

	jsonFs := CreateFilesystemJSON(jsonBuf)
	// parse FilesystemJSON struct and create "real" filesystem
	fs := new(Filesystem)

	// loop for directory list, create directory structs, append to fs list
	for i := range jsonFs.Directories {
		jsonDir := jsonFs.Directories[i]
		realDir := Directory{jsonDir.Name, nil, nil, nil}
		// append directory to fs list
		fs.Directories = append(fs.Directories, &realDir)
	}

	// loop for file list, create file structs, append to fs list, find and set parent
	for i := range jsonFs.Files {
		jsonFile := jsonFs.Files[i]
		realFile := File{jsonFile.Name, jsonFile.Contents, nil}
		fs.Files = append(fs.Files, &realFile)

		// find parent json
		parentJson := FindParentJSON(realFile.Name, jsonFs)
		// find real directory
		parentReal := FindDirectory(parentJson.Name, *fs)
		// set parenting and children
		realFile.ParentDirectory = parentReal
		parentReal.ChildrenFiles = append(parentReal.ChildrenFiles, &realFile)
	}

	// find and set parent of directories (own loop, because all directories must be created for it to work)
	for i := range fs.Directories {
		curDir := fs.Directories[i]

		if curDir.Name != "root" {
			// find parent json
			parentJson := FindParentJSON(curDir.Name, jsonFs)
			// find real directory
			parentReal := FindDirectory(parentJson.Name, *fs)
			curDir.ParentDirectory = parentReal
			parentReal.ChildrenDirectories = append(parentReal.ChildrenDirectories, curDir)
		} else {
			// root is a parent of itself
			// crazy, I know
			curDir.ParentDirectory = curDir
		}
	}

	return *fs
}

func FindDirectory(name string, fs Filesystem) *Directory {
	var dir *Directory

	for i := range fs.Directories {
		curDir := fs.Directories[i]

		if curDir.Name == name {
			dir = curDir
		}
	}

	return dir
}

func FindFile(name string, fs Filesystem) *File {
	var file *File

	for i := range fs.Files {
		curFile := fs.Files[i]

		if curFile.Name == name {
			file = curFile
		}
	}

	return file
}
