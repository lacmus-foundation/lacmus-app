using System;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;

namespace LacmusApp.Models
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

        public static void Zoom(double scale)
        {
            _zoomBorder?.ZoomTo(scale, 0, 0, false);
        }

        public static void MoveTo(double x, double y)
        {
            /* Thanks to @wieslawsoltes https://github.com/wieslawsoltes/PanAndZoom/issues/27 */
            _zoomBorder?.Pan(0,0, false);
            _zoomBorder?.PanDelta(x,y, false);
        }

        public static double GetZoomX()
        {
            return _zoomBorder?.ZoomX ?? 1;
        }
        public static double GetZoomY()
        {
            return _zoomBorder?.ZoomY ?? 1;
        }

        public static void Reset()
        {
            _zoomBorder?.Fill();
        }
        
        public static event EventHandler<KeyEventArgs> KeyDown
        {
            add => _zoomBorder.KeyDown+=value;
            remove => _zoomBorder.KeyDown+=value; 
        }
    }
}