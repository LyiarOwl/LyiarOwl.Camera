#nullable enable
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LyiarOwl.Camera;

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
    /// <returns>A new instance of a <c>BasicEffect</c></returns>
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
    private float mMinimalPpm = 1f;
    private float mZoom = 1f;
    private float mMinimalZoom = 0.1f;
    #endregion


    #region Public Members
    /// <summary>
    /// <para>Is the top-left corner of the visible area in the screen.</para>
    /// <para>Shorthand for: <code>new Vector2(Left, Top)</code></para>
    /// </summary>
    public Vector2 Min
    {
        get
        {
            Viewport viewport = mGraphicsDevice.Viewport;
            Vector2 coords = new Vector2(viewport.X, viewport.Y);
            return ScreenToWorld(coords);
        }
    }
    /// <summary>
    /// <para>Is the bottom-right corner of the visible area in the screen.</para>
    /// <para>Shorthand for: <code>new Vector2(Right, Bottom)</code></para>
    /// </summary>
    public Vector2 Max
    {
        get
        {
            Viewport viewport = mGraphicsDevice.Viewport;
            PresentationParameters presentation = mGraphicsDevice.PresentationParameters;
            Vector2 coords = new Vector2(
                presentation.BackBufferWidth - viewport.X,
                presentation.BackBufferHeight - viewport.Y
            );

            return ScreenToWorld(coords);
        }
    }
    /// <summary>
    /// <para>Is the left side of the visible area in the screen.</para>
    /// <para>Shorthand for: <code>Min.X</code></para>
    /// </summary>
    public float Left => Min.X;
    /// <summary>
    /// <para>Is the right side of the visible area in the screen.</para>
    /// <para>Shorthand for: <code>Max.X</code></para>
    /// </summary>
    public float Right => Max.X;
    /// <summary>
    /// <para>Is the top side of the visible area in the screen.</para>
    /// <para>Shorthand for: <code>Min.Y</code></para>
    /// </summary>
    public float Top => Min.Y;
    /// <summary>
    /// <para>Is the bottom side of the visible area in the screen.</para>
    /// <para>Shorthand for: <code>Max.Y</code></para>
    /// </summary>
    public float Bottom => Max.Y;
    /// <summary>
    /// <para>Is the width of the visible area in the screen.</para>
    /// <para>Shorthand for: <code>Abs(Left - Right)</code></para>
    /// </summary>
    public float Width => MathF.Abs(Left - Right);
    /// <summary>
    /// <para>Is the height of the visible area in the screen.</para>
    /// <para>Shorthand for: <code>Abs(Top - Bottom)</code></para>
    /// </summary>
    public float Height => MathF.Abs(Top - Bottom);

    public BasicEffect? BasicEffect;

    /// <summary>
    /// <para>This is a <c>OrthographicOffCenter</c> projection.</para>
    /// <para>If you are using some effect (like the <c>BasicEffect</c>), this projection
    /// must be used.</para>
    /// </summary>
    public Matrix Projection;

    /// <summary>
    /// This View must be applied to the <c>transformMatrix</c> from the <c>SpriteBatch</c>
    /// or to the <c>View</c> attribute from your effect (like the <c>BasicEffect</c>).
    /// </summary>
    public Matrix SpriteBatchView;
    /// <summary>
    /// <para>This View must be applied to the <c>transformMatrix</c> from the <c>SpriteBatch</c>
    /// or to the <c>View</c> attribute from your effect (like the <c>BasicEffect</c>).</para>
    /// <para>Shorthand for: <b>getter for SpriteBatchView</b></para>
    /// </summary>
    public Matrix TransformMatrix => SpriteBatchView;

    /// <summary>
    /// This view is only useful if you are using the debug view from some physics engine
    /// like Aether Physics or Velcro's Physics. 
    /// </summary>
    public Matrix PhysicsDebugView;

    /// <summary>
    /// <para>Defines the zoom of this camera.</para>
    /// <para>Defaults: <b>0.1 (minimal), 1 (default)</b></para>
    /// </summary>
    public float Zoom
    {
        get => mZoom;
        set
        {
            if (value < mMinimalZoom)
            {
                mZoom = mMinimalZoom;
                return;
            }
            mZoom = value;
        }
    }
    public float MinimalZoom => mMinimalZoom;
    /// <summary>
    /// Defines the position of this camera.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// <para>
    /// Define the scale in which the <c>PhysicsDebugView</c> will be scaled. That is, how much pixels will be
    /// equivalent to 1 meters.
    /// </para>
    /// <para>Defaults: <b>1 (minimal), 16 (default)</b></para>
    /// </summary>
    public float Ppm
    {
        get => mPpm;
        set
        {
            if (value < mMinimalPpm)
            {
                mPpm = mMinimalPpm;
                mInvPpm = 1f / mPpm;
                return;
            }

            mPpm = value;
            mInvPpm = 1f / mPpm;
        }
    }
    public float MinimalPpm => mMinimalPpm;
    public FitViewport? Viewport => mViewport;
    #endregion


    #region Constructors

    /// <summary>
    /// <para>
    /// Initializes an orthographic camera with an extensive view, that is, when the window is
    /// resized, more or less pixels will be visible.</para>
    /// <para>
    /// Can be used in the simplest way, that is, you can pass the <c>TransformMatrix</c>
    /// (or <c>SpriteBatchView</c>, both are the same) directly to the <c>transformMatrix</c> parameter of your
    /// <c>SpriteBatch</c>.
    /// </para>
    /// <para>Make sure to call <c>Update</c>.</para>
    /// <para>Shorthand for: <code>OrthographicCamera(graphicsDevice, null, null)</code></para>
    /// </summary>
    public OrthographicCamera2D(GraphicsDevice graphicsDevice)
    {
        mGraphicsDevice = graphicsDevice;
	Update();
    }
    /// <summary>
    /// <para>
    /// Initializes an orthographic camera using a fit viewport, that is, when the window is
    /// resized, the same amount of pixels will remain visible.</para>
    /// <para>
    /// Can be used in the simplest way, that is, you can pass the <c>TransformMatrix</c>
    /// (or <c>SpriteBatchView</c>, both are the same) directly to the <c>transformMatrix</c> parameter of your
    /// <c>SpriteBatch</c>.
    /// </para>
    /// <para>
    /// Make sure to call <c>Update</c> and also set the <c>Viewport</c> of your <c>GraphicsDevice</c> to use
    /// the <c>Viewport</c> being defined in this camera.
    /// </para>
    /// <para>Shorthand for: <code>OrthographicCamera(graphicsDevice, null, viewport)</code></para>
    /// </summary>
    public OrthographicCamera2D(GraphicsDevice graphicsDevice, FitViewport viewport)
    : this(graphicsDevice, null, viewport) 
    {
  	Update(); 
    }

    /// <summary>
    /// <para>
    /// Initializes an orthographic camera with an extensive view, that is, when the window is
    /// resized, the same amount of pixels will remain visible.
    /// </para>
    /// 
    /// <para>
    /// By using a <c>BasicEffect</c>, a <c>Projection</c> is processed internally and can be used
    /// with anything that needs a projection matrix. Essentially this projection is provided to give support
    /// to the <b>DebugView</b> from <b>Aether.Physics</b>, <b>Velcro's Physics</b> (and/or any physics library
    /// that derives from this same debug view).
    /// </para>
    /// 
    /// <para>
    /// In addition to the <c>TransformMatrix</c> (or <c>SpriteBatchView</c>, both are the same) a 
    /// <c>PhysicsDebugView</c> is also provided to be used with the draw method of the physics engine debug view.
    /// </para>
    /// 
    /// <para>
    /// Make sure to call <c>Update</c>, pass the <c>BasicEffect</c> defined in this camera to the <c>effect</c>
    /// parameter of your <c>SpriteBatch</c> and also change the <c>Ppm</c> property of this camera according with
    /// the scale unit (in pixels) you are working.
    /// </para>
    /// 
    /// <para>Shorthand for: <code>OrthographicCamera(graphicsDevice, basicEffect, null)</code></para>
    /// 
    /// <para><seealso href="https://github.com/nkast/Aether.Physics2D"/></para>
    /// <para><seealso href="https://github.com/Genbox/VelcroPhysics"/></para>
    /// </summary>
    public OrthographicCamera2D(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
    : this(graphicsDevice, basicEffect, null) 
    {
    	Update();
    }

    /// <summary>
    /// <para>
    /// Initializes an orthographic camera with an extensive view, that is, when the window is
    /// resized, the same amount of pixels will remain visible.
    /// </para>
    /// 
    /// <para>
    /// By using a <c>BasicEffect</c>, a <c>Projection</c> is processed internally and can be used
    /// with anything that needs a projection matrix. Essentially this projection is provided to give support
    /// to the <b>DebugView</b> from <b>Aether.Physics</b>, <b>Velcro's Physics</b> (and/or any physics library
    /// that derives from this same debug view).
    /// </para>
    /// 
    /// <para>
    /// In addition to the <c>TransformMatrix</c> (or <c>SpriteBatchView</c>, both are the same) a 
    /// <c>PhysicsDebugView</c> is also provided to be used with the draw method of the physics engine debug view.
    /// </para>
    /// 
    /// <para>
    /// Make sure to call <c>Update</c>, pass the <c>BasicEffect</c> defined in this camera to the <c>effect</c>
    /// parameter of your <c>SpriteBatch</c>, set the <c>Viewport</c> of your <c>GraphicsDevice</c> to use the 
    /// <c>Viewport</c> being defined in this camera and also change the <c>Ppm</c> property of this camera 
    /// according with the scale unit (in pixels) you are working.
    /// </para>
    /// 
    /// <para><seealso href="https://github.com/nkast/Aether.Physics2D"/></para>
    /// <para><seealso href="https://github.com/Genbox/VelcroPhysics"/></para>
    /// </summary>
    public OrthographicCamera2D(GraphicsDevice graphicsDevice, BasicEffect? basicEffect, FitViewport? viewport)
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
        if (BasicEffect != null)
        {
            ProcessBasicEffectOnly();
            return;
        }

        if (mViewport != null)
        {
            ProcessViewportOnly();
            return;
        }

        ProcessViewportAndBasicEffectAbsence();
    }


    #region Util methods

    /// <summary>
    /// <para>Converts from <b>Screen Coordinates</b> to <b>Normalized Device Coordinates</b>.</para>
    /// <para><c>BasicEffect</c> must be defined for this method to work.</para>
    /// <para>Shorthand for: <code>ScreenToNDC(screenPosition.X, screenPosition.Y)</code></para>
    /// </summary>
    public Vector2 ScreenToNDC(Vector2 screenPosition) => ScreenToNDC(screenPosition.X, screenPosition.Y);
    /// <summary>
    /// <para>Converts from <b>Screen Coordinates</b> to <b>Normalized Device Coordinates</b>.</para>
    /// <para><c>BasicEffect</c> must be defined for this method to work.</para>
    /// </summary>
    /// <param name="screenX">X coordinates in the screen space.</param>
    /// <param name="screenY">Y coordinates in the screen space.</param>
    public Vector2 ScreenToNDC(float screenX, float screenY)
    {
        if (BasicEffect is null) return Vector2.Zero;
        Viewport viewport = mGraphicsDevice.Viewport;
        float x = 2f * (screenX - viewport.X) / viewport.Width - 1f;
        float y = 1f - 2f * (screenY - viewport.Y) / viewport.Height;
        return new Vector2(x, y);
    }
    /// <summary>
    /// <para>Converts from <b>Clip Coordinates</b> to <b>View Coordinates</b>.</para>
    /// <para>This method uses <c>ScreenToNDC</c> internally.</para>
    /// <para><c>BasicEffect</c> must be defined for this method to work.</para>
    /// <para>Shorthand for: <code>ClipToView(ndc.X, ndc.Y)</code></para>
    /// </summary>
    public Vector4 ClipToView(Vector2 ndc, float z = 0f, float w = 1f) => ClipToView(ndc.X, ndc.Y, z, w);
    /// <summary>
    /// <para>Converts from <b>Clip Coordinates</b> to <b>View Coordinates</b>.</para>
    /// <para>This method uses <c>ScreenToNDC</c> internally.</para>
    /// <para><c>BasicEffect</c> must be defined for this method to work.</para>
    /// </summary>
    public Vector4 ClipToView(float ndcX, float ndcY, float z = 0f, float w = 1f)
    {
        if (BasicEffect is null) return Vector4.Zero;
        Vector4 clip = new Vector4(ndcX, ndcY, z, w);

        Matrix invProj = Matrix.Invert(Projection);
        Vector4 view = Vector4.Transform(clip, invProj);
        view /= view.W;
        return view;
    }
    /// <summary>
    /// <para>Converts from <b>Screen Coordinates</b> to <b>World Coordinates</b>.</para>
    /// <para>The result will be different if you'd set a <c>BasicEffect</c> for this camera.</para>
    /// <para>This method uses <c>ScreenToNDC</c> and <c>ClipToView</c> internally.</para>
    /// <para>Shorthand for: <code>ScreenToWorld(new Vector2(x, y))</code></para>
    /// </summary>
    public Vector2 ScreenToWorld(float x, float y) => ScreenToWorld(new Vector2(x, y));

    /// <summary>
    /// <para>Converts from <b>Screen Coordinates</b> to <b>World Coordinates</b>.</para>
    /// <para>The result will be different if you'd set a <c>BasicEffect</c> for this camera.</para>
    /// <para>This method uses <c>ScreenToNDC</c> and <c>ClipToView</c> internally.</para>
    /// </summary>
    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        Matrix invView = Matrix.Invert(SpriteBatchView);
        if (BasicEffect is not null)
        {
            Vector2 ndc = ScreenToNDC(screenPosition.X, screenPosition.Y);
            Vector4 view = ClipToView(ndc);
            Vector4 world = Vector4.Transform(view, invView);

            return new Vector2(world.X, world.Y);
        }

        if (mViewport is not null)
        {
            screenPosition.X -= mViewport.Viewport.X;
            screenPosition.Y -= mViewport.Viewport.Y;
        }

        return Vector2.Transform(screenPosition, invView);
    }
    #endregion


    #region Processing Projection and Views
    private void ProcessViewportAndBasicEffectAbsence()
    {
        PresentationParameters parameters = mGraphicsDevice.PresentationParameters;

        Matrix origin = GetOrigin(parameters.BackBufferWidth, parameters.BackBufferHeight);
        Matrix translation = GetTranslation(Position.X, Position.Y);
        Matrix zoom = GetScale(mZoom);

        SpriteBatchView = translation * zoom * origin;
    }

    private void ProcessBasicEffectOnly()
    {
        if (BasicEffect is null) return;

        Matrix origin;

        if (mViewport is null)
        {
            PresentationParameters parameters = mGraphicsDevice.PresentationParameters;
            Projection = Matrix.CreateOrthographicOffCenter(0f, parameters.BackBufferWidth,
                parameters.BackBufferHeight, 0f, 0f, 1f);
            origin = GetOrigin(parameters.BackBufferWidth, parameters.BackBufferHeight);
        }
        else
        {
            Viewport viewport = mViewport.Viewport;
            Projection = Matrix.CreateOrthographicOffCenter(
                0f, viewport.Width, viewport.Height, 0f, 0f, 1f
            );
            origin = GetOrigin(viewport.Width, viewport.Height);
        }

        Matrix translation = GetTranslation(Position.X, Position.Y);
        Matrix zoom = GetScale(mZoom);

        SpriteBatchView = mViewport is not null
            ? translation * zoom * mViewport.ScalingMatrix * origin
            : translation * zoom * origin;

        Matrix physicsViewTranslation = Matrix.CreateTranslation(-Position.X * mInvPpm, -Position.Y * mInvPpm,
            0f);
        Matrix physicsViewScale = GetScale(mZoom, mPpm);
        PhysicsDebugView = mViewport is not null
            ? physicsViewTranslation * physicsViewScale * mViewport.ScalingMatrix * origin
            : physicsViewTranslation * physicsViewScale * origin;

        BasicEffect.Projection = Projection;
        BasicEffect.View = SpriteBatchView;
    }
    private void ProcessViewportOnly()
    {
        if (mViewport is null) return;

        PresentationParameters parameters = mGraphicsDevice.PresentationParameters;
        Viewport viewport = mGraphicsDevice.Viewport;
        Matrix origin = GetOrigin(parameters.BackBufferWidth, parameters.BackBufferHeight, viewport.X, viewport.Y);
        Matrix translation = GetTranslation(Position.X, Position.Y);
        Matrix zoom = GetScale(mZoom);
        SpriteBatchView = translation * zoom * mViewport.ScalingMatrix * origin;
    }
    private Matrix GetScale(float scale, float ppm = 1f)
    {
        return Matrix.CreateScale(scale * ppm);
    }
    private Matrix GetOrigin(float width, float height, float offsetX = 0f, float offsetY = 0)
    {
        return Matrix.CreateTranslation(width * 0.5f - offsetX, height * 0.5f - offsetY, 0f);
    }
    private Matrix GetTranslation(float x, float y)
    {
        return Matrix.CreateTranslation(-x, -y, 0f);
    }
    #endregion
    #endregion
}
