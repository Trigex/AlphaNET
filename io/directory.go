package io

// Directory represents a directory of other directories and files in the Filesystem
type Directory struct {
	Name                string
	ParentDirectory     *Directory
	ChildrenDirectories []*Directory
	ChildrenFiles       []*File
}

// CreateDirectory creates a new Directory object
func CreateDirectory(name string, parentDirectory *Directory) Directory {
	dir := Directory{name, parentDirectory, nil, nil}
	dir.ParentDirectory.ChildrenDirectories = append(dir.ParentDirectory.ChildrenDirectories, &dir)
	return dir
}
