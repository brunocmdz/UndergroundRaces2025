using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System;
using UndergroundRaces;

namespace UndergroundRaces
{
    public class EscenaMenuAjustes : IEscena
    {
        private Texture2D _fondoAjustes;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;
        private Rectangle _botonAtras;
        private MouseState _mouse;

        public Action OnVolverClick;


        public void LoadContent(Game game)
        {
            _graphicsDevice = game.GraphicsDevice;
            _content = game.Content;

            _fondoAjustes = _content.Load<Texture2D>("images/menu-ajustes-underground-races-2025");
            _fondoAjustes = _content.Load<Texture2D>("images/menu-ajustes-underground-races-2025");

            _botonAtras = new Rectangle(20, 20, 60, 60); // ajustá tamaño si querés más grande

        }

        public void Update(GameTime gameTime)
        { 
            _mouse = Mouse.GetState();

            if (_botonAtras.Contains(_mouse.Position) && _mouse.LeftButton == ButtonState.Pressed)
            {
                OnVolverClick?.Invoke();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_fondoAjustes, new Rectangle(0, 0, 1024, 576), Color.White);
            spriteBatch.End();
        }
    }
}
