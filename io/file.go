package io

// File represents a text file in the Filesystem
type File struct {
	Name            string
	Contents        string
	ParentDirectory *Directory
}

// CreateFile creates a new File object
func CreateFile(name string, contents string, parentDirectory *Directory) File {
	file := File{name, contents, parentDirectory}
	parentDirectory.ChildrenFiles = append(parentDirectory.ChildrenFiles, &file)
	return file
}
