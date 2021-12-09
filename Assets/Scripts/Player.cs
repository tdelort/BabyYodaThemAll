using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;

    [SerializeField] private GameObject cursorPrefab;
    GameObject cursor;

    [SerializeField] private Renderer rend;
    [SerializeField] Material hitMat;
    Material oldMat;
    private NetworkVariable<bool> highlighted = new NetworkVariable<bool>(false);
    protected const int maxHealth = 30;
    private NetworkVariable<int> health = new NetworkVariable<int>(maxHealth);
    private NetworkVariable<uint> kills = new NetworkVariable<uint>();
    private NetworkVariable<uint> score = new NetworkVariable<uint>();
    private Coroutine hlCoroutine;
    [SerializeField] Slider healthBar;

    public NetworkVariable<uint> id = new NetworkVariable<uint>();
    protected NetworkVariable<bool> isUsingAction1 = new NetworkVariable<bool>(false);
    protected NetworkVariable<bool> isUsingAction2 = new NetworkVariable<bool>(false);
    protected NetworkVariable<bool> isUsingAction3 = new NetworkVariable<bool>(false);
    protected NetworkVariable<bool> isUsingAction4 = new NetworkVariable<bool>(false);

    [SerializeField] private Image action1cooldown;
    [SerializeField] private Image action2cooldown;
    [SerializeField] private Image action3cooldown;

    [SerializeField] private Text killsText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text idText;

    void Start()
    {
        Debug.Assert(rend != null);
        Debug.Assert(hitMat != null);
        oldMat = rend.material;
        highlighted.OnValueChanged += OnHighlighted;
        healthBar.value = 1;
        health.OnValueChanged += OnHealth;

        kills.OnValueChanged += OnKills;
        score.OnValueChanged += OnScore;
    } 

    private void OnAction1(bool before, bool after)
    {
        action1cooldown.color = after ? Color.red : Color.green;
    }

    private void OnAction2(bool before, bool after)
    {
        action2cooldown.color = after ? Color.red : Color.green;
    }

    private void OnAction3(bool before, bool after)
    {
        action3cooldown.color = after ? Color.red : Color.green;
    }

    private void OnHealth(int before, int after)
    {
        if(health.Value <= 0)
        {
            // Bruh hardcore
            if(NetworkManager.Singleton.IsServer)
            {
                GetComponent<NetworkObject>().Despawn();
            }
            gameObject.SetActive(false);
        }

        healthBar.value = (float)health.Value / (float)maxHealth;
    }

    private void OnKills(uint before, uint after)
    {   
        killsText.text = after.ToString("D4");
    }

    private void OnScore(uint before, uint after)
    {
        scoreText.text = after.ToString("D4");
    }

    public override void OnNetworkSpawn()
    {
        id.OnValueChanged = (uint before, uint after) => idText.text = after.ToString("D1");
        Debug.Log("Player started");
        if (IsLocalPlayer)
        {
            Camera camera = Camera.main;
            camera.transform.rotation = Quaternion.Euler(45, 0, 0);
            CameraController cameraController = camera.GetComponent<CameraController>();
            cameraController.target = transform;
            cameraController.offset = new Vector3(0, 15, -15);
            cursor = Instantiate(cursorPrefab);


            action1cooldown.gameObject.SetActive(true);
            action2cooldown.gameObject.SetActive(true);
            action3cooldown.gameObject.SetActive(true);
            isUsingAction1.OnValueChanged += OnAction1;
            isUsingAction2.OnValueChanged += OnAction2;
            isUsingAction3.OnValueChanged += OnAction3;
        }


        if(NetworkManager.Singleton.IsServer)
        {
            id.Value = GameManager.GetNextPlayerId();
        }


        InitAction1();
        InitAction2();
        InitAction3();
        InitAction4();

    }


    // Update is called once per frame
    void Update()
    {
        if(IsLocalPlayer)
        {
            // Translation
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            Vector3 movement = input * speed * Time.deltaTime;

            // Rotation
            Quaternion rotation = transform.rotation;
            if(!isUsingAction3.Value)
                rotation = ComputeRotation();

            TransformPlayerServerRpc(movement, rotation);

            if (Input.GetButtonDown("Action1"))
            {
                Action1();
            }

            if (Input.GetButtonDown("Action2"))
            {
                Action2();
            }

            if (Input.GetButtonDown("Action3"))
            {
                Action3();
            }

            if (Input.GetButtonDown("Action4"))
            {
                Action4();
            }
        }
    }

    private Quaternion ComputeRotation()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            cursor.transform.position = targetPoint;
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            return targetRotation;
        }
        return Quaternion.Euler(0, 0, 0);
    }

    protected virtual void Action1()
    {
        Debug.Log("Player.Action1");
    }

    protected virtual void Action2()
    {
        Debug.Log("Player.Action2");
    }

    protected virtual void Action3()
    {
        Debug.Log("Player.Action3");
    }

    protected virtual void Action4()
    {
        Debug.Log("Player.Action4");
    }

    protected virtual void InitAction1()
    {
        Debug.Log("Player.InitAction1");
    }

    protected virtual void InitAction2()
    {
        Debug.Log("Player.InitAction2");
    }

    protected virtual void InitAction3()
    {
        Debug.Log("Player.InitAction3");
    }

    protected virtual void InitAction4()
    {
        Debug.Log("Player.InitAction4");
    }

    [ServerRpc]
    public void TransformPlayerServerRpc(Vector3 movement, Quaternion rotation)
    {
        transform.position += movement;
        transform.rotation = rotation;
    }

    //######### TAKING DAMAGE ##########
    
    public void TakeDamage(int damage)
    {
        Debug.Log("Player.TakeDamage");
        health.Value -= damage;
        if(hlCoroutine != null)
            StopCoroutine(hlCoroutine);
        StartCoroutine(HighLight()); 
    }

    private IEnumerator HighLight()
    {
        highlighted.Value = true;
        yield return new WaitForSeconds(0.1f);
        highlighted.Value = false;
    }

    private void OnHighlighted(bool before, bool after)
    {
        //Debug.LogError("Highlighted: " + after);
        if (after)
            rend.material = hitMat;
        else
            rend.material = oldMat;
    }

    //########## GAINING POINTS ##########
    public void AddKill()
    {
        Debug.Log("Player.AddKill");
        if(!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("Don't call this on client side to avoid conflicts !");
            return;
        }
        kills.Value++;
    }

    public void AddScore()
    {
        Debug.Log("Player.AddScore");
        if(!NetworkManager.Singleton.IsServer)
        {
            Debug.LogError("Don't call this on client side to avoid conflicts !");
            return;
        }
        score.Value++;
    }
}