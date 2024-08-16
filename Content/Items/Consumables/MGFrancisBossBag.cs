using FrancisMod.Content.Items.Accessories;
using FrancisMod.Content.Items.NonPlaceables;
using FrancisMod.Content.Items.Placeables;
using FrancisMod.Content.Items.Weapons;
using FrancisMod.Content.NPCs.Bosses.MetalGearFrancis;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FrancisMod.Content.Items.Consumables
{
	// Basic code for a boss treasure bag
	public class MGFrancisBossBag : ModItem
	{
		public override void SetStaticDefaults() {
			// This set is one that every boss bag should have.
			// It will create a glowing effect around the item when dropped in the world.
			// It will also let our boss bag drop dev armor..
			ItemID.Sets.BossBag[Type] = true;
			Item.ResearchUnlockCount = 3;
		}

		public override void SetDefaults() {
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Purple;
			Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
		}

		public override bool CanRightClick() {
			return true;
		}

		public override void ModifyItemLoot(ItemLoot itemLoot) {
			// We have to replicate the expert drops from MinionBossBody here
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<RefinedFrancisBar>(), 1, 13, 17));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrancisBar>(), 1, 5, 7));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrancisDust>(), 1, 5, 9));

			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkinnyPop>(), 1, 5, 7));
			
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheFrancisSlayer>(), 4));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<FishHeadLauncher>(), 4));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<StaffOfTheEggBiscuit>(), 4));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrancisMinionStaff>(), 4));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DiscOfFrancis>(), 4));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<EGlasses>(), 4));
		}
	}
}
