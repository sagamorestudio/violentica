using UnityEngine;

namespace I2.Loc
{
	public static class ScriptLocalization
	{

		public static string Language 		{ get{ return LocalizationManager.GetTranslation ("Language"); } }
		public static string TestKey 		{ get{ return LocalizationManager.GetTranslation ("TestKey"); } }
	}

    public static class ScriptTerms
	{

		public const string Language = "Language";
		public const string TestKey = "TestKey";
	}
}