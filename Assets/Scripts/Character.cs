using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    // Canvas
    [SerializeField] private GameObject playerUI;
    // Movement stuff
    [Tooltip("Speed of the Player. Default: 5")]
    [SerializeField] private float speed = 5;
    private Vector3 myPos;
    private float horizontalInput;
    private float verticalInput;
    CharacterController controller;
    
    // Inventory mechanic to be implemented in future
    private GameObject Inventory;
    
    //Flashlight
    private GameObject flashlight;
    private bool flashStatus = false;
    
    // Animation Stuff
    private GameObject leftarm;
    private GameObject rightarm;
    
    // Weaponry
    private GameObject PrimaryWeps;
    private GameObject SecondaryWeps;
    private GameObject GunWeps;
    private Image PWepUI;
    private Image SWepUI;
    private Image GWepUI;
    
    private GameObject CurrentPrimary;
    private GameObject CurrentSecondary;
    private GameObject CurrentGun;
    private GameObject CurrentWeapon;
    private WeaponBehavior CurrentBehavior;
    
    // Dash mechanic
    private GameObject loadingUI;
    private Text dashval;
    private Vector3 moveDirection;
    [Tooltip("Number of dashes. Default: 3")]
    [SerializeField] private int dashes = 3;
    private bool dashoncd = false;
    [Tooltip("Distance of the dash. Default: 10")]
    [SerializeField] private float dashDistance = 10;
    private bool dashflag = false;
    float dashSpeed = 6;
    
    // Heath Mechanics
    [Tooltip("Health of the Player. Default: 100")]
    [SerializeField] int health;
    private Text healthval;
    private Image healthbar;
    private int healthtot;
    private Material damaged;
    private Material original;
    private bool invincibility = false;
    private GameObject gameover;

    void Start()
    {
        Inventory = playerUI.transform.GetChild(3).gameObject;
        leftarm = transform.GetChild(0).gameObject;
        rightarm = transform.GetChild(1).gameObject;
        PrimaryWeps = rightarm.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        SecondaryWeps = rightarm.transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
        GunWeps = rightarm.transform.GetChild(0).GetChild(0).GetChild(2).gameObject;
        flashlight = leftarm.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
        PWepUI = playerUI.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        SWepUI = playerUI.transform.GetChild(0).GetChild(2).GetComponent<Image>();
        GWepUI = playerUI.transform.GetChild(0).GetChild(3).GetComponent<Image>();
        CurrentPrimary = PrimaryWeps.transform.GetChild(0).gameObject;
        CurrentSecondary = SecondaryWeps.transform.GetChild(0).gameObject;
        CurrentGun = GunWeps.transform.GetChild(0).gameObject;
        CurrentWeapon = CurrentSecondary;
        loadingUI = playerUI.transform.GetChild(1).GetChild(1).gameObject;
        dashval = playerUI.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        healthval = playerUI.transform.GetChild(2).GetChild(1).GetComponent<Text>();
        healthbar = playerUI.transform.GetChild(2).GetChild(0).GetComponent<Image>();
        damaged = Resources.Load("Materials/RedDamaged", typeof(Material)) as Material;
        original = Resources.Load("Materials/Red", typeof(Material)) as Material;
        gameover = playerUI.transform.GetChild(4).gameObject;
        healthtot = health;
        controller = GetComponent<CharacterController>();
        InvokeRepeating("DashRegen", 1, 1);
        CurrentBehavior = CurrentWeapon.GetComponent<WeaponBehavior>();
    }
    
    // Function to take damage
    public void takedamage(int dmg)
    {
        if (invincibility == false)
        {
            health -= dmg;
            healthval.text = health.ToString();
            float diff = (float) health / (float) healthtot;
            gotdamaged();
            healthbar.fillAmount = (float)diff;
            if (health <= 0)
            {
                gameover.SetActive(true);
                this.gameObject.SetActive(false);
                Application.targetFrameRate = 0;
            }
        }
    }
    void gotdamaged()
    {
        GetComponent<MeshRenderer>().material = damaged;
        invincibility = true;
        Invoke("notdamaged", 0.4f);
    }
    void notdamaged()
    {
        GetComponent<MeshRenderer>().material = original;
        invincibility = false;
    }
    
    // Flashlight animation functions
    void FLArmDown()
    {
        leftarm.GetComponent<Animator>().Play("LeftArmDown");
    }
    void FLOn()
    {
        flashlight.GetComponent<Animator>().Play("FlashLightOn");
    }

    // Callback functions for future, will be called from inventory to assign new weapons
    public void setprimary(GameObject primary)
    {
        CurrentPrimary = primary;
        CurrentBehavior = CurrentWeapon.GetComponent<WeaponBehavior>();
    }
    public void setsecondary(GameObject secondary)
    {
        CurrentSecondary = secondary;
        CurrentBehavior = CurrentWeapon.GetComponent<WeaponBehavior>();
    }
    public void setgun(GameObject gun)
    {
        CurrentGun = gun;
        CurrentBehavior = CurrentWeapon.GetComponent<WeaponBehavior>();
    }
    
    // Disables weapons
    void disableweps()
    {
        PrimaryWeps.SetActive(false);
        SecondaryWeps.SetActive(false);
        GunWeps.SetActive(false);
    }

    // Functions to switch to different weapons
    void switchtoprimary()
    {
        PrimaryWeps.SetActive(true);
        CurrentWeapon = CurrentPrimary;
        CurrentBehavior = CurrentWeapon.GetComponent<WeaponBehavior>();
    }
    void switchtosecondary()
    {
        SecondaryWeps.SetActive(true);
        CurrentWeapon = CurrentSecondary;
        CurrentBehavior = CurrentWeapon.GetComponent<WeaponBehavior>();
    }
    void switchtoknife()
    {
        GunWeps.SetActive(true);
        CurrentWeapon = CurrentGun;
        CurrentBehavior = CurrentWeapon.GetComponent<WeaponBehavior>();
    }

    //Dash mechanic regeneration functions
    void DashRegena()
    {
        if (dashes < 3)
            dashes++;
    }
    IEnumerator DashRegen()
    {
        dashoncd = true;
        loadingUI.SetActive(true);
        while (dashes < 3)
        {
            yield return new WaitForSeconds(2);
            dashes++;
            dashval.text = (dashes).ToString();
        }
        dashoncd = false;
        loadingUI.SetActive(false);
    }
    void dashflagtoggle()
    {
        dashflag = false;
    }
    
    //Update function containing all movement, switching etc character mechanics
    void Update()
    {
        // Basic axes for use in character controller
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        // Make user look at mouse
        Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane=new Plane(Vector3.up, Vector3.zero);
        float distance;
        if(plane.Raycast(ray, out distance)) {
            Vector3 target=ray.GetPoint(distance);
            Vector3 direction=target-transform.position;
            float rotation=Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg;
            transform.rotation=Quaternion.Euler(0, rotation, 0);
        }
        
        // Dash Mechanic
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (dashes > 0)
            {
                dashes--;
                dashval.text = (dashes).ToString();
                if (dashoncd == false)
                    StartCoroutine(DashRegen());
                dashflag = true;
                Invoke("dashflagtoggle", 0.1f);
            }
        }
        if(dashflag)
        {
            moveDirection = new Vector3(horizontalInput, 0, verticalInput) * dashDistance;
            controller.Move(moveDirection * Time.deltaTime * dashSpeed);
        }
        else
        {
            moveDirection = Vector3.zero;
        }
        

        //Firing Animations
        if (Input.GetMouseButtonDown(0))
        {
            CurrentBehavior.Fire();
        }if (Input.GetMouseButtonDown(1))
        {
            CurrentBehavior.Alternatefire();
        }
        
        
        //Switch weapons
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            rightarm.GetComponent<Animator>().Play("RightArmSwitchWeps");
            Invoke("disableweps", 0.2f);
            Invoke("switchtoprimary", 0.2f);
            PWepUI.color = Color.green;
            SWepUI.color = Color.white;
            GWepUI.color = Color.white;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            rightarm.GetComponent<Animator>().Play("RightArmSwitchWeps");
            Invoke("disableweps", 0.20f);
            Invoke("switchtosecondary", 0.2f);
            PWepUI.color = Color.white;
            SWepUI.color = Color.green;
            GWepUI.color = Color.white;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            rightarm.GetComponent<Animator>().Play("RightArmSwitchWeps");
            Invoke("disableweps", 0.20f);
            Invoke("switchtoknife", 0.2f);
            PWepUI.color = Color.white;
            SWepUI.color = Color.white;
            GWepUI.color = Color.green;
        }

        //Pull out flashlight and put back flashlight
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashStatus)
            {
                flashlight.GetComponent<Animator>().Play("FlashLightOff");
                Invoke("FLArmDown", 0.25f);
                flashStatus = false;
            }
            else
            {
                leftarm.GetComponent<Animator>().Play("LeftArmUp");
                Invoke("FLOn", 0.25f);
                flashStatus = true;
            }
        }

        
        //Bring up inventory (not implemented)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(Inventory.activeSelf)
                Inventory.SetActive(false);
            else
                Inventory.SetActive(true);
            
        }

        //Character movement
        if(!dashflag)
            controller.Move(new Vector3(horizontalInput, 0, verticalInput) * (Time.deltaTime * speed));
    }
}
