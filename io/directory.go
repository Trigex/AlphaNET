package io

type Directory struct {
	Name                string
	ParentDirectory     *Directory
	ChildrenDirectories []*Directory
	ChildrenFiles       []*File
}

func CreateDirectory(name string, parentDirectory *Directory) Directory {
	dir := Directory{name, parentDirectory, nil, nil}
	dir.ParentDirectory = parentDirectory
	dir.ParentDirectory.ChildrenDirectories = append(dir.ParentDirectory.ChildrenDirectories, &dir)
	return dir
}
