using System;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;

namespace LacmusApp.Avalonia.Models
{
    public class Zoomer
    {
        private static ZoomBorder _zoomBorder;

        public static void Init(ZoomBorder zoomBorder)
        {
            _zoomBorder = zoomBorder;
            _zoomBorder.PanButton = ButtonName.Left;
            _zoomBorder.EnablePan = true;
        }
        
        public static void ZoomIn()
        {
            _zoomBorder?.ZoomIn();
        }
        
        public static void ZoomOut()
        {
            _zoomBorder?.ZoomOut();
        }
        
        public static void MoveUp()
        {
            _zoomBorder?.PanDelta(0, 25, false);
        }
        public static void MoveDown()
        {
            _zoomBorder?.PanDelta(0, -25, false);
        }
        public static void MoveLeft()
        {
            _zoomBorder?.PanDelta(25, 0, false);
        }
        public static void MoveRight()
        {
            _zoomBorder?.PanDelta(-25, 0, false);
        }

        public static void Reset()
        {
            _zoomBorder?.Uniform();
        }
    }
}