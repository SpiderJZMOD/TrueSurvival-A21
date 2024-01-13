using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using WorldGenerationEngineFinal;

namespace BiomePrefabExclusion.Patches_PrefabManager;

[HarmonyPatch(typeof(PrefabManager), "GetPrefabWithDistrict")]
static class GetPrefabWithDistrict
{
	static IEnumerable<CodeInstruction> Transpiler(
			IEnumerable<CodeInstruction> ins)
	{
		CodeMatcher cm = new(ins);

		cm.SearchForward(a => a.operand is MethodInfo {Name: "get_Count"}).
			Advance(-1).
			Insert(
				new CodeInstruction(OpCodes.Ldloc_1),
				new CodeInstruction(OpCodes.Ldarg_S, 7),
				Transpilers.EmitDelegate((
						List<PrefabData> list,
						Township ts) =>
				{
					List<PrefabData> toRemove = new();

					foreach(var data in list){
						// If exclusivity hasn't been defined, allow it.
						if(!Common.AllowedBiomes.TryGetValue(data.Name, out var biomes)){
							continue;
						}

						// If exclusivity has been defined but it doesn't include this
						// township's biome, remove it from the allowed prefabs list.
						if(!biomes.Any(a => a == ts.BiomeType)){
							toRemove.Add(data);
						}
					}

					toRemove.Do(a => list.Remove(a));
				})
			);

		return cm.InstructionEnumeration();
	}
}

[HarmonyPatch(typeof(PrefabManager), "GetWildernessPrefab")]
static class GetWildernessPrefab
{
	static IEnumerable<CodeInstruction> Transpiler(
			IEnumerable<CodeInstruction> ins,
			MethodBase __originalMethod,
			ILGenerator gen)
	{
		CodeMatcher cm = new(ins);

		cm.Start().
			SearchForward(a => a.operand is MethodInfo {Name: "get_Count"}).
			Advance(-1).
			Insert(
				new CodeInstruction(OpCodes.Ldloc_1),
				new CodeInstruction(OpCodes.Ldarg_S, 5),
				Transpilers.EmitDelegate((
						List<PrefabData> list,
						Vector2i center) => 
				{
					List<PrefabData> toRemove = new();
					foreach(var data in list){
						if(!Common.AllowedBiomes.TryGetValue(data.Name, out var biomes)){
							continue;
						}

						var biomeType = WorldBuilder.Instance.GetBiome(center.x, center.y);

						if(!biomes.Any(a => a == biomeType)){
							toRemove.Add(data);
						}
					}

					toRemove.Do(a => list.Remove(a));
				})
			);

		return cm.InstructionEnumeration();
	}
}