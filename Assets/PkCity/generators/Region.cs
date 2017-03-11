using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Pk.Generators {
    public class Region : Generator {
        public Functions.Polygon bounds;
        public int[] triangulated;

        public override void Initialize() {
            bounds.Grow(-10);

            GenerateFoundation();
            GeneratePlots();

            base.Initialize();
        }

        void GenerateFoundation() {
            CreateProp("Foundation", Vector3.zero, Quaternion.Euler(0, 0, 0), Functions.Extruder.ExtrudeCCW(bounds, 2.5f), new Materials.FoundationMaterialGenerator());
        }

        void GeneratePlots() {
            var buildings = CreateProp("Buildings", Vector3.zero, Quaternion.Euler(0, 0, 0));

            var rect = bounds.FastBoundingRect();
            var plots = new Functions.RectTree(rect.width, rect.height, PlayArea.PlayCell.unit * Building.minUnits, PlayArea.PlayCell.unit * Building.minUnits, PlayArea.PlayCell.unit * Building.maxUnits, PlayArea.PlayCell.unit * Building.maxUnits, seed).rects;
            
            foreach (var plot in plots) {
                // By corners
                /*
                bool inside = true;
                foreach (var point in Util.RectVerts(plot))
                    if (!bounds.Inside(point)) {
                        inside = false;
                        break;
                    }
                if (!inside)
                    continue;
                */

                // By center
                if (!bounds.Inside(plot.center))
                    continue;

                var gen = Generate<Plot>(Util.V23(plot.center)/*Util.V23(Functions.Point.Rotate(plot.center, -bounds.AngleOfLongestSide(), Vector2.zero))*/);
                var transPlot = plot;
                transPlot.center = Vector2.zero;
                gen.bounds = transPlot;
                gen.transform.parent = buildings.transform;
                gen.Initialize();
            }
            buildings.transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * -bounds.AngleOfLongestSide(), 0);
        }
    }
}
