using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System;
namespace UndergroundRaces
{
    public interface IEscena
    {
        void LoadContent(Game game);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
