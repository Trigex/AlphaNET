package io

import (
	"encoding/json"
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

func CreateFilesystemJSON(jsonBuf []byte) FilesystemJSON {
	fs := new(FilesystemJSON)
	err := json.Unmarshal(jsonBuf, fs)
	if err != nil {
		log.Fatalln(err)
	}

	return *fs
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
