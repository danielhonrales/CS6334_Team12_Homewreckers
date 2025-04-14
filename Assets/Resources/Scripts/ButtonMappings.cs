using System.Collections.Generic;
using UnityEngine;

public static class ButtonMappings
{
    private static Dictionary<string, string> windowseditor_mappings = new() {
        {"A", "js8"},
        {"B", "js10"},
        {"X", "js1"},
        {"Y", "js0"},
        {"Menu", "js9"},
        {"Trigger", "js3"},
    };

    private static Dictionary<string, string> android_mappings = new() {
        {"A", "js10"},
        {"B", "js5"},
        {"X", "js2"},
        {"Y", "js3"},
        {"Menu", "js11"},
        {"Trigger", "js0"},
    };

    public static string GetMapping(string id) {
        if (DetectPlatform() == "windows editor") {
            if (windowseditor_mappings.TryGetValue(id, out string value)) {
                return value;
            }
        } else if (DetectPlatform() == "android") {
            if (android_mappings.TryGetValue(id, out string value)) {
                return value;
            }
        }
        return "Unknown button";
    }

    static string DetectPlatform()
    {
        RuntimePlatform platform = Application.platform;

        switch (platform)
        {
            case RuntimePlatform.Android:
                return "android";
            case RuntimePlatform.IPhonePlayer:
                return "ios";
            case RuntimePlatform.WindowsPlayer:
                return "windows";
            case RuntimePlatform.OSXPlayer:
                return "macos";
            case RuntimePlatform.WebGLPlayer:
                return "webgl";
            case RuntimePlatform.WindowsEditor:
                return "windows editor";
            case RuntimePlatform.OSXEditor:
                return "macos editor";
            default:
                Debug.Log("Running on an unknown platform: " + platform);
                return "";
        }
    }
}
