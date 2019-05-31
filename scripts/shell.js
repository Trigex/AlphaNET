/*  shell.js
*   Bash is for pussies, SHELL.JS IS ALL YOU NEED MOTHERFUCKER!!
*/

PROMPT = "> ";
VERSION = 0.1;
RUNNING = true;
PATH = ["/bin"];

function init() {
    printLn("shell.js version " + VERSION);
    start();
}

function start() {
    while(true) {
        print(PROMPT);
        var input = readLine();
        printLn(input);
        parseCommand(input);
    }
}

function parseCommand(input) {
    // split string into array by spaces
    var command = input.split(" ");
    // search for command executable
     
}

init();