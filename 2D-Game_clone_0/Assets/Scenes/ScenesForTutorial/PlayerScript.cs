using Mirror;
using UnityEngine;

namespace QuickStart
{
    public class PlayerScript : NetworkBehaviour
    {
        private SceneScript sceneScript;

        public TextMesh playerNameText;
        public GameObject floatingInfo;

        private Material playerMaterialClone;

        private int selectedWeaponLocal = 1;
        public GameObject[] weaponArray;

        private Weapon activeWeapon;
        private float weaponCooldownTime;

        [SyncVar(hook = nameof(OnWeaponChanged))]
        public int activeWeaponSynced = 1;

        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName;

        [SyncVar(hook = nameof(OnColorChanged))]
        public Color playerColor = Color.white;

        void OnNameChanged(string _Old, string _New)
        {
            playerNameText.text = playerName;
        }

        void OnColorChanged(Color _Old, Color _New)
        {
            playerNameText.color = _New;
            playerMaterialClone = new Material(GetComponent<Renderer>().material);
            playerMaterialClone.color = _New;
            GetComponent<Renderer>().material = playerMaterialClone;
        }

        void OnWeaponChanged(int _Old, int _New)
        {
            if (0 < _Old && _Old < weaponArray.Length && weaponArray[_Old] != null)
                weaponArray[_Old].SetActive(false);

            if (0 < _New && _New < weaponArray.Length && weaponArray[_New] != null)
            {
                weaponArray[_New].SetActive(true);
                activeWeapon = weaponArray[activeWeaponSynced].GetComponent<Weapon>();
                if (isLocalPlayer)
                    sceneScript.UIAmmo(activeWeapon.weaponAmmo);
            }
        }

        private void Awake()
        {
            sceneScript = GameObject.Find("SceneReference").GetComponent<SceneReference>().sceneScript;
            foreach (var item in weaponArray)
                if (item != null)
                    item.SetActive(false);
            if (selectedWeaponLocal < weaponArray.Length && weaponArray[selectedWeaponLocal] != null)
                activeWeapon = weaponArray[selectedWeaponLocal].GetComponent<Weapon>();
                sceneScript.UIAmmo(activeWeapon.weaponAmmo);
        }

        public override void OnStartLocalPlayer()
        {
            //Camera.main.transform.SetParent(transform);
            //Camera.main.transform.localPosition = new Vector3(0, 0, 0);

            floatingInfo.transform.localPosition = new Vector3(0, 0.7f, 0.6f);
            floatingInfo.transform.localScale = new Vector3(2f, 2f, 2f);

            string name = "Player" + Random.Range(100, 999);
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            CmdSetupPlayer(name, color);
            sceneScript.playerScript = this;
        }

        [Command]
        public void CmdChangeActiveWeapon(int newIndex)
        {
            activeWeaponSynced = newIndex;
        }

        [Command]
        public void CmdSetupPlayer(string _name, Color _col)
        {
            // player info sent to server, then server updates sync vars which handles it on all clients
            playerName = _name;
            Debug.Log(playerName);
            playerColor = _col;
            sceneScript.statusText = $"{playerName} joined.";
        }

        [Command]
        public void CmdSendPlayerMessage()
        {
            if (sceneScript)
                sceneScript.statusText = $"{playerName} says hello {Random.Range(10, 99)}";
        }


        void Update()
        {
            if (!isLocalPlayer)
            {
                // make non-local players run this
                //floatingInfo.transform.LookAt(Camera.main.transform);
                return;
            }

            float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110.0f;
            float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4f;

            transform.position = transform.position + new Vector3(moveX * 5 * Time.deltaTime,0 , 0);
            transform.Translate(0, 0, moveZ);

            if (Input.GetButtonDown("Fire1"))
            {
                weaponCooldownTime = Time.time + activeWeapon.weaponCooldown;
                activeWeapon.weaponAmmo -= 1;
                sceneScript.UIAmmo(activeWeapon.weaponAmmo);
                CmdShootRay();
            }

            if(Input.GetButtonDown("Fire2"))
            {
                selectedWeaponLocal += 1;
                if (selectedWeaponLocal > weaponArray.Length)
                    selectedWeaponLocal = 1;

                CmdChangeActiveWeapon(selectedWeaponLocal);
            }

            [Command]
            void CmdShootRay()
            {
                RpcFireWeapon();
            }

            [ClientRpc]
            void RpcFireWeapon()
            {
                GameObject bullet = Instantiate(activeWeapon.weaponBullet, activeWeapon.weaponFirePosition.position, activeWeapon.weaponFirePosition.rotation);
                bullet.GetComponent<Rigidbody2D>().velocity= bullet.transform.forward * activeWeapon.weaponSpeed * transform.rotation.x;
                Destroy(bullet, activeWeapon.weaponLife);
            }
        }
    }
}