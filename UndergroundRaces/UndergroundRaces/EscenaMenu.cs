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
    public class EscenaMenu : IEscena
    {
        private Texture2D _fondoMenu;
        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;

        private Rectangle _botonJugar;
        private Rectangle _botonSalir;
        private MouseState _mouse;
        public Action OnJugarClick;

        public void LoadContent(Game game)
        {
            _graphicsDevice = game.GraphicsDevice;
            _content = game.Content;
            _botonJugar = new Rectangle(412, 220, 200, 60);
            _botonSalir = new Rectangle(412, 490, 200, 60);


            _fondoMenu = _content.Load<Texture2D>("images/menu-principal-underground-races-2025");
        }

        public void Update(GameTime gameTime)
        {
            _mouse = Mouse.GetState();

        if (_botonJugar.Contains(_mouse.Position) && _mouse.LeftButton == ButtonState.Pressed)
        {
            OnJugarClick?.Invoke(); 
        }

        if (_botonSalir.Contains(_mouse.Position) && _mouse.LeftButton == ButtonState.Pressed)
        {
            Environment.Exit(0); 
        }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_fondoMenu, new Rectangle(0, 0, 1024, 576), Color.White);
            spriteBatch.End();
        }
    }
}
