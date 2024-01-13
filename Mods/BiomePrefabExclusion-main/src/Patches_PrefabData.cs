using System;
using System.Collections.Generic;
using HarmonyLib;
using WorldGenerationEngineFinal;

namespace BiomePrefabExclusion.Patches__PrefabData;

[HarmonyPatch(typeof(PrefabData), MethodType.Constructor, new[]{
		typeof(PathAbstractions.AbstractedLocation),
		typeof(DynamicProperties)
	})]
static class Ctor1
{
	static void Postfix(PrefabData __instance, DynamicProperties properties)
	{
		string strAllowedBiomes = "AllowedBiomes";

		if(!properties.Values.ContainsKey(strAllowedBiomes)){
			return;
		}

		string value = properties.Values[strAllowedBiomes];
		string[] tags = value.Replace(" ", "").Split(',');

		Ll.Debug($"Prefab '{__instance.Name}' has tags '{string.Join(",", tags)}'.");

		List<BiomeType> biomeTags = new();

		foreach(string t in tags){
			Ll.Debug($"Iterating tags of '{__instance.Name}' -> '{t}'");
			string[] biomeTypeNames = Enum.GetNames(typeof(BiomeType));
			Array biomeTypeValues = Enum.GetValues(typeof(BiomeType));

			for(int i = 0; i < biomeTypeNames.Length; i++){
				Ll.Debug($"Comparing tag '{t}' to enum value '{biomeTypeNames[i]}' ...");
				if (!biomeTypeNames[i].ToLower().Equals(t.ToLower())) continue;
				Ll.Debug($"Comparison was equal. Adding BiomeType ...");
				biomeTags.Add((BiomeType) biomeTypeValues.GetValue(i));
				break;
			}
		}

		Ll.Debug($"Adding '{__instance.Name}' with {biomeTags.Count} BiomeType tags.");

		if(!Common.AllowedBiomes.ContainsKey(__instance.Name)){
			Common.AllowedBiomes.Add(__instance.Name, biomeTags.ToArray());
		}
	}
}