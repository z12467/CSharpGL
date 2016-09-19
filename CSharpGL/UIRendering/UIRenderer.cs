﻿using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace CSharpGL
{
    /// <summary>
    /// Renderer  that supports UI layout.
    /// 支持2D UI布局的渲染器
    /// </summary>
    public class UIRenderer : RendererBase, ILayout
    {
        private ViewportSwitch viewportSwitch;
        private ScissorTestSwitch scissorTestSwitch;
        private GLSwitchList switchList = new GLSwitchList();

        /// <summary>
        ///
        /// </summary>
        [Category(strRenderer)]
        [Description("OpenGL switches.")]
        public GLSwitchList SwitchList
        {
            get { return switchList; }
        }

        /// <summary>
        /// triggered before layout in <see cref="ILayout"/>.Layout().
        /// </summary>
        public event EventHandler BeforeLayout;

        /// <summary>
        /// triggered after layout in <see cref="ILayout"/>.Layout().
        /// </summary>
        public event EventHandler AfterLayout;

        internal void DoBeforeLayout()
        {
            EventHandler BeforeLayout = this.BeforeLayout;
            if (BeforeLayout != null)
            {
                BeforeLayout(this, null);
            }
        }

        internal void DoAfterLayout()
        {
            EventHandler AfterLayout = this.AfterLayout;
            if (AfterLayout != null)
            {
                AfterLayout(this, null);
            }
        }

        /// <summary>
        ///
        /// </summary>
        [Category(strRenderer)]
        [Description("Renderer that actrually renders something.")]
        public RendererBase Renderer { get; protected set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="anchor"></param>
        /// <param name="margin"></param>
        /// <param name="size"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        public UIRenderer(
            System.Windows.Forms.AnchorStyles anchor, System.Windows.Forms.Padding margin,
            System.Drawing.Size size, int zNear, int zFar)
        {
            this.Children = new ChildList<UIRenderer>(this);// new ILayoutList(this);

            this.Anchor = anchor; this.Margin = margin;
            this.Size = size; this.zNear = zNear; this.zFar = zFar;
        }

        /// <summary>
        ///
        /// </summary>
        protected const string strUIRenderer = "UIRenderer";

        /// <summary>
        ///
        /// </summary>
        [Category(strUIRenderer)]
        public System.Windows.Forms.AnchorStyles Anchor { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Category(strUIRenderer)]
        public System.Windows.Forms.Padding Margin { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Category(strUIRenderer)]
        public System.Drawing.Point Location { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Category(strUIRenderer)]
        public System.Drawing.Size Size { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Browsable(false)]
        [Category(strUIRenderer)]
        public System.Drawing.Size ParentLastSize { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Category(strUIRenderer)]
        public int zNear { get; set; }

        /// <summary>
        ///
        /// </summary>
        [Category(strUIRenderer)]
        public int zFar { get; set; }

        /// <summary>
        ///
        /// </summary>
        protected override void DoInitialize()
        {
            this.viewportSwitch = new ViewportSwitch();
            this.scissorTestSwitch = new ScissorTestSwitch();

            RendererBase renderer = this.Renderer;
            if (renderer != null)
            {
                renderer.Initialize();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="arg"></param>
        protected override void DoRender(RenderEventArgs arg)
        {
            this.viewportSwitch.X = this.Location.X;
            this.viewportSwitch.Y = this.Location.Y;
            this.viewportSwitch.Width = this.Size.Width;
            this.viewportSwitch.Height = this.Size.Height;
            this.scissorTestSwitch.X = this.Location.X;
            this.scissorTestSwitch.Y = this.Location.Y;
            this.scissorTestSwitch.Width = this.Size.Width;
            this.scissorTestSwitch.Height = this.Size.Height;

            this.viewportSwitch.On();
            this.scissorTestSwitch.On();
            int count = this.switchList.Count;
            for (int i = 0; i < count; i++) { this.switchList[i].On(); }

            // 把所有在此之前渲染的内容都推到最远。
            // Push all rendered stuff to farest position.
            OpenGL.Clear(OpenGL.GL_DEPTH_BUFFER_BIT);

            RendererBase renderer = this.Renderer;
            if (renderer != null)
            {
                renderer.Render(arg);
            }

            for (int i = count - 1; i >= 0; i--) { this.switchList[i].Off(); }
            this.scissorTestSwitch.Off();
            this.viewportSwitch.Off();
        }

        /// <summary>
        ///
        /// </summary>
        protected override void DisposeUnmanagedResources()
        {
            RendererBase renderer = this.Renderer;
            if (renderer != null)
            {
                renderer.Dispose();
            }
        }

        const string strTreeNode = "TreeNode";
        /// <summary>
        ///
        /// </summary>
        [Category(strTreeNode)]
        [Description("Self.")]
        public UIRenderer Self { get { return this; } }

        /// <summary>
        ///
        /// </summary>
        [Category(strTreeNode)]
        [Description("Parent UI Renderer.")]
        public UIRenderer Parent { get; set; }

        //ChildList<UIRenderer> children;

        /// <summary>
        ///
        /// </summary>
        [Category(strTreeNode)]
        [Editor(typeof(IListEditor<UIRenderer>), typeof(UITypeEditor))]
        [Description("Children UI Renderers.")]
        public ChildList<UIRenderer> Children { get; private set; }
    }
}