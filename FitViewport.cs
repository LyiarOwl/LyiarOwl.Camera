using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LyiarOwl.Camera;

/// <summary>
/// <para>Always when a frame is rendered this viewport ensures that the frame be proportionally
/// scaled to fit the width or height of the window (adding letter or pillarboxing depending on
/// the current size of the window).</para>
/// <para>This class don't need a camera to work, that is, you can use it independently and project
/// your own camera.</para>
/// <para>Always when the image is rescaled, the change is stored in the <c>ScalingMatrix</c>
/// that you can use with your camera.</para>
/// </summary>
public class FitViewport
{
    private readonly Vector2 mInternalResolution;
    private readonly Vector2 mInvInternalResolution;
    private readonly GraphicsDevice mGraphicsDevice;
    private bool mIsResizing;
    
    public Matrix ScalingMatrix;

    public FitViewport(GameWindow window, GraphicsDevice graphicsDevice, int width, int height)
    {
        mGraphicsDevice = graphicsDevice;

        mInternalResolution = new Vector2(width, height);
        mInvInternalResolution = Vector2.One / mInternalResolution;

        Update();

        window.ClientSizeChanged += (_, _) =>
        {
            Rectangle clientBounds = window.ClientBounds;
            if (clientBounds.Width <= 0 || clientBounds.Height <= 0)
                return;

            if (!mIsResizing)
            {
                mIsResizing = true;
                Update();
                mIsResizing = false;
            }
        };
    }

    /// <summary>
    /// If you had associated this viewport to some camera, don't call this method
    /// because the camera itself will call it internally!
    /// </summary>
    private void Update()
    {
        PresentationParameters parameters = mGraphicsDevice.PresentationParameters;
        float clientWidth = parameters.BackBufferWidth;
        float clientHeight = parameters.BackBufferHeight;

        float adjustedViewportWidth;
        float adjustedViewportHeight;

        float horizontalAspect = clientWidth * mInvInternalResolution.X;
        float verticalAspect = clientHeight * mInvInternalResolution.Y;

        if (horizontalAspect > verticalAspect)
        {
            adjustedViewportWidth = verticalAspect * mInternalResolution.X;
            adjustedViewportHeight = clientHeight;
        }
        else
        {
            adjustedViewportWidth = clientWidth;
            adjustedViewportHeight = horizontalAspect * mInternalResolution.Y;
        }

        float scale = MathF.Min(horizontalAspect, verticalAspect);

        ScalingMatrix = Matrix.CreateScale(scale);

        Viewport viewport = mGraphicsDevice.Viewport;
        viewport.X = (int)(clientWidth * 0.5f - adjustedViewportWidth * 0.5f);
        viewport.Y = (int)(clientHeight * 0.5f - adjustedViewportHeight * 0.5f);
        viewport.Width = (int)adjustedViewportWidth;
        viewport.Height = (int)adjustedViewportHeight;
        viewport.MinDepth = 0f;
        viewport.MaxDepth = 1f;
        mGraphicsDevice.Viewport = viewport;
    }
    
}