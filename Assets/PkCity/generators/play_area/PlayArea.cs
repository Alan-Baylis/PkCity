using System;
using UnityEngine;

namespace Pk.Generators.PlayArea {
    public class PlayArea : Generator {
        public PlayAreaInfo info;
        PlayCell.Type[,] map;

        public override void Initialize() {
            map = new PlayCell.Type[info.width + 2, info.height + 2];

            GenerateMap();

            GenerateCells();

            base.Initialize();
        }

        void GenerateCells() {
            for (int i = 0; i < info.width; i++)
                for (int j = 0; j < info.height; j++) {
                    var tile = Functions.Polygon.FromRect(new Rect(0,0, PlayCell.unit, PlayCell.unit));
                    UnityEngine.Random.InitState(seed);
                    CreateProp("Tile", PlayCell.unit * new Vector3(i - (((float) info.width) / 2), 0, j - (((float) info.height) / 2)), Quaternion.Euler(0, 0, 0), Functions.Extruder.Extrude(tile, 0.1f), new Materials.ParkMaterialGenerator(UnityEngine.Random.Range(int.MinValue, int.MaxValue)));
                }
        }

        void GenerateMap() {
            var r = new System.Random(seed);

            var scaffoldPatterns = new Functions.FrequencyList<Action>(
                new Action[] {
                    new Action(delegate() { }),
                    new Action(delegate() {
                        MaskMap(new IntRect(0, 0, info.width, info.height), PlayCell.Type.SCAFFOLD); //TODO more patterns
                    }),
                },
                new int[] { 5, 1, }
                );
            scaffoldPatterns[r.Next(scaffoldPatterns.Count)]();

            MaskMap(new IntRect(1, 1, info.width, info.height), PlayCell.Type.CLEAR);

            foreach (var escape in info.escapes) {
                //TODO
            }

            foreach (var interior in info.interiors) {
                MaskMap(interior.bounds, PlayCell.Type.BLOCKED);
                //TODO build interior
            }
        }

        void MaskMap(IntRect bounds, PlayCell.Type type) {
            for (int i = bounds.x; i < bounds.x + bounds.width && i < info.width; i++)
                for (int j = bounds.y; j < bounds.y + bounds.height && j < info.height; j++)
                    map[i, j] = type;
        }
    }
}
