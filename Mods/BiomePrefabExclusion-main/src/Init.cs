using System.Diagnostics;
using HarmonyLib;

namespace BiomePrefabExclusion;

public class Init : IModApi
{
	public void InitMod(Mod _modInstance)
	{
		Harmony.CreateAndPatchAll(typeof(Init).Assembly, 
			"Redbeardt.7DTD.BiomePrefabExclusion");
	}
}

public static class Ll
{
	private const string cPrefix = "[BiomePrefabExclusion] ";

	public static void Info(string str) =>
		Log.Out(cPrefix + str);

	public static void Warn(string str) =>
		Log.Warning(cPrefix + str);

	public static void Error(string str) =>
		Log.Error(cPrefix + str);

	[Conditional("DEBUG")]
	public static void Debug(string str) =>
		Log.Out(cPrefix + str);
}