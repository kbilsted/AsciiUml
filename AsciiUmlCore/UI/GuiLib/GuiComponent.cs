using System;
using System.Collections.Generic;
using System.Linq;
using AsciiUml.Geo;
using AsciiUml.UI;

namespace AsciiUml
{
    public abstract class GuiComponent
    {
        public bool IsFocusable = true;

        /// <summary> if true the component may be rendered on the screen </summary>
        public bool IsVisible { get; private set; } = true;

        public void SetVisibility(bool isVisible)
        {
            IsVisible = isVisible;
            foreach (var child in Children)
            {
                child.SetVisibility(isVisible);
            }
        }

        public readonly GuiComponent Parent;
        public readonly List<GuiComponent> Children = new List<GuiComponent>();

        public bool IsDisposing = false;

        public virtual void RemoveChild(GuiComponent child)
        {
            Children.Remove(child);
        }

        public ConsoleColor BackGround = ConsoleColor.DarkBlue;
        public ConsoleColor Foreground = ConsoleColor.White;

        protected void TemporarilyForceRepaint()
        {
            Manager.TemporarilyForceRepaint();
        }
        /// <summary>
        /// How much your canvas will be shifted upon merging with the other canvases
        /// </summary>
        public Coord Position;

        public GuiDimensions Dimensions;

        protected readonly WindowManager Manager;

        public Action OnClosing = () => { };

        protected GuiComponent(WindowManager manager)
        {
            Manager = manager;
            Manager.AddComponent(this);
            Position = new Coord(0, 0);
            Dimensions = new GuiDimensions(new Size(), new Size());
        }

        protected GuiComponent(GuiComponent parent)
        {
            this.Parent = parent;
            Manager = parent.Manager;

            Position = parent.GetInnerCanvasTopLeft();
            Dimensions = new GuiDimensions(new Size(), new Size());

            parent.RegisterChildComponent(this);
        }

        protected GuiComponent(GuiComponent parent, Coord position) : this(parent)
        {
            Position = parent.GetInnerCanvasTopLeft() + parent.Position + position;
        }

        public virtual void Focus()
        {
            if(IsFocusable)
                Manager.Focus = this;
        }
            
        public void FocusNextChild(GuiComponent currentComponent)
        {
            int index = Children.FindIndex(x => x == currentComponent);
            for (int i = 1; i < Children.Count; i++)
            {
                var child = Children[(i + index) % Children.Count];
                if (child.IsVisible && child.IsFocusable)
                {
                    child.Focus();
                    return;
                }
            }
        }

        public void FocusPrevChild(GuiComponent currentComponent)
        {
            int index = Children.FindIndex(x => x == currentComponent);
            for (int i = 1; i < Children.Count; i++)
            {
                var child = Children[(Math.Abs(index-i)) % Children.Count];
                if (child.IsVisible && child.IsFocusable)
                {
                    child.Focus();
                    return;
                }
            }
        }

        public abstract bool HandleKey(ConsoleKeyInfo key);
        public abstract Canvass Paint();

        public GuiComponent RegisterChildComponent(GuiComponent child)
        {
            Children.Add(child);
            return child;
        }

        /// <summary>
        /// your parents width and height + your own. for use when you need to embed children inside you
        /// </summary>
        public abstract Coord GetInnerCanvasTopLeft();

        /// <summary>
        /// hook for doing actions upon crash
        /// </summary>
        public virtual void OnException(Exception e)
        {
        }

        public virtual void RemoveMeAndChildren()
        {
            if(IsDisposing)
                return;
            IsDisposing = true;
            foreach (var child in Children.ToArray())
                RemoveChild(child);
            Manager.Remove(this);
            Parent.Focus();
            Parent.RemoveChild(this);
        }

        public bool IsFocused => Manager.Focus == this;

        /// <summary>
        /// Get the size such that dynamic sized components can resize according to their children
        /// </summary>
        public virtual GuiDimensions GetSize()
        {
            if (Dimensions.IsFullyFixed())
            {
                return Dimensions;
            }
            // TODO semi fixed need semi calculations
            Coord upperLeft = Position;
            GuiDimensions dimension = new GuiDimensions(new Size(0),new Size(0));
            foreach (var child in Children)
            {
                if (child.Position.X < upperLeft.X)
                {
                    var dist = upperLeft.X - child.Position.X;
                    dimension.Width.Pixels += dist;
                    upperLeft = new Coord(child.Position.X, upperLeft.Y);
                }
                if (child.Position.X > upperLeft.X)
                {
                    var dist = child.Position.X - upperLeft.X;
                    dimension.Width.Pixels += dist;
                    upperLeft = new Coord(child.Position.X, upperLeft.Y);
                }
                var childSize = child.GetSize();
                dimension.Width.Pixels = Math.Max(dimension.Width.Pixels, childSize.Width.Pixels);

                if (child.Position.Y < upperLeft.Y)
                {
                    var dist = upperLeft.Y - child.Position.Y;
                    dimension.Height.Pixels += dist;
                    upperLeft = new Coord(upperLeft.X, child.Position.Y);
                }
                if (child.Position.Y > upperLeft.Y)
                {
                    var dist = child.Position.Y - upperLeft.Y;
                    dimension.Height.Pixels += dist;
                    upperLeft = new Coord(upperLeft.X, child.Position.Y);
                }
                dimension.Height.Pixels = Math.Max(dimension.Height.Pixels, childSize.Height.Pixels);
            }
            return dimension;
        }
    }
}
