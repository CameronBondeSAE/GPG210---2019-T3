using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts.Checkpoints;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    private Vector3 currentSpeedAngle;
    [SerializeField]
    //private Vector3 maxSpeedAngle;

    private float maxSpeedAngle = -180;
    private float zeroSpeedAngle = 60;
    private float maxSpeed;
    private float currentSpeed;
    
    //private Quaternion speedOMeterHandRot;
    private float fuel;
    private float maxFuel;
    private DylanCar dylanCar;
    public Transform speedOMeterHand;
    private Transform speedLabelTemplateTransform;
    public Text speedLabelText;
    private FuelComponent fuelComponent;
    public TextMeshProUGUI fuelText;

    public List<int> playerPosition = new List<int>();
    //private CheckpointReachedPlayerData checkPointData;

    public Text playerPos1Text;
    public Text playerPos2Text;

    [SerializeField] private int playerPos1 = 3;
    [SerializeField] private int playerPos2 = 2;

    private void Start()
    {
        dylanCar = FindObjectOfType<DylanCar>();
        //fuelComponent = GetComponent<FuelComponent>();
        //the below is getting a reference to the max fuel from the fuel component script
        //maxFuel = fuelComponent.maxFuel;
        //fuel = fuelComponent.currentFuel;
        //fuelText.text = "Fuel: " + fuel.ToString("F0");
        //peedLabelTemplateTransform = transform.Find("SpeedLabelTemplate");
        //speedLabelTemplateTransform.gameObject.SetActive(false);

        currentSpeed = 0f;
        maxSpeed = 200f;
        
        playerPosition.Add(playerPos1);
        playerPosition.Add(playerPos2);
    }

    private void Update()
    {
        /*fuel = fuelComponent.currentFuel;
        fuelText.text = "Fuel: " + fuel.ToString("F0");

        if (fuelComponent.OutOfFuel())
        {
            fuelText.text = "Fuel: Out Of Fuel";
        }
        if(fuel >= maxFuel)
        {
            fuel = maxFuel;
        }*/
        
        /*
        currentSpeedAngle = new Vector3(0, 0, -dylanCar.speed);
        speedOMeterHandRot.eulerAngles = currentSpeedAngle;
        speedOMeterHand.transform.rotation = speedOMeterHandRot;
        */
        
        
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }

        speedOMeterHand.eulerAngles = new Vector3(0, 0, GetSpeedRotation());
        //CreateSpeedLabels();
        
        //Test for leaderboard Stuff
        SortLeaderBoard();
        playerPos1Text.text = "Player Position: " + playerPosition[0].ToString();
        playerPos2Text.text = "Player Position: " + playerPosition[1].ToString(); 
    }

    private void CreateSpeedLabels()
    {
        int labelAmount = 10;
        float totalAngelSize = zeroSpeedAngle - maxSpeedAngle;

        for (int i = 0; i <= labelAmount; i++)
        {
            Transform speedLabelTransform = Instantiate(speedLabelTemplateTransform, transform);
            float labelSpeedNormalised = (float) i / labelAmount;
            float speedLabelAngle = zeroSpeedAngle - labelSpeedNormalised * totalAngelSize;
            speedLabelTransform.eulerAngles = new Vector3(0,0,speedLabelAngle);
            speedLabelTransform.Find("SpeedText").GetComponent<Text>().text = Mathf.RoundToInt(labelSpeedNormalised * maxSpeed).ToString();
            speedLabelTransform.Find("SpeedText").eulerAngles = Vector3.zero;
            speedLabelTransform.gameObject.SetActive(true);

        }
    }
    private float GetSpeedRotation()
    {
        float totalAngleSize = zeroSpeedAngle - maxSpeedAngle;

        float speedNormalised = currentSpeed / maxSpeed;

        return zeroSpeedAngle - speedNormalised * totalAngleSize;
    }

    //TODO figure out why list isn't sorting properly
    private void SortLeaderBoard()
    {
        playerPosition.Sort(SortList);
    }
    private int SortList(int a, int b)
    {
        if (a < b)
        {
            return -1;
        }
        else if (a > b)
        {
            return 1;
        }
        return 0;
    }
    
}
