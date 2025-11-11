using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;

namespace UndergroundRaces
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private IEscena _escenaActual;
        private EscenaMenu _menu;
        private EscenaJuego _juego;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 576;

            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Inicializar escena de menú
            _menu = new EscenaMenu();
            _menu.OnJugarClick = CambiarAEscenaJuego; // callback para cambiar de escena
            _menu.LoadContent(this);

            // Inicializar escena de juego
            _juego = new EscenaJuego();
            _juego.LoadContent(this);

            // Escena inicial: el menú
            _escenaActual = _menu;
        }

        protected override void Update(GameTime gameTime)
        {
            _escenaActual.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _escenaActual.Draw(_spriteBatch);
            base.Draw(gameTime);
        }
        private void CambiarAEscenaJuego()
        {
            _escenaActual = _juego;
        }

    }
}
