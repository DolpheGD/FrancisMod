using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace FrancisMod.Content.Items.Placeables
{
	public class FrancisBar : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 59; // Influences the inventory sort order. 59 is PlatinumBar, higher is more valuable.

		}

		public override void SetDefaults() {
			// ModContent.TileType returns the ID of the tile that this item should place when used. ModContent.TileType<T>() method returns an integer ID of the tile provided to it through its generic type argument (the type in angle brackets)
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.FrancisBar>());
			Item.width = 20;
			Item.height = 20;
			Item.SetShopValues(ItemRarityColor.Blue1, 2590);
		}
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<FrancisOre>(4)
				.AddTile(TileID.Furnaces)
				.Register();
		}
	}
}
