package io

import (
	"encoding/json"
	"fmt"
	"log"
)

type FileJSON struct {
	Name     string `json:"name"`
	Contents string `json:"contents"`
}

type DirectoryJSON struct {
	Name                string   `json:"name"`
	ChildrenDirectories []string `json:"childrenDirectories"`
	ChildrenFiles       []string `json:"childrenFiles"`
}

type FilesystemJSON struct {
	Directories []DirectoryJSON `json:"directories"`
	Files       []FileJSON      `json:"files"`
}

func ConvertJSONToFilesystem(jsonBuf []byte) FilesystemJSON {
	fs := new(FilesystemJSON)
	err := json.Unmarshal(jsonBuf, fs)
	if err != nil {
		log.Fatalln(err)
	}

	return *fs
}

func ConvertFilesystemToJSON(fs Filesystem) []byte {
	fsJson := new(FilesystemJSON)

	for i := range fs.Files {
		file := fs.Files[i]
		fsJson.Files = append(fsJson.Files, FileJSON{file.Name, file.Contents})
	}

	for i := range fs.Directories {
		dir := fs.Directories[i]
		jsonDir := new(DirectoryJSON)
		jsonDir.Name = dir.Name

		// get directory children
		for f := range dir.ChildrenFiles {
			jsonDir.ChildrenFiles = append(jsonDir.ChildrenFiles, dir.ChildrenFiles[f].Name)
		}

		for d := range dir.ChildrenDirectories {
			jsonDir.ChildrenDirectories = append(jsonDir.ChildrenDirectories, dir.ChildrenDirectories[d].Name)
		}

		fsJson.Directories = append(fsJson.Directories, *jsonDir)
	}

	fsJsonBuf, err := json.Marshal(fsJson)

	if err != nil {
		fmt.Println(err)
	}

	return fsJsonBuf
}

func FindParentJSON(name string, jsonFs FilesystemJSON) DirectoryJSON {
	var dir DirectoryJSON

	// loop all directories
	for i := range jsonFs.Directories {
		curDir := jsonFs.Directories[i]
		// loop files
		for f := range curDir.ChildrenFiles {
			cFile := curDir.ChildrenFiles[f]

			if cFile == name {
				dir = curDir
			}
		}
		// loop directories
		for d := range curDir.ChildrenDirectories {
			cDir := curDir.ChildrenDirectories[d]

			if cDir == name {
				dir = curDir
			}
		}
	}

	return dir
}
