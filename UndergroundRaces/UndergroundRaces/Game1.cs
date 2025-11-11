using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace UndergroundRaces
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Fondos (frames)
        private Texture2D _fondoAtlas;
        private List<Rectangle> _framesFondo = new();
        private int _frameActual = 0;
        private int _totalFrames = 14; // cantidad de frames reales (0–13)
        private float _timerFrame = 0f;
        private float _tiempoPorFrame = 0.02f; // velocidad de cambio de frame
        private bool _avanzando = false;

        // Corsa
        private Texture2D _corsaAvanzando;
        private Texture2D _corsaDoblando;
        private Texture2D _corsaActual;
        private Vector2 _corsaPosition;
        private SpriteEffects _spriteEffect = SpriteEffects.None;

        // Sonido motor
        private SoundEffect _motorSound;
        private SoundEffectInstance _motorInstance;
        private float _motorVolume = 0f;
        private const float _volumenMaximo = 1f;
        private const float _velocidadCambioVolumen = 0.01f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 576;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            _corsaPosition = new Vector2(screenWidth / 2f, 500);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Fondo (atlas con varios frames)
            _fondoAtlas = Content.Load<Texture2D>("images/backgroundPLANTILLA2");
            GenerarFramesFondo(_fondoAtlas, 1024, 576);

            _corsaAvanzando = Content.Load<Texture2D>("images/corsa-underground-races-2025-avanzando");
            _corsaDoblando = Content.Load<Texture2D>("images/corsa-underground-races-2025-doblando");
            _corsaActual = _corsaAvanzando;

            _motorSound = Content.Load<SoundEffect>("audio/motor-corsa");
            _motorInstance = _motorSound.CreateInstance();
            _motorInstance.IsLooped = true;
            _motorInstance.Volume = 0f;
            _motorInstance.Play();
        }

        private void GenerarFramesFondo(Texture2D atlas, int anchoFrame, int altoFrame)
        {
            int columnas = atlas.Width / anchoFrame;
            int filas = atlas.Height / altoFrame;

            for (int y = 0; y < filas; y++)
            {
                for (int x = 0; x < columnas; x++)
                {
                    _framesFondo.Add(new Rectangle(x * anchoFrame, y * altoFrame, anchoFrame, altoFrame));
                }
            }

            if (_framesFondo.Count > _totalFrames)
                _framesFondo = _framesFondo.GetRange(0, _totalFrames);
        }

        protected override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            float velocidad = 5f;

            int screenWidth = GraphicsDevice.Viewport.Width;
            float corsaAncho = _corsaActual.Width * 3f;
            float corsaMitad = corsaAncho / 2f;

            float rutaMargenIzquierdo = 200f;
            float rutaMargenDerecho = screenWidth - 200f;
            float limiteIzquierdo = rutaMargenIzquierdo - corsaMitad;
            float limiteDerecho = rutaMargenDerecho + corsaMitad;

            // Movimiento lateral
            if (state.IsKeyDown(Keys.D))
            {
                _corsaActual = _corsaDoblando;
                _spriteEffect = SpriteEffects.None;
                if (_corsaPosition.X + velocidad < limiteDerecho)
                    _corsaPosition.X += velocidad;
            }
            else if (state.IsKeyDown(Keys.A))
            {
                _corsaActual = _corsaDoblando;
                _spriteEffect = SpriteEffects.FlipHorizontally;
                if (_corsaPosition.X - velocidad > limiteIzquierdo)
                    _corsaPosition.X -= velocidad;
            }
            else
            {
                _corsaActual = _corsaAvanzando;
                _spriteEffect = SpriteEffects.None;
            }

            // Control de avance (solo avanza el fondo si se mantiene W)
            _avanzando = state.IsKeyDown(Keys.W);
            if (_avanzando)
            {
                _timerFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timerFrame >= _tiempoPorFrame)
                {
                    _timerFrame = 0f;
                    _frameActual++;
                    if (_frameActual >= _framesFondo.Count)
                        _frameActual = 0;
                }
            }

            // Sonido del motor
            if (_avanzando)
            {
                _motorVolume += _velocidadCambioVolumen;
                if (_motorVolume > _volumenMaximo)
                    _motorVolume = _volumenMaximo;
            }
            else
            {
                _motorVolume -= _velocidadCambioVolumen;
                if (_motorVolume < 0f)
                    _motorVolume = 0f;
            }
            _motorInstance.Volume = _motorVolume;

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp
            );

            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;

            // Fondo animado
            Rectangle frameRect = _framesFondo[_frameActual];
            _spriteBatch.Draw(
                _fondoAtlas,
                new Rectangle(0, 0, screenWidth, screenHeight),
                frameRect,
                Color.White
            );

            // Auto
            Vector2 origin = new Vector2(_corsaActual.Width / 2f, _corsaActual.Height / 2f);
            _spriteBatch.Draw(
                _corsaActual,
                _corsaPosition,
                null,
                Color.White,
                0f,
                origin,
                3f,
                _spriteEffect,
                0f
            );

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
