using Terraria;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ID;

namespace FrancisMod.Content.Items.NonPlaceables
{
    internal class FrancisDust : ModItem
    {
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 100;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;
		}

		public override void SetDefaults() {
			Item.width = 26;
			Item.height = 14;
			Item.maxStack = 9999;
			Item.SetShopValues(ItemRarityColor.LightRed4, 1405);
		}
    }
}