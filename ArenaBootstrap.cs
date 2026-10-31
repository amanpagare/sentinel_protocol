using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

namespace SentinelProtocol
{
    /// <summary>
    /// Creates a playable greybox arena at runtime. This makes the project robust
    /// if scene serialization or references are lost, and keeps the prototype asset-light.
    /// </summary>
    public class ArenaBootstrap : MonoBehaviour
    {
        [SerializeField] private bool generateArena = true;
        [SerializeField] private int sentinelCount = 3;

        private void Awake()
        {
            if (!generateArena) return;
            if (GameManager.Instance == null)
                new GameObject("GameManager").AddComponent<GameManager>();

            CreateArena();
        }

        private void CreateArena()
        {
            Material floorMat = MakeMaterial(new Color(0.13f, 0.16f, 0.20f));
            Material wallMat = MakeMaterial(new Color(0.24f, 0.29f, 0.35f));
            Material playerMat = MakeMaterial(new Color(0.1f, 0.8f, 1f));
            Material enemyMat = MakeMaterial(new Color(0.95f, 0.25f, 0.2f));
            Material goalMat = MakeMaterial(new Color(0.15f, 1f, 0.45f));

            MakeCube("Ground", new Vector3(0, -0.5f, 0), new Vector3(24, 1, 24), floorMat);
            MakeCube("NorthWall", new Vector3(0, 1.5f, 12), new Vector3(24, 3, .5f), wallMat);
            MakeCube("SouthWall", new Vector3(0, 1.5f, -12), new Vector3(24, 3, .5f), wallMat);
            MakeCube("EastWall", new Vector3(12, 1.5f, 0), new Vector3(.5f, 3, 24), wallMat);
            MakeCube("WestWall", new Vector3(-12, 1.5f, 0), new Vector3(.5f, 3, 24), wallMat);

            // Cover blocks make line-of-sight testing visible during play.
            MakeCube("Cover_A", new Vector3(-3, .8f, 1), new Vector3(2.5f, 1.6f, 1.2f), wallMat);
            MakeCube("Cover_B", new Vector3(3, .8f, -3), new Vector3(1.2f, 1.6f, 3f), wallMat);
            MakeCube("Cover_C", new Vector3(1, .6f, 4), new Vector3(3f, 1.2f, 1f), wallMat);

            GameObject player = new GameObject("Player");
            player.transform.position = new Vector3(0, 0, -8);
            var cc = player.AddComponent<CharacterController>();
            cc.center = new Vector3(0, 1, 0);
            cc.height = 2f;
            cc.radius = .4f;
            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(player.transform);
            visual.transform.localPosition = new Vector3(0, 1, 0);
            visual.transform.localScale = new Vector3(.8f, 1, .8f);
            Destroy(visual.GetComponent<Collider>());
            visual.GetComponent<Renderer>().material = playerMat;

            GameObject target = new GameObject("CameraTarget");
            target.transform.SetParent(player.transform);
            target.transform.localPosition = new Vector3(0, 1.5f, 0);

            GameObject cameraGO = new GameObject("Main Camera");
            cameraGO.tag = "MainCamera";
            cameraGO.AddComponent<Camera>();
            cameraGO.AddComponent<AudioListener>();
            var cameraRig = cameraGO.AddComponent<ThirdPersonCamera>();
            cameraRig.Target = target;
            var playerController = player.AddComponent<PlayerController>();
            playerController.CameraTransform = cameraGO.transform;

            Health playerHealth = player.AddComponent<Health>();
            GameManager.Instance.RegisterPlayer(playerHealth);

            var goal = MakeCube("ExtractionZone", new Vector3(0, .05f, 9), new Vector3(3, .1f, 2), goalMat);
            goal.GetComponent<Collider>().isTrigger = true;
            goal.AddComponent<ObjectiveTrigger>();

            GameObject navRoot = new GameObject("Navigation");
            var surface = navRoot.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.BuildNavMesh();

            Transform[] points = new Transform[4];
            Vector3[] positions = {
                new Vector3(-8, 0, 7), new Vector3(8, 0, 7),
                new Vector3(8, 0, -4), new Vector3(-8, 0, -4)
            };
            for (int i = 0; i < points.Length; i++)
            {
                GameObject point = new GameObject("PatrolPoint_" + (i + 1));
                point.transform.position = positions[i];
                points[i] = point.transform;
            }

            for (int i = 0; i < sentinelCount; i++)
            {
                Vector3 spawn = new Vector3(i == 0 ? -7 : i == 1 ? 7 : 0, 0, i == 2 ? 2 : 5);
                GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                enemy.name = "Sentinel_" + (i + 1);
                enemy.transform.position = spawn;
                enemy.GetComponent<Renderer>().material = enemyMat;
                var agent = enemy.AddComponent<NavMeshAgent>();
                agent.speed = 3.5f;
                agent.angularSpeed = 360f;
                agent.stoppingDistance = 1.2f;
                enemy.AddComponent<Health>();
                var sensor = enemy.AddComponent<EnemySensor>();
                var nav = enemy.AddComponent<EnemyNavigator>();
                nav.SetPatrolPoints(points);
                var ai = enemy.AddComponent<SentinelAI>();
                ai.Initialize(player.transform);
                // Sensors should be blocked by walls/cover but not by player/enemy colliders.
                sensor.Target = player.transform;
            }

            if (!FindFirstObjectByType<HUDController>())
                new GameObject("HUD").AddComponent<HUDController>();
        }

        private static GameObject MakeCube(string objectName, Vector3 position, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = objectName;
            go.transform.position = position;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().material = material;
            return go;
        }

        private static Material MakeMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (!shader) shader = Shader.Find("Standard");
            var material = new Material(shader);
            material.color = color;
            return material;
        }
    }
}
