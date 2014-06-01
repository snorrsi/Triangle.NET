﻿
namespace TriangleNet.Rendering
{
    using System.Windows.Forms;
    using TriangleNet.Geometry;
    using TriangleNet.Meshing;
    using TriangleNet.Rendering.GDI;
    using TriangleNet.Tools;
    using System.Collections.Generic;
    using TriangleNet.Rendering.Util;

    public class RenderManager
    {
        IRenderControl control;
        IRenderContext context;
        IRenderer renderer;
        Projection zoom;

        public IRenderControl Control
        {
            get { return control; }
        }

        public IRenderContext Context
        {
            get { return context; }
        }

        public RenderManager()
        {
        }

        public RenderManager(IRenderControl control)
        {
            Initialize(control);
        }

        public RenderManager(IRenderControl control, IRenderer renderer)
        {
            Initialize(control, renderer);
        }

        public void Initialize(IRenderControl control)
        {
            Initialize(control, new LayerRenderer());
        }

        public void Initialize(IRenderControl control, IRenderer renderer)
        {
            this.zoom = new Projection(control.ClientRectangle);

            this.context = new RenderContext(zoom, ColorManager.Default());

            this.renderer = renderer;
            this.renderer.Context = context;

            this.control = control;
            this.control.Initialize();
            this.control.Renderer = renderer;
        }

        public bool TryCreateControl(string assemblyName, IEnumerable<string> dependencies,
            out IRenderControl control)
        {
            if (!ReflectionHelper.TryCreateControl(assemblyName, dependencies, out control))
            {
                return false;
            }

            return control is Control;
        }

        public void Resize()
        {
            control.HandleResize();
        }

        public void Click(float x, float y, MouseButtons button)
        {
            control.HandleMouseClick(x, y, button);
        }

        public void Zoom(float x, float y, int delta)
        {
            control.HandleMouseWheel(x, y, delta);
        }

        public void Enable(int layer, bool enabled)
        {
            context.Enable(layer, enabled);

            control.Refresh();
        }

        public void Set(IPolygon data, bool refresh = true)
        {
            context.Add(data);

            if (refresh)
            {
                control.Refresh();
            }
        }

        public void Set(IMesh data, bool reset, bool refresh = true)
        {
            context.Add(data, reset);

            if (refresh)
            {
                control.Refresh();
            }
        }

        // Voronoi
        public void Set(IVoronoi voronoi, bool reset, bool refresh = true)
        {
            context.Add(voronoi, reset);

            if (refresh)
            {
                control.Refresh();
            }
        }

        public void Update(float[] values)
        {
            context.Add(values);
            control.Refresh();
        }

        public void Update(int[] partition)
        {
            context.Add(partition);
            control.Refresh();
        }
    }
}