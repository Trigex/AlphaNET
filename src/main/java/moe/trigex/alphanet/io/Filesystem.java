package moe.trigex.alphanet.io;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.List;

import com.google.gson.stream.JsonReader;
import com.google.gson.stream.JsonToken;

public class Filesystem {
    private String fsJsonPath;
    private Directory rootDirectory;
    private List<Directory> directories;
    private List<File> files;

    public Filesystem(String fsJsonPath) {
        this.fsJsonPath = fsJsonPath;
        directories = new ArrayList<Directory>();
        files = new ArrayList<File>();

        loadFilesystemFromPath();
    }

    private void loadFilesystemFromPath() {
        String fsString;
        JsonReader r;

        try {
            fsString = readTextFromFile(fsJsonPath);
            r = new JsonReader(new StringReader(fsString));

            System.out.println(fsString);

            r.beginObject(); // Start of fs ( { )
            this.rootDirectory = new Directory("root", null,this);
            Directory currentDirectory = rootDirectory;
            Directory lastDirectory;

            while(r.hasNext()) {
                JsonToken token = r.peek();
                switch(token) {
                    // Directory or file
                    case NAME:
                        String name = r.nextName();

                        if(name.startsWith("/")) { // Directory
                            Directory directory = new Directory(name, currentDirectory, this);

                            lastDirectory = currentDirectory;
                            currentDirectory = directory;

                            r.beginObject();
                            System.out.println("Directory: " + name);
                        } else { // File
                            File file = new File(name, null, currentDirectory, this);
                            System.out.println("File: " + name);
                        }
                        break;
                    // File contents
                    case STRING:
                        String contents = r.nextString();

                        // get last file
                        File file = files.get(files.size() - 1);
                        file.setContents(contents);
                        System.out.println("File contents: " + contents);
                        break;
                }
            }

            r.endObject(); // End of fs ( } )
        } catch (IOException e) {
            e.printStackTrace();
            return;
        }
    }

    private String readTextFromFile(String path) throws IOException {
        String contents;
        String line;
        BufferedReader r;

        r = new BufferedReader(new FileReader(path));
        contents = "";

        while((line = r.readLine()) != null) {
            contents += line;
        }

        return contents;
    }

    public void addDirectoryToMasterList(Directory directory) {
        directories.add(directory);
    }

    public void addFileToMasterList(File file) {
        files.add(file);
    }
}
