using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace UndergroundRaces;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _fondo;
    private Texture2D _corsaAvanzando;
    private Texture2D _corsaDoblando;
    private Texture2D _corsaActual;

    private SoundEffect _motorSound;
    private SoundEffectInstance _motorInstance;
    private float _motorVolume = 0f;
    private const float _volumenMaximo = 1f;
    private const float _velocidadCambioVolumen = 0.01f;

    private Vector2 _corsaPosition;
    private SpriteEffects _spriteEffect = SpriteEffects.None;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        int screenWidth = GraphicsDevice.Viewport.Width;
        _corsaPosition = new Vector2(screenWidth / 2f, 700);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _fondo = Content.Load<Texture2D>("images/background");
        _corsaAvanzando = Content.Load<Texture2D>("images/corsa-underground-races-2025-avanzando");
        _corsaDoblando = Content.Load<Texture2D>("images/corsa-underground-races-2025-doblando");
        _corsaActual = _corsaAvanzando;

        _motorSound = Content.Load<SoundEffect>("audio/motor-corsa");
        _motorInstance = _motorSound.CreateInstance();
        _motorInstance.IsLooped = true;
        _motorInstance.Volume = 0f;
        _motorInstance.Play();
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

        if (state.IsKeyDown(Keys.W))
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
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        int screenWidth = GraphicsDevice.Viewport.Width;
        int screenHeight = GraphicsDevice.Viewport.Height;

        Vector2 origin = new Vector2(_corsaActual.Width / 2f, _corsaActual.Height / 2f);

        _spriteBatch.Draw(
            _fondo,
            new Rectangle(0, 0, screenWidth, screenHeight),
            Color.White
        );

        _spriteBatch.Draw(
            _corsaActual,
            _corsaPosition,
            null,
            Color.White,
            0f,
            origin,
            3f, // escala x3
            _spriteEffect,
            0f
        );

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}