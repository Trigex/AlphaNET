package io

import (
	"errors"
	"fmt"
	"io/ioutil"
	"log"
)

type Filesystem struct {
	Directories []*Directory
	Files       []*File
	fsJsonPath  string
}

func (fs *Filesystem) AddFile(file File) {
	fs.Files = append(fs.Files, &file)
	fs.Save()
}

func (fs *Filesystem) AddDirectory(dir Directory) {
	fs.Directories = append(fs.Directories, &dir)
	fs.Save()
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

func (fs *Filesystem) Save() error {
	fsJson := ConvertFilesystemToJSON(*fs)
	//fmt.Println(string(fsJson))
	fmt.Println("FS Saved")

	err := ioutil.WriteFile(fs.fsJsonPath, fsJson, 0644)

	if err != nil {
		return err
	}

	return nil
}

func (fs *Filesystem) FindDirectory(name string) (*Directory, error) {
	var dir *Directory

	for i := range fs.Directories {
		curDir := fs.Directories[i]

		if curDir.Name == name {
			dir = curDir
		}
	}

	if dir == nil {
		return nil, errors.New("Unable to find directory")
	}

	return dir, nil
}

func (fs *Filesystem) FindFile(name string) (*File, error) {
	var file *File

	for i := range fs.Files {
		curFile := fs.Files[i]

		if curFile.Name == name {
			file = curFile
		}
	}

	if file == nil {
		return nil, errors.New("Unable to find file")
	}

	return file, nil
}

func CreateFilesystem(fsJsonPath string) Filesystem {
	// convert json to FilesystemJSON struct
	jsonBuf, ioErr := ioutil.ReadFile(fsJsonPath)

	if ioErr != nil {
		log.Fatalln(ioErr)
	}

	jsonFs := ConvertJSONToFilesystem(jsonBuf)
	// parse FilesystemJSON struct and create "real" filesystem
	fs := new(Filesystem)
	fs.fsJsonPath = fsJsonPath

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
		parentReal, err := fs.FindDirectory(parentJson.Name)
		if err != nil {
			fmt.Println(err)
		} else {
			// set parenting and children
			realFile.ParentDirectory = parentReal
			parentReal.ChildrenFiles = append(parentReal.ChildrenFiles, &realFile)
		}
	}

	// find and set parent of directories (own loop, because all directories must be created for it to work)
	for i := range fs.Directories {
		curDir := fs.Directories[i]

		if curDir.Name != "root" {
			// find parent json
			parentJson := FindParentJSON(curDir.Name, jsonFs)
			// find real directory
			parentReal, err := fs.FindDirectory(parentJson.Name)
			if err != nil {
				fmt.Println(err)
			} else {
				curDir.ParentDirectory = parentReal
				parentReal.ChildrenDirectories = append(parentReal.ChildrenDirectories, curDir)
			}

		} else {
			// root is a parent of itself
			// crazy, I know
			curDir.ParentDirectory = curDir
		}
	}

	return *fs
}
