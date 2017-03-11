using UnityEngine;
using System.Collections;

namespace Pk.Generators {
    public class Plot : Generator {
        public Rect bounds;

        public override void Initialize() {
            if (!GenerateBuilding())
                GeneratePark();

            base.Initialize();
        }

        bool GenerateBuilding() {
            var size = new Rect(0, 0, ((int) bounds.width) / PlayArea.PlayCell.unit, ((int) bounds.height) / PlayArea.PlayCell.unit);

            if (size.width  >= Building.minUnits &&
                size.width  <= Building.maxUnits &&
                size.height >= Building.minUnits &&
                size.height <= Building.maxUnits) {

                size.center = bounds.center;

                var gen = Generate<Building>(Vector2.zero);
                gen.size = size;
                gen.Initialize();
                return true;
            }
            return false;
        }

        void GeneratePark() {
            var park = CreateProp("Park", Vector2.zero, Quaternion.Euler(0, 0, 0), Functions.Extruder.Extrude(Functions.Polygon.FromRect(bounds), 5), new Materials.ParkMaterialGenerator(seed));
        }
    }
}
