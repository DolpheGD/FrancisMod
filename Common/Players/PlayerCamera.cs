using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace FrancisMod.Common.Players
{
	public class PlayerCamera : ModPlayer
	{
        public static Vector2 destination;
        public static float step;
        public static bool setNPCPos = false;
        public override void ModifyScreenPosition()
        {
            if (setNPCPos)
            {
                Vector2 correctedDestination = new Vector2(destination.X - Main.screenWidth/2, destination.Y - Main.screenHeight/2);
                Main.screenPosition = Vector2.Lerp(Main.screenPosition, correctedDestination, step);
            }
        }
    }
}
