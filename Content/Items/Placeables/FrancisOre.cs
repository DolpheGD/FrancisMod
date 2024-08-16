using Terraria;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ID;

namespace FrancisMod.Content.Items.Placeables
{
    internal class FrancisOre : ModItem
    {
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 100;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;

			// This ore can spawn in slime bodies like other pre-boss ores. (copper, tin, iron, etch)
			// It will drop in amount from 3 to 13.
			ItemID.Sets.OreDropsFromSlime[Type] = (1, 4);
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.FrancisOre>());
			Item.width = 12;
			Item.height = 12;
			Item.SetShopValues(ItemRarityColor.Blue1, 750);
		}
    }
}