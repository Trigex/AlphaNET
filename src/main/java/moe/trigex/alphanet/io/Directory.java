package moe.trigex.alphanet.io;

import java.util.ArrayList;
import java.util.List;

public class Directory {
    private String title;
    private List<Directory> childrenDirectories;
    private List<File> childrenFiles;
    private Directory parentDirectory;
    private Filesystem fs;

    public Directory(String title, Directory parentDirectory, Filesystem fs) {
        this.title = title;
        this.fs = fs;
        this.childrenDirectories = new ArrayList<Directory>();
        this.childrenFiles = new ArrayList<File>();
        this.parentDirectory = parentDirectory;

        fs.addDirectoryToMasterList(this);
    }

    public void addFile(File file) {
        childrenFiles.add(file);
    }

    public void addDirectory(Directory directory) {
        childrenDirectories.add(directory);
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public List<Directory> getChildrenDirectories() {
        return childrenDirectories;
    }

    public List<File> getChildrenFiles() {
        return childrenFiles;
    }
}
