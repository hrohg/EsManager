﻿using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;


namespace Xceed.Wpf.AvalonDock.ExtendedAvalonDock.Layouts
{
    [ContentProperty("Children")]
    [Serializable]
    public class LayoutAnchorablePaneGroup : LayoutPositionableGroup<ILayoutAnchorablePane>, ILayoutAnchorablePane, ILayoutOrientableGroup
    {
        public LayoutAnchorablePaneGroup()
        {
        }
        
        public LayoutAnchorablePaneGroup(LayoutAnchorablePane firstChild)
        {
            Children.Add(firstChild);
        }

        #region Orientation

        private Orientation _orientation;
        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (_orientation != value)
                {
                    RaisePropertyChanging("Orientation");
                    _orientation = value;
                    RaisePropertyChanged("Orientation");
                }
            }
        }

        #endregion

        protected override bool GetVisibility()
        {
            return Children.Count > 0 && Children.Any(c => c.IsVisible);
        }

        protected override void OnIsVisibleChanged()
        {
            UpdateParentVisibility();
            base.OnIsVisibleChanged();
        }

        void UpdateParentVisibility()
        {
            var parentPane = Parent as ILayoutElementWithVisibility;
            if (parentPane != null)
                parentPane.ComputeVisibility();
        }

        protected override void OnDockWidthChanged()
        {
            if (DockWidth.IsAbsolute && ChildrenCount == 1)
                ((ILayoutPositionableElement)Children[0]).DockWidth = DockWidth;

            base.OnDockWidthChanged();
        }

        protected override void OnDockHeightChanged()
        {
            if (DockHeight.IsAbsolute && ChildrenCount == 1)
                ((ILayoutPositionableElement)Children[0]).DockHeight = DockHeight;
            base.OnDockHeightChanged();
        }

        protected override void OnChildrenCollectionChanged()
        {
            if (DockWidth.IsAbsolute && ChildrenCount == 1)
                ((ILayoutPositionableElement)Children[0]).DockWidth = DockWidth;
            if (DockHeight.IsAbsolute && ChildrenCount == 1)
                ((ILayoutPositionableElement)Children[0]).DockHeight = DockHeight;
            base.OnChildrenCollectionChanged();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Orientation", Orientation.ToString());
            base.WriteXml(writer);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("Orientation"))
                Orientation = (Orientation)Enum.Parse(typeof(Orientation), reader.Value, true);
            base.ReadXml(reader);
        }

#if DEBUG
        public override void ConsoleDump(int tab)
        {
            System.Diagnostics.Debug.Write(new string(' ', tab * 4));
            System.Diagnostics.Debug.WriteLine("AnchorablePaneGroup({0})", Orientation);

            foreach (LayoutElement child in Children)
                child.ConsoleDump(tab + 1);
        }
#endif
    
    
    }

}
