package moe.trigex.alphanet.io;

public class File {
    private String title;
    private String contents;
    private Directory parentDirectory;
    private Filesystem fs;

    public File(String title, String contents, Directory parentDirectory, Filesystem fs) {
        this.title = title;
        this.contents = contents;
        this.parentDirectory = parentDirectory;
        this.fs = fs;

        fs.addFileToMasterList(this);
        parentDirectory.addFile(this);
    }

    public String getTitle() {
        return title;
    }
    public void setTitle(String title) {
        this.title = title;
    }
    public String getContents() {
        return contents;
    }
    public void setContents(String contents) {
        this.contents = contents;
    }
    public Directory getParentDirectory() {
        return parentDirectory;
    }
    public void setParentDirectory(Directory parentDirectory) {
        this.parentDirectory = parentDirectory;
    }
}
