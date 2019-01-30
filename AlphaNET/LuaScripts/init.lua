Computer.WriteLine("Welcome to AlphaNET!")
Computer.WriteLine("Starting sh.lua...")
-- get sh code
sh = Filesystem.GetFileByTitle("sh.lua").data
ExecuteScript(sh)