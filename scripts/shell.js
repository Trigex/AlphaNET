/*  shell.js
*   Bash is for pussies, SHELL.JS IS ALL YOU NEED MOTHERFUCKER!!
*/

PROMPT = "> ";
VERSION = 0.1;

function init(version) {
    printLn("shell.js version " + version);
    start();
}

function start() {
    while(true) {
        print(prompt);
        var input = readLine();
        printLn(input);
    }
}

init(VERSION)