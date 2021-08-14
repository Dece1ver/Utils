using Microsoft.Xaml.Behaviors;
using MVVM.Template.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace MVVM.Template.Infrastructure.Behaviors
{
    class ResizeWindowPanel : Behavior<Panel>
    {
        public enum SizingAction
        {
            West = 1,
            East = 2,
            North = 3,
            NorthWest = 4,
            NorthEast = 5,
            South = 6,
            SouthWest = 7,
            SouthEast = 8,
        }

        protected override void OnAttached() => AssociatedObject.MouseLeftButtonDown += OnButtonDown;

        protected override void OnDetaching() => AssociatedObject.MouseLeftButtonDown -= OnButtonDown;

        private void OnButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(AssociatedObject.FindVisualRoot() is Window window)) return;

            switch (e.OriginalSource)
            {
                case Line line:
                    ResizeLine(line, window);
                    break;
                case Rectangle rect:
                    ResizeRect(rect, window);
                    break;
                default: return;
            }
        }

        private static void ResizeLine(Line line, Window window)
        {
            switch (line.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.North), IntPtr.Zero);
                    break;
                case VerticalAlignment.Bottom:
                    window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.South), IntPtr.Zero);
                    break;
                default:
                    switch (line.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.West), IntPtr.Zero);
                            break;
                        case HorizontalAlignment.Right:
                            window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.East), IntPtr.Zero);
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }

        private static void ResizeRect(Rectangle rect, Window window)
        {
            switch (rect.VerticalAlignment)
            {
                case VerticalAlignment.Top when rect.HorizontalAlignment == HorizontalAlignment.Left:
                    window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.NorthWest), IntPtr.Zero);
                    break;
                case VerticalAlignment.Top when rect.HorizontalAlignment == HorizontalAlignment.Right:
                    window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.NorthEast), IntPtr.Zero);
                    break;
                case VerticalAlignment.Bottom when rect.HorizontalAlignment == HorizontalAlignment.Left:
                    window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.SouthWest), IntPtr.Zero);
                    break;
                case VerticalAlignment.Bottom when rect.HorizontalAlignment == HorizontalAlignment.Right:
                    window.SendMessage(WM.SYSCOMMAND, (IntPtr)((int)SC.SIZE + SizingAction.SouthEast), IntPtr.Zero);
                    break;
                default:
                    break;
            }
        }
    }
}
