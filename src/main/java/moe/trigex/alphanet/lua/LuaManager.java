package moe.trigex.alphanet.lua;

import org.luaj.vm2.Globals;
import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.jse.JsePlatform;

public class LuaManager {
    static Globals globals;

    public LuaManager() {
        globals = JsePlatform.standardGlobals();
    }

    public void runScript(String script) {
        LuaValue val = globals.load(script).call();
    }
}
