using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
// ... dentro de tu clase Game1 (o similar)

namespace UndergroundRaces;

public class Game1 : Game
{
    private Texture2D miSprite;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Crear un nuevo SpriteBatch, que se usa para dibujar texturas 2D.
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Carga la textura. Asume que agregaste un archivo llamado 'personaje.png' 
        // en la raíz de tu proyecto de Contenido.
        miSprite = Content.Load<Texture2D>("personaje"); 
        
        // Si lo hubieras puesto en una carpeta 'Imagenes' dentro del Content Pipeline:
        // miSprite = Content.Load<Texture2D>("Imagenes/personaje"); 
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}
