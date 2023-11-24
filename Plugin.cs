using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace FixInverseWheel
{

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class FixInverseWheel : BaseUnityPlugin
    {
        private static FixInverseWheel _instance;
        public static FixInverseWheel Instance => _instance ??= FindAnyObjectByType<FixInverseWheel>();

        internal new static ManualLogSource Logger;

        public ConfigEntry<bool> IsInvertScrollDirection { get; private set; }

        private Harmony _harmony;
        private bool _isPatched;

        private void Awake()
        {
            _instance = this;

            Logger = base.Logger;

            IsInvertScrollDirection = Config.Bind("General", "InvertScrollDirection", true, "Invert scroll direction");
            Logger.LogInfo($"Invert scroll direction loaded from config: {IsInvertScrollDirection.Value}");

            PatchAll();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public void PatchAll()
        {
            if (_isPatched)
            {
                Logger.LogWarning("Already patched!");
                return;
            }

            Logger.LogDebug("Patching...");

            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();
            _isPatched = true;

            Logger.LogDebug("Patched!");
        }

        public void UnpatchAll()
        {
            if (!_isPatched)
            {
                Logger.LogWarning("Not patched!");
                return;
            }

            Logger.LogDebug("Unpatching...");

            _harmony.UnpatchSelf();
            _isPatched = false;

            Logger.LogDebug("Unpatched!");
        }
    }
}
