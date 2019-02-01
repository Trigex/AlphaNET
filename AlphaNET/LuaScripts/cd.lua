if currentArgs[1] == ".." then
	currentDirectory = currentDirectory.parentDirectory
else
	dir = Filesystem.GetDirectoryByTitle(currentArgs[1])
	if not(dir == null) then
		currentDirectory = dir
	else
		Computer.WriteLine("The directory \"" .. currentArgs[1] .. "\" wasn't found!")
	end
end