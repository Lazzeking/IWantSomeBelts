using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace IWantSomeBelts;

public class IWantSomeBeltsModSystem : ModSystem
{
    private const string ConfigName = "ToolbeltConfig.json";

    //Insert bugs bunny ours meme
    private static readonly (string Code, int Slots)[] Belts =
    {
        //Tier 1 slot belts
        ("game:clothes-waist-linen-rope", 1),
        ("game:clothes-waist-peasantapron", 1),

        //Tier 2 slot belts
        ("game:clothes-waist-ruralhunter", 2),
        ("game:clothes-waist-marketeer", 2),
        ("game:clothes-waist-moss-embroidered-belt", 2),
        ("game:clothes-waist-wanderer", 2),
        ("game:clothes-waist-reindeer-herder-waistband", 2),
        ("game:clothes-waist-tailor-belt", 2),
        ("game:clothes-waist-malefactor-sash", 2),

        //Tier 3 slot belts
        ("game:clothes-waist-heavy-belt", 3),
        ("game:clothes-waist-warrior", 3),
        ("game:clothes-waist-bronzebelt", 3),
        ("game:clothes-waist-blackguard-belt", 3),
        ("game:clothes-waist-clockmaker-belt", 3),
        ("game:clothes-waist-forlorn-belt", 3),
        ("game:clothes-waist-jailor-belt", 3),
        ("game:clothes-waist-squire-belt", 3),

        //Tier 4 slot belts - Maximum allowed
        ("game:clothes-waist-sturdy-belt", 4),
        ("game:clothes-waist-miner", 4),
        ("game:clothes-waist-sturdy-leather-belt", 4),
        ("game:clothes-waist-heavy-tool-belt", 4)
    };

    public override double ExecuteOrder() => 0;

    public override void StartServerSide(ICoreServerAPI api)
    {
        try
        {
            RegisterBelts(api);
        }
        catch (Exception ex)
        {
            /*
            Here we had some checks to see if the mod was enabled and then register the belts
            recipes without toolbelts support, but idk doesnt seem like someone would use the
            mod without toolbelts.
            Remember: we need to check also client side in case we want to add some desc based
            on the mod presence or not.
            */
            api.Logger.Error("I Want Some Belts has failed to register belts with Tool Belt. {0}", ex.ToString());
        }
    }

    private static void RegisterBelts(ICoreServerAPI api)
    {
        ToolbeltConfigFile config = api.LoadModConfig<ToolbeltConfigFile>(ConfigName) ?? new ToolbeltConfigFile();
        if (config.BeltSlots == null) config.BeltSlots = new List<BeltSlotEntry>();

        int changed = 0;
        foreach ((string code, int slots) in Belts)
        {
            if (SetSlot(config, code, slots)) changed++;
        }

        if (changed == 0) return;

        api.StoreModConfig(config, ConfigName);
        api.Logger.Notification("I Want Some Belts has correctly updated Toolbelt's config file with {0} new or changed belts.", changed);
    }

    private static bool SetSlot(ToolbeltConfigFile config, string code, int slots)
    {
        foreach (BeltSlotEntry entry in config.BeltSlots)
        {
            if (entry == null) continue;
            if (!string.Equals(entry.BeltCode, code, StringComparison.OrdinalIgnoreCase)) continue;

            if (entry.Slots == slots) return false;
            entry.Slots = slots;
            return true;
        }

        config.BeltSlots.Add(new BeltSlotEntry { BeltCode = code, Slots = slots });
        return true;
    }
}

public class ToolbeltConfigFile
{
    public int Version { get; set; } = 1;
    public List<BeltSlotEntry> BeltSlots { get; set; } = new();
}

public class BeltSlotEntry
{
    public string BeltCode { get; set; } = "";
    public int Slots { get; set; }
}
