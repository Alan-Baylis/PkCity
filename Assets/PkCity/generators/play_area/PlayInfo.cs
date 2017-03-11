using UnityEngine;
using System.Collections.Generic;

namespace Pk.Generators.PlayArea {
    public class PlayAreaInfo {
        int _width, _height;
        List<Vector3>   _escapes;
        List<InteriorInfo> _interiors;

        public PlayAreaInfo(int width, int height) {
            _width  = width;
            _height = height;
            _escapes   = new List<Vector3>();
            _interiors = new List<InteriorInfo>();
        }

        public void AddEscape(Vector2 pos, float height) {
            _escapes.Add(new Vector3(pos.x, pos.y, height));
        }

        public void AddInterior(InteriorInfo interior) {
            _interiors.Add(interior);
        }

        public int width  { get { return _width;  } }
        public int height { get { return _height; } }
        public Vector3[]      escapes   { get { return _escapes.ToArray();   } }
        public InteriorInfo[] interiors { get { return _interiors.ToArray(); } }
    }

    public class InteriorInfo {
        IntRect _bounds;
        //TODO
        public IntRect bounds { get { return _bounds; } }
    }
}