using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items;
using Terraria.Enums;
using CalamityMod.Rarities;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using FrancisMod.Content.Projectiles.FriendlyProjectiles;
using Terraria.Audio;
using FrancisMod.Content.Items.Placeables;
using CalamityMod.Items.Weapons.DraedonsArsenal;


namespace FrancisMod.Content.Items.Weapons
{
    public class DiscOfFrancis : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.damage = 373;
            Item.width = 86;
            Item.height = 86;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            
            Item.useAnimation = 25;
            Item.useTime = 25;

            Item.useStyle = ItemUseStyleID.Swing;

            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;


            Item.maxStack = 1;
            Item.SetShopValues(ItemRarityColor.Cyan9, 10101010);
            Item.shoot = ModContent.ProjectileType<DiscOfFrancisProjectile>();

            Item.shootSpeed = 14f;

            Item.rare = ModContent.RarityType<Turquoise>();
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            if (player.Calamity().StealthStrikeAvailable())
            {
                    int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, Item.shootSpeed, 0f, 2f);
                    
                    if (p.WithinBounds(Main.maxProjectiles))
                        Main.projectile[p].Calamity().stealthStrike = true;
    
                return false;
            }
            return true;
        }

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<RefinedFrancisBar>(), 25);
			recipe.AddIngredient(ModContent.ItemType<TrackingDisk>(), 1);
			recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
			recipe.AddIngredient(ModContent.ItemType<LifeAlloy>(), 5);
			
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}


    }
}