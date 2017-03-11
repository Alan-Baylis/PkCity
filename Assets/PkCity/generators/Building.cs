using UnityEngine;
using System;

namespace Pk.Generators {
    public class Building : Generator {
        public enum Type {
            DEFAULT,
            SQUARED,
            ROUNDED,
        }

        public static float
            minUnits = 5,
            maxUnits = 15;

        public Rect size;
        public Type type;

        public override void Initialize() {
            GenerateStructure();

            base.Initialize();
        }
        
        void GenerateStructure() {
            UnityEngine.Random.InitState(seed);
            var r = new System.Random(seed);

            var bounds = size;
            bounds.size *= PlayArea.PlayCell.unit;
            var vertices = Functions.Polygon.FromRect(bounds);
            vertices.center = Vector2.zero;

            var minHeight = 10f;
            var height = minHeight + Mathf.Pow(5 * Mathf.PerlinNoise(transform.position.x / 10, transform.position.y / 10), 3);
            if (UnityEngine.Random.value >= 0.98f) height *= Mathf.PI;
            
            var bevel = new Functions.FrequencyList<Func<Mesh>> (
                new Func<Mesh>[] {
                    new Func<Mesh>(delegate() {
                        return Functions.Extruder.Extrude(vertices, height - PlayArea.PlayCell.unit);
                    }),
                    new Func<Mesh>(delegate() {
                        vertices.Square(PlayArea.PlayCell.unit);
                        type = Type.SQUARED;
                        return Functions.Extruder.ExtrudeCCW(vertices, height - PlayArea.PlayCell.unit);
                    }),
                    new Func<Mesh>(delegate() {
                        vertices.Round(PlayArea.PlayCell.unit);
                        type = Type.ROUNDED;
                        return Functions.Extruder.ExtrudeCCW(vertices, height - PlayArea.PlayCell.unit);
                    }),
                },
                new int[] { 3, 2, 1 }
                );
            var first = bevel[r.Next(bevel.Count)]();
            CreateProp("Structure", Vector3.zero, Quaternion.Euler(0, 0, 0), first, new Materials.BuildingMaterialGenerator());

            // Buildings are not actually the right height, so this compensates
            var raycastInfo = new RaycastHit();
            Physics.Raycast(transform.position + new Vector3(0, height, 0), Vector3.down, out raycastInfo);

            var playInfo = new PlayArea.PlayAreaInfo((int) size.width, (int) size.height);

            //var play = Generate<PlayArea.PlayArea>(raycastInfo.point - transform.position);
            //play.info = playInfo;
            //play.Initialize();
        }
    }
}
