#nullable enable
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LyiarOwl.Camera;

/// <summary>
/// <para>2d camera with an orthographic <c>Projection</c> and two more views, one
/// to use with your <c>SpriteBatch</c> (via <c>transformMatrix</c> or <c>BasicEffect</c>) 
/// and another to use with the debug view of any physics engine that derives 
/// from Aether.Physics (like the Aether.Physics itself or Velcro Physics).</para>
/// 
/// <para><see cref="https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SpriteBatch.html#parameters-2"/></para>
/// <para><see cref="https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BasicEffect.html"/></para>
/// <para><see cref="https://github.com/nkast/Aether.Physics2D"/></para>
/// <para><see cref="https://github.com/Genbox/VelcroPhysics"/></para>
/// </summary>
public class OrthographicCamera2D
{
    #region Private Members
    private BasicEffect? mBasicEffect;
    private FitViewport? mViewport;
    private float mPpm = 16f;
    private float mInvPpm;
    private readonly GraphicsDevice mGraphicsDevice;
    #endregion


    #region Public Members
    /// <summary>
    /// <para>This is a <c>OrthographicOffCenter</c> projection.</para>
    /// <para>If you are using some effect (like the <c>BasicEffect</c>), this projection
    /// should be used.</para>
    /// </summary>
    public Matrix Projection;
    /// <summary>
    /// This View must be applied to the <c>transformMatrix</c> from the <c>SpriteBatch</c>
    /// or to the <c>View</c> attribute from your effect (like the <c>BasicEffect</c>).
    /// </summary>
    public Matrix SpriteBatchView;
    /// <summary>
    /// This view is only useful if you are using the debug view from some physics engine
    /// like Aether Physics or Velcro's Physics. 
    /// </summary>
    public Matrix DebugRendererView;

    public float Zoom = 1f;
    public Vector2 Position;

    /// <summary>
    /// <para>Define the scale in which the <c>DebugRendererView</c> will be scaled.</para>
    /// <para>Ensure to set this attribute according with the scale you want to work.</para>
    /// <para>To convert from meters to pixels, multiply the value by this attribute. To convert from
    /// pixels to meters, just divide.</para>
    /// </summary>
    public float Ppm
    {
        get => mPpm;
        set
        {
            mPpm = value;
            mInvPpm = 1f / mPpm;
        }
    }
    /// <summary>
    /// When converting from pixels to meters you can multiply the value in pixels by this
    /// property and the result will be in meters (avoiding divisions).
    /// </summary>
    public float InvPpm => mInvPpm;
    #endregion


    #region Constructors
    /// <summary>
    /// Creates an orthographic camera with an extensive view, that is, when the game is
    /// maximized, more the player can see.
    /// </summary>
    public OrthographicCamera2D(GraphicsDevice graphicsDevice)
    {
        mGraphicsDevice = graphicsDevice;
    }

    /// <param name="graphicsDevice"></param>
    /// <param name="basicEffect">Only useful if you're using some physics engine.</param>
    /// <param name="viewport">Very useful if you want that, when the game is maximized, the image
    /// be scaled to fit the new window size.</param>
    public OrthographicCamera2D(GraphicsDevice graphicsDevice, BasicEffect? basicEffect, FitViewport? viewport = null)
        : this(graphicsDevice)
    {
        mBasicEffect = basicEffect;
        mViewport = viewport;
        mInvPpm = 1f / mPpm;
    }
    #endregion


    #region Public Methods
    public void Update()
    {
        PresentationParameters parameters = mGraphicsDevice.PresentationParameters;
        Matrix origin;
        Matrix translation;
        Matrix zoom;

        if (mBasicEffect != null && mViewport != null)
        {
            return;
        }
        if (mBasicEffect != null)
        {
            return;
        }

        // only transformation and viewport
        if (mViewport != null)
        {
            Viewport viewport = mGraphicsDevice.Viewport;
            origin = Matrix.CreateTranslation(
                parameters.BackBufferWidth * 0.5f - viewport.X,
                parameters.BackBufferHeight * 0.5f - viewport.Y,
                0f
            );
            translation = Matrix.CreateTranslation(-Position.X, -Position.Y, 0f);
            zoom = Matrix.CreateScale(Zoom);
            SpriteBatchView = translation * zoom * mViewport.ScalingMatrix * origin;
            return;
        }

        // isnt using viewport and basic effects
        origin = Matrix.CreateTranslation(
            parameters.BackBufferWidth * 0.5f,
            parameters.BackBufferHeight * 0.5f,
            0f
        );
        translation = Matrix.CreateTranslation(-Position.X, -Position.Y, 0f);
        zoom = Matrix.CreateScale(Zoom);

        SpriteBatchView = translation * zoom * origin;
    }
    #endregion
}