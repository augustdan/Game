using UnityEngine;
using UnityEngine.AI;

namespace Mirror.Examples.Tanks
{
    public class Tank : NetworkBehaviour
    {
        [Header("Components")]
        public NavMeshAgent agent;
        public Animator animator;
        public TextMesh healthBar;
        public Transform turret;

        [Header("Movement")]
        public float rotationSpeed = 100;

        [Header("Firing")]
        public KeyCode shootKey = KeyCode.Space;
        public GameObject projectilePrefab;
        public Transform projectileMount;

        [Header("Stats")]
        [SyncVar] public int health = 4;

        void Update()
        {
            // always update health bar.
            // (SyncVar hook would only update on clients, not on server)
            healthBar.text = new string('-', health);

            // movement for local player
            if (isLocalPlayer)
            {
                // rotate
                float horizontal = Input.GetAxis("Horizontal");
                transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);

                // move
                float vertical = Input.GetAxis("Vertical");
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                agent.velocity = forward * Mathf.Max(vertical, 0) * agent.speed;
                animator.SetBool("Moving", agent.velocity != Vector3.zero);

                // shoot
                if (Input.GetKeyDown(shootKey))
                {
                    CmdFire();
                }

                RotateTurret();
            }
        }

        // this is called on the server
        [Command]
        void CmdFire()
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, projectileMount.rotation);
            NetworkServer.Spawn(projectile);
            RpcOnFire();
        }

        // this is called on the tank that fired for all observers
        [ClientRpc]
        void RpcOnFire()
        {
            animator.SetTrigger("Shoot");
        }
        public GameObject[] objectsToHide;
        private SceneScript sceneScript;

        [SyncVar(hook = nameof(OnChanged))]
        public bool isDead = false;

        void OnChanged(bool _Old, bool _New)
        {
            if (isDead == false) // respawn
            {
                // allow user to kill themselves via button, just for prototyping
                foreach (var obj in objectsToHide)
                {
                    obj.SetActive(true);
                }

                if (isLocalPlayer)
                {
                    // Uses NetworkStartPosition feature, optional.
                    this.transform.position = NetworkManager.startPositions[Random.Range(0, NetworkManager.startPositions.Count)].position;
                }

                sceneScript.statusText.text = "Player Respawned";
            }
            else if (isDead == true) // death
            {
                // have meshes hidden, disable movement and show respawn button
                foreach (var obj in objectsToHide)
                {
                    obj.SetActive(false);
                }

                sceneScript.statusText.text = "Player Defeated";
            }

            if (isLocalPlayer)
            {
                sceneScript.SetupScene();
            }
        }

        void Awake()
        {
            //allow all players to run this, maybe they will need references to it in the future
            sceneScript = GameObject.FindObjectOfType<SceneScript>();
        }

        public override void OnStartLocalPlayer()
        {
            // local player sets reference to scene scripts variable, so they can communicate with each other
            // you could also use regular Start() and if( isLocalPlayer ) { } instead of  OnStartLocalPlayer()
            sceneScript.playerScript = this;
            sceneScript.SetupScene();
        }

        [Command]
        public void CmdPlayerStatus(bool _value)
        {
            // player info sent to server, then server changes sync var which updates, causing hooks to fire
            isDead = _value;
        }
        [ServerCallback]
        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Projectile>() != null)
            {
                --health;
                if (health == 0)
                    NetworkServer.Destroy(gameObject);
            }
        }

        void RotateTurret()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.DrawLine(ray.origin, hit.point);
                Vector3 lookRotation = new Vector3(hit.point.x, turret.transform.position.y, hit.point.z);
                turret.transform.LookAt(lookRotation);
            }
        }
    }
}
