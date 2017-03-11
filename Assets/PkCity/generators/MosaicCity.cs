using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Pk.Generators {
    public class MosaicCity : Generator {
        [Header("Region Generation")]
        public float cullingRadius = 100;
        public float resolution = 10;
        public float threshold = 0.1f;
        public bool oneShot = false;

        Vector3 lastCameraPosition;

        void Start() {
            base.Initialize();

            if (oneShot)
                OneShotGeneration();
        }
        
        void LateUpdate() {
            if (!oneShot)
                UpdateGeneration();
        }

        void UpdateGeneration() {
            var camera = Camera.current;
            if (camera == null || camera.transform.position == lastCameraPosition)
                return;
            lastCameraPosition = camera.transform.position;

            var points = Functions.Point.Around(Util.V32(transform.position), cullingRadius, multSeed, resolution, threshold);
            var graph = new Functions.DualGraph(points);

            for (int i = 0; i < children.Count; i++) {
                var child = children[0];
                if (!points.Select(o => { return transform.position + Util.V23(o); }).Contains(child.transform.position)) {
                    children.RemoveAt(i--);
                    Destroy(child.gameObject);
                    Debug.Log("Region Destroyed!");
                }
            }

            foreach (var point in points)
                if (!children.Select(o => { return o.transform.position; }).Contains(transform.position + Util.V23(point))) {
                    var bounds = graph.GetPolygon(point);
                    if (bounds != null) {
                        var region = Generate<Region>(Util.V23(point));
                        region.bounds = bounds;
                    }
                }
        }
        /*
        void UpdateGeneration() {
            if (UpdateRoots()) {
                graph.Refresh(newGraph);
                foreach (var root in newRoots) {
                    var bounds = graph.GetPolygon(root);
                    if (bounds != null) {
                        var region = Generate<Region>(root);
                        region.bounds = bounds;
                        region.Initialize();
                        Debug.Log("Region Created!");
                    }
                }

                foreach (var root in oldRoots) {
                    var oldRegion = children.Find(o => (o as Region).seed == seed);
                    if (oldRegion != null) {
                        children.Remove(oldRegion);
                        Destroy(oldRegion.gameObject);
                        Debug.Log("Region Destroyed!");
                    }
                }
            }
        }

        bool UpdateRoots() {
            var camera = Camera.current;
            if (camera != null && camera.transform.position != lastCameraPosition) {
                lastCameraPosition = camera.transform.position;
                oldGraph = newGraph;
                newGraph = Functions.Point.At(new Vector2(camera.transform.position.x, camera.transform.position.z), cullingRadius, multSeed, resolution, threshold);
                if (!newGraph.SequenceEqual(oldGraph)) {
                    //TODO figure out how to cull old regions and spawn new ones
                    newRoots  = newGraph.Except(oldGraph).ToList();
                    oldRoots = oldGraph.Except(newGraph).ToList();
                    return true;
                }
            }
            return false;
        }
        */
        void OneShotGeneration() {
            var points = Functions.Point.Around(Util.V32(transform.position), cullingRadius, multSeed, resolution, threshold);
            var graph = new Functions.DualGraph(points);
            foreach (var point in points) {
                var bounds = graph.GetPolygon(point);
                if (bounds != null) {
                    var region = Generate<Region>(Util.V23(bounds.center) - transform.position);
                    var transBounds = bounds;
                    transBounds.center = Vector2.zero;
                    region.bounds = transBounds;
                    region.Initialize();
                    Debug.Log("Region Created!");
                }
            }
        }

        float multSeed { get {
                Random.InitState(seed);
                return Random.value;
            } }
    }
}

/*
namespace Pk {
    public class Director : MonoBehaviour {
        public float
            cullingRadius = 100,
            resolution = 10,
            threshold = 0.1f;
        public bool randomSeed = true;
        public float seed = 0;

        List<Generators.Region> regions;
        Vector2[] roots, lastRoots;
        Vector3 lastCameraPosition;

        Functions.DualGraph graph;

        void Start() {
            SetSeed();

            regions = new List<Generators.Region>();
            roots = new Vector2[0];
            lastRoots = roots;
            graph = new Functions.DualGraph();
        }

        void Update() {
            UpdateGeneration();
        }

        void UpdateGeneration() {
            if (UpdateRoots()) {
                var newRegions = new List<Generators.Region>();
                graph.Refresh(roots.ToList());
                foreach (var root in roots.Except(lastRoots)) {
                    var bounds = graph.GetPolygon(root);
                    if (bounds != null) {
                        var region = Generators.Generator.Generate<Generators.Region>(transform.position, transform.rotation, transform);
                        region.root = root;
                        region.bounds = bounds;
                        region.Initialize();
                        newRegions.Add(region);
                    }
                }
                
                foreach (var root in lastRoots)
                    if (!roots.Contains(root)) {
                        var oldRegion = regions.Find(o => o.root == root);
                        regions.Remove(oldRegion);
                        Destroy(oldRegion);
                    }
                
                regions = regions.Concat(newRegions).ToList();
            }
        }

        bool UpdateRoots() {
            var camera = Camera.current;
            if (camera != null && camera.transform.position != lastCameraPosition) {
                lastCameraPosition = camera.transform.position;
                var newRoots = Functions.Point.At(new Vector2(camera.transform.position.x, camera.transform.position.z), cullingRadius, seed, resolution, threshold);
                if (!newRoots.SequenceEqual(roots)) {
                    lastRoots = roots;
                    roots = newRoots;
                    return true;
                }
            }
            return false;
        }

        void SetSeed() {
            if (randomSeed) {
                seed = Random.value;
            } else {
                // Seed is used for multiplying.  This guarantees that the result never overflows.
                Random.seed = (int) seed;
                seed = Random.value;
            }
        }
    }
}
*/
