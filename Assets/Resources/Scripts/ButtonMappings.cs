using System.Collections.Generic;
using UnityEngine;

public static class ButtonMappings
{
    private static Dictionary<string, string> mappings = new() {
        {"A", "js8"},
        {"B", "js10"},
        {"X", "js1"},
        {"Y", "js0"},
        {"Menu", "js9"},
        {"Trigger", "js3"},
    };

    public static string GetMapping(string id) {
        if (mappings.TryGetValue(id, out string value)) {
            return value;
        }
        return "Unknown button";
    }
}
