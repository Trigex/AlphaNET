-- ls - Get contents of directory and print
fileContents = currentDirectory.GetAllFiles()
directoryContents = currentDirectory.GetAllDirectories()

for i=1,#fileContents do
	Computer.WriteLine(fileContents[i].title)
end

for i=1,#directoryContents do
	Computer.WriteLine(directoryContents[i].title)
end