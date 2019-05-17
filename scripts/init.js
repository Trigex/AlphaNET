/*  init.js
*   The first script the system runs on startup (technically, when the computer struct is created and started)
    Linsux has systemdicks for init, we use a shitty js script.
    Who wins?
*/

printLn("Loading shell...")

// load shell
var shell = getFile("shell.js")
run(shell)