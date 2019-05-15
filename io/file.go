package io

type File struct {
	Name            string
	Contents        string
	ParentDirectory *Directory
}

func CreateFile(name string, contents string, parentDirectory *Directory) File {
	file := File{name, contents, parentDirectory}
	parentDirectory.ChildrenFiles = append(parentDirectory.ChildrenFiles, &file)
	return file
}
