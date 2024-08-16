    using Microsoft.Xna.Framework;
    using System;
    using Terraria;
    using Terraria.Audio;
    using Terraria.ModLoader;
    using Terraria.GameContent.Bestiary;
    using Terraria.GameContent.ItemDropRules;
    using Terraria.ID;
    using Terraria.ModLoader.Utilities;
    using FrancisMod.Content.Items;

    namespace FrancisMod.Content.Common
    {
        public class MyGlobalNPC : GlobalNPC
        {
            public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
            {
                if (npc.type == NPCID.Skeleton)
                {
                    npcLoot.Add(ItemDropRule.Common(ItemID.Shackle, 6, 1, 4));
                }
            }
        }
    }