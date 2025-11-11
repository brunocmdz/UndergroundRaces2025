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

        // --- Fondo (atlas de frames) ---
        private Texture2D _fondoAtlas;
        private List<Rectangle> _framesFondo = new();
        private int _frameActual = 0;
        private int _totalFrames = 14;
        private float _timerFrame = 0f;
        private float _tiempoPorFrame = 0.02f;
        private bool _avanzando = false;

        // --- Corsa ---
        private Texture2D _corsaAtlas;                 // atlas con 2 frames (quieto/acelerando)
        private List<Rectangle> _framesCorsa = new();  // frames del atlas
        private int _frameCorsaActual = 0;             // frame actual
        private float _timerCorsa = 0f;                // temporizador animación
        private float _tiempoPorFrameCorsa = 0.08f;    // velocidad de cambio
        private Texture2D _corsaDoblando;              // textura del corsa doblando
        private bool _usandoAtlas = true;              // true = usa corsaPLANTILLA, false = doblando
        private Vector2 _corsaPosition;
        private SpriteEffects _spriteEffect = SpriteEffects.None;

        // --- Sonido motor ---
        private SoundEffect _motorSound;
        private SoundEffectInstance _motorInstance;
        private float _motorVolume = 0f;
        private const float _volumenMaximo = 0.5f;
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

            // Corsa
            _corsaAtlas = Content.Load<Texture2D>("images/corsaPLANTILLA"); // atlas con 2 frames
            GenerarFramesCorsa(_corsaAtlas, _corsaAtlas.Width, _corsaAtlas.Height / 2);
            _corsaDoblando = Content.Load<Texture2D>("images/corsa-underground-races-2025-doblando");

            // Sonido motor
            _motorSound = Content.Load<SoundEffect>("audio/motor-corsa");
            _motorInstance = _motorSound.CreateInstance();
            _motorInstance.IsLooped = true;
            _motorInstance.Volume = 0f;
            _motorInstance.Play();
        }

        // --- Métodos auxiliares ---
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

        private void GenerarFramesCorsa(Texture2D atlas, int anchoFrame, int altoFrame)
        {
            int filas = atlas.Height / altoFrame;
            for (int y = 0; y < filas; y++)
            {
                _framesCorsa.Add(new Rectangle(0, y * altoFrame, anchoFrame, altoFrame));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            float velocidad = 5f;

            int screenWidth = GraphicsDevice.Viewport.Width;
            float corsaAncho = _corsaAtlas.Width * 3f;
            float corsaMitad = corsaAncho / 2f;

            float rutaMargenIzquierdo = 200f;
            float rutaMargenDerecho = screenWidth - 200f;
            float limiteIzquierdo = rutaMargenIzquierdo - corsaMitad;
            float limiteDerecho = rutaMargenDerecho + corsaMitad;

            // Movimiento lateral
            if (state.IsKeyDown(Keys.D))
            {
                _usandoAtlas = false;
                _spriteEffect = SpriteEffects.None;
                if (_corsaPosition.X + velocidad < limiteDerecho)
                    _corsaPosition.X += velocidad;
            }
            else if (state.IsKeyDown(Keys.A))
            {
                _usandoAtlas = false;
                _spriteEffect = SpriteEffects.FlipHorizontally;
                if (_corsaPosition.X - velocidad > limiteIzquierdo)
                    _corsaPosition.X -= velocidad;
            }
            else
            {
                _usandoAtlas = true;
                _spriteEffect = SpriteEffects.None;
            }

            // Control de avance (solo avanza el fondo si se mantiene W)
            _avanzando = state.IsKeyDown(Keys.W);
            if (_avanzando)
            {
                // Fondo animado
                _timerFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timerFrame >= _tiempoPorFrame)
                {
                    _timerFrame = 0f;
                    _frameActual++;
                    if (_frameActual >= _framesFondo.Count)
                        _frameActual = 0;
                }

                // Corsa animado (alternando entre los dos frames)
                _timerCorsa += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timerCorsa >= _tiempoPorFrameCorsa)
                {
                    _timerCorsa = 0f;
                    _frameCorsaActual = (_frameCorsaActual + 1) % _framesCorsa.Count;
                }
            }
            else
            {
                _frameCorsaActual = 0; // quieto
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

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

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

            // Auto (usa atlas o doblando según estado)
            if (_usandoAtlas)
            {
                Rectangle corsaRect = _framesCorsa[_frameCorsaActual];
                Vector2 origin = new Vector2(corsaRect.Width / 2f, corsaRect.Height / 2f);
                _spriteBatch.Draw(
                    _corsaAtlas,
                    _corsaPosition,
                    corsaRect,
                    Color.White,
                    0f,
                    origin,
                    3f,
                    _spriteEffect,
                    0f
                );
            }
            else
            {
                Vector2 origin = new Vector2(_corsaDoblando.Width / 2f, _corsaDoblando.Height / 2f);
                _spriteBatch.Draw(
                    _corsaDoblando,
                    _corsaPosition,
                    null,
                    Color.White,
                    0f,
                    origin,
                    3f,
                    _spriteEffect,
                    0f
                );
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
