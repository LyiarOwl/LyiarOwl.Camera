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
/// <para><see cref="https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SpriteBatch.html#parameters-2"/></para>
/// <para><see cref="https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.BasicEffect.html"/></para>
/// <para><see cref="https://github.com/nkast/Aether.Physics2D"/></para>
/// <para><see cref="https://github.com/Genbox/VelcroPhysics"/></para>
/// </summary>
public class OrthographicCamera2D
{
    #region Private Members

    private readonly BasicEffect? mBasicEffect;
    private readonly FitViewport? mViewport;
    private readonly GraphicsDevice mGraphicsDevice;
    private float mInvPpm;
    private float mPpm;

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
        mBasicEffect = basicEffect;
        mViewport = viewport;

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
        // detail: the view will be restricted to the resolution (regardless of the size of the window, the image will be
        // rescaled to fit it proportionally), but keeping usage support for the physics engine's debug view
        if (mBasicEffect != null && mViewport != null)
        {
            return;
        }

        // case: the dev want to integrate the spritebatch with the debug view of some physics engine
        // detail: the view will be extensive (the more large is the window, more pixels will be visible), but keeping
        // usage support for the physics engine's debug view
        if (mBasicEffect != null)
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
        if (mBasicEffect is null) return;
        
        
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

        SpriteBatchView = translation * zoom * origin;

        Matrix physicsViewTranslation = Matrix.CreateTranslation(-Position.X * mInvPpm, -Position.Y * mInvPpm, 
            0f);
        Matrix physicsViewScale = Matrix.CreateScale(mPpm * Zoom);
        PhysicsDebugView = physicsViewTranslation * physicsViewScale * origin;

        mBasicEffect.Projection = Projection;
        mBasicEffect.View = SpriteBatchView;
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