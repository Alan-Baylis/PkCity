using UnityEngine;
using System.Collections.Generic;

namespace Pk.Generators.PlayArea {
    public class PlayCell : Generator {
        public static float unit = 2;

        public enum Type {
            AIR, // Array default
            CLEAR,
            BLOCKED,
            SHORT_BLOCK,
            CLIMB_BLOCK,
            TALL_BLOCK,
            SCAFFOLD,
            PIPE,
            STAIRS,
            SHORT_ESCAPE,
            LONG_ESCAPE,
            DOOR,
            INTERIOR,
            ELEVATOR,
        }

        public static HashSet<Type> blockedTiles = new HashSet<Type>() {
            Type.AIR,
            Type.BLOCKED,
            Type.TALL_BLOCK,
        };

        public override void Initialize() {
            base.Initialize();
        }
    }
}