package moe.trigex.alphanet.core;

import moe.trigex.alphanet.io.Filesystem;
import moe.trigex.alphanet.lua.LuaManager;

public class Computer {
    private Filesystem fs;
    private LuaManager lm;
    private Console console;

    public Computer(String fsJsonPath) {
        fs = new Filesystem(fsJsonPath);
        lm = new LuaManager();
        console = new Console();
    }
}
