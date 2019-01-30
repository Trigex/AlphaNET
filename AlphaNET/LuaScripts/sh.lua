-- sh
-- (c) 2019 Trigex
-- Licensed under the MIT License

Computer.WriteLine("sh")
-- super epic globals
running = true
currentDirectory = Filesystem.rootDirectory
currentArgs = {}
while running do
	-- prompt
	Computer.Write("$> ")
	-- get input
	input = Computer.ReadLine()
	-- parse input
	-- split command into spaces
	command = StringMagick.SplitStringBySpaces(input)
	-- the first argument should always be the name of the program
	programArg = command[1]
	-- allow not needing to type .lua
	if (not string.match(programArg, ".lua")) then
		programArg = programArg..".lua"
	end
	-- get additional args (start loop after programArg)
	for i=2,#command do
		currentArgs[i-1] = command[i]
	end
	-- get program code
	programCode = Filesystem.GetFileByTitle(programArg).data
	-- run
	ExecuteScript(programCode)
end
