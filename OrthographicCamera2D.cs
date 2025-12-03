#nullable enable
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LyiarOwl.Camera;

/// <summary>
/// <para>2d camera with an orthographic <c>Projection</c> and two more views, one
/// to use with your <c>SpriteBatch</c> (via <c>transformMatrix</c> or <c>BasicEffect</c>) 
/// and another to use with the debug view of any physics engine that derives 
/// from Aether.Physics (like the Aether.Physics itself or Velcro Physics).</para>
/// 
/// <para><see cref="SpriteBatch.Begin"/></para>
/// <para><see cref="BasicEffect"/></para>
/// <para><see href="https://github.com/nkast/Aether.Physics2D"/></para>
/// <para><see href="https://github.com/Genbox/VelcroPhysics"/></para>
/// </summary>
public class OrthographicCamera2D
{
    /// <summary>
    /// <para>
    /// Creates a <c>BasicEffect</c> with <c>TextureEnabled</c> and <c>VertexColorEnabled</c> enabled.
    /// </para>
    /// <seealso cref="BasicEffect"/>
    /// <seealso cref="Microsoft.Xna.Framework.Graphics.BasicEffect.TextureEnabled"/>
    /// <seealso cref="Microsoft.Xna.Framework.Graphics.BasicEffect.VertexColorEnabled"/>
    /// </summary>
    /// <param name="graphicsDevice"></param>
    /// <returns></returns>
    public static BasicEffect CreateDefaultEffect(GraphicsDevice graphicsDevice)
    {
        return new BasicEffect(graphicsDevice)
        {
            TextureEnabled = true,
            VertexColorEnabled = true
        };
    }

    #region Private Members

    private readonly FitViewport? mViewport;
    private readonly GraphicsDevice mGraphicsDevice;
    private float mInvPpm;
    private float mPpm;

    #endregion


    #region Public Members

    public BasicEffect? BasicEffect;

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
    public Matrix PhysicsDebugView;

    public float Zoom = 1f;
    public Vector2 Position;

    /// <summary>
    /// Rotates the camera in radians.
    /// </summary>
    public float Angle = 0f;

    /// <summary>
    /// <para>Define the scale in which the <c>PhysicsDebugView</c> will be scaled.</para>
    /// <para>Ensure to set this attribute according to the scale you want to work.</para>
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
    /// <param name="basicEffect">Only useful if you're using some physics engine. If this is the case, make sure
    /// to change the <c>Ppm</c> attribute (<c>16</c> is the default, that is each 16px is equal to 1 meter).</param>
    /// <param name="viewport">Very useful if you want that, when the game is maximized, the image
    /// be scaled to fit the new window size.</param>
    public OrthographicCamera2D(GraphicsDevice graphicsDevice, BasicEffect? basicEffect, FitViewport? viewport = null)
        : this(graphicsDevice)
    {
        BasicEffect = basicEffect;
        mViewport = viewport;

        viewport?.Apply();

        if (basicEffect is null)
            return;

        Ppm = 16f;
        Update();
    }

    #endregion


    #region Public Methods

    public void Update()
    {
        // case: the dev want to integrate the spritebatch with the debug view of some physics engine and want a fitting view
        // (mViewport isn't null)
        // detail: the view will be restricted to the resolution (regardless of the size of the window, the image will be
        // rescaled to fit it proportionally), but keeping usage support for the physics engine's debug view

        // case: the dev want to integrate the spritebatch with the debug view of some physics engine
        // (mViewport is null)
        // detail: the view will be extensive (the more large is the window, more pixels will be visible), but keeping
        // usage support for the physics engine's debug view
        if (BasicEffect != null)
        {
            HandleBasicEffectOnly();
            return;
        }

        // case: the dev want simple transformation + fitting viewport
        // detail: the view will be restricted to the resolution (regardless of the size of the window, the image will be
        // rescaled to fit it proportionally)
        if (mViewport != null)
        {
            HandleViewportOnly();
            return;
        }

        // case: the dev want just simple transformation (such as transformMatrix)
        // detail: the view will be extensive (the more large is the window, more pixels will be visible)
        HandleViewportAndBasicEffectAbsent();
    }

    public Vector2 Unproject()
    {
        return Vector2.Zero;
    }

    public Vector2 ScreenToWorldPosition(float x, float y) => ScreenToWorldPosition(new Vector2(x, y));

    public Vector2 ScreenToWorldPosition(Vector2 screenPosition)
    {
        Matrix invView = Matrix.Invert(SpriteBatchView);
        
        if (mViewport is not null)
        {
            screenPosition.X -= mViewport.Viewport.X;
            screenPosition.Y -= mViewport.Viewport.Y;
        }

        return Vector2.Transform(screenPosition, invView);
    }

    private void HandleViewportAndBasicEffectAbsent()
    {
        PresentationParameters parameters = mGraphicsDevice.PresentationParameters;

        Matrix origin = Matrix.CreateTranslation(
            parameters.BackBufferWidth * 0.5f,
            parameters.BackBufferHeight * 0.5f,
            0f
        );
        Matrix translation = Matrix.CreateTranslation(-Position.X, -Position.Y, 0f);
        Matrix zoom = Matrix.CreateScale(Zoom);

        SpriteBatchView = translation * zoom * origin;
    }

    private void HandleBasicEffectOnly()
    {
        if (BasicEffect is null) return;

        PresentationParameters parameters = mGraphicsDevice.PresentationParameters;

        Projection = Matrix.CreateOrthographicOffCenter(0f, parameters.BackBufferWidth,
            parameters.BackBufferHeight, 0f, 0f, 1f);

        Matrix origin = Matrix.CreateTranslation(
            parameters.BackBufferWidth * 0.5f,
            parameters.BackBufferHeight * 0.5f,
            0f
        );
        Matrix translation = Matrix.CreateTranslation(-Position.X, -Position.Y, 0f);
        Matrix zoom = Matrix.CreateScale(Zoom);

        SpriteBatchView = mViewport is not null
            ? translation * zoom * mViewport.ScalingMatrix * origin
            : translation * zoom * origin;

        Matrix physicsViewTranslation = Matrix.CreateTranslation(-Position.X * mInvPpm, -Position.Y * mInvPpm,
            0f);
        Matrix physicsViewScale = Matrix.CreateScale(mPpm * Zoom);
        PhysicsDebugView = mViewport is not null
            ? physicsViewTranslation * physicsViewScale * mViewport.ScalingMatrix * origin
            : physicsViewTranslation * physicsViewScale * origin;

        BasicEffect.Projection = Projection;
        BasicEffect.View = SpriteBatchView;
    }

    private void HandleViewportOnly()
    {
        if (mViewport is null) return;

        PresentationParameters parameters = mGraphicsDevice.PresentationParameters;
        Viewport viewport = mGraphicsDevice.Viewport;
        Matrix origin = Matrix.CreateTranslation(
            parameters.BackBufferWidth * 0.5f - viewport.X,
            parameters.BackBufferHeight * 0.5f - viewport.Y,
            0f
        );
        Matrix translation = Matrix.CreateTranslation(-Position.X, -Position.Y, 0f);
        Matrix zoom = Matrix.CreateScale(Zoom);
        SpriteBatchView = translation * zoom * mViewport.ScalingMatrix * origin;
    }

    #endregion
}